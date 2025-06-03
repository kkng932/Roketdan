using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    [SerializeField]
    Slider hpSlider;
    float MAX_HP = 100f;
    float currHp;

    private void OnEnable()
    {
        SetHP(MAX_HP);
    }
    // 데미지 받음
    private void SetHP(float hp)
    {
        currHp = hp;
        hpSlider.value = currHp / MAX_HP;
    }
    // 데미지 받음
    public void TakeDamage(float damage)
    {
        currHp -= damage;
        SetHP(currHp);
    }

}
