using UnityEditor;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CustomEditor(typeof(IntGlobalEvent), editorForChildClasses: true)]
    public class IntGlobalEventEditor : Editor
    {
        int value = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            IntGlobalEvent ie = target as IntGlobalEvent;
            value = EditorGUILayout.IntField("Value", value);
            if (GUILayout.Button("Raise"))
                ie.Raise(value);
        }
    }
}
