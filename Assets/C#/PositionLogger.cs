using UnityEngine;

public class PositionLogger : MonoBehaviour
{
    void Start()
    {
        Debug.Log(gameObject.name + " World Position: " + transform.position);
        if (transform.parent != null)
            Debug.Log(gameObject.name + " Parent: " + transform.parent.name);
        else
            Debug.Log(gameObject.name + " has no parent.");
    }
}