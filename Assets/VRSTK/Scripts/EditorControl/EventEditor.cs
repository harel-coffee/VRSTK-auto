﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRSTK.Scripts.Telemetry;

namespace VRSTK
{
    namespace Scripts
    {
        namespace EditorControl
        {
            ///<summary>Property drawer for defining Event Parameters. Can also block the changing of Events that were Auto-generated</summary>
            [CustomPropertyDrawer(typeof(EventParameter))]
            public class EventEditor : PropertyDrawer
            {

                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    string[] choices = new string[EventTypeChecker.allowedTypes.Length]; //Contains options for type selections

                    for (int i = 0; i < EventTypeChecker.allowedTypes.Length; i++)
                    {
                        choices[i] = EventTypeChecker.allowedTypes[i].ToString();
                    }

                    EditorGUI.indentLevel++;



                    if (property.FindPropertyRelative("hideFromInspector").boolValue == false)
                    {
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 17), property.FindPropertyRelative("name"));
                        property.FindPropertyRelative("typeIndex").intValue = EditorGUI.Popup(new Rect(position.x, position.y + 20f, position.width, 17), property.FindPropertyRelative("typeIndex").intValue, choices);
                    }
                    else
                    {
                        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 17), property.FindPropertyRelative("name").stringValue);
                    }

                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return 50.0f;
                }

            }
        }
    }
}
