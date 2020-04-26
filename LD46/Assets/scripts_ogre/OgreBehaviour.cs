

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class OgreBehaviour : MonoBehaviour
{

    // Main file todo list
    //  - cleanup variables and stop getting components everywhere
    //  - make default state machine assert or at least log that form => to does not exist
    //  - log state machine states, from, to, etc

    // start refactoring
    public enum eHand { RIGHT, LEFT };
    [System.Serializable]
    public class OgreHand
    {
        public enum eSprite { REST, GET_TARGET, ANGRY }
        public Transform RestPosition;
        public Transform AngryUpPosition;
        public Sprite[] Sprites;
        public GameObject GO;
    }
    public List<OgreHand> Hands;

    public int Food; // public until littleOgre cleanup
    public int Moral;

    // state machine
    public enum eStates { EATING, REST, GETTING_TARGET, ANGRY, HUNGRY, RAMPAGE, COUNT }
    public eStates CurrentState = eStates.REST;
    private class StateFunction
    {
        public delegate void Start();
        public delegate void Tick();
        public delegate void End();

        public Start FStart;
        public Tick FTick;
        public End FEnd;
    }
    private StateFunction[] StatesLogic = new StateFunction[(int)eStates.COUNT];

    public GameObject[] Bones;
    public GameObject[] Puff;

    public GameObject Mouth;
    public bool needEat = false;
    public bool needAskFood = false;
    public bool goUp = false;
    public bool goDown = false;

    [System.Serializable]
    public class TimedTick
    {
        public float Time;
        public float CurrentTime = 0;
        public bool Done = false;
        public float Tick(float time)
        {
            CurrentTime += time;
            if (CurrentTime > Time)
            {
                Done = true;
            }
            return CurrentTime;
        }
    }
    private TimedTick EatingAnimationTick = new TimedTick();
    private TimedTick LastRampage = new TimedTick();
    private TimedTick FoodTickTimer = new TimedTick();
    private Villager CurrentCommand;

    private BeltConveyor Belt;
    private Village Village;
    private GameObject CurrentTarget;
    private Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        // States definition
        for (int i = 0; i < (int)eStates.COUNT; ++i) StatesLogic[i] = new StateFunction();
        StatesLogic[(int)eStates.EATING].FStart = EatingAnimationStart;
        StatesLogic[(int)eStates.EATING].FTick = EatingAnimation;
        StatesLogic[(int)eStates.EATING].FEnd = EatingAnimationStop;
        StatesLogic[(int)eStates.ANGRY].FStart = AngryAnimationStart;
        StatesLogic[(int)eStates.ANGRY].FTick = AngryAnimation;
        // reference initialization
        var Belt_go = GameObject.Find("conveyor_belt");
        Belt = Belt_go ? Belt_go.GetComponent<BeltConveyor>() : null;
        var Village_go = GameObject.Find(Constants.VILLAGE_GO_NAME);
        Village = Village_go ? Village_go.GetComponent<Village>() : null;
        Animator = GetComponent<Animator>();
        Mouth = GameObject.Find("Mouth");
        // Ticks
        EatingAnimationTick.Time = 2;
        LastRampage.Time = 5;
        FoodTickTimer.Time = Constants.Ogre_Food_Tick_Time;

        Init();
    }

    // Can be called from outside to reset state
    public void Init()
    {
        Food = Constants.MAX_FOOD;
        Moral = Constants.MAX_MORAL;

        Hands[(int)eHand.RIGHT].GO.transform.position = Hands[(int)eHand.RIGHT].RestPosition.position;
        Hands[(int)eHand.LEFT].GO.transform.position = Hands[(int)eHand.LEFT].RestPosition.position;

        UpdateState(eStates.REST);
    }

    // internal state mutators
    // use - value to remove
    public void AddMoral(int m)
    {
        // clamp value
        Moral += m;
        Mathf.Clamp(Moral, 0, Constants.MAX_MORAL);
    }

    public void AddFood(int f)
    {
        // clamp value
        Food += f;
        Food = Mathf.Clamp(Food, 0, Constants.MAX_FOOD);
    }

    // This is used to update state when something is on the belt
    // it should be cleaner to make it happen inside belt and send an event...
    private void OnTriggerStay2D(Collider2D collision)
    {
        // this merans something on the bgelt is copming, start to go towards it to take it
        // first find collider
        if (CurrentState == eStates.REST || CurrentState == eStates.HUNGRY)
        {
            if (collision.gameObject.name != Constants.BELT)
            {
                CurrentTarget = collision.gameObject;
                UpdateState(eStates.GETTING_TARGET);
            }
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

    void DisplayCommand()
    {
        var ui = GetComponentInChildren<Canvas>();
        if (ui) ui.enabled = true;
        var icon = GetComponentInChildren<Image>();

        icon.sprite = CurrentCommand.job.getJobSprite(CurrentCommand.sex);
    }

    void AskFood()
    {
        int maxLvl = Village.getMaxLevelVillager();
        int commandLvl = Random.Range(0, maxLvl + 1);
        int maxJob = jobs.Job.availableJobsForLevel(maxLvl); // available job for selected level
        int commandJob = Random.Range(0, maxJob);
        int commandSex = Random.Range(0, 2);

        CurrentCommand = new Villager();
        CurrentCommand.sex = (Villager.SEX)commandSex;
        CurrentCommand.job = getJob(commandJob);
        CurrentCommand.level = commandSex;


        DisplayCommand();
        needAskFood = false;
    }

    void EatingAnimationStart()
    {
        EatingAnimationTick.CurrentTime = 0;
        Animator.SetBool("isEating", true);
        AudioManager.Instance.Play(Constants.EATING);
        // to move into state logic
        needEat = false;
        needAskFood = false;
        AddFood(Constants.Villager_food);
        var ui = GetComponentInChildren<Canvas>();
        if (ui) ui.enabled = false;
    }
    void EatingAnimationStop()
    {
        Animator.SetBool("isEating", false);
        AudioManager.Instance.Stop(Constants.EATING);

        EatingAnimationTick.CurrentTime = 0;
        EatingAnimationTick.Done = false;
    }
    void EatingAnimation()
    {
        if (EatingAnimationTick.CurrentTime % 0.5f > EatingAnimationTick.Tick(Time.deltaTime) % 0.5)
        {
            var go = Instantiate(Bones[Random.Range(1, Bones.Length)], Mouth.transform.position, Quaternion.Euler(new Vector3(
                                                                                       0,
                                                                                       0,
                                                                                       UnityEngine.Random.Range(0, 360))));
            Destroy(go, 2);
            go.GetComponent<Rigidbody2D>().AddForce(new Vector3(1, 1, 0));

            //also spawn blood randomly next to Mouth positoin
            var bounds = Mouth.GetComponent<BoxCollider2D>().bounds;
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

        if (EatingAnimationTick.Done)
        {
            UpdateState(eStates.REST);
        }
    }

    //Should be directly inside the timer to execute functions
    void FoodTick()
    {
        FoodTickTimer.Tick(Time.deltaTime);
        if (FoodTickTimer.Done)
        {
            AddFood(Constants.Ogre_Food_Tick_Loss);
            FoodTickTimer.CurrentTime = 0;
            FoodTickTimer.Done = false;
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
        if (goDown && Vector3.Distance(Hands[(int)eHand.RIGHT].GO.transform.position, Hands[(int)eHand.RIGHT].RestPosition.transform.position) < threshold)
        {
            goUp = true;
            goDown = false;
            var go = GameObject.Find(Constants.VILLAGE_GO_NAME);
            if (go)
            {
                var script = go.GetComponent<Village>();
                script.DamageHouses();
                AddMoral(Constants.OGRE_MORAL_GAIN_IN_ANGER);

                // spawn puff cloud
                Instantiate(Puff[0], Hands[(int)eHand.RIGHT].GO.transform.position - new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
                Instantiate(Puff[1], Hands[(int)eHand.RIGHT].GO.transform.position + new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
                Instantiate(Puff[0], Hands[(int)eHand.LEFT].GO.transform.position - new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
                Instantiate(Puff[1], Hands[(int)eHand.LEFT].GO.transform.position + new Vector3(Puff[0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 0), Quaternion.identity);
            }
        }
        if (goUp && Vector3.Distance(Hands[(int)eHand.RIGHT].GO.transform.position, Hands[(int)eHand.RIGHT].AngryUpPosition.transform.position) < threshold)
        {
            goDown = true;
            goUp = false;
        }

    }

    int GetMoralFromCommand(Villager v)
    {
        int result = 0;

        if (CurrentCommand)
        {
            int deltaLevel = (v.level - CurrentCommand.level);
            result += deltaLevel * Constants.OGRE_WRONG_LVL_COMMAND_PENALTY;
            int deltaSex = System.Convert.ToInt32(v.sex != CurrentCommand.sex);
            result -= deltaSex * Constants.OGRE_WRONG_SEX_COMMAND_PENALTY;
            int deltaJob = System.Convert.ToInt32(v.job != CurrentCommand.job);
            result -= deltaJob * Constants.OGRE_WRONG_JOB_COMMAND_PENALTY;
        }
        else
        {
            result = (int)Constants.Ogre_Free_Food_Moral;
        }
        return result;
    }


    public void UpdateState(eStates newState)
    {
        // state machine transition
        switch (CurrentState)
        {
            case eStates.REST:
                switch (newState)
                {
                    case eStates.GETTING_TARGET:
                        {
                            needEat = false;
                        }
                        break;
                    case eStates.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case eStates.HUNGRY:
                        {
                            needAskFood = true;
                        }
                        break;
                    case eStates.RAMPAGE:
                        LastRampage.CurrentTime = 0;
                        break;
                    default:
                        break;
                }

                break;
            case eStates.GETTING_TARGET:
                switch (newState)
                {
                    case eStates.EATING:
                        {
                            if (CurrentTarget)
                            {
                                var villager = CurrentTarget.GetComponent<Villager>();
                                if (villager)
                                {
                                    AddMoral(GetMoralFromCommand(villager));
                                    villager.Kill();
                                    CurrentCommand = null;
                                }
                            }
                            EatingAnimationStart();
                        }
                        break;
                    case eStates.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case eStates.RAMPAGE:
                        LastRampage.CurrentTime = 0;
                        break;
                    default: break;
                }

                break;
            case eStates.EATING:
                switch (newState)
                {
                    case eStates.REST:
                        {
                            EatingAnimationStop();
                        }
                        break;
                    case eStates.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case eStates.RAMPAGE:
                        LastRampage.CurrentTime = 0;
                        break;
                    default: break;
                }
                break;
            case eStates.ANGRY:
                switch (newState)
                {
                    case eStates.REST:
                        {
                            GetComponent<Animator>().SetBool("isEating", false);
                        }
                        break;
                    case eStates.RAMPAGE:
                        LastRampage.CurrentTime = 0;
                        break;
                    default: break;
                }
                break;
            case eStates.HUNGRY:
                switch (newState)
                {
                    case eStates.GETTING_TARGET:
                        {
                            needEat = false;
                        }
                        break;
                    case eStates.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    case eStates.RAMPAGE:
                        LastRampage.CurrentTime = 0;
                        LastRampage.Done = false;
                        break;
                    default: break;
                }
                break;
            case eStates.RAMPAGE:
                switch (newState)
                {
                    case eStates.REST:
                        {
                            // probably wanna call Init() here
                            Moral = Constants.MAX_MORAL;
                            Food = Constants.MAX_FOOD;
                            EnableAllSR();
                            LastRampage.Done = false;
                            LastRampage.CurrentTime = 0;
                        }
                        break;
                    case eStates.ANGRY:
                        {
                            AngryAnimationStart();
                        }
                        break;
                    default: break;
                }
                break;
        }
        CurrentState = newState;
    }

    private void UpdateSprites()
    {
        switch (CurrentState)
        {
            case eStates.REST:
                {
                    Hands[(int)eHand.RIGHT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.RIGHT].Sprites[(int)OgreHand.eSprite.REST];
                    Hands[(int)eHand.LEFT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.LEFT].Sprites[(int)OgreHand.eSprite.REST];
                }
                break;
            case eStates.EATING:
                {
                    Hands[(int)eHand.RIGHT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.RIGHT].Sprites[(int)OgreHand.eSprite.ANGRY];
                }
                break;
            case eStates.GETTING_TARGET:
                {
                    if (!needEat)
                        Hands[(int)eHand.RIGHT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.RIGHT].Sprites[(int)OgreHand.eSprite.GET_TARGET];
                    else
                        Hands[(int)eHand.RIGHT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.RIGHT].Sprites[(int)OgreHand.eSprite.ANGRY];
                }
                break;
            case eStates.ANGRY:
                {
                    Hands[(int)eHand.RIGHT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.RIGHT].Sprites[(int)OgreHand.eSprite.ANGRY];
                    Hands[(int)eHand.LEFT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.LEFT].Sprites[(int)OgreHand.eSprite.ANGRY];
                }
                break;
            default:
                {
                    Hands[(int)eHand.RIGHT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.RIGHT].Sprites[(int)OgreHand.eSprite.REST];
                    Hands[(int)eHand.LEFT].GO.GetComponent<SpriteRenderer>().sprite = Hands[(int)eHand.LEFT].Sprites[(int)OgreHand.eSprite.REST];
                }
                break;
        }
    }

    private void UpdatePositions()
    {
        switch (CurrentState)
        {
            case eStates.GETTING_TARGET:
                {
                    if (!needEat)
                    {
                        // move right hand towards the incoming food
                        Hands[(int)eHand.RIGHT].GO.transform.position =
                            Vector3.MoveTowards(Hands[(int)eHand.RIGHT].GO.transform.position
                                               , CurrentTarget.transform.position
                                               , Constants.HandSpeed * Time.deltaTime);
                    }  // we have something to eat, let s go to the Mouth position
                    else
                    {
                        Hands[(int)eHand.RIGHT].GO.transform.position =
                            Vector3.MoveTowards(Hands[(int)eHand.RIGHT].GO.transform.position
                                               , Mouth.transform.position
                                               , Constants.HandSpeed * Time.deltaTime);
                    }
                }
                break;
            case eStates.REST:
                {
                    Hands[(int)eHand.RIGHT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.RIGHT].GO.transform.position
                                               , Hands[(int)eHand.RIGHT].RestPosition.position
                                               , Constants.HandSpeed * Time.deltaTime);
                    Hands[(int)eHand.LEFT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.LEFT].GO.transform.position
                                               , Hands[(int)eHand.LEFT].RestPosition.position
                                               , Constants.HandSpeed * Time.deltaTime);
                }
                break;
            case eStates.ANGRY:
                {
                    if (goUp)
                    {
                        Hands[(int)eHand.RIGHT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.RIGHT].GO.transform.position
                               , Hands[(int)eHand.RIGHT].AngryUpPosition.position
                               , Constants.HandSpeed * Time.deltaTime);
                        Hands[(int)eHand.LEFT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.LEFT].GO.transform.position
                                                   , Hands[(int)eHand.LEFT].AngryUpPosition.position
                                                   , Constants.HandSpeed * Time.deltaTime);
                    }

                    if (goDown)
                    {
                        Hands[(int)eHand.RIGHT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.RIGHT].GO.transform.position
                           , Hands[(int)eHand.RIGHT].RestPosition.position
                           , Constants.HandSpeed * Time.deltaTime);
                        Hands[(int)eHand.LEFT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.LEFT].GO.transform.position
                                                   , Hands[(int)eHand.LEFT].RestPosition.position
                                                   , Constants.HandSpeed * Time.deltaTime);
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
        // Apply current state
        UpdateSprites();
        UpdatePositions();

        // FROM ANY STATES
        // NOTe : this is bugged because you can then start a rampage while food is coming
        // you can also get stuck in eating state, etc...
        if (Food > Constants.ogre_hungry_threshold_start)
        {
            var ui = GetComponentInChildren<Canvas>();
            if (ui) ui.enabled = false;
        }
        if (Moral < Constants.OGRE_ANGER_THR)
        {
            UpdateState(eStates.ANGRY);
        }
        if (Food < Constants.ogre_rampage_threshold_start)
        {
            UpdateState(eStates.RAMPAGE);
        }

        // apply logic and update state accordingly
        switch (CurrentState)
        {
            case eStates.REST:
                {
                    FoodTick();
                    // from state rest to hungry
                    if (Food < Constants.ogre_hungry_threshold_start)
                    {
                        UpdateState(eStates.HUNGRY);
                    }
                }
                break;
            case eStates.GETTING_TARGET:
                {
                    FoodTick();
                    if (CurrentTarget)
                    {
                        //UpdateSprites();
                        //UpdatePositions();

                        if (!needEat && Physics2D.IsTouching(Hands[(int)eHand.RIGHT].GO.GetComponent<BoxCollider2D>(), CurrentTarget.GetComponent<BoxCollider2D>()))
                        {
                            // remove from conmveyor belt
                            if (Belt) Belt.removeOnBelt(CurrentTarget);

                            CurrentTarget.transform.parent = Hands[(int)eHand.RIGHT].GO.transform;
                            CurrentTarget.transform.localPosition = new Vector3(0, 0.56f ,0); // as hand position becomes parent, always same local pos
                            needEat = true;
                        }
                        else if (needEat)
                        {
                            { // this should go in villager logic
                                AudioManager.Instance.Play(Constants.OH_NO_VOICE);
                            }

                            // probably a fuck in villager that makes them move even when in ogre s hand, this is a quick fix
                            CurrentTarget.transform.localPosition = new Vector3(0, 0.56f ,0); // as hand position becomes parent, always same local pos
                            if (Physics2D.IsTouching(Hands[(int)eHand.RIGHT].GO.GetComponent<BoxCollider2D>(), Mouth.GetComponent<BoxCollider2D>()))
                            {
                                UpdateState(eStates.EATING);
                            }
                        }
                    }
                }
                break;
            case eStates.EATING:
                {
                    FoodTick();
                    EatingAnimation();
                }
                break;
            case eStates.ANGRY:
                {
                    FoodTick();
                    // destroy village buildings
                    AngryAnimation();
                    if (Moral >= Constants.OGRE_GET_CALM_THR)
                    {
                        UpdateState(eStates.REST);
                    }
                }
                break;
            case eStates.HUNGRY:
                {
                    FoodTick();
                    // if we are not in the resting position, let s reset the position
                    if (Hands[(int)eHand.RIGHT].GO.transform.position
                        != Hands[(int)eHand.RIGHT].RestPosition.position)
                    {
                        Hands[(int)eHand.RIGHT].GO.transform.position = Vector3.MoveTowards(Hands[(int)eHand.RIGHT].GO.transform.position, Hands[(int)eHand.RIGHT].RestPosition.transform.position, Constants.HandSpeed * Time.deltaTime);
                        if (Vector3.Distance(Hands[(int)eHand.RIGHT].GO.transform.position, Hands[(int)eHand.RIGHT].RestPosition.transform.position) < 0.1f)
                        {
                            // close enough we reset the position
                            Hands[(int)eHand.RIGHT].GO.transform.position = Hands[(int)eHand.RIGHT].RestPosition.transform.position;
                        }
                    }
                    // ask for food
                    if (needAskFood) AskFood(); // to do only once
                }
                break;
            case eStates.RAMPAGE:
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
                    if (LastRampage.Tick(Time.deltaTime) >= Constants.max_time_in_rampage)
                    {
                        // Exit rampage, its too long
                        UpdateState(eStates.REST);
                    }
                }
                break;
        }
    }
}
