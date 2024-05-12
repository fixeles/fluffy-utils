using UnityEditor;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// Draws the dictionary and a warning-box if there are duplicate keys.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float VertSpace = EditorGUIUtility.standardVerticalSpacing;
        private const float WarningBoxHeight = 1.5f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw list of key/value pairs.
            var list = property.FindPropertyRelative("list");
            EditorGUI.PropertyField(position, list, label, true);

            // Draw key collision warning.
            var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
            if (keyCollision)
            {
                position.y += EditorGUI.GetPropertyHeight(list, true);
                if (!list.isExpanded)
                {
                    position.y += VertSpace;
                }
                position.height = LineHeight * WarningBoxHeight;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.HelpBox(position, "Duplicate keys will not be serialized.", MessageType.Warning);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Height of KeyValue list.
            float height = 0f;
            var list = property.FindPropertyRelative("list");
            height += EditorGUI.GetPropertyHeight(list, true);

            // Height of key collision warning.
            bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
            if (keyCollision)
            {
                height += WarningBoxHeight * LineHeight;
                if (!list.isExpanded)
                {
                    height += VertSpace;
                }
            }
            return height;
        }
    }
}