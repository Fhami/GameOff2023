namespace DefaultNamespace
{
    /// <summary>
    /// The runtime instance of a character. This can be modified during runtime.
    /// </summary>
    public class RuntimeCharacter
    {
        /// <summary>
        /// The base data of the character.
        /// </summary>
        public CharacterData characterData;
        
        /// <summary>
        /// The properties/stats of the character.
        /// </summary>
        public PropertyContainer properties;
    }
}