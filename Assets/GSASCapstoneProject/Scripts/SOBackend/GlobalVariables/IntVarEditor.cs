///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///
using UnityEditor;
using UnityEngine;

namespace Core.GlobalVariables
{
    [CustomEditor(typeof(IntVariable), editorForChildClasses: true)]
    public class IntVarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            IntVariable iv = target as IntVariable;
            if (GUILayout.Button("Raise"))
                iv.Raise(iv.Value);
            GUILayout.Label("" + iv.GetSubscribers());
        }
    }
}
