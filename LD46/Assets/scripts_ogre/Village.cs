using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Village : MonoBehaviour
{
    // Prefabs
    public GameObject villager_ref;
    public GameObject village_center;
    public GameObject conveyor_belt;
    public GameObject house_ref;
    // STATE
    public int food;
    public int moral;
    public List<GameObject> villagers;
    public List<GameObject> houses;

    // TIME MGT
    private float last_job_update;
    private float last_time_villagers_ate;
    
    public void spawnVillager()
    {
        GameObject new_villager = Instantiate(villager_ref) as GameObject;
        new_villager.transform.position = village_center.transform.position;
        this.villagers.Add( new_villager );
    }

    public void spawnHouse()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        Vector3 spawnPosition = new Vector3(
               Random.Range(bounds.min.x, bounds.max.x),
               Random.Range(bounds.min.y, bounds.max.y),
               Random.Range(bounds.min.z, bounds.max.z)
           );
        GameObject new_house = Instantiate(house_ref, spawnPosition, Quaternion.identity) as GameObject;
        houses.Add(new_house);
    }

    public void removeVillager(GameObject iGo)
    {
        villagers.Remove(iGo);
    }

    public void sendVillagerToBelt(Villager v)
    {
        BeltConveyor bc = conveyor_belt.GetComponent<BeltConveyor>();
        if(!!bc){
            v.destination   = bc.getLoadingPoint();
            v.go_to_belt    = true;
        }
    }

    // Returns the villager with highest levels
    public int getMaxLevelVillager()
    {
        int max_lvl_found = 0;
        foreach( GameObject v_go in villagers )
        {
            Villager v = v_go.GetComponent<Villager>();
            if (!!v)
                max_lvl_found = ( v.level > max_lvl_found ) ? v.level : max_lvl_found ;
        }
        return max_lvl_found;
    }

    public void DamageHouses()
    {
        foreach (GameObject house in houses)
        {
            var script = house.GetComponent<House>();
            if (script) script.Damage();
        }
    }
    // Start is called before the first frame update
    void Start()
    {   
        this.food    = Constants.MAX_FOOD;
        this.moral   = Constants.MAX_MORAL;

        villagers = new List<GameObject>(Constants.START_POP);
        houses = new List<GameObject>(Constants.START_HOUSE);
        
        for (int i=0; i<Constants.START_POP; i++)
        {
            //GameObject new_villager = Instantiate(villager_ref) as GameObject;
            //this.villagers.Add( new_villager );
            this.spawnVillager();
        }

        for (int i=0; i<Constants.START_HOUSE; i++)
        {
            //GameObject new_villager = Instantiate(villager_ref) as GameObject;
            //this.villagers.Add( new_villager );
            this.spawnHouse();
        }
        last_job_update = Time.time;
        last_time_villagers_ate = Time.time;
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

        // Villagers consume food
        if (Time.time - last_time_villagers_ate >= Constants.villagers_hungry_time_step)
        {
            Debug.Log("EAT FOOD");
            int food_to_consume = villagers.Count * Constants.VILLAGER_FOOD_NEED;
            this.food = (int) Mathf.Max( this.food - food_to_consume, 0);

            last_time_villagers_ate = Time.time;
        }

        // check if player is game over
        if ( villagers.Count == 0 )
            SceneManager.LoadScene(Constants.GAME_OVER_SCENE, LoadSceneMode.Single);

    }//! Update

    void OnTriggerExit2D( Collider2D other) 
    {
        // a villager is at the limit of the village
        Villager v = other.GetComponent<Villager>();
        if (!!v) 
        {
            v.revertDirection();
        }
    }
}
