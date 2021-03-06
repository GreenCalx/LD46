﻿using System.Collections;
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
        if (Village)
        {
            var bounds = Village.GetComponent<BoxCollider2D>().bounds;
            transform.position = new Vector3(
                   Random.Range(bounds.min.x, bounds.max.x),
                   Random.Range(bounds.min.y, bounds.max.y),
                   Random.Range(bounds.min.z, bounds.max.z));
        }

        AudioManager.Instance.Play(Constants.FRESH_MEAT);

        CurrentBlockFrameCount = 0;
        CurrentDisabledTime = 0;
    }


    public void DeActivate()
    {
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
        if (SR) SR.enabled = false;
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
            if (Physics2D.IsTouching(BC, currentTarget.GetComponent<BoxCollider2D>()))
            {
                currentTarget.GetComponent<Villager>().Kill();
                BigOgre.AddFood(Constants.Villager_food);
                if (BigOgre)
                {
                    if (BigOgre.Food > Constants.ogre_rampage_threshold_stop)
                    {
                        DeActivate();
                        BigOgre.Init(); // this will make the ogre come back to its window
                    }
                }

                currentState = States.GET_TARGET;
            }
            else
            {
                // let s try to be a tiny bit smarter (lul) on the path finding

                // if we are gonna it a house and not the currentTarget between us and the house
                var target_position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, Constants.LittleOgreSpeed * Time.fixedDeltaTime);
                var target_direction = target_position - transform.position;
                var raycast_result = Physics2D.BoxCastAll(transform.position, BC.size, 0, target_direction.normalized, Vector2.Distance(transform.position, target_position));
                bool house_on_the_path = false;
                foreach(RaycastHit2D hit in raycast_result)
                {
                    if ( hit.collider.gameObject.GetComponent<House>() )
                    {
                        house_on_the_path = true;
                    }
                }

                if (house_on_the_path)
                {
                    // are we going mostly up/down or right/left?
                    if (Mathf.Abs(target_direction.x) > Mathf.Abs(target_direction.y)) {
                        // mostly going right/left
                        if (target_direction.x > 0)
                        {
                            // mostly going right
                            target_position = transform.position + new Vector3(10, 0, 0);
                        } else
                        {
                            target_position = transform.position + new Vector3(-10, 0, 0);
                        }
                    } else
                    {
                        // mostly going up/down 
                        if (target_direction.y > 0)
                        {
                            // mostly going up
                            target_position = transform.position + new Vector3(0, 10, 0);
                        } else
                        {
                            target_position = transform.position + new Vector3(0, 10, 0);
                        }
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, target_position, Constants.LittleOgreSpeed * Time.fixedDeltaTime);
            }

            // this is a hack to avoid letting the ogre getting stuck for too long in a house
            // to avoid this the correct solution would be something like a A*
            List<Collider2D> result = new List<Collider2D>();
            Physics2D.OverlapCollider(BC, new ContactFilter2D(), result);
            bool FoundHouseCollider = false;
            foreach (Collider2D c in result)
            {
                // is this a villager that we might wanna kill anyway?
                if (c.GetComponent<Villager>())
                {
                    c.GetComponent<Villager>().Kill();
                    BigOgre.AddFood(Constants.Villager_food);
                    if (BigOgre)
                    {
                        if (BigOgre.Food > Constants.ogre_rampage_threshold_stop)
                        {
                            DeActivate();
                            BigOgre.Init(); // this will make the ogre come back to its window
                            break;
                        }
                    }
                }

                // emergency fix
                // are we blocked by a fucking house?
                if (c.GetComponent<House>())
                {
                    CurrentBlockFrameCount += 1;
                    FoundHouseCollider = true;
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
        } // END EAT_TARGET
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != States.DEACTIVATED)
        {
            if (BigOgre)
            {
                if (BigOgre.Food > Constants.ogre_rampage_threshold_stop)
                {
                    DeActivate();
                    BigOgre.Init(); // this will make the ogre come back to its window
                }
            }

            if (currentState == States.GET_TARGET)
            {
                GetTarget();
                if (currentTarget) currentState = States.EAT_TARGET;
            }

            if (currentState == States.EAT_TARGET && !currentTarget)
            {
                // probably in wrong state...
                currentState = States.GET_TARGET;
            }

        }
    }
}
