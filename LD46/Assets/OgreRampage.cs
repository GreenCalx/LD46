using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreRampage : MonoBehaviour
{
    // state machine
    public enum States { GET_TARGET, EAT_TARGET, DEACTIVATED };
    public States currentState = States.DEACTIVATED;
    // target
    private GameObject currentTarget;
    // refs
    private OgreBehaviour BigOgre;
    private Village Village;
    private SpriteRenderer SR;
    private BoxCollider2D BC;

    // debug if blocked vars
    private int UnblockFrameCount = 240; // environ 4s
    private int CurrentBlockFrameCount = 0;
    private int DisableColliderTime = 1;
    private float CurrentDisabledTime = 0;



    public void Activate()
    {
        // enable the sprite renderer
        SR.enabled = true;
        // update state
        currentState = States.GET_TARGET;
        // make it pop inside the village to go RAMPAGE
        if(Village)
        {
            var bounds = Village.GetComponent<BoxCollider2D>().bounds;
            transform.position = new Vector3(
                   Random.Range(bounds.min.x, bounds.max.x),
                   Random.Range(bounds.min.y, bounds.max.y),
                   Random.Range(bounds.min.z, bounds.max.z));
        }
    }


    public void DeActivate() {
        // disable sprite
        // should we disable/enable the whole object to avoid physics bugs or something
        SR.enabled = false;
        // update state
        currentState = States.DEACTIVATED;
    }

    void Start()
    {
        // init state
        var ogre_go = GameObject.Find("ogre");
        if (ogre_go) BigOgre = ogre_go.GetComponent<OgreBehaviour>();
        SR = GetComponent<SpriteRenderer>();
        if(SR) SR.enabled = false;
        currentState = States.DEACTIVATED;
        var village_go = GameObject.Find(Constants.VILLAGE_GO_NAME);
        if (village_go) Village = village_go.GetComponent<Village>();
        BC = GetComponent<BoxCollider2D>();
    }

    private void GetTarget()
    {
        // pick random villager
        if (Village)
        {
                if (Village.villagers.Count != 0)
                {
                    currentTarget = Village.villagers[Random.Range(0, Village.villagers.Count)];//using int on random.range is exclusive on bounds
                }
        }
    }

    private void FixedUpdate()
    {
        if (currentState == States.EAT_TARGET && currentTarget)
        {
            // move until reaching current target
            if (Physics2D.IsTouching(BC, currentTarget.GetComponent<BoxCollider2D>() ))
            {
                currentTarget.GetComponent<Villager>().Kill();
                BigOgre.AddFood(Constants.Villager_food);
                currentState = States.GET_TARGET;
            } else
            {
                    transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, Constants.LittleOgreSpeed * Time.fixedDeltaTime);
            }

            // this is a hack to avoid letting the ogre getting stuck for too long in a house
            // to avoid this the correct solution would be something like a A*
            List<Collider2D> result = new List<Collider2D>();
            Physics2D.OverlapCollider(BC, new ContactFilter2D(), result);
            bool FoundHouseCollider = false;
            foreach(Collider2D c in result)
            {
                // are we blocked by a fucking house?
                if (c.GetComponent<House>())
                {
                    CurrentBlockFrameCount += 1;
                    FoundHouseCollider = true;
                    break;
                }
            }

            if (CurrentBlockFrameCount >= UnblockFrameCount)
            {
                GetComponent<BoxCollider2D>().enabled = false; 
            }

            if (GetComponent<BoxCollider2D>().enabled == false)
            {
                CurrentDisabledTime -= Time.fixedDeltaTime;
                if (CurrentDisabledTime <= 0)
                {
                    CurrentDisabledTime = DisableColliderTime;
                    GetComponent<BoxCollider2D>().enabled = true; 
                }
            }

            if (GetComponent<BoxCollider2D>().enabled)
            {
                if (!FoundHouseCollider)
                {
                  CurrentBlockFrameCount = 0;
                  CurrentDisabledTime = DisableColliderTime;
                }
            }
        } // END EAT_TARGET
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != States.DEACTIVATED)
        {
            if (BigOgre)
            {
                    if (BigOgre.food > Constants.ogre_rampage_threshold_stop)
                    {
                        DeActivate();
                        BigOgre.ResetBehavior(); // this will make the ogre come back to its window
                    }
            }

            if (currentState == States.GET_TARGET)
            {
                GetTarget();
                if (currentTarget) currentState = States.EAT_TARGET;
            }

        }
    }
}
