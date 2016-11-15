using UnityEngine;
using System.Collections;

public class ExtensionMethods
{

    public static void Reset(this GameObject gameObject)
    {
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
    }
}
