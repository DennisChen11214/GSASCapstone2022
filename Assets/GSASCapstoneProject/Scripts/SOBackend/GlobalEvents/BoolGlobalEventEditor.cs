using UnityEditor;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CustomEditor(typeof(BoolGlobalEvent), editorForChildClasses: true)]
    public class BoolGlobalEventEditor : Editor
    {
        bool value = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            BoolGlobalEvent be = target as BoolGlobalEvent;
            value = EditorGUILayout.Toggle("Value", value);
            if (GUILayout.Button("Raise"))
                be.Raise(value);
        }
    }
}
