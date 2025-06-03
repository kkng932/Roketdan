using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    [SerializeField]
    Slider hpSlider;
    [SerializeField]
    private SpriteRenderer boxSr;

    float MAX_HP = 100f;
    float currHp;

    private Material flashMat;
    // 반짝이는 시간
    float flashTime = 1f;
    Coroutine flashClr;
    

    private void Awake()
    {
        flashMat = boxSr.material;
    }
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
    [ContextMenu("TakeDamage")]
    public void Debug_TakeDamage()
    {
        TakeDamage(10f);
    }
    public void TakeDamage(float damage)
    {
        currHp -= damage;
        Flash();
        SetHP(currHp);
        if (currHp <= 0f)
            Destroy(gameObject);
    }
    private void Flash()
    {
        if (flashClr != null) StopCoroutine(flashClr);
        flashClr = StartCoroutine(IEFlash());
    }
    IEnumerator IEFlash()
    {
        float t = 0f;
        while (t < flashTime)
        {
            float amount = Mathf.Lerp(1f, 0f, t / flashTime);
            flashMat.SetFloat("flashAmount", amount);
            t += Time.deltaTime;
            yield return null;
            
        }
        flashMat.SetFloat("flashAmount", 0f);
    }
}
