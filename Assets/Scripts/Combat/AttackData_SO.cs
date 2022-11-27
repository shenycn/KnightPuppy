using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack",menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;//攻击距离
    public float skillRange;//技能距离
    public float CoolDown;//技能冷却
    public int minDamage;//最小攻击伤害
    public int maxDamage;//最大攻击伤害
    public float criticalMutiplier;//暴击加成百分比
    public float criticalChance;//暴击率
}
