///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.GlobalVariables
{
    [CustomEditor(typeof(StringVariable), editorForChildClasses: true)]
    public class StringVarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            StringVariable sv = target as StringVariable;
            if (GUILayout.Button("Raise"))
                sv.Raise(sv.Value);
            GUILayout.Label("" + sv.GetSubscribers());
        }
    }
}
#endif
