using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Ref: https://www.youtube.com/watch?v=LFOXge7Ak3E&ab_channel=git-amend
 */

public abstract class M_Singleton<T> : MonoBehaviour where T: Component
{
    protected static T instance = null;

    protected static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if(instance == null)
                {
                    var go = new GameObject(typeof(T).Name + "Singleton");
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
       InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;
        instance = this as T;
    }
}
