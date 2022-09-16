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
        }
    }
}
