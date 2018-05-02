using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    int totalScore = 0;

public Entity currentEntity;
public Player currentPlayer;
public Enemy currentEnemy;

private List<Entity> entities;
private GameObject input;
private Entity highlightedEntity;
private EnemyBuilder enemyBuilder;

public CardPile cardPile;

//used for health and energy graphics
public Slider p1HealthBar;
public Slider p2HealthBar;
public Slider e1HealthBar;
public Slider e2HealthBar;

public Text p1HealthText;
public Text p2HealthText;
public Text e1HealthText;
public Text e2HealthText;

public GameObject energy;
public List<GameObject> p1Energy;
public List<GameObject> p2Energy;

private System.Random rand;
List<CardStateBuilder> AllCardStates;
public enum GameType {
        Battle,
        BattleSetup,
        PickACard,
        NullState,
        InitialMenu,
        LoseMenu
};
public GameType gameType;

private bool runningTasks = true;
private int numCardsHighlighted = 0;
private int entityIndex;
private int selectionIndex;
private int lastEnemySelectedIndex = -1;
private const KeyCode UNUSED_KEY = KeyCode.A;
public KeyCode prevCode = UNUSED_KEY;

public enum SelectionType {
        SelectCardToPlay,
        SelectCard,
        CardPileSelect,
        SelectEntity
};


Menu menu;
public SelectionType selectionType;

void Awake () {

        // **************** NEW CODE *******************************
        AllCardStates = new List<CardStateBuilder>();
        rand = new System.Random();
        // *********************************************************

        gameType = GameType.InitialMenu;

        Quaternion q = Quaternion.identity;
        selectionType = SelectionType.SelectCardToPlay;

        List<GameObject> gameObjectEntities = new List<GameObject> {
                InstantiateEntity("Player", -8.1F, 1),
                InstantiateEntity("Player", -5.2F, 1),
        };

        if(entities != null && entities.Count != 0){
            int size = entities.Count;
            for (int i = 0; i < size; i++) Kill(entities[0]);
        }
        entities = new List<Entity>();
        foreach (GameObject o in gameObjectEntities) {
                entities.Add(o.GetComponent<Entity>());
        }

        CardStateBuilder.AddToAll(new StrikeState());
        CardStateBuilder.AddToAll(new PoisonState());
        CardStateBuilder.AddToAll(new BetrayState());
        CardStateBuilder.AddToAll(new HealState());
        CardStateBuilder.AddToAll(new NukeState());
        CardStateBuilder.AddToAll(new DumpState());
        CardStateBuilder.AddToAll(new HealEffectState());
        CardStateBuilder.AddToAll(new DamageAllState());
        CardStateBuilder.AddToAll(new MultiStrikeState());

        CardStateBuilder.AddToAllEnemy(new StrikeState());
        CardStateBuilder.AddToAllEnemy(new PoisonState());
        CardStateBuilder.AddToAllEnemy(new NukeState());
        CardStateBuilder.AddToAllEnemy(new DumpState());
        CardStateBuilder.AddToAllEnemy(new MultiStrikeState());



        menu = Menu.InitialMenu().GetComponent<Menu>();
}

public void AddToScore(int n) {
        totalScore += n;
}

public static GameObject InstantiateEntity(string name, float x, float y) {
        return Instantiate(Resources.Load(name), new Vector2(x, y), Quaternion.identity) as GameObject;
}

// Use this for initialization
void Start () {

        ((Player)entities[0]).SetPlayerOne(true);
        ((Player)entities[1]).SetPlayerOne(false);
        entities[0].SetHealthBar(p1HealthBar, p1HealthText);
        entities[1].SetHealthBar(p2HealthBar, p2HealthText);

        // **************** NEW CODE *******************************
        Task ex = new Damage(10);
        List<Task> li = new List<Task> {
                ex
        };
        for (int i = 0; i < 6; i++) {
                List<float> means = new List<float> {
                        9f + i / 6f,
                        10f + i / 6f,
                        11f + i / 6f
                };
                List<float> stdDev = new List<float> {
                        2.5f + i / 20f,
                        3.75f + i / 20f,
                        1f + i / 20f
                };
                TaskBuilder strike = new TaskBuilder(new Damage(0), means, stdDev);
                List<TaskBuilder> tasks = new List<TaskBuilder> {
                        strike
                };
                CardStateBuilder stateBuilder = new CardStateBuilder(new StrikeState(), tasks);
                CardPile.AddToAll(stateBuilder);
                AllCardStates.Add(stateBuilder);

                enemyBuilder = EnemyBuilder.GetEnemyBuilder();
        }
        // *********************************************************
}

