using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(menuName = "Gamejam/Effect Modifier", fileName = "New Effect Modifier")]
public class EffectModifier : ScriptableObject
{
    public Modifier modifier;

    public void AttachToRuntimeCard(RuntimeCard runtimeCard)
    {
        IProperty property = runtimeCard.properties.Get(modifier.propertyKey);
        property.AddModifier(modifier);
    }

    // TODO: I'm not sure how to display this in the card front, but below are some ideas
    // TODO: it's easy if we have modifiers that only check the player size, then we just
    // need 2 numbers (the size requirement and the modifier value (e.g. +5 damage)
    
    // NOTE: This is just an example of how to get the value for card UI.
    // NOTE: I'm not sure of the exact implementation, but you need to access the
    // modifier and the modifier's equality
    public int GetValue()
    {
        return modifier.value; // <- the modifier value, e.g. 5 attack
    }

    // NOTE: This is just an example of how to get the value for card UI.
    // NOTE: I'm not sure of the exact implementation, but you need to access the
    // modifier and the modifier's equality
    public int GetRequirement()
    {
        return modifier.conditions[0].value; // <- the condition value, e.g. SIZE equal to 8
    }
}