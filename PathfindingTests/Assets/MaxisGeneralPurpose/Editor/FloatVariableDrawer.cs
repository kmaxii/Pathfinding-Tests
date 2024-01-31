using Scriptable_objects;
using UnityEditor;
using UnityEngine;

namespace MaxisGeneralPurpose.Editor
{
    [CustomPropertyDrawer(typeof(FloatVariable))]
    public class FloatVariableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Start the property drawing
            EditorGUI.BeginProperty(position, label, property);

            // Split the position Rect into two parts
            Rect objectFieldRect = new Rect(position.x, position.y, position.width * 0.8f, position.height);
            Rect valueFieldRect = new Rect(position.x + position.width * 0.8f, position.y, position.width * 0.2f, position.height);

            // Draw the field to assign the ScriptableObject instance
            EditorGUI.PropertyField(objectFieldRect, property, label, true);

            // Draw the field for the default value, but only if the ScriptableObject is not null
            if (property.objectReferenceValue != null)
            {
                var floatVar = (FloatVariable)property.objectReferenceValue;
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.FloatField(valueFieldRect, "", floatVar.Value);
                if (EditorGUI.EndChangeCheck())
                {
                    // Set the new value if it has changed
                    floatVar.Value = newValue;
                    EditorUtility.SetDirty(floatVar); // Mark the object as dirty to ensure the change is saved
                }
            }

            // End the property drawing
            EditorGUI.EndProperty();
        }
    }
}