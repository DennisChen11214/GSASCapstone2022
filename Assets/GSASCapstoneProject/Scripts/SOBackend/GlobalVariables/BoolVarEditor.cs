///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///
using UnityEditor;
using UnityEngine;

namespace Core.GlobalVariables
{
    [CustomEditor(typeof(BoolVariable), editorForChildClasses: true)]
    public class BoolVarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            BoolVariable bv = target as BoolVariable;
            if (GUILayout.Button("Raise"))
                bv.Raise(bv.Value);
            GUILayout.Label("" + bv.GetSubscribers());
        }
    }
}

