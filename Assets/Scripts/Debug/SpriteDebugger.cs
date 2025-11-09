using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpriteDebugger : MonoBehaviour
{
    [SerializeField] internal Image image;
    [SerializeField] internal int id;
    [SerializeField] internal uint ID { get => (uint)id; }
    [SerializeField] internal bool show_front = true;

}


#if UNITY_EDITOR

[CustomEditor(typeof(SpriteDebugger))]
public class SpriteDebuggerEditor: Editor
{
    public override async void OnInspectorGUI()
    {
        var script = (SpriteDebugger)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Set Sprite"))
        {
            script.image.sprite = await SpriteRetriever.Instance.GetSprite(script.ID, script.show_front);
        }


    }
}

#endif

