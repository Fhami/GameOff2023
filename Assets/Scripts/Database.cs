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
        public static Dictionary<string, SkillData> skillData;

        public static void Initialize()
        {
            cardData = Resources.LoadAll<CardData>("Cards").ToDictionary(card => card.name);
            characterData = Resources.LoadAll<CharacterData>("Characters").ToDictionary(character => character.name);
            skillData = Resources.LoadAll<SkillData>("").ToDictionary(skill => skill.name);
        }
    }
}