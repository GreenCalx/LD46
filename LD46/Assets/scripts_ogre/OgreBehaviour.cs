using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreBehaviour : MonoBehaviour
{
    public int food;
    public int moral;

    // Start is called before the first frame update
    void Start()
    {
        this.food   = Constants.MAX_FOOD;
        this.moral  = Constants.MAX_MORAL;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
