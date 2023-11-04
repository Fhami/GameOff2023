using System;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CustomPropertyDrawer(typeof(Effect))]
    public class EffectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty effectTypeProperty = property.FindPropertyRelative("effectType");
            EffectType effectType = (EffectType)effectTypeProperty.enumValueIndex;

            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, "Effect", EditorStyles.boldLabel);
            
            Rect effectTypePosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(effectTypePosition, effectTypeProperty);

            switch (effectType)
            {
                case EffectType.NONE:
                    break;
                case EffectType.ATTACK:
                {
                    Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty valueProperty = property.FindPropertyRelative("value");
                    EditorGUI.PropertyField(valueRect, valueProperty);

                    Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
                    EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
                    break;
                }
                case EffectType.SIZE:
                {
                    Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty valueProperty = property.FindPropertyRelative("value");
                    EditorGUI.PropertyField(valueRect, valueProperty);

                    Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
                    EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
                    break;
                }
                case EffectType.HEAL:
                {
                    Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty valueProperty = property.FindPropertyRelative("value");
                    EditorGUI.PropertyField(valueRect, valueProperty);

                    Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
                    EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty effectTypeProperty = property.FindPropertyRelative("effectType");

            EffectType effectType = (EffectType)effectTypeProperty.enumValueIndex;

            switch (effectType)
            {
                case EffectType.NONE:
                    return EditorGUIUtility.singleLineHeight * 2;
                case EffectType.ATTACK:
                    return EditorGUIUtility.singleLineHeight * 4;
                case EffectType.SIZE:
                    return EditorGUIUtility.singleLineHeight * 4;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

