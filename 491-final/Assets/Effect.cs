using UnityEngine;
using UnityEngine;


public class Effect
{
    private int damage;
    private int turnNum;
    private bool heals;

    public Effect(int damgage, int turnNum, bool heals)
    {
        this.damage = damgage;
        this.turnNum = turnNum;
        this.heals = heals;
    }

    public bool GetHeals()
    {
        return heals;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public int GetTurnNum()
    {
        return turnNum;
    }

    public void DecrementTurn()
    {
        turnNum--;
    }
}