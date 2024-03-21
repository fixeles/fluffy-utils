using UnityEditor;
using UnityEngine;

namespace FPS
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class GetAttribute : PropertyAttribute
    {
        public readonly bool ShowInInspector;

        public GetAttribute(bool showInInspector = false)
        {
            ShowInInspector = showInInspector;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GetAttribute))]
    public class GetAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                if (property.serializedObject.targetObject is Component owner)
                {
                    string typeName = property.type.Replace("PPtr<$", "").TrimEnd('>');
                    property.objectReferenceValue = owner.GetComponent(typeName);
                    if (property.objectReferenceValue == null)
                        Debug.LogWarning($"{owner.name} doesn't contain a {typeName}");
                }
            }

            if (attribute is not GetAttribute { ShowInInspector: true })
                return;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is GetAttribute { ShowInInspector: true })
                return base.GetPropertyHeight(property, label);

            return 0;
        }
    }
#endif
}