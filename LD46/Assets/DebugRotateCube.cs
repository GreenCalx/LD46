using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRotateCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool running = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            running = !running;
        }
            // rotate cube 90 degrees
            if(running)
            transform.RotateAround(Vector3.forward, -0.1f * Mathf.Deg2Rad*(90) * Time.deltaTime);
   
    }
}
