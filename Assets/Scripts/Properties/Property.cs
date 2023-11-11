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
                _value = value;
                OnChanged?.Invoke(this);
            } 
        }

        public List<Modifier> Modifiers { get; set; }

        /// <summary>
        /// Invoked when the base value (<see cref="Value"/>) changes or when
        /// modifiers are added or removed from <see cref="Modifiers"/> list.
        /// </summary>
        public Action<Property<T>> OnChanged { get; set; }

        private T _value;
        
        public Property(T value)
        {
            Value = value;
        }

        public void AddModifier(Modifier modifier)
        {
            Modifiers ??= new();

            if (Modifiers.Contains(modifier))
            {
                throw new Exception("Modifier has already been added.");
            }

            Modifiers.Add(modifier);
            
            OnChanged?.Invoke(this);
        }

        public void RemoveModifier(Modifier modifier)
        {
            if (!Modifiers.Contains(modifier))
            {
                throw new Exception("Trying to remove a modifier which doesn't not exist.");
            }

            Modifiers.Remove(modifier);

            OnChanged?.Invoke(this);
        }
    }
}