using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] characterBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    public Player Unit { get; set; }

    public void Setup()
    {
        Unit = new Player(_base, level);
    }
}
