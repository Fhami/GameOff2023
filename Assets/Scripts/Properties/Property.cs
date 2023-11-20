using System.Collections.Generic;
using System;

namespace DefaultNamespace
{
    public class Property<T> : IProperty
    {
        public T Value 
        { 
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value;
                OnChanged?.Invoke(oldValue, this);
            } 
        }

        public List<Modifier> Modifiers { get; set; }

        /// <summary>
        /// Invoked when the base value (<see cref="Value"/>) changes or when
        /// modifiers are added or removed from <see cref="Modifiers"/> list.
        /// OldValue, CurrentValue
        /// </summary>
        public Action<T, Property<T>> OnChanged { get; set; }

        private T _value;
        
        public PropertyKey Key { get; }
        
        public Property(T value, PropertyKey key)
        {
            Value = value;
            Key = key;
        }

        public void AddModifier(Modifier modifier)
        {
            Modifiers ??= new();

            if (Modifiers.Contains(modifier))
            {
                throw new Exception("Modifier has already been added.");
            }

            Modifiers.Add(modifier);
            
            OnChanged?.Invoke(_value, this);//Not sure should we create separate callback for Modifier?
        }

        public void RemoveModifier(Modifier modifier)
        {
            if (!Modifiers.Contains(modifier))
            {
                throw new Exception("Trying to remove a modifier which doesn't not exist.");
            }

            Modifiers.Remove(modifier);

            OnChanged?.Invoke(_value, this);//Not sure should we create separate callback for Modifier?
        }
    }
}