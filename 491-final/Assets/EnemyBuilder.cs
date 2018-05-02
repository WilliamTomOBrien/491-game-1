using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyBuilder {

    private int level;

    private System.Random r;
    private List<CardStateBuilder> cardBuilders;
    private List<string> spritePaths;

    private List<float> mean;
    private List<float> stdDev;

    private static EnemyBuilder enemyBuilder;
    public static EnemyBuilder GetEnemyBuilder() {
        if (enemyBuilder == null) {
            enemyBuilder = new EnemyBuilder();
        }
        return enemyBuilder;
    }

    private EnemyBuilder() {
        spritePaths = new List<string> {
            "Sprites/enemies/bee_512px",
            "Sprites/enemies/derp_512px",
            "Sprites/enemies/gremlin_512px",
            "Sprites/enemies/ice_slime_512px",
            "Sprites/enemies/pizza_512px",
            "Sprites/enemies/slimy_512px",
            "Sprites/enemies/small_pink_slime_512px",
            "Sprites/enemies/snowman_512px",
            "Sprites/enemies/staff_512px",
            "Sprites/enemies/witch_hat_512px",
            "Sprites/enemies/flame_512px",
            "Sprites/enemies/king_512px",
            "Sprites/enemies/pawn_512px",
            "Sprites/enemies/rook_512px"
        };
        r = new System.Random();
    }

    public void SetState(int level, List<CardStateBuilder> cardBuilders) {
        this.level = level;
        this.cardBuilders = cardBuilders;
    }

    public Enemy CreateEnemy(string enemyType, float x, float y) {

        //Debug.Log("create enemy");
        Enemy e = (Enemy)GameController.InstantiateEntity(enemyType, x, y).GetComponent<Entity>();
        List<CardState> enemyCards = new List<CardState>();
        for (int i = 0; i < cardBuilders.Count; i++) {
            //enemyCards.Add(cardBuilders[i].GetCardState(level));
            enemyCards.Add(CardStateBuilder.GetRandomEnemyCard());
        }
        e.SetCards(enemyCards);
        e.SetSprite(spritePaths[r.Next(spritePaths.Count)]);
        //Debug.Log("setting enemy");
        e.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "EntityLayer";
        return e;
    }
}
