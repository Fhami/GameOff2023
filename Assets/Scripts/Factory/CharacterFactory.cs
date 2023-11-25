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
            runtimeCharacter.properties.Add(PropertyKey.CHARACTER_STATE, new Property<CharacterState>(CharacterState.ALIVE, PropertyKey.CHARACTER_STATE));
            runtimeCharacter.properties.Add(PropertyKey.HEALTH, new Property<int>(characterData.health, PropertyKey.HEALTH));
            runtimeCharacter.properties.Add(PropertyKey.MAX_HEALTH, new Property<int>(characterData.health, PropertyKey.MAX_HEALTH));
            runtimeCharacter.properties.Add(PropertyKey.SIZE, new Property<int>(characterData.startSize, PropertyKey.SIZE));
            runtimeCharacter.properties.Add(PropertyKey.MAX_SIZE, new Property<int>(characterData.maxSize, PropertyKey.MAX_SIZE));
            runtimeCharacter.properties.Add(PropertyKey.MIN_SIZE, new Property<int>(characterData.minSize, PropertyKey.MIN_SIZE));
            runtimeCharacter.properties.Add(PropertyKey.HAND_SIZE, new Property<int>(characterData.handSize, PropertyKey.HAND_SIZE));
            runtimeCharacter.properties.Add(PropertyKey.ATTACK, new Property<int>(0, PropertyKey.ATTACK));
            runtimeCharacter.properties.Add(PropertyKey.SHIELD, new Property<int>(0, PropertyKey.SHIELD));
            runtimeCharacter.properties.Add(PropertyKey.STRENGTH, new Property<int>(0, PropertyKey.STRENGTH));
            runtimeCharacter.properties.Add(PropertyKey.EVASION, new Property<int>(0, PropertyKey.EVASION));
            runtimeCharacter.properties.Add(PropertyKey.STUN, new Property<int>(0, PropertyKey.STUN));
            runtimeCharacter.properties.Add(PropertyKey.STABLE, new Property<int>(0, PropertyKey.STABLE));
            runtimeCharacter.properties.Add(PropertyKey.THORNS, new Property<int>(0, PropertyKey.THORNS));
            runtimeCharacter.properties.Add(PropertyKey.DECAY, new Property<int>(0, PropertyKey.DECAY));
            runtimeCharacter.properties.Add(PropertyKey.GROW, new Property<int>(0, PropertyKey.GROW));
            runtimeCharacter.properties.Add(PropertyKey.UNSTABLE, new Property<int>(0, PropertyKey.UNSTABLE));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT, new Property<int>(0, PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT, new Property<int>(0, PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT, new Property<int>(0, PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT, new Property<int>(0, PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT, new Property<int>(0, PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT, new Property<int>(0, PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT));
            runtimeCharacter.properties.Add(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN, new Property<bool>(false, PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN));
            runtimeCharacter.properties.Add(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN, new Property<int>(0, PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN));
            runtimeCharacter.properties.Add(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX, new Property<int>(0, PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX));
            runtimeCharacter.properties.Add(PropertyKey.CARDS_DISCARDED_BY_CURRENTLY_BEING_PLAYED_CARD, new Property<int>(0, PropertyKey.CARDS_DISCARDED_BY_CURRENTLY_BEING_PLAYED_CARD));
            runtimeCharacter.properties.Add(PropertyKey.NEXT_CARD_PLAY_EXTRA_TIMES, new Property<int>(0, PropertyKey.NEXT_CARD_PLAY_EXTRA_TIMES));

            runtimeCharacter.SetupPassiveSlots();
            
            runtimeCharacter.skills = new();
            
            foreach (FormData formData in characterData.forms)
            {
                int passiveSlotIndex = 0;
                
                // Create runtime versions of character's skills. This is used for enemies only! player doesn't have passives assigned in this list.
                foreach (PassiveData passiveData in formData.passives)
                {
                    if (!passiveData) continue;
                    
                    RuntimePassive runtimePassive = PassiveFactory.Create(passiveData);
                    runtimeCharacter.AddPassiveToSlot(formData, passiveSlotIndex, runtimePassive);
                    passiveSlotIndex++;
                }
                
                // Create runtime versions of character's skills. This list is used by both player AND enemies.
                foreach (SkillData skillData in formData.skills)
                {
                    if (!skillData) continue;
                    
                    RuntimeSkill runtimeSkill = SkillFactory.Create(skillData);
                    runtimeCharacter.skills.Add(runtimeSkill);
                }
            }
            
            return runtimeCharacter;
        }

        private const string CharacterPrefabPath = "Prefabs/Characters/BaseCharacterPrefab";
        private static Character characterPrefab;
        
        public static Character CreateCharacterObject(RuntimeCharacter runtimeCharacter)
        {
            //Load and cache Character prefab 
            if (!characterPrefab)
            {
                characterPrefab = Resources.Load<Character>(CharacterPrefabPath);
            }

            //Use prefab from data or generic prefab
            var newChar = Object.Instantiate(runtimeCharacter.characterData.characterPrefab
                ? runtimeCharacter.characterData.characterPrefab
                : characterPrefab);
            
            newChar.name = runtimeCharacter.characterData.name;
            newChar.Init(runtimeCharacter);

            return newChar;
        }

        public static Character CreateCharacterObject(string name)
        {
            return CreateCharacterObject(Create(name));
        }
    }
}