using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

  private GameController controller;
  private List<Text> selectableText;
  private List<Text> normalText;


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


      gameObject.transform.GetChild(0).GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
  }


  public void AddText(Vector3 position, string text){
    Vector3 p = position + new Vector3(10.3f, 6.3f, 100f);
    GameObject g = Instantiate(Resources.Load("Text"), p, Quaternion.identity) as GameObject;

    g.transform.SetParent(gameObject.transform.GetChild(0).transform, false);

    Text t = g.GetComponent<Text>();//g.GetComponent<Text>();
    t.text = text;
    t.alignment = TextAnchor.MiddleCenter;
    t.fontSize = 1;

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

  //  t.transform.position = p;


    if(selectableText.Count == 0){
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
    menu.AddSelectableText(new Vector3(0,0,0), "Start Game");

    return g;
  }

  public static GameObject PickACardMenu(){
    GameObject g = Instantiate(Resources.Load("Menu"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
    Menu menu = g.GetComponent<Menu>();
    menu.AddText(new Vector3(0,1,0), "Select a Card to add to a Player's Deck");

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

    }





}
