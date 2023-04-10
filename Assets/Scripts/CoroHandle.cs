using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroHandle 
{
    private static readonly GameObject gameObject = new GameObject("CoroHandle");

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() {
        gameObject.AddComponent<CoroComponent>();
        MonoBehaviour.DontDestroyOnLoad(gameObject);
    }

    public class CoroComponent : MonoBehaviour
    {
        
    }
}
