using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.FadingEffect.Boss.Editor
{
    [CustomEditor(typeof(BossHealthFiller))]
    public class BossUIBuilder : UnityEditor.Editor
    {
        BossHealthFiller bossHealthFiller;
        public override void OnInspectorGUI()
        {
            bossHealthFiller = (BossHealthFiller)target;
            
            DrawDefaultInspector();
            if (GUILayout.Button("Create New Stage"))
            {
                Debug.Log("Creating new stage");
                bossHealthFiller.CreateNewStage();
            }
        }
    }
}