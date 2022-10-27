using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        //145 event
        //1: Tao 1 event
        //2: Tim cho thich hop de Add Event
        //3: Tim ham de Subsribe (+=) vao event
        public event Action onLevelUp;

        //142 Events And Delegates
        //int currentLevel = 0;

        LazyValue<int> currentLevel;

        Experience experience;
        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            //142
            //currentLevel = CalculateLevel();

            currentLevel.ForceInit();

            //if(experience != null)
            //{
            //    experience.onExperienceGained += UpdateLevel;
            //}
        }
        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }
        private void UpdateLevel()
        {
            //if(gameObject.tag == "Player")
            //{
            //    print(GetLevel());
            //}
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect,transform);
        }

        //Call this in Health
        public float GetStat(Stat stat)
        {
            //Old
            //return progression.GetStat(stat,characterClass,GetLevel());
            //147 New
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat)/100);
        }

       

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }


        //147
        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
          
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        //148 
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiesr(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }


        // OLD Method for get reward (EXP)
        //public float GetExperienceReward()
        //{
        //    return 10;
        //}
        //void Update()
        //{

        //}

        //142 tach GetLevel ra de ko tinh toan nhung ham ben duoi Calculate Level moi frame
        public int GetLevel()
        {
            //if(currentLevel < 1)
            //{
            //    currentLevel = CalculateLevel();
            //}
            return currentLevel.value;
        }
        //Sua Get Level thanh Calculate Level
        //142
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;
            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for(int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if(XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}
