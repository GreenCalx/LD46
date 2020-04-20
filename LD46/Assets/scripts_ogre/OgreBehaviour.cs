using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OgreBehaviour : MonoBehaviour
{
    public int food;
    public int moral;
    public GameObject village_go_ref;

    public GameObject rightHand;
    public GameObject rightHandRestingPosition;
    public GameObject leftHand;
    public GameObject leftHandRestingPosition;
    public bool goUp = false;
    public bool goDown = false;


    public Sprite[] HandSprite;

    public Sprite[] Bones;

    public GameObject mouth;
    private GameObject currentTarget;
    public bool needEat = false;
    public bool needAskFood = false;

    public GameObject rightHandAngryTopTarget;
    public GameObject leftHandAngryTopTarget;

    public float EatingAnimationDuration = 2; // 2 seconds animation
    public float CurrentAnimationTime = 0;

    public float CurrentFoodTick = 0;

    public enum States { EATING, REST, GETTING_TARGET, ANGRY, HUNGRY, RAMPAGE }
    public States currentState = States.REST;

    private Villager currentCommand;
    private bool has_a_command = false;

    // Start is called before the first frame update
    void Start()
    {
        this.food = Constants.MAX_FOOD;
        this.moral = Constants.MAX_MORAL;

        this.village_go_ref = GameObject.Find(Constants.VILLAGE_GO_NAME);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // this merans something on the bgelt is copming, start to go towards it to take it
        // first find collider
        if ( currentState != States.GETTING_TARGET)
        {
            if (collision.gameObject.name != "conveyor_belt")
            {
                currentTarget = collision.gameObject;
                currentState = States.GETTING_TARGET;
            }
        }
    }

    public void ResetBehavior()
    {
        this.moral = Constants.MAX_MORAL;
        this.food = Constants.MAX_FOOD;

        currentState = States.REST;

        rightHand.transform.position = rightHandRestingPosition.transform.position;
        leftHand.transform.position = leftHandRestingPosition.transform.position;

            GetComponent<SpriteRenderer>().enabled = true;
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = true;
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

    public void AddFood( int food)
    {
        this.food += food;
        this.food = Mathf.Clamp(this.food, 0, 100);
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
        if (this.village_go_ref==null)
            this.village_go_ref = GameObject.Find( Constants.VILLAGE_GO_NAME );
        Village village = this.village_go_ref.GetComponent<Village>();
        int max_lvl_in_village = village.getMaxLevelVillager();


        // randomize food type
        int food_level = (int) (Random.value * max_lvl_in_village);
        
        int n_available_jobs = jobs.Job.availableJobsForLevel( food_level ); // available job for selected level
        int food_job = (int)(Random.value * n_available_jobs);

        int food_sex = (int)(Random.value * 2);

        DisplayNeededFood(food_level, food_job, food_sex);
        needAskFood = false;
    }

    void EatingAnimationStart()
    {
        // here is the animation process if needed
        CurrentAnimationTime = EatingAnimationDuration;
        needEat = false;
        AddFood(Constants.Villager_food);

        rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[0];

        GetComponent<Animator>().SetBool("isEating", true);
    }
    void EatingAnimationStop()
    {
        // here is the animation stopping proicess if needed
        GetComponent<Animator>().SetBool("isEating", false);
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
        var go = Instantiate(Bones[Random.Range(0, Bones.Length)], mouth.transform.position, Quaternion.Euler(new Vector3(
                                                                                   UnityEngine.Random.Range(0,360),
                                                                                   UnityEngine.Random.Range(0,360),
                                                                                   UnityEngine.Random.Range(0,360))));
        Destroy(go, 2);

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
    }

    void AngryAnimation()
    {
        // toggle between top and rest hand positions
        // each time at rest position destroy village
        float threshold = 0.1f;
        if(goDown && Vector3.Distance( rightHand.transform.position, rightHandRestingPosition.transform.position ) < threshold )
        {
            goUp = true;
            goDown = false;
            var go = GameObject.Find(Constants.VILLAGE_GO_NAME);
            if (go)
            {
                var script = go.GetComponent<Village>();
                script.DamageHouses();
                this.moral = (int) Mathf.Min( this.moral + Constants.OGRE_MORAL_GAIN_IN_ANGER, Constants.MAX_MORAL);
            }
        }
        if ( goUp && Vector3.Distance(rightHand.transform.position, rightHandAngryTopTarget.transform.position) < threshold)
        {
            goDown = true;
            goUp = false;
        }

        if (goUp)
        {
           rightHand.transform.position =  Vector3.Lerp(rightHand.transform.position, rightHandAngryTopTarget.transform.position, Time.deltaTime);
           leftHand.transform.position =  Vector3.Lerp(leftHand.transform.position, leftHandAngryTopTarget.transform.position, Time.deltaTime);
        }

        if (goDown)
        {
           rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, rightHandRestingPosition.transform.position, Time.deltaTime);
           leftHand.transform.position =  Vector3.Lerp(leftHand.transform.position, leftHandRestingPosition.transform.position, Time.deltaTime);
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

        int delta_sex = System.Convert.ToInt32(v.sex != currentCommand.sex );
        Result -= delta_sex * Constants.OGRE_WRONG_SEX_COMMAND_PENALTY;

        int delta_job = System.Convert.ToInt32(v.job != currentCommand.job );
        Result -= delta_job * Constants.OGRE_WRONG_JOB_COMMAND_PENALTY;

        return Result;
    }
    // Update is called once per frame
    void Update()
    {
        if (currentState != States.RAMPAGE)
            // food update
            FoodTick();

        if (currentState == States.GETTING_TARGET)
        {

            rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[1];

            rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, currentTarget.transform.position, Time.deltaTime);
            if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), currentTarget.GetComponent<BoxCollider2D>()))
            {
                // remove from conmveyor belt
                var conveyor = GameObject.Find("conveyor_belt");
                var conveyor_script = conveyor ? conveyor.GetComponent<BeltConveyor>() : null;
                if (conveyor_script) conveyor_script.removeOnBelt(currentTarget);

                currentTarget.transform.position = rightHand.transform.position + new Vector3( 0, 0.5f );
                currentTarget.transform.parent = rightHand.transform;
                needEat = true;
            }


            if (needEat)
            {
                rightHand.GetComponent<SpriteRenderer>().sprite = HandSprite[2];

                // we have something to eat, let s go to the mouth position
                rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, mouth.transform.position, Time.deltaTime);
                if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), mouth.GetComponent<BoxCollider2D>()))
                {
                    currentState = States.EATING;
                    var v =currentTarget.GetComponent<Villager>();
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
            }

            
        }


        if (currentState == States.EATING) EatingAnimation();

        if (currentState == States.REST)
        {
            // from state rest to hungry
            if (food < 75)
            {
                currentState = States.HUNGRY;
                needAskFood = true;
            }

            // if we are not in the resting position, let s reset the position
            if (rightHand.transform.position != rightHandRestingPosition.transform.position)
            {
                rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, rightHandRestingPosition.transform.position, Time.deltaTime);
                if (Vector3.Distance(rightHand.transform.position, rightHandRestingPosition.transform.position) < 0.1f)
                {
                    // close enough we reset the position
                    rightHand.transform.position = rightHandRestingPosition.transform.position;
                }
            }

        }

        if (moral < Constants.OGRE_ANGER_THR && currentState != States.ANGRY )
        {
            currentState = States.ANGRY;
            AngryAnimationStart();
        }

        if (currentState == States.HUNGRY)
        {
            // ask for food
            if(needAskFood) AskFood(); // to do only once

            if (food < 25)
            {
                currentState = States.RAMPAGE;
            }
        }

        if (currentState == States.ANGRY)
        {
            // destroy village buildings
            AngryAnimation();
            if ( this.moral >= Constants.OGRE_GET_CALM_THR )
                currentState = States.REST;
                
        }

        if(currentState == States.RAMPAGE)
        {
            // disable main ogre sprite
            // activate little ogre sprite in village
            // eat villagers until full
            // NOTE : ResetBehavior is called by littleOgre when rampage is finished
            GetComponent<SpriteRenderer>().enabled = false;
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;

            var go =GameObject.Find("littleOgre");
            if (go)
            {
                var script = go.GetComponent<OgreRampage>();
                if (script.currentState == OgreRampage.States.DEACTIVATED) script.Activate();
            }
        }
    }
}
