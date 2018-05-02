using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Card : MonoBehaviour {

    private CardState cardState;
    private GameController controller;

    void Awake () {
        controller = GameController.GetGameController();
        //SetIconSprite();
    }

    private SpriteRenderer GetRenderer() {
        return gameObject.GetComponent<SpriteRenderer>();
    }

    private void SetSprite(SpriteRenderer r, string filePath) {
        r.sprite = Resources.Load<Sprite>(filePath);
    }

    private void SetIconSprite()
    {
        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardState.GetIcon();
    }


    public void Highlight(){
        this.tag = "SelectedCard";
        SpriteRenderer r = GetRenderer();
        SetSprite(r, "Sprites/CardFrontHighlighted");
        r.sortingOrder = 2;

        GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot((AudioClip) Resources.Load("Sounds/SelectionSound"), 1);

        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 3;
    }

    public void UnHighlight() {
        this.tag = "Untagged";
        SpriteRenderer r = GetRenderer();
        SetSprite(r, "Sprites/CardFront");
        r.sortingOrder = 0;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;

    }

    public bool IsHighlighted() {
        SpriteRenderer r = GetRenderer();
        return r.sprite == Resources.Load<Sprite>("Sprites/CardFrontHighlighted");
    }

    public void HighlightSelect(){
        SpriteRenderer r = GetRenderer();
        SetSprite(r, "Sprites/CardFrontSelected");
        r.sortingOrder = 2;
    }

    public void SetState(CardState c) {
        cardState = c;
        SetIconSprite();
    }

    public void AddState(CardState c){
        cardState = c;
        SetIconSprite();
    }

    public CardState GetState() {
        return cardState;
    }

	public void ActivateCard () {
        if(controller.currentEntity is Player) {
            Renderer[] r = gameObject.GetComponents<Renderer>();
            for(int i = 0; i < r.Length; i++){
                Debug.Log("Fam I ran");
                r[i].enabled = false;
            }

            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            //Get input needed,
            //give it to function in cardState
            //repeat until finished

            StartCoroutine("DoTasks");
        }
	}

    override
    public string ToString(){
        return cardState.ToString();
    }

    private IEnumerator DoTasks() {

        controller.RunningTasks(true);
        List<Task> tasks = cardState.GetTasks();
        bool flag = false;

        int i = 0;
        foreach (Task task in tasks) {

            //set the appropriate task
            switch (tasks[i].type) {

                case Task.Input.Null:
                    tasks[i].Run(null);
                    break;

                case Task.Input.Enemy:
                    controller.SetSelectionType(GameController.SelectionType.SelectEntity);
                    while (!(controller.GetInput().GetComponent<Entity>() is Enemy)) { yield return null; }
                    task.Run(controller.GetInput());
                    break;

                case Task.Input.Player:
                    controller.SetSelectionType(GameController.SelectionType.SelectEntity);
                    while (!(controller.GetInput().GetComponent<Entity>() is Player)) {
                        yield return null;
                    }
                    task.Run(controller.GetInput());
                    break;

                case Task.Input.Entity:
                    controller.SetSelectionType(GameController.SelectionType.SelectEntity);
                    flag = true;
                    while (controller.GetInput() == null) {
                        yield return null;
                    }
                    task.Run(controller.GetInput());
                    StartCoroutine(Animation(controller.GetInput()));
                    controller.SetInput(null);
                    break;

                case Task.Input.Card:
                    controller.SetSelectionType(GameController.SelectionType.SelectCard);
                    while(controller.GetInput() == null){
                        yield return null;
                    }
                    cardState.GetTasks()[i].Run(controller.GetInput());
                    break;

                case Task.Input.DiscardCard:
                    GameObject g = Instantiate(Resources.Load("Card_Select"), new Vector3(0,0), Quaternion.identity) as GameObject;
                    g.tag = "CardPile";
                    CardPile c = g.GetComponent<CardPile>();
                    c.AddList(controller.currentPlayer.GetDiscards());
                    c.Initiate(5);

                    controller.SetSelectionType(GameController.SelectionType.CardPileSelect);
                    while(controller.GetInput() == null) {
                        yield return null;
                    }
                    cardState.GetTasks()[i].Run(controller.GetInput());
                    c.DestroyAll();
                    break;
            }
            i++;
        }

        GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(cardState.soundEffect, 1);

        // Lose energy and discard played card
        Player p = (Player)controller.currentEntity.gameObject.GetComponent<Entity>();
        p.LoseEnergy(cardState.GetCost());
        p.Discard(gameObject);

        if(!flag) Destroy(gameObject);
        controller.RunningTasks(false);
        controller.SetInput(null);
        controller.SetSelectionType(GameController.SelectionType.SelectCardToPlay);
    }

    private bool animateState = false;
    private bool animateState2 = false;

    public IEnumerator Flash(GameObject g) {
        int i = 0;
        bool thing = false;
        while (i < 10 && !thing) {
            try {
                Renderer[] r = g.GetComponents<Renderer>();
                for (int k = 0; k < r.Length; k++) {
                    r[k].enabled = !r[k].enabled;
                }
            } catch (Exception e) {
                thing = true;
            }
            if (thing) {
                yield return null;
            } else {
                yield return new WaitForSeconds(.08f);
            }
            i++;
        }
        if (!thing) {
            Destroy(gameObject);
        }
    }

    public IEnumerator Animation(GameObject g){
        GameController gC = GameController.GetGameController();
        //gC.SetGameState(GameController.GameType.NullState);

        Sprite newSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;//Resources.Load<Sprite>(cardState.GetSpriteString());
        GameObject animated = new GameObject();
        SpriteRenderer renderer = animated.AddComponent<SpriteRenderer>();
        renderer.sprite = newSprite;



        animated.transform.position = gC.currentEntity.gameObject.transform.position;

        if(g.transform.position.x < animated.transform.position.x){
          animated.transform.Rotate(new Vector3(0,180,0));
        }

        animated.GetComponent<SpriteRenderer>().sortingLayerName = "EntityLayer";
        StartCoroutine(AnimateAttack(animated, animated.transform.position, g.transform.position, 25));


        animateState = true;
        while(animateState) {
                yield return null;
        }



        Destroy(animated);
        StartCoroutine(Flash(g));
}

private IEnumerator AnimateAttack(GameObject g, Vector3 start, Vector3 finish, int loops){

        int sum = 0;
        int total = (loops*loops)/2;

        for(int i = 0; i <= loops; i++) {
                if(g == null) break;
                sum += i;

                float f = sum/ (float) total;
                g.transform.position = Vector3.Lerp(start, finish, f);
                yield return null;
        }
        GameController gC = GameController.GetGameController();


        //gC.SetGameState(GameController.GameType.Battle);


        animateState = false;
}



}
