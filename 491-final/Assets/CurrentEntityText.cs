//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CurrentEntityText : MonoBehaviour {
//	public Text text;
//	Entity currentEntity;
//	Player currentPlayer;
//	Enemy currentEnemy;
//	string previousText;
//	string s;
//    public Slider healthBar;

//	// Use this for initialization
//	void Start () {
//		previousText = "";
//		s = "";
//		currentEntity = null;
//		currentPlayer = null;
//		currentEnemy = null;

//		text.text = "hello fame";

//        healthBar.value = CalculateHealth();
//	}
	
//	// Update is called once per frame
//	void Update () {
//        float percent;
//		currentEntity = GameObject.FindWithTag("CurrentEntity").GetComponent<Entity>();
//		if(currentEntity != null && currentEntity is Player) {
//			currentPlayer = (Player) currentEntity;
//			s = "Player Information:\n";
//			s += "HP: " + currentPlayer.hp + "/" + currentPlayer.maxHP +"\n";
//			s += "Energy: " + currentPlayer.currentEnergy + "/" + currentPlayer.energyPerTurn;

//            percent = (float)currentPlayer.hp / (float)currentPlayer.maxHP;
//            //percent = 0.5F;
//            healthBar.value = percent;
//            s += "\nHealthBar% " + percent;
//            if (text.text != s) text.text = s;
//		} else if(currentEntity != null && currentEntity is Enemy) {
//			currentEnemy = (Enemy) currentEntity;
//			s = "Enemy Information:\n";
//			s += "HP: " + currentEnemy.hp + "/" + currentEnemy.maxHP +"\n";

//            percent = (float)currentEnemy.hp / (float)currentEnemy.maxHP;
//            healthBar.value = percent;
//            s += "\nHealthBar% " + percent;
//            if (text.text != s) text.text = s;
//		} else {
//            //percent = 0;
//            //healthBar.value = percent;
//            text.text = "";
//		}
//        //healthBar.value = CalculateHealth();
//	}

//    float CalculateHealth()
//    {
//        float percent = 0;
//        currentEntity = GameObject.FindWithTag("CurrentEntity").GetComponent<Entity>();
//        if (currentEntity != null && currentEntity is Player)
//        {
//            currentPlayer = (Player)currentEntity;
//            percent = currentPlayer.hp / currentPlayer.maxHP;
//        }
//        else if (currentEntity != null && currentEntity is Enemy)
//        {
//            currentEnemy = (Enemy)currentEntity;
//            percent = currentEnemy.hp / currentEnemy.maxHP;
//        }
//        return percent;
//    }
//}
