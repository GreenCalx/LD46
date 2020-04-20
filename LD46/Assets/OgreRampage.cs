using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreRampage : MonoBehaviour
{

    GameObject currentTarget;
    public enum States { GET_TARGET, EAT_TARGET, DEACTIVATED };
    public States currentState = States.DEACTIVATED;

    public float speed = 2;

    GameObject BigOgre;

    int UnblockFrameCount = 240; // environ 4s
    int CurrentBlockFrameCount = 0;
    public int DisableColliderTime = 1;
    private float CurrentDisabledTime = 0;

    public void Activate()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        currentState = States.GET_TARGET;
        var village = GameObject.Find("Village");
        if(village)
        {
            var bounds = village.GetComponent<BoxCollider2D>().bounds;

            transform.position = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                   Random.Range(bounds.min.y, bounds.max.y),
                   Random.Range(bounds.min.z, bounds.max.z));
        }
    }

    public void DeActivate() {
        GetComponent<SpriteRenderer>().enabled = false;
        currentState = States.DEACTIVATED;
    }
    // Start is called before the first frame update
    void Start()
    {
        BigOgre = GameObject.Find("ogre"); 
        GetComponent<SpriteRenderer>().enabled = false;
        currentState = States.DEACTIVATED;
    }

    private void FixedUpdate()
    {
        if (currentState == States.EAT_TARGET)
        {
            // move until reaching current target
            if (Physics2D.IsTouching(GetComponent<BoxCollider2D>(), currentTarget.GetComponent<BoxCollider2D>() ))
            {
                currentTarget.GetComponent<Villager>().Kill();
                BigOgre.GetComponent<OgreBehaviour>().AddFood(Constants.Villager_food);
                currentState = States.GET_TARGET;
            } else
            {
                transform.position += Vector3.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.fixedDeltaTime);
            }

            List<Collider2D> result = new List<Collider2D>();
            Physics2D.OverlapCollider(GetComponent<BoxCollider2D>(), new ContactFilter2D(), result);
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

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != States.DEACTIVATED)
        {
            if (BigOgre)
            {
                var script = BigOgre.GetComponent<OgreBehaviour>();
                if (script)
                {
                    if (script.food > 90)
                    {
                        DeActivate();
                        script.ResetBehavior();
                    }
                }
            }

            // pick random villager
            if (currentState == States.GET_TARGET)
            {
                var village = GameObject.Find("Village");
                if (village)
                {
                    var script = village.GetComponent<Village>();
                    if (script)
                    {
                        if (script.villagers.Count != 0)
                        {
                            currentTarget = script.villagers[Random.Range(0, script.villagers.Count)];
                            if (currentTarget) currentState = States.EAT_TARGET;
                        }
                    }
                }
            }
        }
    }
}
