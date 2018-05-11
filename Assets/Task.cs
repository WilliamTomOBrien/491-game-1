using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task {

    public enum Input {
        Card,
        Entity,
        Enemy,
        Player,
        DiscardCard,
        Null
    };

    public Input type;
    public abstract void Run(GameObject input);
    public abstract Task Clone();
    public abstract void Set(float n);
}

public class DebugTask : Task {
    private int i;
    override public void Run(GameObject input){
        Debug.Log("Task " + i + " Run!");
    }

    public DebugTask(int i){
        type = Input.Null;
        this.i = i;
    }

    public override Task Clone() {
        return new DebugTask(i);
    }

    public override void Set(float n) {
    }
}

public class Example : Task {

    public Example() {
        type = Input.Null;
    }

    public override void Run(GameObject input) {
        Debug.Log("Task Run!");
    }

    public override Task Clone() {
        return new Example();
    }

    public override void Set(float n) {
    }
}

public class Damage : Task
{

    int damage;

    public Damage(int damage)
    {
        this.damage = damage;
        type = Input.Entity;
    }

    public override void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        p.Damage(damage);
    }

    public override Task Clone()
    {
        return new Damage(damage);
    }

    public override void Set(float n)
    {
        damage = (int)n;
    }
}

public class MultiDamage : Task
{

    int damage;
    int max;

    public MultiDamage(int damage, int max)
    {
        this.damage = damage;
        this.max = max;
        this.type = Input.Entity;
    }

    override public void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        GameController gc = GameController.GetGameController();
        int n = gc.r.Next(1, max);
        for (int i = 0; i < n; i++)
        {
            p.Damage(damage);
        }
    }

    public override Task Clone()
    {
        return new MultiDamage(damage, max);
    }

    public override void Set(float n)
    {
        this.damage = (int) n;
    }
}

public class Heal : Task
{
    public int amount = 0;

    public Heal(int amount) {
        this.amount = amount;
        type = Input.Entity;
    }

    public override void Run(GameObject input) {
        Entity p = input.GetComponent<Entity>();
        p.Heal(amount);
    }

    public override Task Clone() {
        return new Heal(amount);
    }

    public override void Set(float n) {
        amount = (int) n;
    }
}

public class Betray : Task
{
    int damage = 0;
    int heal = 0;

    public Betray(int damage, int heal)
    {
        this.damage = damage;
        this.type = Input.Null;
    }

    override public void Run(GameObject input)
    {
        //Entity p = input.GetComponent<Entity>();
        GameController gc = GameController.GetGameController();
        Entity currentPlayer = gc.currentPlayer;

        //Used to check for p1 and p2, then creates energy for the gui
        if (currentPlayer == gc.GetPlayers()[0])
        {
            //is p1
            gc.GetPlayers()[0].Heal(heal);
            gc.GetPlayers()[1].Damage(damage);
        }
        else
        {
            //is p2
            gc.GetPlayers()[0].Damage(damage);
            gc.GetPlayers()[0].GetComponent<Entity>().Heal(heal);
        }
    }

    public override Task Clone()
    {
        return new Betray(damage, heal);
    }

    public override void Set(float n)
    {
        damage = (int) n;
        heal = (int) n;
    }
}

public class DamageAll : Task
{
    int damage = 0;

    public DamageAll(int damage)
    {
        this.damage = damage;
        this.type = Input.Null;
    }

    override public void Set(float n){
      damage = (int) n;
    }

    override public void Run(GameObject input)
    {
        //Entity p = input.GetComponent<Entity>();
        GameController gc = GameController.GetGameController();
        Entity currentPlayer = gc.currentPlayer;

        //Used to check for p1 and p2, then creates energy for the gui
        if (currentPlayer.Equals(gc.GetPlayers()[0]))
        {
            //is p1
            for (int i = 1; i < gc.GetEntities().Count; i++)
            {
                gc.GetEntities()[i].Damage(damage);
            }
        }
        else
        {
            //is p2
            gc.GetPlayers()[0].Damage(damage);
            for (int i = 2; i < gc.GetEntities().Count; i++)
            {
                gc.GetEntities()[i].Damage(damage);
            }
        }
    }

    public override Task Clone()
    {
        return new DamageAll(damage);
    }

}

public class Nuke : Task
{
    int damage = 0;

    public Nuke(int damage)
    {
        this.damage = damage;
        this.type = Input.Null;
    }

    override public void Run(GameObject input)
    {
        //Entity p = input.GetComponent<Entity>();
        GameController gc = GameController.GetGameController();
        Entity currentPlayer = gc.currentPlayer;

        for (int i = 0; i < gc.GetEntities().Count; i++)
        {
            gc.GetEntities()[i].Damage(damage);
        }
    }

    public override Task Clone()
    {
        return new Nuke(damage);
    }

    public override void Set(float n)
    {
      damage = (int) n;
    }
}

public class InsertCard : Task
{
    CardState card;

    public InsertCard(CardState card)
    {
        this.type = Input.Entity;
        this.card = card;
    }

    override public void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        p.AddToDeck(card);

    }

    public override Task Clone()
    {
        return new InsertCard(card);
    }

    public override void Set(float n)
    {
    }
}

//IS THIS A BAD IDEA??
public class PermaInsertCard : Task
{
    CardState card;

    public PermaInsertCard(CardState card)
    {
        this.type = Input.Entity;
        this.card = card;
    }

    override public void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        p.AddCard(card);

    }

    public override Task Clone()
    {
        return new PermaInsertCard(card);
    }

    public override void Set(float n)
    {
    }
}

public class InsertCards : Task
{
    CardState card;
    int num;

    public InsertCards(CardState card, int num)
    {
        this.type = Input.Entity;
        this.card = card;
    }

    override public void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        for(int i=0; i<num; i++)
        {
            p.AddToDeck(card);
        }
    }

    public override Task Clone()
    {
        return new InsertCard(card);
    }

    public override void Set(float n)
    {
    }
}

public class PoisonEffect : Task
{

    int damage;
    int turnNum;

    public PoisonEffect(int damage, int turnNum)
    {
        this.damage = damage;
        this.turnNum = turnNum;
        this.type = Input.Entity;
    }

    override public void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        p.AddEffect(new Effect(damage, turnNum, false));
    }

    public override Task Clone()
    {
        return new PoisonEffect(damage, turnNum);
    }

    public override void Set(float n)
    {
      damage = (int) n;
    }
}

public class HealEffect : Task
{

    int damage;
    int turnNum;

    public HealEffect(int damage, int turnNum)
    {
        this.damage = damage;
        this.turnNum = turnNum;
        this.type = Input.Entity;
    }

    override public void Run(GameObject input)
    {
        Entity p = input.GetComponent<Entity>();
        p.AddEffect(new Effect(damage, turnNum, true));
    }

    public override Task Clone()
    {
        return new HealEffect(damage, turnNum);
    }

    public override void Set(float n)
    {
      damage = (int) n;
    }
}

public class HealBoth : Task
{
    int heal = 0;

    public HealBoth(int heal)
    {
        this.heal = heal;
        this.type = Input.Null;
    }

    override public void Run(GameObject input)
    {
        //Entity p = input.GetComponent<Entity>();
        GameController gc = GameController.GetGameController();
        gc.GetPlayers()[0].Heal(heal);
        gc.GetPlayers()[1].Heal(heal);
    }

    public override Task Clone()
    {
        return new HealBoth(heal);
    }

    public override void Set(float n)
    {
        heal = (int)n;
    }
}