///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

#if UNITY_EDITOR
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
            GUILayout.Label("" + ie.GetSubscribers());
        }
    }
}
#endif
