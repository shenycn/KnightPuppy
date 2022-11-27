using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator anim;
    private CharacterStates characterStates;

    private GameObject attackTarget;
    private float lastAttackTime;//上一次攻击时间
    
    
    
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        characterStates.MaxHealth = 2;
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;


    }

    private void Update()
    {
        SwichAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    private void SwichAnimation()
    {
        anim.SetFloat("speed",agent.velocity.sqrMagnitude);//将vector3的值转换为浮点值传进来
        
        
        
    }
    
    public void MoveToTarget(Vector3 target)
    {
        
        StopAllCoroutines();
        agent.isStopped = false;//恢复移动
        
        agent.destination = target;//触发启用了这个事件 所有订阅了这个事件添加进去的方法都会执行
    }

    public void EventAttack(GameObject target)
    {
        if (target != null) //确定攻击目标是否为空
        {
            attackTarget = target;
            characterStates.isCritical = UnityEngine.Random.value<characterStates.AttackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
      
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;//一开始确定人物在移动
        //false:恢复沿着当前路劲移动
        transform.LookAt(attackTarget.transform);
        //判断攻击目标和当前的攻击距离
        while (Vector3.Distance(attackTarget.transform.position, transform.position) 
               > characterStates.AttackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;//下一帧再次执行
        }
        
        agent.isStopped = true;
        //true：停止当前移动

        if (lastAttackTime < 0)
        {
            anim.SetBool("critical",characterStates.isCritical);
            anim.SetTrigger("attack");
            
            //重置攻击时间
            lastAttackTime = characterStates.AttackData.CoolDown;
        }
    }
    //Animation EVENT
    void Hit()
    {
        //获得攻击目标的状态
        var targetStats = attackTarget.GetComponent<CharacterStates>();
        
        targetStats.TakeDamage(characterStates,targetStats);
    }
    
}
