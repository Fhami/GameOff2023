namespace DefaultNamespace
{
    /// <summary>
    /// For creating character instances from character data.
    /// </summary>
    public static class CharacterFactory
    {
        public static RuntimeCharacter Create(string name)
        {
            CharacterData characterData = Database.characterData[name];
            
            RuntimeCharacter runtimeCharacter = new RuntimeCharacter();

            runtimeCharacter.characterData = characterData;
            
            runtimeCharacter.properties = new();
            runtimeCharacter.properties.Add(PropertyKey.HEALTH, new Property<int>(characterData.health));
            runtimeCharacter.properties.Add(PropertyKey.MAX_HEALTH, new Property<int>(characterData.health));
            runtimeCharacter.properties.Add(PropertyKey.SIZE, new Property<int>(characterData.size));
            runtimeCharacter.properties.Add(PropertyKey.MAX_SIZE, new Property<int>(characterData.maxSize));
            
            return runtimeCharacter;
        }
    }
}