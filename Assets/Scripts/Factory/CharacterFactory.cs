using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// For creating character instances from character data.
    /// </summary>
    public static class CharacterFactory
    {
        public static RuntimeCharacter Create(string name)
        {
            // Use the character name to get the character data/template from the database.
            CharacterData characterData = Database.characterData[name];
            
            // Create new instance of a character.
            RuntimeCharacter runtimeCharacter = new RuntimeCharacter
            {
                characterData = characterData,
                properties = new()
            };

            // Create character's properties.
            runtimeCharacter.properties.Add(PropertyKey.CHARACTER_STATE, new Property<CharacterState>(CharacterState.ALIVE));
            runtimeCharacter.properties.Add(PropertyKey.HEALTH, new Property<int>(characterData.health));
            runtimeCharacter.properties.Add(PropertyKey.MAX_HEALTH, new Property<int>(characterData.health));
            runtimeCharacter.properties.Add(PropertyKey.SIZE, new Property<int>(characterData.startSize));
            runtimeCharacter.properties.Add(PropertyKey.MAX_SIZE, new Property<int>(characterData.maxSize));
            runtimeCharacter.properties.Add(PropertyKey.HAND_SIZE, new Property<int>(characterData.handSize));
            runtimeCharacter.properties.Add(PropertyKey.ATTACK, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.SHIELD, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.STRENGTH, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.EVADE, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.STUN, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN, new Property<bool>(false));
            runtimeCharacter.properties.Add(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX, new Property<int>(0));

            // Create runtime versions of character's skills
            runtimeCharacter.skills = new();
            foreach (FormData formData in characterData.forms)
            {
                foreach (SkillData skillData in formData.skills)
                {
                    if (!skillData) continue;
                    
                    RuntimeSkill runtimeSkill = SkillFactory.Create(skillData.name);
                    runtimeCharacter.skills.Add(runtimeSkill);
                }
            }
            
            return runtimeCharacter;
        }

        private const string CharacterPrefabPath = "CharacterPrefab";
        private static Character characterPrefab;
        
        public static Character CreateCharacterObject(RuntimeCharacter runtimeCharacter)
        {
            //Load and cache Character prefab 
            if (!characterPrefab)
            {
                characterPrefab = Resources.Load<Character>(CharacterPrefabPath);
            }

            var newChar = Object.Instantiate(characterPrefab);
            newChar.Init(runtimeCharacter);

            return newChar;
        }

        public static Character CreateCharacterObject(string name)
        {
            return CreateCharacterObject(Create(name));
        }
    }
}