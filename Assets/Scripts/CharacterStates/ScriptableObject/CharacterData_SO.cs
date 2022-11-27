using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "New Data",menuName = "Character States/Data")]
    public class CharacterData_SO : ScriptableObject
    {
        [Header("States Info")] 
        public int maxHealth;//最大血量
        
        public int currentHealth;//当前血量

        public int baseDenfence;//最大防御值

        public int currentDenfence;//当前防御值








    }
