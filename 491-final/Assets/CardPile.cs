using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardPile : MonoBehaviour {
    public List<GameObject> displayed;
    public List<CardState> deck;
    private Sprite rightArrow;
    private Sprite leftArrow;

    public static List<CardStateBuilder> allCardBuilders;

    public static List<Vector3> cardPositions;


    int indexFive = 0;
    int currentIndex = 0;

    void Awake() {
        displayed = new List<GameObject>();
        deck = new List<CardState>();
        cardPositions = new List<Vector3>();
        // for(int i = 0; i < 49; i++){
        //     List<Task> cList = new List<Task>();
        //     cList.Add(new DebugTask(i));
        //     CardState c = new CardState("Debug", 0, cList, "Sounds/PunchSound");
        //     deck.Add(c);
        // }
        // Initiate(5);



    }

    public static void AddToAll(CardStateBuilder c){
      if(allCardBuilders == null) allCardBuilders = new List<CardStateBuilder>();

      allCardBuilders.Add(c);
    }

    public static CardPile MakeCardPile(int i){
      System.Random rand = new System.Random();
      GameObject g = Instantiate(Resources.Load("Card_Select"), new Vector3(0,0), Quaternion.identity) as GameObject;
      g.tag = "CardPile";
      CardPile c = g.GetComponent<CardPile>();
      List<CardState> cards = new List<CardState>();
      for(int k = 0; k < i + 1; k++){
          //cards.Add(allCardBuilders[rand.Next(allCardBuilders.Count)].GetCardState(0));
          cards.Add(CardStateBuilder.GetRandomCard());
      }

      c.AddList(cards);
      int num = 5;
      if(i < num) num = i;
      c.Initiate(num);

      return c;
    }

    public void AddList(List<CardState> cards){
        deck.AddRange(cards);
    }


    public void Initiate(int numOfDisplayVar){
        int numOfDisplay = (numOfDisplayVar <= deck.Count) ? numOfDisplayVar : deck.Count;

        for(int i = 0; i < numOfDisplay; i++){
            GameObject g = Instantiate(Resources.Load("Card"), new Vector3(-4 + i*2, 0, 0), Quaternion.identity) as GameObject;
            g.GetComponent<SpriteRenderer>().sortingLayerName = "New Menu Canvas";
            g.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "New Menu Canvas";
          //  g.layer = LayerMask.NameToLayer("CardAboveCanvas");
            displayed.Add(g);
            cardPositions.Add(displayed[i].transform.position);
        }

        for(int i = 0; i < displayed.Count; i++){
            displayed[i].GetComponent<Card>().AddState(deck[i]);
        }
        displayed[indexFive].GetComponent<Card>().Highlight();

    }

    public void Select(){
        deck[currentIndex].RunTask(null, 0);
    }

    public GameObject GetSelected(){
        return displayed[indexFive];
    }

    public void Right(){
       if(currentIndex == deck.Count - 1){

        } else {
            displayed[indexFive].GetComponent<Card>().UnHighlight();
            if(indexFive == displayed.Count - 1){
                //animate moving over and destroy 0th index
                Destroy(displayed[0]);

                for(int i = 1; i < displayed.Count; i++) StartCoroutine(AnimateMovement(displayed[i], cardPositions[i], cardPositions[i - 1], 20));
                for(int i = 0; i < displayed.Count - 1; i++) displayed[i] = displayed[i + 1];
                displayed[displayed.Count - 1] = Instantiate(Resources.Load("Card"), cardPositions[displayed.Count - 1], Quaternion.identity) as GameObject;
                displayed[displayed.Count - 1].GetComponent<Card>().AddState(deck[currentIndex + 1]);
                displayed[displayed.Count - 1].GetComponent<SpriteRenderer>().sortingLayerName = "New Menu Canvas";
                displayed[displayed.Count - 1].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "New Menu Canvas";


            } else {
                indexFive++;
            }
            displayed[indexFive].GetComponent<Card>().Highlight();
            currentIndex++;
        }
    }
    //Menu Canvas
    public void Left(){
        if(currentIndex == 0){

        } else {
            displayed[indexFive].GetComponent<Card>().UnHighlight();
            if(indexFive == 0){
                //animate moving over and destroy 0th index
                Destroy(displayed[displayed.Count - 1]);

                for(int i = 1; i <= displayed.Count - 1; i++) StartCoroutine(AnimateMovement(displayed[i - 1], cardPositions[i - 1], cardPositions[i], 20));
                for(int i = displayed.Count - 1; i > 0; i--) displayed[i] = displayed[i - 1];

                displayed[0] = Instantiate(Resources.Load("Card"), cardPositions[0], Quaternion.identity) as GameObject;
                displayed[0].GetComponent<Card>().AddState(deck[currentIndex - 1]);
                displayed[0].GetComponent<SpriteRenderer>().sortingLayerName = "New Menu Canvas";
                displayed[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "New Menu Canvas";



            } else {
                indexFive--;
            }
            displayed[indexFive].GetComponent<Card>().Highlight();
            currentIndex--;
        }
    }

    private static IEnumerator AnimateMovement(GameObject g, Vector3 start, Vector3 finish, int loops){
        for(int i = 0; i < loops; i++){
            if(g == null) break;

            float f = 1/ (float) loops;
            //Debug.Log("Coroutine going");
            g.transform.position = Vector3.Lerp(start, finish, i*f);
            yield return null;
        }
    }

    public void DestroyAll(){
        for(int i = 0; i < displayed.Count; i++){
            Destroy(displayed[i]);
        }
        Destroy(gameObject);
    }

}
