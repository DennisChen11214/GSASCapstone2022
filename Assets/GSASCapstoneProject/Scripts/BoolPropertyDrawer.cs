///
/// Created by Dennis Chen
/// Reference: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
///

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoolAttribute))]
public class BoolPropertyDrawer : PropertyDrawer
{
    // Reference to the attribute on the property.
    BoolAttribute boolAttribute;

    // Field that is being compared.
    SerializedProperty comparedField;

    // The object that the comparedField comes from
    SerializedObject parent;

    // Height of the property.
    private float propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        boolAttribute = attribute as BoolAttribute;

        if(boolAttribute.VarParent != "")
        {
            Object objectRef = property.serializedObject.FindProperty(boolAttribute.VarParent).objectReferenceValue;
            if (!objectRef) return;
            parent = new SerializedObject(property.serializedObject.FindProperty(boolAttribute.VarParent).objectReferenceValue);
            comparedField = parent.FindProperty(boolAttribute.BoolVarName);
        }
        else
        {
            comparedField = property.serializedObject.FindProperty(boolAttribute.VarParent + boolAttribute.BoolVarName);
        }
        bool comparedFieldValue = comparedField.boolValue == boolAttribute.ComparedBoolValue;

        // The height of the property should be defaulted to the default height.
        propertyHeight = base.GetPropertyHeight(property, label);

        // If the condition is met, simply draw the field. Else...
        if (comparedFieldValue)
        {
            EditorGUI.PropertyField(position, property, label);
        }
        else
        {
            propertyHeight = 0f;
        }
    }
}
#endif