using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestController : MonoBehaviour {

    GameObject cardPileObject;
    CardPile cardPile;
    static KeyCode unusedKey = KeyCode.A;
    KeyCode prevCode = unusedKey;


	void Awake () {
        cardPileObject = Instantiate(Resources.Load("Card_Select"), new Vector2(0, 0), Quaternion.identity) as GameObject;
        cardPile = cardPileObject.GetComponent<CardPile>();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
                KeyCode key = GetKey();
                if (prevCode == unusedKey) {
                    switch (key) {
                        case KeyCode.LeftArrow:
                            cardPile.Left();
                            break;

                        case KeyCode.RightArrow:
                            cardPile.Right();
                            break;

                        case KeyCode.Return:
                            cardPile.Select();
                            break;
                    }
                }
                prevCode = key;
	}



    public static TestController GetGameController() {
        return GameObject.FindWithTag("MainCamera").GetComponent<TestController>();
    }

    public static KeyCode GetKey() {
        if (Input.GetKey(KeyCode.Return)) return KeyCode.Return;
        if (Input.GetKey(KeyCode.LeftArrow)) return KeyCode.LeftArrow;
        if (Input.GetKey(KeyCode.RightArrow)) return KeyCode.RightArrow;
        return unusedKey;
    }
}
