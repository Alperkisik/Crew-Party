using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicBossUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    GameObject followObject;
    GameObject boss;
    int hp;

    void Start()
    {
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;
    }

    private void _OnBossFightFinished(object sender, System.EventArgs e)
    {
        boss.GetComponent<BossManager>().OnHitPointLost -= _OnHitPointLost;
        LevelControl.instance.OnBossFightFinished -= _OnBossFightFinished;
        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = followObject.transform.position;
    }

    public void Setup(GameObject boss, GameObject followObject)
    {
        this.followObject = followObject;
        this.boss = boss;
        hp = boss.GetComponent<BossManager>().GetHitPoints();
        textMesh.text = hp.ToString();
        boss.GetComponent<BossManager>().OnHitPointLost += _OnHitPointLost;
    }

    private void _OnHitPointLost(object sender, BossManager.OnHitPointLostEventArgs e)
    {
        hp -= e.damage;
        if (hp <= 0) hp = 0;

        textMesh.text = hp.ToString();
    }
}
