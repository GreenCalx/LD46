using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Buildings
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool canWorkHere( jobs.Job iJob )
    {
        return string.Equals( iJob.getJobName(), Constants.farmer_job_name); 
    }

}
