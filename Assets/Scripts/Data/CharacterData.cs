﻿using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

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
        public int startSize;
        public int maxSize;
        public int handSize;
        [Expandable] public List<FormData> forms;
    }
}