using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        //138 Performant Lookup with Dictionaries

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;
        public float GetStat(Stat stat,CharacterClass characterClass, int level)
        {
            //OLD Method
            //foreach (ProgressionCharacterClass progressionClass in characterClasses)
            //{
            //    if(progressionClass.characterClass == characterClass)
            //    {
            //        //return progressionClass.health[level - 1];
            //    }
            //}
            //return 0;

            //135
            //New Method Finding a Stat by enum 
            //Su dung vong lap foreach se co hieu nang khong tot vi phai goi moi frame neu data co 1000 du lieu thi hieu nang se rat cham, nen su dung Dictionary
            //foreach(ProgressionCharacterClass progressionClass in characterClasses)
            //{
            //    if (progressionClass.characterClass != characterClass) continue;

            //    foreach(ProgressionStat progressionStat in progressionClass.stats)
            //    {
            //        if(progressionStat.stat != stat) continue;
            //        if (progressionStat.levels.Length < level) continue;
            //        return progressionStat.levels[level - 1];
            //    }
            //}

          

            //138 Using Dictionaries
            BuildLookUp();

            float[] levels = lookupTable[characterClass][stat];
            if(levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
        }

        //139 Levelling up 
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookUp();
            float[] levels = lookupTable[characterClass][stat];
            Debug.Log(levels);
            return levels.Length;
        }


        private void BuildLookUp()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            foreach(ProgressionCharacterClass progressionClass in characterClasses)
            { 
                var statLookupTable = new Dictionary<Stat, float[]>();
                foreach(ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        //Phai them dong nay moi co the su dung duoc dong tren
        [System.Serializable]
        class ProgressionCharacterClass
        {
            //phai de all variable public (SerializeField OK nhung phai co System.Serializable);
            //Xem trong ScriptableObject
            public CharacterClass characterClass;
            //public float[] health;
            public ProgressionStat[] stats; 

        }
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}