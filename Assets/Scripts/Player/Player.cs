using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public characterBase Base { get; set; }
    public int Level { get; set; }
    public int LastDamage { get; set; }
    public int OriginalPower { get; set; }
    public float PowerMultiplier { get; set; } = 1f;
    public bool IsBlinded { get; set; } = false;

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Player(characterBase pBase, int plevel)
    {
        Base = pBase;
        Level = plevel;
        HP = MaxHP;

        //Generate the Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SPAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SPDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public bool TakeDamage(Move move, Player attacker, bool isDefending = false)
    {
        float criticalHit = 1f;
        if (Random.value * 100f < 50.25f)
            criticalHit = 2f;

        float modifiers = Random.Range(0.85f, 1f) * criticalHit;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        LastDamage = damage;

        if(isDefending)
        {
            damage /= 2;
        }
        
        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public Move RandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}
