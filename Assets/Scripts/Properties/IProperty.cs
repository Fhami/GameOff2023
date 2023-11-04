using System.Collections.Generic;

namespace DefaultNamespace
{
    public interface IProperty
    {
        List<Modifier> Modifiers { get; set; }
        void AddModifier(Modifier modifier);
        void RemoveModifier(Modifier modifier);
    }
}