using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon/Create", order = 1)]
public class WeaponData : ScriptableObject
{
    [Header("Stats")]
    public int damage;
    public float range;
    public float attackRate;
    [Header("Type")]
    public WeaponType weapon;
    public AttackType attackType;
}
