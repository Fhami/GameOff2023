using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class EffectData : ScriptableObject
    {
        /// <summary>
        /// A coroutine for playing a single card. Executes card's effects and can also trigger different skills.
        /// </summary>
        /// <param name="card">The card being played.</param>
        /// <param name="characterPlayingTheCard">The character who is playing the card.</param>
        /// <param name="player">The player character (human).</param>
        /// <param name="cardTarget">The target character. Can be either player or enemy.</param>
        /// <param name="enemies">List of all enemies in the current battle. Can be used for AOE etc.</param>
        /// <returns></returns>
        public abstract IEnumerator Execute(
            RuntimeCard card, 
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player, 
            RuntimeCharacter cardTarget, 
            List<RuntimeCharacter> enemies);
        
        /// <summary>
        /// Get the effect text with modifier for the card front.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="playerCharacter">The player character (human)</param>
        /// <returns></returns>
        public abstract string GetDescriptionTextWithModifier(RuntimeCard card, RuntimeCharacter playerCharacter);
        
        /// <summary>
        /// Get the effect text for the card front.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDescriptionText();
        
        /// <summary>
        /// This is where we write description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract string GetDescriptionText(string value);
        
    }
}