using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HealthBar hpBar;

    Player _unit;

    public void SetData(Player player)
    {
        _unit = player;
         
        nameText.text = player.Base.Name;
        levelText.text = "Lv. " + player.Level;
        hpBar.SetHP((float) player.HP / player.MaxHP);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetSmoothHP((float)_unit.HP / _unit.MaxHP);
    }
}
