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

            SerializedProperty propertyKeyProperty = property.FindPropertyRelative("propertyKey");
            PropertyKey propertyKey = (PropertyKey)propertyKeyProperty.enumValueIndex;

            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, "Effect", EditorStyles.boldLabel);
            
            Rect propertyKeyRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyKeyRect, propertyKeyProperty);

            switch (propertyKey)
            {
                case PropertyKey.NONE:
                    break;
                default:
                    Rect operationRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty operationProperty = property.FindPropertyRelative("operation");
                    EditorGUI.PropertyField(operationRect, operationProperty);
                    
                    Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty valueProperty = property.FindPropertyRelative("value");
                    EditorGUI.PropertyField(valueRect, valueProperty);
                    
                    Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 4, position.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
                    EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
                    break;
            }
            
            // switch (propertyKey)
            // {
            //     case EffectType.NONE:
            //         break;
            //     case EffectType.ATTACK:
            //     {
            //         Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
            //         SerializedProperty valueProperty = property.FindPropertyRelative("value");
            //         EditorGUI.PropertyField(valueRect, valueProperty);
            //
            //         Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
            //         SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
            //         EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
            //         break;
            //     }
            //     case EffectType.SIZE:
            //     {
            //         Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
            //         SerializedProperty valueProperty = property.FindPropertyRelative("value");
            //         EditorGUI.PropertyField(valueRect, valueProperty);
            //
            //         Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
            //         SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
            //         EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
            //         break;
            //     }
            //     case EffectType.HEAL:
            //     {
            //         Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
            //         SerializedProperty valueProperty = property.FindPropertyRelative("value");
            //         EditorGUI.PropertyField(valueRect, valueProperty);
            //
            //         Rect effectTargetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
            //         SerializedProperty effectTargetProperty = property.FindPropertyRelative("effectTarget");
            //         EditorGUI.PropertyField(effectTargetRect, effectTargetProperty);
            //         break;
            //     }
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty propertyKeyProperty = property.FindPropertyRelative("propertyKey");

            PropertyKey propertyKey = (PropertyKey)propertyKeyProperty.enumValueIndex;

            switch (propertyKey)
            {
                case PropertyKey.NONE:
                    return EditorGUIUtility.singleLineHeight * 2;
                default:
                    return EditorGUIUtility.singleLineHeight * 5;
                
                // case EffectType.NONE:
                //     return EditorGUIUtility.singleLineHeight * 2;
                // case EffectType.ATTACK:
                //     return EditorGUIUtility.singleLineHeight * 4;
                // case EffectType.SIZE:
                //     return EditorGUIUtility.singleLineHeight * 4;
                // default:
                //     throw new ArgumentOutOfRangeException();
            }
        }
    }
}

