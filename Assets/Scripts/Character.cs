using System;
using EPOOutline;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    // TODO: This can be attached to character prefab
    public class Character : MonoBehaviour
    {
        public RuntimeCharacter runtimeCharacter;
        public CardController cardController;

        [SerializeField] private Outlinable outlinable;
        [SerializeField] private TextMeshPro hpTxt;
        [SerializeField] private TextMeshPro statText;

        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;
            
            //Update visual
            UpdateHpVisual(runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH));
            UpdateSizeVisual(runtimeCharacter.properties.Get<int>(PropertyKey.SIZE));
            UpdateFormVisual(runtimeCharacter.GetCurrentForm());
            
            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged += UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).OnChanged += UpdateSizeVisual;
            
            outlinable.AddAllChildRenderersToRenderingList();
        }

        public void Highlight(bool _value)
        {
            outlinable.enabled = _value;
        }

        public void HighlightSelected(bool _value)
        {
            if (_value)
                outlinable.OutlineParameters.FillPass.Shader =
                    Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
            else
                outlinable.OutlineParameters.FillPass.Shader = null;
        }
        
        public void UpdateStat()
        {
            
        }

        public void UpdateHpVisual(Property<int> _value)
        {
            hpTxt.SetText($"HP: {_value.Value.ToString()}");
        }

        public void UpdateSizeVisual(Property<int> _size)
        {
            
        }

        public void UpdateFormVisual(FormData _form)
        {
            
        }
    }
}