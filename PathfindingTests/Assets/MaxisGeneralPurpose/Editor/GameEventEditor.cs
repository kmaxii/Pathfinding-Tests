using MaxisGeneralPurpose.Scriptable_objects;
using UnityEditor;
using UnityEngine;

namespace MaxisGeneralPurpose.Editor
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameEvent gameEvent = (GameEvent)target;

            if (GUILayout.Button("Raise"))
            {
                gameEvent.Raise();
            }
        }
    }
}