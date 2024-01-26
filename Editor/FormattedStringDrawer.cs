using System.Reflection;
using Microsoft.SqlServer.Server;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Halcyon
{
    [CustomPropertyDrawer(typeof(FormattedString))]
    public class FormattedStringDrawer : PropertyDrawer
    {
        private bool expanded = true;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
           
            
            
            var baseText = property.FindPropertyRelative("baseText");
            EditorGUILayout.LabelField(property.displayName);
            baseText.stringValue = EditorGUILayout.TextArea(baseText.stringValue, new GUILayoutOption[]
            {
                GUILayout.Height(100),
            });

            var useIndices = property.FindPropertyRelative("useIndices");
            EditorGUILayout.PropertyField(useIndices);

            GUI.enabled = false;
            if (useIndices.boolValue)
            {
                EditorGUILayout.TextField("Example Text: Deals {0} damage the target.");
            }
            else
            {
               
                EditorGUILayout.TextField("Example Text: Deals {fireDamage} damage the target.");
            }
            GUI.enabled = true;
            expanded = EditorGUILayout.Foldout(expanded, "String Keys", true);

            if (expanded)
            {
                var collection = property.FindPropertyRelative("dropDownCollection");
                for (int i = 0; i < collection.arraySize; i++)
                {
                    if (i % 4 == 0 && i != 0)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    if (i % 4 == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    int index = i;
                    GUILayout.Label($"({index}): {collection.GetArrayElementAtIndex(index).stringValue}");

                }
                if(collection.arraySize> 0)
                    EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Clear Values"))
                {
                    var obj = (FormattedString)property.GetUnderlyingValue().ConvertTo(typeof(FormattedString));
                    obj.ClearValues();
                }

            }




        }
    }
}
