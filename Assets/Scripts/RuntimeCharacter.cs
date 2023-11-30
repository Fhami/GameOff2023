﻿using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public enum CharacterState
    {
        NONE,
        ALIVE,
        DEAD,
        ESCAPED
    }
    
    /// <summary>
    /// The runtime instance of a character. This can be modified during runtime.
    /// </summary>
    public class RuntimeCharacter : RuntimeEntity
    {
        /// <summary>
        /// The base data of the character.
        /// </summary>
        public CharacterData characterData;
        
        public Character Character;
        public Dictionary<FormData, RuntimePassive[]> passiveSlots;
        public List<RuntimeSkill> skills = new();
        
        /// <summary>
        /// Get character's current form based on their character power.
        /// </summary>
        public FormData GetCurrentForm()
        {
            int size = properties.Get<int>(PropertyKey.SIZE).Value;

            foreach (FormData formData in characterData.forms)
            {
                if (size >= formData.sizeMin && size <= formData.sizeMax)
                {
                    return formData;
                }
            }

            throw new Exception($"Could not find FormData for {characterData.name} when power is {size}. " +
                                $"Make sure there's proper FormData assigned for this character in CharacterData.");
        }

        /// <summary>
        /// Setup the passive slots for the character. By default these are empty
        /// and there's 3 slots per form. During the game player can acquire passives
        /// and put them into these slots.
        /// </summary>
        public void SetupPassiveSlots()
        {
            passiveSlots = new();
            
            // Create 3 slots for passives for each form.
            foreach (FormData form in characterData.forms)
            {
                passiveSlots.Add(form, new RuntimePassive[3]);
            }
        }

        /// <summary>
        /// Add a passive to a slot.
        /// </summary>
        /// <param name="form">Which form?</param>
        /// <param name="slotIndex">Which slot index? (0-2)</param>
        /// <param name="runtimePassive">The passive to add.</param>
        public void AddPassiveToSlot(FormData form, int slotIndex, RuntimePassive runtimePassive)
        {
            passiveSlots[form][slotIndex] = runtimePassive;
        }
        
        /// <summary>
        /// Activates passives for the provided form.
        /// </summary>
        public void EnablePassives(FormData form)
        {
            // Get the runtime passives for this form
            if (passiveSlots.TryGetValue(form, out RuntimePassive[] slots))
            {
                // Loop through each passive slot
                for (int i = 0; i < slots.Length; i++)
                {
                    // If there's a passive assigned to that slot, add the modifier to character's property
                    RuntimePassive passive = slots[i];
                    if (passive != null && passive.passiveData.triggerGameEvent == GameEvent.NONE)
                    {
                        IProperty property = properties.Get(passive.modifier.propertyKey);
                        property.AddModifier(passive.modifier);
                    }
                }
            }
        }

        /// <summary>
        /// Disables passives for the provided form.
        /// </summary>
        public void DisablePassives(FormData form)
        {
            // Get the runtime passives for this form
            if (passiveSlots.TryGetValue(form, out RuntimePassive[] slots))
            {
                // Loop through each passive slot
                for (int i = 0; i < slots.Length; i++)
                {
                    // If there's a passive assigned to that slot, add the modifier to character's property
                    RuntimePassive passive = slots[i];
                    if (passive != null)
                    {
                        IProperty property = properties.Get(passive.modifier.propertyKey);
                        property.RemoveModifier(passive.modifier);
                    }
                }
            }
        }

        public void EnablePassive(RuntimePassive passive)
        {
            if (passive == null) return;
            IProperty property = properties.Get(passive.modifier.propertyKey);
            property.AddModifier(passive.modifier);
        }

        public void DisablePassive(RuntimePassive passive)
        {
            if (passive == null) return;
            IProperty property = properties.Get(passive.modifier.propertyKey);
            property.RemoveModifier(passive.modifier);
        }

        /// <summary>
        /// Effects that will also play on opponent's turn
        /// </summary>
        public void ClearBuffStackTurnStart()
        {
            //TODO: very wip, need to decide which effect to clear when turn start
            properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value = 0;
            properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value = false;
            properties.Get<int>(PropertyKey.NEXT_CARD_PLAY_EXTRA_TIMES).Value = 0;
            properties.Get<int>(PropertyKey.EVASION).Value = 0;
            properties.Get<int>(PropertyKey.THORNS).Value = 0;
            properties.Get<int>(PropertyKey.STABLE).Value = 0;
        }
        
        /// <summary>
        /// Effects that only play on character's own turn
        /// </summary>
        public void ClearBuffStackTurnEnd()
        {
            //TODO: very wip, need to decide which effect to clear when turn end
            properties.Get<int>(PropertyKey.STUN).Value = 0;
            properties.Get<int>(PropertyKey.DECAY).Value = 0;
            properties.Get<int>(PropertyKey.FRAGILE).Value = 0;
            properties.Get<int>(PropertyKey.GROW).Value = 0;
        }
        
        /// <summary>
        /// Get active buffs and debuffs. 
        /// </summary>
        public List<Property<int>> GetActiveBuffsAndDebuffs()
        {
            List<Property<int>> buffsAndDebuffs = new();
            
            foreach (PropertyKey buffPropertyKey in Database.buffData.Keys)
            {
                if (buffPropertyKey == PropertyKey.NONE) continue;
                
                Property<int> buffProperty = properties.Get<int>(buffPropertyKey);
                int value = buffProperty.GetValueWithModifiers(this);
                if (value > 0)
                {
                    buffsAndDebuffs.Add(buffProperty);
                }
            }

            return buffsAndDebuffs;
        }
    }
}