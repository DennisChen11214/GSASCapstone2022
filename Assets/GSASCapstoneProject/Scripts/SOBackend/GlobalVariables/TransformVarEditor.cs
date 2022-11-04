///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///
using UnityEditor;
using UnityEngine;

namespace Core.GlobalVariables
{
    [CustomEditor(typeof(TransformVariable), editorForChildClasses: true)]
    public class TransformVarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            TransformVariable tv = target as TransformVariable;
            if (GUILayout.Button("Raise"))
                tv.Raise(tv.Value);
            GUILayout.Label("" + tv.GetSubscribers());
        }
    }
}
