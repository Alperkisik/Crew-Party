using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossUIManager : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject healthBar;
    [SerializeField] TextMeshProUGUI textMesh;

    float scaleDecrease;
    float bossScale;
    int hp;
    void Start()
    {
        hp = boss.GetComponent<BossManager>().GetHitPoints();
        /*healthBar.GetComponent<SliderBar>().SetMaxValue(hp);

        scaleDecrease = 1f / (float)hp;*/
        boss.GetComponent<BossManager>().OnHitPointLost += _OnHitPointLost;
        //UpdateUI();
        bossScale = boss.transform.localScale.x;
        textMesh.text = hp.ToString();
    }

    private void _OnHitPointLost(object sender, BossManager.OnHitPointLostEventArgs e)
    {
        /*int damage = e.damage;
        float scale = damage * scaleDecrease;
        Vector3 hpScale = healthBar.transform.localScale;
        hpScale.x -= scale;

        if(hpScale.x <= 0f) hpScale.x = 0f;
        healthBar.transform.localScale = hpScale;*/
        
        hp -= e.damage;
        float scale = (bossScale - 0.16f) / (float)hp;
        //scale *= 2;
        Vector3 targetScale = transform.localScale + new Vector3(scale, scale, scale);

        if (targetScale.x >= 1.84f) targetScale = new Vector3(1.84f, 1.84f, 1.84f);

        transform.localScale = targetScale;

        if (hp <= 0) hp = 0;
        textMesh.text = hp.ToString();
    }

    private void UpdateUI()
    {
        int hitPoints = boss.GetComponent<BossManager>().GetHitPoints();
        healthBar.GetComponent<SliderBar>().SetValue(hitPoints);
    }
}
