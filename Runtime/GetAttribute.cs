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
    public class ComponentSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObject is Component owner)
            {
                string typeName = property.type.Replace("PPtr<$", "").TrimEnd('>');
                var getComponent = owner.GetComponent(typeName);
                bool isTypeAvailable = property.propertyType == SerializedPropertyType.ObjectReference;
                bool isNullRef = isTypeAvailable && property.objectReferenceValue == null;
                var isAnotherGameObjectComponent = isTypeAvailable && !ShowInInspector && property.objectReferenceValue != getComponent;
                if (isNullRef || isAnotherGameObjectComponent)
                {
                    property.objectReferenceValue = getComponent;
                    if (property.objectReferenceValue == null)
                        Debug.LogWarning($"{owner.name} doesn't contain a {typeName}");
                }
            }
            
            if (!ShowInInspector)
                return;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShowInInspector)
                return base.GetPropertyHeight(property, label);

            return 0;
        }

        private bool ShowInInspector => attribute is GetAttribute { ShowInInspector: true };
    }
#endif
}