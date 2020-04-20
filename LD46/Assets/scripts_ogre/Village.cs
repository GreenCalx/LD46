using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class Village : MonoBehaviour
{
    // Prefabs
    public GameObject villager_ref;
    public GameObject village_center;
    public GameObject conveyor_belt;
    public GameObject house_ref;
    public GameObject audio_manager_ref;
    // STATE
    public int food;
    public int moral;
    public int tried_to_mate_n_times;
    public List<GameObject> villagers;
    public List<GameObject> houses;

    // TIME MGT
    private float last_job_update;
    private float last_time_villagers_ate;
    private float last_time_moral_updated;
    private float last_time_tried_mate;

    public void refreshAudioManager()
    {
        if (audio_manager_ref==null)
        {
            audio_manager_ref = GameObject.Find(Constants.AUDIO_MANAGER_GO_NAME);
        }
    }

    public void spawnVillager()
    {
        GameObject new_villager = Instantiate(villager_ref) as GameObject;
        new_villager.transform.position = village_center.transform.position;
        this.villagers.Add( new_villager );
    }

    public void spawnVillager(Vector2 iLocation)
    {
        GameObject new_villager = Instantiate(villager_ref) as GameObject;
        new_villager.transform.position = iLocation;
        this.villagers.Add( new_villager );
    }

    public void spawnHouse()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        Vector3 spawnPosition = new Vector3(
               Random.Range(bounds.min.x + 0.1f, bounds.max.x - 0.1f),
               Random.Range(bounds.min.y + 0.1f, bounds.max.y - 0.1f),
               Random.Range(bounds.min.z + 0.1f, bounds.max.z - 0.1f)
           );
        GameObject new_house = Instantiate(house_ref, spawnPosition, Quaternion.identity) as GameObject;
        houses.Add(new_house);
    }

    public void removeVillager(GameObject iGo)
    {
        villagers.Remove(iGo);
        this.moral = Mathf.Max( this.moral - Constants.VILLAGE_SACRIFICE_MORAL_LOSS, 0);
    }

    public void sendVillagerToBelt(Villager v)
    {
        BeltConveyor bc = conveyor_belt.GetComponent<BeltConveyor>();
        if(!!bc){
            v.destination   = bc.getLoadingPoint();
            v.go_to_belt    = true;
        }

        // SFX
        refreshAudioManager();
        if (!!audio_manager_ref)
        {
            AudioManager am = audio_manager_ref.GetComponent<AudioManager>();
            int selected_sound = Random.Range( 0, 2);
            string sound_name_to_play = 
                (selected_sound == 0) ? Constants.WHY_ME_VOICE : Constants.ALRIGHT_LETS_GO_VOICE;

            am.Play( sound_name_to_play );
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

    public void updateVillageMoral()
    {
        int moral_update = 0;

        // broko house
        foreach(GameObject house_go in houses)
        {
            House h = house_go.GetComponent<House>();
            if (!!h)
            {
                if (h.state == House.HOUSE_STATE.HALF_DAMAGED)
                    moral_update -= Constants.MORAL_LOSS_HALF_BROKEN_HOUSE;
                else if (h.state == House.HOUSE_STATE.DESTROYED)
                    moral_update -= Constants.MORAL_LOSS_BROKEN_HOUSE;
            }
        }//! foreach house

        // famine
        if ( this.food < Constants.VILLAGE_FAMINE_TRH )
        {
            moral_update -= Constants.VILLAGE_FAMINE_MORAL_LOSS;
        } else {
            moral_update += Constants.VILLAGE_FAMINE_MORAL_LOSS;
        }
        this.moral = Mathf.Min( this.moral + moral_update, Constants.MAX_MORAL);
        this.moral = Mathf.Max( this.moral, 0);
    }

    public void birth( Vector2 iBirthLocation )
    {
        spawnVillager(iBirthLocation);
        // SFX?
    }
    public bool mateHappens()
    {
        int n_proba = Constants.villager_mate_proba.Length;
        int proba_index = Mathf.Min( this.tried_to_mate_n_times, n_proba - 1);
        double rand_res = Random.Range(0f,1f);
        return (rand_res < Constants.villager_mate_proba[proba_index]);
    }

    public void tryMate()
    {
        // check if high moral
        if ( this.moral < Constants.VILLAGE_MORAL_REQ_TO_MATE )
            return;

        // check if M + F available
        bool hasMale = false, hasFemale = false;
        foreach( GameObject villager_go in villagers )
        {
            Villager v = villager_go.GetComponent<Villager>();
            if (!!v)
            {
                hasMale     = (v.sex == Villager.SEX.Male)   ? true : hasMale;
                hasFemale   = (v.sex == Villager.SEX.Female) ? true : hasFemale;
            }
            if ( !!hasMale && !!hasFemale )
                break;
        }
        if ( !hasMale || !hasFemale )
            return;

        // Check if it happens probastically
        if ( !mateHappens() )
        {
            this.tried_to_mate_n_times++;
            return;
        }

        /// MATE OK
        // > Select 2 villagers ( M + F )
        List<GameObject> males_only   = getMalesInVillage();
        List<GameObject> females_only = getFemalesInVillage(); // could do substract instead..
        males_only      = filterVillagerOnBelt(males_only);
        females_only    = filterVillagerOnBelt(females_only);
        males_only      = filterVillagerGoToBelt(males_only);
        females_only    = filterVillagerGoToBelt(females_only);
        males_only      = filterVillagerNotChangingJob(males_only);
        females_only    = filterVillagerNotChangingJob(females_only);
        males_only      = filterVillagerNotAlreadyMating(males_only);
        females_only    = filterVillagerNotAlreadyMating(females_only);

        if ( (males_only.Count == 0) || (females_only.Count == 0) )
            return;

        int male_to_select = Random.Range(0, males_only.Count);
        int female_to_select = Random.Range(0, females_only.Count);
        
        GameObject selected_male   = males_only[male_to_select];
        GameObject selected_female = females_only[female_to_select];

        // > Make them run to each other by setting trying_to_mate
        Villager selected_male_v   = selected_male.GetComponent<Villager>();
        Villager selected_female_v = selected_female.GetComponent<Villager>();
        selected_male_v.trying_to_mate   = true;
        selected_female_v.trying_to_mate = true;
        selected_male_v.destination = selected_female.transform;
        selected_female_v.destination = selected_male.transform;

        // > reset proba and exit method
        this.tried_to_mate_n_times = 0;
        return;

        // >> When 2 villagers collider in 'mate' state they produce a child ( random villager )
        // >> When they collide we pop new villager at collide location
    }

    public List<GameObject> getMalesInVillage()
    {
        return villagers.Where( e => (e.GetComponent<Villager>().sex == Villager.SEX.Male) ).ToList();
    }
    public List<GameObject> getFemalesInVillage()
    {
        return villagers.Where( e => (e.GetComponent<Villager>().sex == Villager.SEX.Female) ).ToList();
    }
    public List<GameObject> filterVillagerOnBelt( List<GameObject> iListToFilter )
    {
        return iListToFilter.Where( e => (e.GetComponent<Villager>().is_on_belt == false) ).ToList();
    }
    public List<GameObject> filterVillagerGoToBelt( List<GameObject> iListToFilter )
    {
        return iListToFilter.Where( e => (e.GetComponent<Villager>().go_to_belt == false) ).ToList();
    }
    public List<GameObject> filterVillagerNotChangingJob( List<GameObject> iListToFilter )
    {
        return iListToFilter.Where( e => (e.GetComponent<Villager>().changing_job == false) ).ToList();
    }
    public List<GameObject> filterVillagerNotAlreadyMating( List<GameObject> iListToFilter )
    {
        return iListToFilter.Where( e => (e.GetComponent<Villager>().trying_to_mate == false) ).ToList();
    }

    public int countMaleKings()
    {
        List<GameObject> males = getMalesInVillage();
        return males.Where( e => string.Equals(e.GetComponent<Villager>().job_str, Constants.king_job_name) ).ToList().Count;
    }
    public int countFemaleKings()
    {
        List<GameObject> females = getFemalesInVillage();
        return females.Where( e => string.Equals(e.GetComponent<Villager>().job_str, Constants.king_job_name) ).ToList().Count;
    }

    // Start is called before the first frame update
    void Start()
    {   
        refreshAudioManager();

        this.food    = Constants.MAX_FOOD;
        this.moral   = Constants.MAX_MORAL;
        this.tried_to_mate_n_times = 0;

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
        last_time_moral_updated = Time.time;
        last_time_tried_mate = Time.time;
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
            
            // kill random villager if no more food
            if ( this.food <= 0 )
            {
                List<GameObject> eligible_v_to_kill   = filterVillagerOnBelt(villagers);
                eligible_v_to_kill  = filterVillagerGoToBelt(eligible_v_to_kill);
                int to_kill_index = Random.Range(0, eligible_v_to_kill.Count);
                GameObject selected_to_kill_go = eligible_v_to_kill[to_kill_index];

                Villager selected_to_kill = selected_to_kill_go.GetComponent<Villager>();
                if (!!selected_to_kill)
                    selected_to_kill.Kill();
            }

            last_time_villagers_ate = Time.time;
        }

        // Update village moral
        if (Time.time - last_time_moral_updated >= Constants.village_moral_time_step)
        {
            Debug.Log("UPDATE VILLAGE MORAL");
            updateVillageMoral();
            last_time_moral_updated = Time.time;
        }

        if (Time.time - last_time_tried_mate >= Constants.village_mate_time_step)
        {
            Debug.Log("TRY MATE");
            tryMate();
            last_time_tried_mate = Time.time;
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
