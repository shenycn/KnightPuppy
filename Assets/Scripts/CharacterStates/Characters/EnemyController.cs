using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyStates{ GUARD, PATROL,CHASE,DEAD }// 待机 巡逻 追击 死亡

    [RequireComponent(typeof(NavMeshAgent))] //约束保证有此类型/
    public class EnemyController : MonoBehaviour
    {
         private NavMeshAgent agent;
         private EnemyStates enemystates;
         private CharacterStates characterStates;

        [Header("Besic Settings")] 
        public float sightRedius;//可视范围
        public float lookAtTime;//巡视时间
        private float remainLookAtTime;//用于判断巡视时间
        private float lastAttackTime;//判断最后一次攻击的时间
        private GameObject attackTarget;//攻击目标
        public bool isGuard;//判断是否是守卫敌人
        private float speed;//记录敌人的速度

        
        private Animator anim;//控制敌人动画
        //bool配合动画
        private bool isWalk;
        private bool isChase;
        private bool isFollow;
        
        [Header("Patrol State")]
        public float patrolRange;//敌人巡逻的范围

        private Vector3 wayPoint;//随机点 未初始化
        private Vector3 guardPos;
        
        private void Awake()
    {
        // 待机 巡逻 追击 死亡
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        speed = agent.speed;
        guardPos = transform.position;//一开始拿到出场坐标
        remainLookAtTime = lookAtTime;

    }

        private void Start()
        {
            if (isGuard)
            {
                enemystates =  EnemyStates.GUARD;
            }
            else
            {
                enemystates = EnemyStates.PATROL;
                GetNewWayPoint();
            }
        }

        private void Update()
    {
        SwitchStates();
        SwichAnimatio();
        lastAttackTime -= Time.deltaTime;
    }
    //动画
    void SwichAnimatio()
    {
        anim.SetBool("walk",isWalk);
        anim.SetBool("chase",isChase);
        anim.SetBool("follow",isFollow);
        anim.SetBool("critical",characterStates.isCritical);
    }
    void SwitchStates()
    {
        //如果发现 player 切换case
        if (FoundPlayer())
        {
            enemystates = EnemyStates.CHASE;
            //Debug.Log("找到player");
        }
        
        
        switch (enemystates)
        {
            case EnemyStates.GUARD://守卫
                isChase = false;
                break;
            case EnemyStates.PATROL://巡逻
                //保证追击的情况
                isChase = false;
                agent.speed = speed * 0.5f;//乘法的性能开销小于除法的开销
                
                //判断坐标的距离是否相同 到达巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;

                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                    
                }
                else
                {

                    isWalk = true;
                    agent.destination = wayPoint;

                }
                break;
            case EnemyStates.CHASE://追击
                //todo:配合动画
                isWalk = false;//追击前是巡逻的状态
                isChase = true;
              
                agent.speed = speed;
                //如果没有发现player就脱战
                if (!FoundPlayer())
                {
                    //todo:拉拖回上一个状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    //判断原有的状态
                    else if (isGuard)
                    {
                        enemystates = EnemyStates.GUARD;
                    }else
                    {
                        enemystates = EnemyStates.PATROL;
                    }
                   
                }
                else
                {
                    isFollow = true;
                    //当player不在的时候 就要取消停止状态
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                
                //todo:在攻击范围内攻击
                if (TargetInAttackRange() || TargetInAttackRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        //计时器归零 
                        lastAttackTime = characterStates.AttackData.CoolDown;
                        
                        //暴击判断
                        //知识点：Random.value
                        characterStates.isCritical = Random.value < characterStates.AttackData.criticalChance;
                        
                        //执行攻击
                        Attack();
                    }
                }
                
                break;
            case EnemyStates.DEAD:
                break;
        }
        
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            //攻击目标和当前坐标
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                   characterStates.AttackData.attackRange;
        }
        else
        {return false;}
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                   characterStates.AttackData.skillRange;
        }
        else
            return false;
    }
    bool FoundPlayer()
    {
        //在敌人周围的半径去查找碰撞体
        var colliders = Physics.OverlapSphere(transform.position, sightRedius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        
        //获得随机点 
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y,
            guardPos.z + randomZ);
        //FIXME:可能出现问题
        NavMeshHit hit;
        
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    void Attack()
    {
        //转向目标
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击
            anim.SetTrigger("attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻击
            anim.SetTrigger("skill");

        }
    }
    private void OnDrawGizmosSelected()
    {
        //选中当前目标才会在scene窗口去画监视范围
        Gizmos.color = Color.blue;//可视化
        Gizmos.DrawWireSphere(transform.position,sightRedius);
    }

    //Animation Event
    void Hit()
    {
        //首先判断攻击目标是否为空
        if (attackTarget != null)
        {
            //获得攻击目标的状态
            var targetStats = attackTarget.GetComponent<CharacterStates>();
            targetStats.TakeDamage(characterStates,targetStats);   
        }
        
     
        
         
    }
    
    
    }
