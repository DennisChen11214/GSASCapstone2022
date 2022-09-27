using UnityEditor;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CustomEditor(typeof(TransformGlobalEvent), editorForChildClasses: true)]
    public class TransformGlobalEventEditor : Editor
    {
        [SerializeField]
        Transform value;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            TransformGlobalEvent se = target as TransformGlobalEvent;
            value = EditorGUILayout.ObjectField("Object", value, typeof(Transform), true) as Transform;
            if (GUILayout.Button("Raise"))
                se.Raise(value);
        }
    }
}
