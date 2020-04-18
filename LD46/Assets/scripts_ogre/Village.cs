using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    // Prefabs
    public GameObject villager_ref;
    public GameObject village_center;

    // STATE
    public int food;
    public int moral;
    public List<GameObject> villagers;

    // TIME MGT
    private float last_job_update;
    
    public void spawnVillager()
    {
        GameObject new_villager = Instantiate(villager_ref) as GameObject;
        new_villager.transform.position = village_center.transform.position;
        this.villagers.Add( new_villager );
    }

    // Start is called before the first frame update
    void Start()
    {   
        this.food    = Constants.MAX_FOOD;
        this.moral   = Constants.MAX_MORAL;

        villagers = new List<GameObject>(Constants.START_POP);
        
        for (int i=0; i<Constants.START_POP; i++)
        {
            //GameObject new_villager = Instantiate(villager_ref) as GameObject;
            //this.villagers.Add( new_villager );
            this.spawnVillager();
        }

        last_job_update = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // UPDATE JOBS
        if (Time.time - last_job_update >= Constants.job_time_step)
        {
            Debug.Log("DO JOB");
            foreach(GameObject v_go in villagers)
            {
                Villager v = v_go.GetComponent<Villager>();
                if (!!v)
                    v.doJob();
            }
            last_job_update = Time.time;
        }

        // UPDATE VILLAGER POSITION


    }//! Update

    void OnTriggerExit2D( Collider2D other)
    {
        Villager v = other.GetComponent<Villager>();
        if (!!v) // a villager is at the limit of the village
        {
            Debug.Log("outta boundz");
            v.revertDirection();
        }
    }
}
