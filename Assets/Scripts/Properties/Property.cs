using System.Collections.Generic;
using System;

namespace DefaultNamespace
{
    public class Property<T> : IProperty
    {
        public T Value { get; set; }

        public List<Modifier> Modifiers { get; set; }

        public Action<Property<T>> OnChanged { get; set; }

        public Property(T value)
        {
            Value = value;
        }

        public void SetValue(T baseValue)
        {
            Value = baseValue;
            
            OnChanged?.Invoke(this);
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