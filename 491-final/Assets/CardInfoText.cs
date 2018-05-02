using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoText : MonoBehaviour
{
    public Text info;
    Entity currentEntity;
    Player currentPlayer;
    Card selectedCard;
    Enemy currentEnemy;
    public string s;

    // Use this for initialization
    void Start()
    {
        info = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject temp = GameObject.FindWithTag("SelectedCard");
        if (temp != null)
        {
            selectedCard = temp.GetComponent<Card>();
            s = "Name: " + selectedCard.GetState().ToString() + "\n";
            s += "Required Energy: " + selectedCard.GetState().GetCost();
            info.text = s;
        }
        else
        {
            info.text = "null";
        }
    }
}
