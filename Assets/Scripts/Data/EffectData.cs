using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class EffectData : ScriptableObject
    {
        public IntentData intent;
        
        /// <summary>
        /// A coroutine for playing a single card. Executes card's effects and can also trigger different skills.
        /// </summary>
        /// <param name="card">The card being played.</param>
        /// <param name="characterPlayingTheCard">The character who is playing the card.</param>
        /// <param name="player">The player character (human).</param>
        /// <param name="cardTarget">The target character. Can be either player or enemy.</param>
        /// <param name="enemies">List of all enemies in the current battle. Can be used for AOE etc.</param>
        public abstract IEnumerator Execute(
            RuntimeCard card, 
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player, 
            RuntimeCharacter cardTarget, 
            List<RuntimeCharacter> enemies);
        
        /// <summary>
        /// Get the effect text with modifiers for the card front. This should be used in battle.
        /// </summary>
        public abstract string GetDescriptionTextWithModifiers(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies);
        
        /// <summary>
        /// Get the effect text for the card front. This should be used outside the battle.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDescriptionText();

        /// <summary>
        /// Get the effect value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public abstract int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies);
        /// <summary>
        /// Get effect value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public abstract string GetEffectValue(RuntimeCard card = null);
        /// <summary>
        /// Get the times value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public abstract int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies);
        /// <summary>
        /// Get times value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public abstract string GetTimesValue(RuntimeCard card = null);
    }
}