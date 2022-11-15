///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CustomEditor(typeof(ObjectGlobalEvent), editorForChildClasses: true)]
    public class ObjectGlobalEventEditor : Editor
    {
        [SerializeField]
        Object value;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            ObjectGlobalEvent oe = target as ObjectGlobalEvent;
            value = EditorGUILayout.ObjectField("Object", value, typeof(Object), true) as Object;
            if (GUILayout.Button("Raise"))
                oe.Raise(value);
            GUILayout.Label("" + oe.GetSubscribers());
        }
    }
}
#endif
