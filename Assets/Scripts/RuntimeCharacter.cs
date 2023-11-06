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
        /// Try get the current form based on the character size.
        /// </summary>
        public bool TryGetCurrentForm(out FormData form)
        {
            int size = properties.Get<int>(PropertyKey.SIZE).Value;

            foreach (FormData formData in characterData.forms)
            {
                if (size >= formData.sizeMin && size <= formData.sizeMax)
                {
                    form = formData;
                    return true;
                }
            }

            form = null;
            return false;
        }
    }
}