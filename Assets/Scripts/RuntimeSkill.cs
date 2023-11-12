namespace DefaultNamespace
{
    public enum SkillState
    {
        NONE,
        USED,
        READY,
    }
    
    /// <summary>
    /// The runtime instance of a skill. This can be modified during runtime.
    /// </summary>
    public class RuntimeSkill : RuntimeEntity
    {
        /// <summary>
        /// The base data of the skill.
        /// </summary>
        public SkillData skillData;
    }
}