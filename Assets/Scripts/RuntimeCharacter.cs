using System;

namespace DefaultNamespace
{
    /// <summary>
    /// The runtime instance of a character. This can be modified during runtime.
    /// </summary>
    public class RuntimeCharacter : RuntimeEntity
    {
        /// <summary>
        /// The base data of the character.
        /// </summary>
        public CharacterData characterData;

        /// <summary>
        /// Get character's current form based on their character power.
        /// </summary>
        public FormData GetCurrentForm()
        {
            int size = properties.Get<int>(PropertyKey.POWER).Value;

            foreach (FormData formData in characterData.forms)
            {
                if (size >= formData.powerMin && size <= formData.powerMax)
                {
                    return formData;
                }
            }

            throw new Exception($"Could not find FormData for {characterData.name} when power is {size}. " +
                                $"Make sure there's proper FormData assigned for this character in CharacterData.");
        }
    }
}