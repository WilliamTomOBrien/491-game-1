using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardState {

    private string name;
    private int cost;
    private List<Task> tasks;
    private string audioPath;
    private string spritePath;
    private string info;
    public AudioClip soundEffect;
    public Sprite icon;
    

    public CardState(string name, int cost, List<Task> tasks, string audioPath, string spritePath, string info){
        this.name = name;
        this.cost = cost;
        this.tasks = tasks;
        this.audioPath = audioPath;
        this.spritePath = spritePath;
        this.info = info;
        soundEffect = Resources.Load<AudioClip>(audioPath);
        icon = Resources.Load<Sprite>(spritePath);
    }

    public CardState(CardState c) : this(c.name, c.cost, c.tasks, c.audioPath, c.spritePath, c.info) {
    }

    public void SetTasks(List<Task> tasks) {
        this.tasks = tasks;
    }

    public List<Task> GetTasks() {
        return tasks;
    }

    public int GetCost() {
        return cost;
    }

    public AudioClip GetSoundEffect()
    {
        return soundEffect;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public void RunTask(GameObject input, int i){
        tasks[i].Run(input);
    }

    override
    public string ToString() {
        return name;
    }

    public virtual IEnumerator Coroutine(GameObject g){
        yield return null;
    }
}

public class StrikeState : CardState {
    private static string NAME = "Strike";
    private static int COST = 1;

    public StrikeState() : base(NAME, COST, MyTasks(), "Sounds/PunchSound", "Sprites/CardIcons/FistStatic", "Deals 10 damage") {
    }

    private static List<Task> MyTasks() {
        return new List<Task> {
            new Damage(10)
        };
    }
}

public class HealState : CardState
{
    private static string NAME = "Heal";
    private static int COST = 1;

    public HealState() : base(NAME, COST, MyTasks(), "Sounds/HealSound", "Sprites/CardIcons/Heart", "Heals an entity 5 hp")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
            new Heal(5)
        };
    }
}

public class UnstableState : CardState {
    private static string NAME = "Unstable State";
    private static int COST = 1;

    public UnstableState() : base(NAME, COST, MyTasks(), "Sounds/PunchSound", "Sprites/CardIcons/FistStatic", "Deals 20 damage to an entity") {
    }

    private static List<Task> MyTasks() {
        return new List<Task> {
            new Damage(20)
        };
    }
}
//NOT WORKING YET
public class TrashState : CardState
{
    private static string NAME = "Trash";
    private static int COST = 1;

    public TrashState() : base(NAME, COST, MyTasks(), "Sounds/PunchSound", "Sprites/CardIcons/Null", "Does nothing of interest")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
        };
    }
}

public class DumpState : CardState
{
    private static string NAME = "Trash Dump";
    private static int COST = 4;

    public DumpState() : base(NAME, COST, MyTasks(), "Sounds/PunchSound", "Sprites/CardIcons/Heart", "Adds a card that does nothing into an entity's deck for the battle")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
            new InsertCard(new TrashState())
        };
    }
}

public class BetrayState : CardState
{
    private static string NAME = "Betrayal";
    private static int COST = 2;

    public BetrayState() : base(NAME, COST, MyTasks(), "Sounds/BetraySound", "Sprites/CardIcons/Betrayal", "Deals 15 damage to your partner and heals you 20 hp")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new Betray(10, 20)
        };
    }
}

public class PoisonState : CardState
{
    private static string NAME = "Poison";
    private static int COST = 2;

    public PoisonState() : base(NAME, COST, MyTasks(), "Sounds/PoisonSound", "Sprites/CardIcons/Skull", "Deals 5 damage to an entity each turn for 3 turns")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new PoisonEffect(10, 3)
        };
    }
}

public class HealEffectState : CardState
{
    private static string NAME = "Regenerate";
    private static int COST = 3;

    public HealEffectState() : base(NAME, COST, MyTasks(), "Sounds/HealEffectSound", "Sprites/CardIcons/HeartPlus", "Heals a player 5 hp a turn for 3 turns")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new HealEffect(5, 3)
        };
    }
}

public class LeechState : CardState
{
    private static string NAME = "Leech";
    private static int COST = 3;

    public LeechState() : base(NAME, COST, MyTasks(), "Sounds/PoisonSound", "Sprites/CardIcons/LifeSteal", "Steals 5 health from an entity a turn for 3 turns and gives it to you")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new PoisonEffect(5, 3),
           new HealEffect(5, 3)
        };
    }
}

public class DamageAllState : CardState
{
    private static string NAME = "Flail";
    private static int COST = 2;

    public DamageAllState() : base(NAME, COST, MyTasks(), "Sounds/HealEffectSound", "Sprites/CardIcons/MultiFist", "Deals 15 damage to every entity but you")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new DamageAll(15)
        };
    }
}

public class NukeState : CardState
{
    private static string NAME = "Rampage";
    private static int COST = 3;

    public NukeState() : base(NAME, COST, MyTasks(), "Sounds/HealEffectSound", "Sprites/CardIcons/Nuke", "Deals 25 damage to all entities")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new Nuke(25)
        };
    }
}

public class MultiStrikeState : CardState
{
    private static string NAME = "Multi Strike";
    private static int COST = 2;

    public MultiStrikeState() : base(NAME, COST, MyTasks(), "Sounds/PunchSound", "Sprites/CardIcons/MultiFist", "Deals anywhere from 10 to 40 damage to an entity")
    {
    }

    private static List<Task> MyTasks()
    {
        return new List<Task> {
           new MultiDamage(10, 4)
        };
    }
}