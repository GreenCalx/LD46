using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OgreBehaviour : MonoBehaviour
{
    public int food;
    public int moral;

    public GameObject rightHand;
    public GameObject rightHandRestingPosition;
    public GameObject leftHand;
    public GameObject leftHandRestingPosition;
    public bool goUp = false;
    public bool goDown = false;

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

    // Start is called before the first frame update
    void Start()
    {
        this.food = Constants.MAX_FOOD;
        this.moral = Constants.MAX_MORAL;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this merans something on the bgelt is copming, start to go towards it to take it
        // first find collider
        if (collision.gameObject.name != "conveyor_belt")
        {
            currentTarget = collision.gameObject;
            currentState = States.GETTING_TARGET;
        }
    }

    public void ResetBehavior()
    {
        this.moral = Constants.MAX_MORAL;
        this.food = Constants.MAX_FOOD;

        currentState = States.REST;

        rightHand.transform.position = rightHandRestingPosition.transform.position;
        leftHand.transform.position = leftHandRestingPosition.transform.position;

            GetComponent<SpriteRenderer>().enabled = false;
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;

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

        Villager v = new Villager();
        v.sex = (Villager.SEX)sex;
        v.job = getJob(job);
        v.level = level;
        icon.sprite = v.job.getJobSprite(v.sex);
    }

    void AskFood()
    {
        // randomize food type
        int food_level = (int) (Random.value * 3);
        int food_job = (int)(Random.value * 6);
        int food_sex = (int)(Random.value * 2);

        DisplayNeededFood(food_level, food_job, food_sex);
        needAskFood = false;
    }

    void EatingAnimationStart()
    {
        // here is the animation process if needed
        CurrentAnimationTime = EatingAnimationDuration;
        needEat = false;

        food = Mathf.Min( food + Constants.Villager_food, 100);
    }
    void EatingAnimationStop()
    {
        // here is the animation stopping proicess if needed
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
        }
    }

    void FoodTick()
    {
        CurrentFoodTick -= Time.deltaTime;
        if (CurrentFoodTick <= 0)
        {
            food = Mathf.Min(0, food - Constants.Ogre_Food_Tick_Loss);
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
            var go = GameObject.Find("Village");
            if (go)
            {
                var script = go.GetComponent<Village>();
                script.DamageHouses();
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

    // Update is called once per frame
    void Update()
    {
        if (currentState != States.RAMPAGE)
            // food update
            FoodTick();

        if (currentState == States.GETTING_TARGET)
        {
            rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, currentTarget.transform.position, Time.deltaTime);
            if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), currentTarget.GetComponent<BoxCollider2D>()))
            {
                // remove from conmveyor belt
                var conveyor = GameObject.Find("conveyor_belt");
                var conveyor_script = conveyor ? conveyor.GetComponent<BeltConveyor>() : null;
                if (conveyor_script) conveyor_script.removeOnBelt(currentTarget);

                currentTarget.transform.parent = rightHand.transform;
                needEat = true;
            }


            if (needEat)
            {
                // we have something to eat, let s go to the mouth position
                rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, mouth.transform.position, Time.deltaTime);
                if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), mouth.GetComponent<BoxCollider2D>()))
                {
                    currentState = States.EATING;
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
                if (Vector3.Dot(rightHand.transform.position, rightHandRestingPosition.transform.position) >= 0.9f)
                {
                    // close enough we reset the position
                    rightHand.transform.position = rightHandRestingPosition.transform.position;
                }
            }

        }

        if (moral < 75 && currentState != States.ANGRY )
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
