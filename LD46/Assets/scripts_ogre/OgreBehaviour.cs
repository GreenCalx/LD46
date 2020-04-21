
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OgreBehaviour : MonoBehaviour
{

    // Main file todo list
    //  - cleanup variables and stop getting components everywhere
    //  - make default state machine assert or at least log that form => to does not exist
    //  - log state machine states, from, to, etc


    private BeltConveyor Belt;


    public int food;
    public int moral;
    public GameObject village_go_ref;
    public GameObject audio_manager_ref;

    public GameObject rightHand;
    public GameObject rightHandRestingPosition;
    public GameObject leftHand;
    public GameObject leftHandRestingPosition;
    public bool goUp = true;
    public bool goDown = false;

    public Sprite[] HandSprite;

    public GameObject[] Bones;

    public GameObject[] Puff;

    public GameObject mouth;
    private GameObject currentTarget;
    public bool needEat = false;
    public bool needAskFood = false;

    public GameObject rightHandAngryTopTarget;
    public GameObject leftHandAngryTopTarget;

    public float EatingAnimationDuration = 2; // 2 seconds animation
    public float CurrentAnimationTime = 0;

    public float last_time_rampage = 0f;
    public float CurrentFoodTick = 0;

    public enum States { EATING, REST, GETTING_TARGET, ANGRY, HUNGRY, RAMPAGE }
    public States currentState = States.REST;

    private Villager currentCommand;
    private bool has_a_command = false;


    // Start is called before the first frame update
    void Start()
    {
        var belt_go = GameObject.Find("conveyor_belt");
        Belt = belt_go ? belt_go.GetComponent<BeltConveyor>() : null;


        food = Constants.MAX_FOOD;
        moral = Constants.MAX_MORAL;

        village_go_ref = GameObject.Find(Constants.VILLAGE_GO_NAME);
    }

    // internal state mutators
    // use - value to remove
    public void AddMoral(int m)
    {
        // clamp value
        moral += m;
        Mathf.Clamp(moral, 0, Constants.MAX_MORAL);
    }

    public void AddFood(int f)
    {
        // clamp value
        food += f;
        food = Mathf.Clamp(food, 0, Constants.MAX_FOOD);
    }

    // This is used to update state when something is on the belt
    // it should be cleaner to make it happen inside belt and send an event...
    private void OnTriggerStay2D(Collider2D collision)
    {
        // this merans something on the bgelt is copming, start to go towards it to take it
        // first find collider
        if (currentState == States.REST || currentState == States.HUNGRY)
        {
            if (collision.gameObject.name != Constants.BELT)
            {
                currentTarget = collision.gameObject;
                UpdateState(States.GETTING_TARGET);
            }
        }
    }


    // do we need this???????
    public void refreshAudioManager()
    {
        if (audio_manager_ref == null)
        {
            audio_manager_ref = GameObject.Find(Constants.AUDIO_MANAGER_GO_NAME);
        }
    }


    // quick and dirty way to enable/disable all ogre sprites visualisations
    // TODO : should we use directly gameObject instead of only the SR of each object??
    public void DisableAllSR()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;
    }
    public void EnableAllSR()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = true;
    }

    // used by little ogre
    // better way to do this would be with event
    // NOTE : probably not needed anymore with new state machine, little ogre can simply call UpdateState
    public void ResetBehavior()
    {
        UpdateState(States.REST);
    }

    jobs.Job getJob(int job)
    {
        switch (job)
        {
            case 0: return new jobs.Beggar();
            case 1: return new jobs.Farmer();
            case 2: return new jobs.Builder();
            case 3: return new jobs.Cleric();
            case 4: return new jobs.Bard();
            case 5: return new jobs.King();
            default: return new jobs.Job();
        }
    }


    void DisplayNeededFood(int level, int job, int sex)
    {
        Debug.Log("NEED FOOD :" + level + " " + job + " " + sex);
        var ui = GetComponentInChildren<Canvas>();
        if (ui) ui.enabled = true;
        var icon = GetComponentInChildren<Image>();

        currentCommand = new Villager();
        currentCommand.sex = (Villager.SEX)sex;
        currentCommand.job = getJob(job);
        currentCommand.level = level;
        icon.sprite = currentCommand.job.getJobSprite(currentCommand.sex);

        has_a_command = true;
    }

    void AskFood()
    {
        // Get constraints ( villager max level, and thus associated job )
        if (this.village_go_ref == null)
            this.village_go_ref = GameObject.Find(Constants.VILLAGE_GO_NAME);
        Village village = this.village_go_ref.GetComponent<Village>();
        int max_lvl_in_village = village.getMaxLevelVillager();


        // randomize food type
        int food_level = Random.Range(0, max_lvl_in_village + 1);
        if (food_level > max_lvl_in_village) Debug.Log("BAD FOOD LEVEL");

        int n_available_jobs = jobs.Job.availableJobsForLevel(food_level); // available job for selected level
        int food_job = Random.Range(0, n_available_jobs);
        if (food_job > n_available_jobs) Debug.Log("BAD FOOD JOB");

        int food_sex = Random.Range(0, 2);

        DisplayNeededFood(food_level, food_job, food_sex);
        needAskFood = false;
    }

    void EatingAnimationStart()
    {
        // here is the animation process if needed
        CurrentAnimationTime = EatingAnimationDuration;
        needEat = false;
        needAskFood = false;

        AddFood(Constants.Villager_food);

        var ui = GetComponentInChildren<Canvas>();
        if (ui) ui.enabled = false;

        AudioManager.Instance.Play(Constants.EATING);

        GetComponent<Animator>().SetBool("isEating", true);
    }
    void EatingAnimationStop()
    {
        // here is the animation stopping proicess if needed
        GetComponent<Animator>().SetBool("isEating", false);

        rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[0];


        AudioManager.Instance.Stop(Constants.EATING);
    }
    void EatingAnimation()
    {
        // here is during eatig animation process if needed
        Debug.Log("NOM NOM NOM");
        CurrentAnimationTime -= Time.deltaTime;
        if (CurrentAnimationTime <= 0)
        {
            currentState = States.REST;
            CurrentAnimationTime = 0;
            EatingAnimationStop();
        }

        // randomly instanciate sprites for fun and profit
        if (CurrentAnimationTime % 0.5f > (CurrentAnimationTime + Time.fixedDeltaTime) % 0.5)
        {
            var go = Instantiate(Bones[Random.Range(1, Bones.Length)], mouth.transform.position, Quaternion.Euler(new Vector3(
                                                                                       0,
                                                                                       0,
                                                                                       UnityEngine.Random.Range(0, 360))));
            Destroy(go, 2);
            go.GetComponent<Rigidbody2D>().AddForce(new Vector3(1, 1, 0));

            //also spawn blood randomly next to mouth positoin
            var bounds = mouth.GetComponent<BoxCollider2D>().bounds;
            Vector3 spawnPosition = new Vector3(
                   Random.Range(bounds.min.x, bounds.max.x),
                   Random.Range(bounds.min.y, bounds.max.y),
                   Random.Range(bounds.min.z, bounds.max.z)
               );

            var go2 = Instantiate(Bones[0], spawnPosition, Quaternion.Euler(new Vector3(
                                                                                       0,
                                                                                       0,
                                                                                       UnityEngine.Random.Range(0, 360))));
            Destroy(go2, 0.5f);
        }

    }

    void FoodTick()
    {
        CurrentFoodTick -= Time.deltaTime;
        if (CurrentFoodTick <= 0)
        {
            AddFood(Constants.Ogre_Food_Tick_Loss);
            CurrentFoodTick = Constants.Ogre_Food_Tick_Time;
        }
    }

    void AngryAnimationStart()
    {
        goUp = true;
        goDown = false;


        rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[2];
        leftHand.GetComponent<SpriteRenderer>().sprite = HandSprite[3];
    }

    void AngryAnimation()
    {
        // toggle between top and rest hand positions
        // each time at rest position destroy village
        float threshold = 0.1f;
        if (goDown && Vector3.Distance(rightHand.transform.position, rightHandRestingPosition.transform.position) < threshold)
        {
            goUp = true;
            goDown = false;
            var go = GameObject.Find(Constants.VILLAGE_GO_NAME);
            if (go)
            {
                var script = go.GetComponent<Village>();
                script.DamageHouses();
                this.moral = (int)Mathf.Min(this.moral + Constants.OGRE_MORAL_GAIN_IN_ANGER, Constants.MAX_MORAL);

                // spawn puff cloud
                Instantiate(Puff[0], rightHand.transform.position - new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
                Instantiate(Puff[1], rightHand.transform.position + new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
                Instantiate(Puff[0], leftHand.transform.position - new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
                Instantiate(Puff[1], leftHand.transform.position + new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
            }
        }
        if (goUp && Vector3.Distance(rightHand.transform.position, rightHandAngryTopTarget.transform.position) < threshold)
        {
            goDown = true;
            goUp = false;
        }

        if (goUp)
        {
            rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, rightHandAngryTopTarget.transform.position, Constants.HandSpeed * Time.deltaTime);
            leftHand.transform.position = Vector3.MoveTowards(leftHand.transform.position, leftHandAngryTopTarget.transform.position, Constants.HandSpeed * Time.deltaTime);
        }

        if (goDown)
        {
            rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, rightHandRestingPosition.transform.position, Constants.HandSpeed * Time.deltaTime);
            leftHand.transform.position = Vector3.MoveTowards(leftHand.transform.position, leftHandRestingPosition.transform.position, Constants.HandSpeed * Time.deltaTime);
        }
    }

    int GetMoralFromCommand(Villager v)
    {
        int Result = 0;

        if (has_a_command == false)
            return Result;

        /*
                if (v.level < currentCommand.level) Result -= 10;
                if (v.level > currentCommand.level) Result += 10;
                if (v.sex != currentCommand.sex) Result -= 10;
                if (v.job != currentCommand.job) Result -= 30;
        */
        int delta_level = (v.level - currentCommand.level);
        Result += delta_level * Constants.OGRE_WRONG_LVL_COMMAND_PENALTY;

        int delta_sex = System.Convert.ToInt32(v.sex != currentCommand.sex);
        Result -= delta_sex * Constants.OGRE_WRONG_SEX_COMMAND_PENALTY;

        int delta_job = System.Convert.ToInt32(v.job != currentCommand.job);
        Result -= delta_job * Constants.OGRE_WRONG_JOB_COMMAND_PENALTY;

        return Result;
    }


    public void UpdateState(States newState)
    {
        // state machine transition
        switch (currentState)
        {
            case States.REST:
                switch (newState)
                {
                    case States.GETTING_TARGET:
                        {
                            needEat = false;
                        }
                        break;
                    case States.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case States.HUNGRY:
                        {
                            needAskFood = true;
                        }
                        break;
                    case States.RAMPAGE:
                        last_time_rampage = Time.time;
                        break;
                    default:
                        break;
                }

                break;
            case States.GETTING_TARGET:
                switch (newState)
                {
                    case States.EATING:
                        {
                            var v = currentTarget.GetComponent<Villager>();
                            if (v)
                            {
                                moral += GetMoralFromCommand(v);
                                moral = Mathf.Clamp(moral, 0, 100);
                                v.Kill();

                                // no more command
                                currentCommand = null;
                                has_a_command = false;
                            }

                            currentTarget.GetComponent<Villager>().Kill();
                            EatingAnimationStart();
                        }
                        break;
                    case States.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case States.RAMPAGE:
                        last_time_rampage = Time.time;
                        break;
                    default: break;
                }

                break;
            case States.EATING:
                switch (newState)
                {
                    case States.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case States.RAMPAGE:
                        last_time_rampage = Time.time;
                        break;
                    default: break;
                }
                break;
            case States.ANGRY:
                switch (newState)
                {
                    case States.REST:
                        {
                            GetComponent<Animator>().SetBool("isEating", false);

                            rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[0];
                            leftHand.GetComponent<SpriteRenderer>().sprite = HandSprite[0];

                        }
                        break;
                    case States.RAMPAGE:
                        last_time_rampage = Time.time;
                        break;
                    default: break;
                }
                break;
            case States.HUNGRY:
                switch (newState)
                {
                    case States.GETTING_TARGET:
                        {
                            needEat = false;
                        }
                        break;
                    case States.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case States.RAMPAGE:
                        last_time_rampage = Time.time;
                        break;
                    default: break;
                }
                break;
            case States.RAMPAGE:
                last_time_rampage = 0f;
                switch (newState)
                {
                    case States.REST:
                        {
                            moral = Constants.MAX_MORAL;
                            food = Constants.MAX_FOOD;

                            rightHand.transform.position = rightHandRestingPosition.transform.position;
                            leftHand.transform.position = leftHandRestingPosition.transform.position;

                            EnableAllSR();

                        }
                        break;
                    case States.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case States.RAMPAGE:
                        last_time_rampage = Time.time;
                        break;
                    default: break;
                }
                break;
        }
        currentState = newState;
    }

    private void UpdateSprites()
    {
        switch (currentState)
        {
            case States.GETTING_TARGET:
                {
                    if (!needEat)
                        rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[1];
                    else
                        rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[2];
                }
                break;
            default: break;
        }
    }

    private void UpdatePositions()
    {
        switch (currentState)
        {
            case States.GETTING_TARGET:
                {
                    if (!needEat)
                    {
                        // move right hand towards the incoming food
                        rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, currentTarget.transform.position, Constants.HandSpeed * Time.deltaTime);
                    }  // we have something to eat, let s go to the mouth position
                    else
                    {
                        rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, mouth.transform.position, Constants.HandSpeed * Time.deltaTime);
                    }
                }
                break;
            default: break;
        }
    }

    // Update is called once per frame

    // TODO move everything linked to physics in fixedUpdate!!!!!
    void Update()
    {
        // FROM ANY STATES
        if (food > Constants.ogre_hungry_threshold_start)
        {
            var ui = GetComponentInChildren<Canvas>();
            if (ui) ui.enabled = false;
        }
        if (moral < Constants.OGRE_ANGER_THR)
        {
            UpdateState(States.ANGRY);
        }
        if (food < Constants.ogre_rampage_threshold_start)
        {
            UpdateState(States.RAMPAGE);
        }

        // apply logic and update state accordingly
        switch (currentState)
        {
            case States.REST:
                {
                    FoodTick();
                    // from state rest to hungry
                    if (food < Constants.ogre_hungry_threshold_start)
                    {
                        UpdateState(States.HUNGRY);
                    }
                    // if we are not in the resting position, let s reset the position
                    if (rightHand.transform.position != rightHandRestingPosition.transform.position)
                    {
                        rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, rightHandRestingPosition.transform.position, Constants.HandSpeed * Time.deltaTime);
                        if (Vector3.Distance(rightHand.transform.position, rightHandRestingPosition.transform.position) < 0.1f)
                        {
                            // close enough we reset the position
                            rightHand.transform.position = rightHandRestingPosition.transform.position;
                        }
                    }
                }
                break;
            case States.GETTING_TARGET:
                {
                    FoodTick();
                    if (currentTarget)
                    {
                        UpdateSprites();
                        UpdatePositions();

                        if (!needEat && Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), currentTarget.GetComponent<BoxCollider2D>()))
                        {
                            // remove from conmveyor belt
                            if (Belt) Belt.removeOnBelt(currentTarget);

                            currentTarget.transform.position = rightHand.transform.position + new Vector3(0, 0.5f);
                            currentTarget.transform.parent = rightHand.transform;
                            needEat = true;
                        }
                        else if (needEat)
                        {
                            { // this should go in villager logic
                                // SFX play Oh nooooo...
                                refreshAudioManager();
                                if (!!audio_manager_ref)
                                {
                                    AudioManager.Instance.Play(Constants.OH_NO_VOICE);
                                }
                            }

                            if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), mouth.GetComponent<BoxCollider2D>()))
                            {
                                UpdateState(States.EATING);
                            }
                        }
                    }
                }
                break;
            case States.EATING:
                {
                    FoodTick();
                    EatingAnimation();
                }
                break;
            case States.ANGRY:
                {
                    FoodTick();
                    // destroy village buildings
                    AngryAnimation();
                    if (this.moral >= Constants.OGRE_GET_CALM_THR)
                    {
                        UpdateState(States.REST);


                    }

                }
                break;
            case States.HUNGRY:
                {
                    FoodTick();
                    // if we are not in the resting position, let s reset the position
                    if (rightHand.transform.position != rightHandRestingPosition.transform.position)
                    {
                        rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, rightHandRestingPosition.transform.position, Constants.HandSpeed * Time.deltaTime);
                        if (Vector3.Distance(rightHand.transform.position, rightHandRestingPosition.transform.position) < 0.1f)
                        {
                            // close enough we reset the position
                            rightHand.transform.position = rightHandRestingPosition.transform.position;
                        }
                    }
                    // ask for food
                    if (needAskFood) AskFood(); // to do only once
                }
                break;
            case States.RAMPAGE:
                {
                    // disable main ogre sprite
                    // activate little ogre sprite in village
                    // eat villagers until full
                    // NOTE : ResetBehavior is called by littleOgre when rampage is finished
                    GetComponent<SpriteRenderer>().enabled = false;
                    foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;

                    var go = GameObject.Find("littleOgre");
                    if (go)
                    {
                        var script = go.GetComponent<OgreRampage>();
                        if (script.currentState == OgreRampage.States.DEACTIVATED) script.Activate();
                    }
                    if (Time.time - last_time_rampage >= Constants.max_time_in_rampage)
                    {
                        // Exit rampage, its too long
                        UpdateState(States.REST);
                    }

                }
                break;
        }
    }
}
