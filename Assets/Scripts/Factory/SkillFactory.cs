namespace DefaultNamespace
{
    /// <summary>
    /// For creating skill instances from skill data.
    /// </summary>
    public static class SkillFactory
    {
        public static RuntimeSkill Create(SkillData skillData)
        {
            // Create new instance of a skill.
            RuntimeSkill runtimeSkill = new RuntimeSkill
            {
                skillData = skillData,
                properties = new()
            };
            
            // Create cards's properties.
            runtimeSkill.properties.Add(PropertyKey.SKILL_STATE, new Property<SkillState>(SkillState.READY, PropertyKey.SKILL_STATE));

            return runtimeSkill;
        }
        
        public static RuntimeSkill Create(string name)
        {
            // Use the skill name to get the skill data/template from the database.
            SkillData skillData = Database.skillData[name];
            
            // Create new instance of a skill.
            RuntimeSkill runtimeSkill = new RuntimeSkill
            {
                skillData = skillData,
                properties = new()
            };
            
            // Create cards's properties.
            runtimeSkill.properties.Add(PropertyKey.SKILL_STATE, new Property<SkillState>(SkillState.READY, PropertyKey.SKILL_STATE));

            return runtimeSkill;
        }
    }
}