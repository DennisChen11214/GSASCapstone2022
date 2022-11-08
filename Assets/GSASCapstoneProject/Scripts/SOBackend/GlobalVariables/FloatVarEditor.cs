///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.GlobalVariables
{
    [CustomEditor(typeof(FloatVariable), editorForChildClasses: true)]
    public class FloatVarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            FloatVariable fv = target as FloatVariable;
            if (GUILayout.Button("Raise"))
                fv.Raise(fv.Value);
            GUILayout.Label("" + fv.GetSubscribers());
        }
    }
}
#endif
