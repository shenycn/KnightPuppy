using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CharacterStates : MonoBehaviour
    {
        public CharacterData_SO characterData;
        
        public AttackData_SO AttackData;
        
        public bool isCritical;//是否暴击
        [HideInInspector]
        #region Read From Data_SO

        public int MaxHealth
        {
            //直接读取最大生命值
            get
            {
                if (characterData != null)
                {
                    return characterData.maxHealth;
                  
                }
                else{ return 0;}
            }
            set
            {
                characterData.maxHealth = value;
            }
        }
        public int CurrentHealth
        {
            //直接读取当前生命值
            get
            {
                if (characterData != null)
                {
                    return characterData.currentHealth;
                  
                }
                else{ return 0;}
            } 
            set
            {
                characterData.currentHealth = value;
            }
        }
        public int BaseDefence
        {
            //直接读取基础防御力
            get
            {
                if (characterData != null)
                {
                    return characterData.baseDenfence;
                  
                }
                else{ return 0;}
            } 
            set
            {
                characterData.baseDenfence = value;
            }
        }
        public int CurrentDefence
        {
            //直接读取当前防御力
            get
            {
                if (characterData != null)
                {
                    return characterData.currentDenfence;
                  
                }
                else{ return 0;}
            } 
            set
            {
                characterData.currentDenfence = value;
            }
           
        }

        #endregion

        #region Character Combat

        public void TakeDamage(CharacterStates attacker, CharacterStates defenner)
        {
            //判断伤害=攻击力-防御力
            //知识点：Mathf.Max (最大0)
            int damage = Mathf.Max(attacker.CurrentDamage() - defenner.CurrentDefence,0);
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
            //TODO:UI
            //TODO:经验升级
        }

        private int CurrentDamage()
        {
            //获得随机伤害：从最小伤害和最大伤害之间选取一个值
            float coreDamage = UnityEngine.Random.Range(AttackData.minDamage, AttackData.maxDamage);
            //判断暴击
            if (isCritical)
            {
                coreDamage *= AttackData.criticalMutiplier;//爆伤
                Debug.Log("暴击！"+coreDamage);
            }
            return (int)coreDamage;
        }

        #endregion
        
    }