// Update is called once per frame
void Update() {
  //Debug.Log(gameType);
        if(gameType == GameType.InitialMenu) {
                KeyCode key = GetKey();
                if (prevCode == UNUSED_KEY) {
                        switch (key) {
                        case KeyCode.LeftArrow:
                                menu.Left();
                                break;

                        case KeyCode.RightArrow:
                                menu.Right();
                                break;

                        case KeyCode.Return:
                                if (menu.Select() == "Start Game") {
                                       // Debug.Log("This should run once start game");
                                        //gameType = GameType.BattleSetup;
                                        menu.DestroyAll();
                                }
                                break;
                        }
                }
                prevCode = key;
            //Debug.Log(gameType + " here");

        }
        else if (gameType == GameType.Battle) {

                if(GetPlayers().Count != 2) {
                         menu = Menu.DeathMenu(totalScore).GetComponent<Menu>();
                         gameType = GameType.LoseMenu;
                         int size = entities.Count;
                         for(int i = 0; i < size; i++){
                            Kill(entities[0]);
                         }
                } else if (entities.Count < 3)
                {
                 AddToScore(30);
                    currentPlayer.EndTurn();
                    gameType = GameType.PickACard;
                    cardPile = CardPile.MakeCardPile(3);
                    menu = Menu.PickACardMenu().GetComponent<Menu>();
                  }

            if (currentEntity is Player) {
                        currentPlayer = (Player)currentEntity;
                        currentPlayer.OrganizeCards();

                        // If the player should select a card
                        if ((selectionType == SelectionType.SelectCardToPlay || selectionType == SelectionType.SelectCard) && currentPlayer.HasCards()) {

                                int numCards = currentPlayer.GetHandSize();
                                if (numCardsHighlighted == 0 && numCards > 0) {
                                        
                                        currentPlayer.HighlightCard(0);
                                        numCardsHighlighted++;
                                        selectionIndex = 0;
                                }

                                KeyCode key = GetKey();
                                if (prevCode == UNUSED_KEY) {
                                        switch (key) {
                                        case KeyCode.LeftArrow:
                                                CardSelect(-1, numCards + 1);
                                                break;

                                        case KeyCode.RightArrow:
                                                CardSelect(1, numCards + 1);
                                                break;

                                        case KeyCode.Return:
                                                if (selectionType == SelectionType.SelectCardToPlay) {
                                                        if(selectionIndex != numCards) {
                                                            CardState c = currentPlayer.GetCard(selectionIndex).GetComponent<Card>().GetState();
                                                            if (c.GetCost() <= currentPlayer.GetEnergy())
                                                             {
                                                                 currentPlayer.ActivateCard(selectionIndex);
                                                                 numCardsHighlighted--;
                                                             }

                                                        } else { 
                                                               GameObject.FindWithTag("EndTurnButton").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/button");

                                                               numCardsHighlighted = 0;
                                                               NextTurn();

                                                        }
                                                } else {
                                                        input = currentPlayer.GetCardObject(selectionIndex);
                                                        currentPlayer.UnSelectAll();
                                                }
                                                break;
                                        }
                                }
                                prevCode = key;

                                // If The player is out of energy or cards and there are no running tasks
                                if (selectionType == SelectionType.SelectCardToPlay && (currentPlayer.GetEnergy() <= 0 || numCards == 0) && !runningTasks) {
                                        numCardsHighlighted = 0;
                                        NextTurn();
                                }

                                // If the Player should select an entity
                        } else if (selectionType == SelectionType.SelectEntity) {

                                int numEntities = entities.Count;

                                if (NoEntitiesHighlighted()) {
                                        int enemyIndex;
                                        if (lastEnemySelectedIndex == -1 || lastEnemySelectedIndex >= numEntities - 2) {
                                                enemyIndex = 0;
                                                for (int i = 0; i < numEntities; i++) {
                                                        if (entities[i] is Enemy) {
                                                                enemyIndex = i;
                                                                break;
                                                        }
                                                }
                                        } else {
                                                enemyIndex = lastEnemySelectedIndex;
                                        }
                                        HighlightEntity(enemyIndex);
                                        selectionIndex = enemyIndex;
                                }

                                KeyCode key = GetKey();
                                if (prevCode == UNUSED_KEY) {
                                        switch (key) {
                                        case KeyCode.LeftArrow:
                                                selectionIndex = (selectionIndex - 1 + numEntities) % numEntities;
                                                HighlightEntity(selectionIndex);
                                                break;

                                        case KeyCode.RightArrow:
                                                selectionIndex = (selectionIndex + 1) % numEntities;
                                                HighlightEntity(selectionIndex);
                                                break;

                                        case KeyCode.Return:
                                                input = entities[selectionIndex].GetGameObject();
                                                Entity.UnHighlightAll();
                                                lastEnemySelectedIndex = selectionIndex;
                                                selectionIndex = 0;
                                                break;
                                        }
                                }
                                prevCode = key;

                                // If the player should select from the card pile
                        } else if (selectionType == SelectionType.CardPileSelect) {
                                cardPile = GameObject.FindWithTag("CardPile").GetComponent<CardPile>();

                                KeyCode key = GetKey();
                                if (prevCode == UNUSED_KEY) {
                                        switch (key) {
                                        case KeyCode.LeftArrow:
                                                cardPile.Left();
                                                break;

                                        case KeyCode.RightArrow:
                                                cardPile.Right();
                                                break;

                                        case KeyCode.Return:
                                                input = cardPile.GetSelected();
                                                break;
                                        }
                                }
                                prevCode = key;

                        }
                } else if (currentEntity is Enemy) {
                        currentEnemy = (Enemy)currentEntity;
                        currentEnemy.ActivateEffect();
                        NextTurn();
                }
          //  Debug.Log(gameType + " here");

        }
        else if (gameType == GameType.PickACard) {
           // Debug.Log(gameType + " here");

            cardPile = GameObject.FindWithTag("CardPile").GetComponent<CardPile>();

                KeyCode key = GetKey();
                if (prevCode == UNUSED_KEY) {
                        switch (key) {
                        case KeyCode.LeftArrow:
                                cardPile.Left();
                                break;

                        case KeyCode.RightArrow:
                                cardPile.Right();
                                break;

                        case KeyCode.Return:
                                (entities[rand.Next(2)]).AddCard(cardPile.GetSelected().GetComponent<Card>().GetState());
                                cardPile.DestroyAll();
                                menu.DestroyAll();
                                gameType = GameType.BattleSetup;
                                break;
                        }
                }
                prevCode = key;
           // Debug.Log(gameType + " here");

        }
        else if (gameType == GameType.BattleSetup) {
            Debug.Log("top of battle setup");
                //Debug.Log("This should only come up once");
                // Create up to 2 enemies
                float[] positions = { 4.8F, 8.5F };
                int count = entities.Count;
                for (int i = 0; i < (4 - count); i++) {
                        //Debug.Log("This should run twice");
                        List<CardStateBuilder> c = new List<CardStateBuilder>();
                        for (int j = 0; j < 6; j++) {
                                c.Add(AllCardStates[rand.Next(AllCardStates.Count)]);
                        }
                        enemyBuilder.SetState(1, c);
                        entities.Add(enemyBuilder.CreateEnemy("Enemy", positions[i], 1));
                }
                entities[2].SetHealthBar(e1HealthBar, e1HealthText);
                entities[3].SetHealthBar(e2HealthBar, e2HealthText);

                BeginFight();
                gameType = GameType.Battle;
           // Debug.Log(gameType + " here");
        }
        else if (gameType == GameType.LoseMenu)
        {
            totalScore = 0;
            KeyCode key = GetKey();
            if (prevCode == UNUSED_KEY)
            {
                switch (key)
                {
                    case KeyCode.LeftArrow:
                        menu.Left();
                        break;

                    case KeyCode.RightArrow:
                        menu.Right();
                        break;

                    case KeyCode.Return:
                        if (menu.Select() == "Play Again?")
                        {
                            // Debug.Log("This should run once start game");
                            gameType = GameType.BattleSetup;
                            menu.DestroyAll();
                            Awake();
                            Start();
                        }
                        break;
                }
            }
            prevCode = key;
            //Debug.Log(gameType + " here");

        }

        //Debug.Log("end of update");
}

