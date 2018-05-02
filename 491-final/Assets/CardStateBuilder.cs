using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardStateBuilder {

    CardState baseCard;

    List<TaskBuilder> taskBuilders;

    List<float> mean;
    List<float> stdDev;

    private static System.Random r;
    public static List<CardState> allCardState;
    public static List<CardState> allEnemyCardState;

    public static void AddToAll(CardState c){
      if(allCardState == null){
          allCardState = new List<CardState>();
          if(r == null) r = new System.Random();
        }
      allCardState.Add(c);
    }

    public static void AddToAllEnemy(CardState c){
      if(allEnemyCardState == null){
          allEnemyCardState = new List<CardState>();
          if(r == null) r = new System.Random();
        }
      allEnemyCardState.Add(c);
    }

    public static CardState GetRandomCard(){
      if(allCardState != null) return allCardState[r.Next(allCardState.Count)];
      else return null;
    }

    public static CardState GetRandomEnemyCard(){
      if(allEnemyCardState != null) return allEnemyCardState[r.Next(allCardState.Count)];
      else return null;
    }


    public CardStateBuilder(CardState baseCard, List<TaskBuilder> taskBuilders) {
        this.baseCard = baseCard;
        this.taskBuilders = taskBuilders;
    }

    public virtual CardState GetCardState(int level) {
        CardState c = new CardState(baseCard);
        List<Task> taskList = new List<Task>();
        for (int i = 0; i < taskBuilders.Count; i++) {
            taskList.Add(taskBuilders[i].GetTask(level));
        }
        c.SetTasks(taskList);

        return c;
    }
}

public class EnemyCardStateBuilder : CardStateBuilder {
    public EnemyCardStateBuilder(int level) : base(null, null) {

    }
}
