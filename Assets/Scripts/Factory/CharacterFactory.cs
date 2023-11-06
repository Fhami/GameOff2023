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
            RuntimeCharacter runtimeCharacter = new RuntimeCharacter();

            runtimeCharacter.characterData = characterData;
            
            runtimeCharacter.properties = new();
            runtimeCharacter.properties.Add(PropertyKey.HEALTH, new Property<int>(characterData.health));
            runtimeCharacter.properties.Add(PropertyKey.MAX_HEALTH, new Property<int>(characterData.health));
            runtimeCharacter.properties.Add(PropertyKey.SIZE, new Property<int>(characterData.startSize));
            runtimeCharacter.properties.Add(PropertyKey.MAX_SIZE, new Property<int>(characterData.maxSize));
            runtimeCharacter.properties.Add(PropertyKey.ATTACK, new Property<int>(0));
            runtimeCharacter.properties.Add(PropertyKey.POWER_UP, new Property<int>(0));
       // foreach (Modifier modifier in characterData.formModifiers)
            // {
            //     
            // }
     
            
            return runtimeCharacter;
        }
    }
}