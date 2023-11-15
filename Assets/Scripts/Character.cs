using UnityEngine;

namespace DefaultNamespace
{
    // TODO: This can be attached to character prefab
    public class Character : MonoBehaviour
    {
        public RuntimeCharacter runtimeCharacter;

        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;
            
            //Update visual
        }
    }
}