using UnityEngine;

public class InputManagerDontDestroyOnLoad : MonoBehaviour
{
    private static InputManagerDontDestroyOnLoad instanceRef;

    void Awake()
    {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}
