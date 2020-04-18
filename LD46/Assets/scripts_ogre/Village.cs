using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    // Prefabs
    public GameObject villager_ref;

    // const TWEAKS
    public const int START_POP  = 6;
    public const int MAX_FOOD   = 100;
    public const int MAX_MORAL  = 100;
    public const float time_step = 50f; // 5 seconds

    // STATE
    public int food;
    public int moral;
    public List<GameObject> villagers;

    // TIME MGT
    private float last_job_update;
    

    // Start is called before the first frame update
    void Start()
    {   
        this.food    = MAX_FOOD;
        this.moral   = MAX_MORAL;

        villagers = new List<GameObject>(START_POP);
        
        for (int i=0; i<START_POP; i++)
        {
            GameObject new_villager = Instantiate(villager_ref) as GameObject;
            this.villagers.Add( new_villager );
        }

        last_job_update = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - last_job_update >= time_step)
        {
            Debug.Log("DO JOB");
            foreach(GameObject v_go in villagers)
            {
                Villager v = v_go.GetComponent<Villager>();
                if (!!v)
                    v.doJob();
            }
        }

    }//! Update
}
