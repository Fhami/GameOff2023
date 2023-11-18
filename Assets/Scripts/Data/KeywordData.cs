using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Keyword Description", fileName = "New Keyword Description")]
    public class KeywordData : ScriptableObject
    {
        public string header;
        [ResizableTextArea]
        public string description;
    }
}