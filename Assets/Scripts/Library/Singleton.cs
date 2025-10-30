using System;
using UnityEngine;

public  abstract class Singleton<T> where T : class
{
    public static T Instance { get => instance ?? (instance = Activator.CreateInstance(typeof(T)) as T); }

    private static T instance ;
}
