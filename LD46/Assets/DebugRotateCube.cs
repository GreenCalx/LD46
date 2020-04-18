using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRotateCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            // rotate cube 90 degrees
            transform.RotateAround(Vector3.up, 0.1f * Mathf.Deg2Rad*(90));
        }        
    }
}
