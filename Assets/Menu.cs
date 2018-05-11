using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

  private GameController controller;
  private List<Text> selectableText;
  private List<Text> normalText;
    public Font bridgnorth;
    public Font bridgnorthBold;


  Color highlighted;
  Color normal;
  private int selectableIndex;

  public void Awake () {
      normal = new Color(0f, 0f, 0f, 1f);
      highlighted = new Color(1f, 0f, 1f, 1f);
      controller = GameController.GetGameController();
      selectableText = new List<Text>();
      normalText = new List<Text>();
      selectableIndex = 0;

        //gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("menu_no_text");
        bridgnorth = Resources.Load<Font>("bridgnorth/Bridgnorth-Regular");
        bridgnorthBold = Resources.Load<Font>("bridgnorth/Bridgnorth-Bold");

        gameObject.transform.GetChild(0).GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
  }


  public void AddText(Vector3 position, string text){
    Vector3 p = position + new Vector3(0, 0, 100f);
    GameObject g = Instantiate(Resources.Load("Text"), p, Quaternion.identity) as GameObject;

    g.transform.SetParent(gameObject.transform.GetChild(0).transform, false);
    g.transform.position = p;
    Text t = g.GetComponent<Text>();//g.GetComponent<Text>();
    t.text = text;
    t.alignment = TextAnchor.MiddleCenter;
    t.fontSize = 1;
    t.font = bridgnorth;

    normalText.Add(t);
  }

    public void AddTextTitle(Vector3 position, string text)
    {
        Vector3 p = position + new Vector3(0, 0, 100f);
        GameObject g = Instantiate(Resources.Load("Text"), p, Quaternion.identity) as GameObject;

        g.transform.SetParent(gameObject.transform.GetChild(0).transform, false);
        g.transform.position = p;
        Text t = g.GetComponent<Text>();//g.GetComponent<Text>();
        t.text = text;
        t.alignment = TextAnchor.MiddleCenter;
        t.fontSize = 2;
        t.font = bridgnorthBold;

        normalText.Add(t);
    }

    public void AddSelectableText(Vector3 position, string text){
    Vector3 p = position + new Vector3(0f, 0f, 100f);
    GameObject g = Instantiate(Resources.Load("Text"), p, Quaternion.identity) as GameObject;

    g.transform.SetParent(gameObject.transform.GetChild(0).transform, false);

    Text t = g.GetComponent<Text>();//g.GetComponent<Text>();
    t.text = text;
    t.alignment = TextAnchor.MiddleCenter;
    t.fontSize = 1;
    t.font = bridgnorth;

        //  t.transform.position = p;


    if (selectableText.Count == 0){
      t.color = highlighted;
    }

    selectableText.Add(t);
  }

  public void Left(){
    if(selectableText.Count != 0){
      selectableText[selectableIndex].color = normal;
      selectableIndex = (selectableIndex - 1 + selectableText.Count) % selectableText.Count;
      selectableText[selectableIndex].color = normal;
    }
  }

  public void Right(){
    if(selectableText.Count != 0){
      selectableText[selectableIndex].color = normal;
      selectableIndex = (selectableIndex + 1 + selectableText.Count) % selectableText.Count;
      selectableText[selectableIndex].color = normal;
    }
  }

  public string Select(){
    if(selectableText.Count != 0){
      return selectableText[selectableIndex].GetComponent<Text>().text;
    }
    return null;
  }

  public void DestroyAll() {

    for(int i = 0; i < selectableText.Count; i++){
      Destroy(selectableText[i]);
    }
    for(int i = 0; i < normalText.Count; i++){
      Destroy(normalText[i]);
    }

    StartCoroutine(fadeOut());

  }


  public static GameObject InitialMenu(){
    GameObject g = Instantiate(Resources.Load("Menu"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
    Menu menu = g.GetComponent<Menu>();
    menu.AddSelectableText(new Vector3(10.3f, 0f,0), "Start Game");
    menu.AddTextTitle(new Vector3(0f, 3f, 0), "Decked Out");
    menu.AddText(new Vector3(0f, 2f, 0), "Welcome to Decked Out. This is a cooperative digital card game where you will be battling enemies.");
    menu.AddText(new Vector3(0f, 1f, 0), "Each player will have 4 energy with which they can use to play cards.");
    menu.AddText(new Vector3(0f, 0f, 0), "Player one will begin and once their turn is over it will be Player two's turn.");
    menu.AddText(new Vector3(0f, -1f, 0), "Take turns using the arrow keys and the enter button to select cards.");
    menu.AddText(new Vector3(0f, -2f, 0), "After a win, you will be given a chance to add a special new card to one of the player's decks.");

    return g;
  }

  public static GameObject PickACardMenu(){
    GameObject g = Instantiate(Resources.Load("Menu"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
    Menu menu = g.GetComponent<Menu>();
    menu.AddText(new Vector3(0f, -3f,0), "Select a Card to add to a Player's Deck");

        GameObject[] cardInfoText = GameObject.FindGameObjectsWithTag("CardInfo");
        GameObject[] cardInfoBox = GameObject.FindGameObjectsWithTag("CardInfoBox");

        cardInfoBox[0].GetComponent<SpriteRenderer>().sortingLayerName = "Menu Canvas";
        cardInfoBox[0].GetComponent<SpriteRenderer>().sortingOrder = 5;
        for (int i=0; i<cardInfoText.Length; i++)
        {
            cardInfoText[i].GetComponent<Canvas>().sortingLayerName = "Menu Layer";
            cardInfoText[i].GetComponent<Canvas>().sortingOrder = 6;
        }
    
    return g;
  }

   public static GameObject DeathMenu(int score)
   {
        GameObject g = Instantiate(Resources.Load("Menu"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Menu menu = g.GetComponent<Menu>();
        menu.AddText(new Vector3(0f, 1.3f, 0), "Oh no! One of your heroes died!");
        menu.AddText(new Vector3(0f, 0f, 0), "You scored " + score + " points.");
        menu.AddSelectableText(new Vector3(10.3f, 3.3f, 0), "Play Again?");

        return g;
    }

    public IEnumerator fadeOut(){
        GameController.GetGameController().gameType = GameController.GameType.NullState;
    float opacity = 1f;
    SpriteRenderer s = gameObject.GetComponent<SpriteRenderer>();
    for(int i = 0; i < 50; i++){
      opacity = (50f - (float)i)/50f;
      Color col = s.color;
      col.a = opacity;
      s.color = col;
      yield return new WaitForSeconds(.02f);
    }
    Destroy(gameObject);
        GameController.GetGameController().gameType = GameController.GameType.BattleSetup;
        GameObject[] cardInfoText = GameObject.FindGameObjectsWithTag("CardInfo");
        GameObject[] cardInfoBox = GameObject.FindGameObjectsWithTag("CardInfoBox");
        cardInfoBox[0].GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        cardInfoBox[0].GetComponent<SpriteRenderer>().sortingOrder = 2;
        for (int i = 0; i < cardInfoText.Length; i++)
        {
            cardInfoText[i].GetComponent<Canvas>().sortingLayerName = "Default";
            cardInfoText[i].GetComponent<Canvas>().sortingOrder = 3;
        }
    }





}
