using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Enemy : Entity {

    private System.Random r;
    private int count;

    new void Awake() {

        base.Awake();

        r = new System.Random();

        effects = new List<Effect>();


        Task ex = new Damage(10);
        List<Task> li = new List<Task> {
            ex
        };
        for (int i = 0; i < 3; i++) {
            AddCard(new StrikeState());
        }
    }

    public override void DestroyAll()
    {
        base.DestroyAll();
        Destroy(gameObject);
    }

    public void ActivateEffect() {
      List<Player> players = GameController.GetGameController().GetPlayers();
      List<CardState> hand = GetHand();
      CardState c = hand[r.Next(hand.Count)];
      GameController gC = GameController.GetGameController();
      //gC.SetGameState(GameController.GameType.NullState);
      GameObject attackedPlayer = players[r.Next(players.Count)].GetGameObject();
      foreach (Task task in c.GetTasks()) {
          task.Run(attackedPlayer);
      }
      StartCoroutine(Animation(gameObject, attackedPlayer, c));
    }

    bool animateState = false;
public IEnumerator flash(GameObject g) {
        int i = 0;
        while(i < 10) {
                i++;

                Renderer[] r = g.GetComponents<Renderer>();
                for(int k = 0; k < r.Length; k++) {
                        r[k].enabled = !r[k].enabled;
                }

                yield return new WaitForSeconds(.08f);
        }
        GameController gC = GameController.GetGameController();
        //gC.SetGameState(GameController.GameType.Battle);
        isAnimating = false;
}

public IEnumerator Animation(GameObject g, GameObject attacked, CardState cardState){
        isAnimating = true;
        Sprite newSprite = Resources.Load<Sprite>(cardState.GetSpriteString());//gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;//Resources.Load<Sprite>(cardState.GetSpriteString());
        GameObject animated = new GameObject();
        SpriteRenderer renderer = animated.AddComponent<SpriteRenderer>();
        renderer.sprite = newSprite;
        renderer.sortingLayerName = "ArrowLayer";

        animated.transform.SetParent(gameObject.transform);

        GameController gC = GameController.GetGameController();
        animated.transform.position = g.transform.position;

        if(g.transform.position.x > attacked.transform.position.x){
          animated.transform.Rotate(new Vector3(0,180,0));
        }

        animated.GetComponent<SpriteRenderer>().sortingLayerName = "EntityLayer";
        StartCoroutine(AnimateAttack(animated, animated.transform.position, attacked.transform.position, 50));


        animateState = true;
        while(animateState) {
                yield return null;
        }

        Destroy(animated);
        StartCoroutine(flash(attacked));
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
        animateState = false;
}

}
