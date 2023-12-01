using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Simple "database" to keep references to various things.
    /// </summary>
    public static class Database
    {
        public static Dictionary<string, CardData> cardData;
        public static Dictionary<string, CharacterData> characterData;
        public static Dictionary<string, PassiveData> passiveData;
        public static Dictionary<string, SkillData> skillData;
        public static Dictionary<PropertyKey, BuffData> buffData;
        public static Dictionary<IntentType, IntentData> intentData;

        public static void Initialize()
        {
            cardData = Resources.LoadAll<CardData>("Cards").ToDictionary(card => card.name);
            characterData = Resources.LoadAll<CharacterData>("Characters").ToDictionary(character => character.name);
            passiveData = Resources.LoadAll<PassiveData>("").ToDictionary(passive => passive.name);
            skillData = Resources.LoadAll<SkillData>("").ToDictionary(skill => skill.name);
            buffData = Resources.LoadAll<BuffData>("Buffs").ToDictionary(buff => buff.buffPropertyKey);
            intentData = Resources.LoadAll<IntentData>("Intents").ToDictionary(intent => intent.intentType);
        }
    }
}