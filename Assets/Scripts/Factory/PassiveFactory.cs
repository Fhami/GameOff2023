namespace DefaultNamespace
{
    /// <summary>
    /// For creating passive instances from passive data.
    /// </summary>
    public static class PassiveFactory
    {
        public static RuntimePassive Create(PassiveData passiveData)
        {
            return Create(passiveData.name);
        }
        
        public static RuntimePassive Create(string name)
        {
            // Use the passive name to get the passive data/template from the database.
            PassiveData passiveData = Database.passiveData[name];
            
            // Create new instance of a passive.
            RuntimePassive runtimePassive = new RuntimePassive
            {
                passiveData = passiveData,
                properties = new(),
                modifier = new Modifier // Create a runtime instance of the modifier.
                {
                    propertyKey = passiveData.propertyKey,
                    operation = passiveData.operation,
                    value = passiveData.value
                }
            };

            return runtimePassive;
        }
    }
}