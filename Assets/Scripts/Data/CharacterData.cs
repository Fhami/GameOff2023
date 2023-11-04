using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// The base data for a character which is immutable and should not be modified during runtime.
    /// This data is used as a template for creating character instances.
    /// </summary>
    [CreateAssetMenu(menuName = "Gamejam/Character", fileName = "New Character")]
    public class CharacterData : ScriptableObject
    {
        public int health;
        public int size;
        public int maxSize;
    }
}