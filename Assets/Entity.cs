using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour {


    private void Update()
    {
        if (!isAnimating)
        {
            if (gameObject.GetComponent<SpriteRenderer>().enabled == false) gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (isDead && !isAnimating)
        {
            DestroyAll();
        }
    }

    protected bool isDead = false;
    protected bool isAnimating = false;

    new private GameObject gameObject;
    protected GameController controller;
    private Slider hpBar;
    private Text text;

    protected int maxHP = 100;
    public int hp;
    protected int cardsPerTurn = 5;
    protected int energyPerTurn = 1;
    protected int handLimit = 3;
    protected int currentEnergy;

    private List<CardState> allCards = new List<CardState>();
    private List<CardState> deck = new List<CardState>();
    public List<CardState> hand = new List<CardState>();
    private List<CardState> discards = new List<CardState>();
    private List<CardState> trash = new List<CardState>();
    protected List<Effect> effects;

    public virtual void DestroyAll()
    {

    }
    
    public void DieSoon()
    {
        isDead = true;
    }

    public void Awake () {
        gameObject = base.gameObject;
        controller = GameController.GetGameController();
        currentEnergy = energyPerTurn;
        hp = maxHP;
        effects = new List<Effect>();
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public void SetHealthBar(Slider hpBar, Text text) {
        this.hpBar = hpBar;
        this.text = text;
        RefreshHealthBar();
    }

    public void AddEffect(Effect e)
    {
        effects.Add(e);
    }

    public void ApplyEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].GetTurnNum() > 0)
            {
                if (effects[i].GetHeals() == true)
                {
                    Heal(effects[i].GetDamage());
                }
                else
                {
                    Damage(effects[i].GetDamage());
                }
                effects[i].DecrementTurn();
            }
            else
            {
                effects.RemoveAt(i);
            }
        }
    }

    // Do not call during a fight
    public void AddCard(CardState card) {
        allCards.Add(card);
    }

    public void AddToDeck(CardState card)
    {
        deck.Add(card);
    }

    public void SetCards(List<CardState> cards) {
        allCards = cards;
    }

    public void SetSprite(string s) {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(s);
    }

    public void BeginFight() {
        deck.Clear();
        discards.Clear();
        hand.Clear();
        trash.Clear();
        effects.Clear();
        foreach (CardState card in allCards) {
            deck.Add(card);
        }
        Shuffle(deck);
    }

    public virtual bool BeginTurn() {

        Card[] cards = GetComponents<Card>();
        int size = cards.Length;

        for (int i = 0; i < size; i++)
        {
            Destroy(cards[i].gameObject);
        }


        currentEnergy = energyPerTurn;
        ApplyEffects();
        for (int i = 0; i < cardsPerTurn; i++) {
            if (!DrawCard()) {
                return false;
            }
        }

        return true;
    }

    public virtual void EndTurn() {
        discards.AddRange(hand);
        hand.Clear();
    }

    public int GetMaxHP() {
        return maxHP;
    }

    public int GetHP() {
        return hp;
    }

    public bool HasCards() {
        return hand.Count != 0;
    }

    public void Heal(int amount) {
        hp = Math.Min(Math.Max(hp + amount, 0), maxHP);
        RefreshHealthBar();
        if (hp <= 0) {
            controller.Kill(this);
        }
    }

    private void RefreshHealthBar() {
        String message = hp + "/" + maxHP;
        text.text = message;
        hpBar.value = (float)hp / maxHP;
    }

    public void Damage(int amount) {
        Heal(-amount);
    }

    public int GetEnergy() {
        return currentEnergy;
    }

    public virtual void LoseEnergy(int n) {
        currentEnergy -= n;
    }

    public bool DrawCard() {
        if (deck.Count == 0) {
            if (discards.Count == 0) {
                return false;
            }
            foreach (CardState card in discards) {
                deck.Add(card);
            }
            discards.Clear();
            Shuffle(deck);
        }
        CardState drawCard = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        if (hand.Count == handLimit) {
            discards.Add(drawCard);
            return false;
        }
        hand.Add(drawCard);
        return true;
    }

    protected bool Discard(int i) {
        if (i >= hand.Count || i < 0) {
            return false;
        }
        discards.Add(hand[i]);
        hand.RemoveAt(i);
        return true;
    }

    public List<CardState> GetHand() {
        return hand;
    }

    public virtual int GetHandSize() {
        return hand.Count;
    }

    public List<CardState> GetDiscards() {
        return discards;
    }

    public bool Trash(CardState card) {
        if (hand.Remove(card) || deck.Remove(card) || discards.Remove(card)) {
            trash.Add(card);
            return true;
        }
        return false;
    }

    public bool Destroy(CardState card) {
        deck.Remove(card);
        hand.Remove(card);
        discards.Remove(card);
        trash.Remove(card);
        return allCards.Remove(card);
    }

    private static System.Random r = new System.Random();
    private static void Shuffle<T>(IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = r.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void Highlight() {
        if (gameObject.GetComponent<SpriteRenderer>().enabled == false) gameObject.GetComponent<SpriteRenderer>().enabled = true;

        if (GameObject.FindWithTag("Arrow")) {
            Destroy(GameObject.FindWithTag("Arrow"));
        }
        
        Vector3 currentEntityVector = GameController.GetGameController().GetHighlightedEntity().transform.position - new Vector3(0, 1.8f, 0f);
        GameObject g = Instantiate(Resources.Load("Arrow"), currentEntityVector, Quaternion.identity) as GameObject;
        g.tag = "Arrow";

        StartCoroutine(UpAndDown(g, currentEntityVector, currentEntityVector - new Vector3(0, 1f, 0), 50));

    }

    public static void UnHighlightAll() {
        if (GameObject.FindWithTag("Arrow")) {
            Destroy(GameObject.FindWithTag("Arrow"));
        }
    }

    private static IEnumerator UpAndDown(GameObject g, Vector3 start, Vector3 finish, int loops) {
        while (true) {
            if (g == null) break;
            for (int i = 0; i < loops; i++) {
                if (g == null) break;

                float f = 1 / (float)loops;
                //Debug.Log("Coroutine going");
                g.transform.position = Vector3.Lerp(start, finish, i * f);
                yield return null;
            }

            for (int i = 0; i < loops; i++) {
                if (g == null) break;

                float f = 1 / (float)loops;
                //Debug.Log("Coroutine going");
                g.transform.position = Vector3.Lerp(finish, start, i * f);
                yield return null;
            }
        }
    }
}