private void NextTurn() {
        currentEntity.EndTurn();
        entityIndex = (entityIndex + 1) % entities.Count;
        currentEntity = entities[entityIndex];
        currentEntity.BeginTurn();
}

public void Kill(Entity e) {
        AddToScore(15);
        e.DestroyAll();
        entities.Remove(e);
        Destroy(e.GetGameObject());
}

public void CardSelect(int n, int numCards) {
        if (selectionIndex != numCards - 1) currentPlayer.UnHighlightCard(selectionIndex);
        else {
            GameObject.FindWithTag("EndTurnButton").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/button");
        }
        selectionIndex = (selectionIndex + n + numCards) % numCards;
        if(selectionIndex != numCards - 1) currentPlayer.HighlightCard(selectionIndex);
        else {
            GameObject.FindWithTag("EndTurnButton").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/button2");
        }
    }

public GameObject GetInput() {
        return input;
}

public void SetInput(GameObject o) {
        input = o;
}

public List<Player> GetPlayers() {
        List<Player> players = new List<Player>();
        foreach (Entity e in entities) {
                if (e is Player) {
                        players.Add((Player)e);
                }
        }
        return players;
}

public List<Entity> GetEntities()
{
        return entities;
}

private void BeginFight() {
        entityIndex = 0;
        currentEntity = entities[0];
        for (int i = 0; i < entities.Count; i++) {
                entities[i].BeginFight();
        }
        currentEntity.BeginTurn();
}

private void HighlightEntity(int n) {
        highlightedEntity = entities[n];
        highlightedEntity.Highlight();
}

private bool NoEntitiesHighlighted() {
        return !GameObject.FindWithTag("Arrow");
}

public void SetSelectionType(SelectionType s) {
        selectionType = s;
}

public void RunningTasks(bool b){
        runningTasks = b;
}

public static GameController GetGameController() {
        return GameObject.FindWithTag("MainCamera").GetComponent<GameController>();
}

public static KeyCode GetKey() {
        if (Input.GetKey(KeyCode.Return)) return KeyCode.Return;
        if (Input.GetKey(KeyCode.LeftArrow)) return KeyCode.LeftArrow;
        if (Input.GetKey(KeyCode.RightArrow)) return KeyCode.RightArrow;
        return UNUSED_KEY;
}

public Entity GetHighlightedEntity() {
        return highlightedEntity;
}
}
