using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity {

    public List<GameObject> handObjects;
    private List<GameObject> offHand;
    private List<GameObject> energyObjects;
    public float r = 4f;
    private bool player1 = false;
    
    public void RemoveFromHand(GameObject card)
    {
        card.tag = "Untagged";
        handObjects.Remove(card);
        hand.Remove(card.GetComponent<Card>().GetState());
        offHand.Add(card);
    }

    public override int GetHandSize()
    {
        return handObjects.Count;
    }

    public int GetNumCards()
    {
        return handObjects.Count;
    }

    public new void Awake() {

        base.Awake();

        offHand = new List<GameObject>();

        

        cardsPerTurn = 5;
        handLimit = 5;
        energyPerTurn = 4;
        currentEnergy = 4;

        handObjects = new List<GameObject>();
        energyObjects = new List<GameObject>();

        energyPerTurn = 4;
        currentEnergy = energyPerTurn;

        Task ex = new Damage(10);
        List<Task> li = new List<Task> {
            ex
        };

        CardState baseCard = new CardState("example", 1, li, "Sounds/PunchSound", "Sprites/CardIcons/Fist", "Enemy attack");
        List<float> means = new List<float> {
            9f,
            10f,
            11f
        };
        List<float> stdDev = new List<float> {
            2.5f,
            3.75f,
            1f
        };
        TaskBuilder strike = new TaskBuilder(new Damage(0), means, stdDev);
        List<TaskBuilder> tasks = new List<TaskBuilder> {
            strike
        };
        CardStateBuilder stateBuilder = new CardStateBuilder(new StrikeState(), tasks);

        //CardPile.Add(stateBuilder);

        for (int i = 0; i < 12; i++) {
            //AddCard(stateBuilder.GetCardState(1));
            AddCard(new StrikeState());
            //AddCard(new PoisonState());
            //AddCard(new BetrayState());
            //AddCard(new HealState());
            //AddCard(new NukeState());
            //AddCard(new LeechState());
            //AddCard(new DumpState());  
            //AddCard(new LootBoxState());
            //AddCard(new PermaLootBoxState());
            //AddCard(new ImprovedStrikeState());
            //AddCard(new ImprovedHealState());
            //AddCard(new InvestStrikeState());
            //AddCard(new InvestHealState());
        }

        for(int i = 0; i < 3; i++)
        {
            AddCard(new HealState());
            AddCard(new ImprovedStrikeState());
        }
    }

    public override void DestroyAll()
    {
        Debug.Log("Player destroy all");

        base.DestroyAll();
        int size = handObjects.Count;
        for (int i = 0; i < size; i++)
        {
            Destroy(handObjects[0]);
            handObjects.Remove(handObjects[0]);
        }
        size = hand.Count;
        for(int i = 0; i < size; i++)
        {
            hand.Remove(hand[0]);
        }

        int numOffCards = offHand.Count;
        for (int i = 0; i < numOffCards; i++)
        {
            //Discard(0);
            Destroy(offHand[0]);
            offHand.RemoveAt(0);
        }



        size = energyObjects.Count;
        for (int i = 0; i < size; i++)
        {
            Destroy(energyObjects[0]);
            energyObjects.Remove(energyObjects[0]);
        }

        Destroy(gameObject);

    }

    public void SetAnimating(bool b)
    {
        isAnimating = b;
    }

    public IEnumerator ToBeDestroyed()
    {
        int numCards1 = handObjects.Count;
        for (int i = 0; i < numCards1; i++)
        {
            handObjects[i].GetComponent<SpriteRenderer>().enabled = false;
            handObjects[i].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            //Destroy(handObjects[0]);
            //handObjects.RemoveAt(0);
        }


        while (isAnimating)
        {
            Debug.Log("is animating");
            yield return null;
        }
        int numCards = handObjects.Count;
        for (int i = 0; i < numCards; i++)
        {
            //Discard(0);
            Destroy(handObjects[0]);
            handObjects.RemoveAt(0);
        }

        int numoffcards = offHand.Count;
        for (int i = 0; i < numoffcards; i++)
        {
            //discard(0);
            Destroy(offHand[0]);
            offHand.RemoveAt(0);
        }


        Card[] cards = GetComponents<Card>();
        int size = cards.Length;

        for (int i = 0; i < size; i++)
        {
            Destroy(cards[i].gameObject);
        }


        int sizeEnergy = energyObjects.Count;
        for (int i = 0; i < sizeEnergy; i++)
        {
            if (energyObjects[0] != null)
            {
                Destroy(energyObjects[0]);
            }
            energyObjects.Remove(energyObjects[0]);
        }
    }

    override public bool BeginTurn() {
        bool result = base.BeginTurn();
        int sizeObjects = handObjects.Count;
        
        for(int i = 0; i < sizeObjects; i++)
        {
            if(handObjects[0] != null)
            {
                Destroy(handObjects[0]);
            }
            handObjects.Remove(handObjects[0]);
        }

        int sizeEnergy = energyObjects.Count;
        for(int i = 0; i < sizeEnergy; i++)
        {
            if(energyObjects[0] != null)
            {
                Destroy(energyObjects[0]);
            }
            energyObjects.Remove(energyObjects[0]);
        }

        List<CardState> hand = GetHand();
        int numCards = hand.Count;
        for (int i = 0; i < numCards; i++) {
            handObjects.Add(Instantiate(Resources.Load("Card"), new Vector2((float) (r*Math.Sin((Math.PI/180)*i*90/(numCards - 1) - (Math.PI/180)*45)), 
            (float) (-6f + r*Math.Cos((Math.PI/180)*i*90/(numCards - 1) - (Math.PI/180)*45))), Quaternion.identity) as GameObject);
            handObjects[i].GetComponent<Card>().SetState(hand[i]);
        }
        float y = -4F;
        if (player1) {
            y = -2.3F;
        }
        
        for (int i = 0; i < GetEnergy(); i++) {
            energyObjects.Add(Instantiate(GameController.GetGameController().energy, new Vector2( -5.75f + (i * -.5f), y), Quaternion.identity));
        }
        return result;
    }

    override public void EndTurn() {
        StartCoroutine(ToBeDestroyed());
        //int numCards = handObjects.Count;
        //for (int i = 0; i < numCards; i++) {
        //    //Discard(0);
        //    Destroy(handObjects[0]);
        //    handObjects.RemoveAt(0);
        //}

        //Card[] cards = GetComponents<Card>();
        //int size = cards.Length;

        //for(int i = 0; i < size; i++)
        //{
        //    Destroy(cards[i].gameObject);
        //}

        //int sizeEnergy = energyObjects.Count;
        //for (int i = 0; i < sizeEnergy; i++)
        //{
        //    if (energyObjects[0] != null)
        //    {
        //        Destroy(energyObjects[0]);
        //    }
        //    energyObjects.Remove(energyObjects[0]);
        //}
    }

    public bool Discard(GameObject card) {
        int i = handObjects.IndexOf(card);
        if (!Discard(i)) {
            return false;
        }
        Destroy(card);
        handObjects.Remove(card);
        return true;
    }

    public void SetPlayerOne(bool b) {
        player1 = b;
    }

    public void ActivateCard(int i) {
        GetCard(i).ActivateCard();
    }

    public void HighlightCard(int i) {
        GetCard(i).Highlight();
    }

    public void UnHighlightCard(int i) {
        GetCard(i).UnHighlight();
    }

    public bool NoCardHighlighted() {
        for(int i = 0; i < handObjects.Count; i++){
            if(GetCard(i).IsHighlighted()){
                return false;
            }
        }
        return true;
    }

    public GameObject GetCardObject(int i) {
        return handObjects[i];
    }

    public Card GetCard(int i) {
        return handObjects[i].GetComponent<Card>();
    }

    override public void LoseEnergy(int n) {
        base.LoseEnergy(n);
        int energy = GetEnergy();
        int numObjects = energyObjects.Count;
        while (numObjects > energy) {
            Destroy(energyObjects[numObjects - 1]);
            energyObjects.RemoveAt(numObjects - 1);
            numObjects--;
        }
    }

    public void UnSelectAll(){
        foreach (GameObject o in handObjects) {
            o.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/FistStatic");
        }
    }

    public void HighlightCardSelect(int i) {
        GetCard(i).HighlightSelect();
    }

    public bool CardIsHighlighted(int i) {
        return GetCard(i).IsHighlighted();
    }

    public void OrganizeCards() {
        int numCards = handObjects.Count;
        float freedom = numCards * 90f / 5f;
        float bound = (180f - freedom) / 2;
        if (numCards > 1) {
            for (int i = 0; i < numCards; i++) {
                if (handObjects.Count != numCards) break;
                handObjects[i].transform.position = new Vector3((float)(r * Math.Cos((Math.PI / 180) * (numCards - 1 - i) * freedom / (numCards - 1) + (Math.PI / 180) * bound)),
                (float)(-6f + r * Math.Sin((Math.PI / 180) * (numCards - 1 - i) * freedom / (numCards - 1) + (Math.PI / 180) * bound)), 0);
            }
        } else if (numCards == 1) {
            handObjects[0].transform.position = new Vector3(0, -2f, 0);
        }
    }
}