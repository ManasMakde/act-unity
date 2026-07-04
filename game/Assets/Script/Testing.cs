using System;
using UnityEngine;

public class MyThing : MonoBehaviour
{
    void FixedUpdate()
    {
        Debug.Log("Fixed Update!");
    }
    void Update()
    {
        Debug.Log("Update!");
    }
    void LateUpdate()
    {
        Debug.Log("Late Update!");
    }

}