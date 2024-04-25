using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    // Finds a child by path (e.g., "Object1/Child2/Grandchild3")
    public static Transform FindDeepChild(this Transform parent, string path)
    {
        var currentTransform = parent;
        string[] paths = path.Split('/');

        foreach (string p in paths)
        {
            currentTransform = currentTransform.Find(p);
            if (currentTransform == null) return null;
        }

        return currentTransform;
    }
    public static void SetLocalTransform(GameObject target, Vector3 localPosition, Vector3 localRotation, Vector3 localScale)
    {
        target.transform.position = Vector3.zero;
        target.transform.rotation = Quaternion.identity;
        target.transform.localPosition = localPosition;
        target.transform.localRotation = Quaternion.Euler(localRotation.x, localRotation.y, localRotation.z);
        target.transform.localScale = localScale;
    }
}