///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CustomEditor(typeof(GlobalEvent), editorForChildClasses: true)]
    public class GlobalEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GlobalEvent e = target as GlobalEvent;
            if (GUILayout.Button("Raise"))
                e.Raise();
            GUILayout.Label("" + e.GetSubscribers());
        }
    }
}
#endif
