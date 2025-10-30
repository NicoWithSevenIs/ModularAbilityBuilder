using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpriteDebugger : MonoBehaviour
{
    [SerializeField] internal Image image;
    [SerializeField] internal int id;
    internal uint ID { get => (uint)id; }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SpriteDebugger))]
public class SpriteDebuggerEditor: Editor
{
    public async override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (SpriteDebugger)target;

        if (GUILayout.Button("Spawn"))
        {
            script.image.sprite = await SpriteRetriever.Instance.GetSprite(script.ID);
        }
    }
}



#endif 