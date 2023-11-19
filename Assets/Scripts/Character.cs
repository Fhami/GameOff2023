using System;
using EPOOutline;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    // TODO: This can be attached to character prefab
    public class Character : MonoBehaviour, ICardTarget
    {
        public RuntimeCharacter runtimeCharacter;
        public CardController cardController;

        [SerializeField] private Outlinable outlinable;
        [SerializeField] private StatsUI statUI;
        [SerializeField] private SizeUI sizeUI;
        [SerializeField] private IntentionUI intentionUI;

        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;
            
            //Update visual
            UpdateHpVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH));
            UpdateSizeVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.SIZE));
            UpdateFormVisual(runtimeCharacter.GetCurrentForm());
            
            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged += UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).OnChanged += UpdateSizeVisual;
            
            outlinable.AddAllChildRenderersToRenderingList();
        }

        public GameObject GameObject => gameObject;

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

        public void UpdateHpVisual(int _oldValue, Property<int> _value)
        {
            statUI.SetHp(_oldValue, _value.Value, runtimeCharacter.properties.Get<int>(PropertyKey.MAX_HEALTH).Value);
        }

        public void UpdateSizeVisual(int _oldValue, Property<int> _size)
        {
            var _sizeEffect = _oldValue > _size.Value ? SizeEffectType.Increase : SizeEffectType.Decrease;
            sizeUI.SetSize(_size.Value, _sizeEffect);
        }

        public void UpdateFormVisual(FormData _form)
        {
            
        }
    }
}