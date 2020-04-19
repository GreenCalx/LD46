using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreRampage : MonoBehaviour
{

    GameObject currentTarget;
public    enum States { GET_TARGET, EAT_TARGET, DEACTIVATED };
    public States currentState = States.DEACTIVATED;

    public float speed = 2;

    GameObject BigOgre;

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
    }

    private void FixedUpdate()
    {
        if (currentState == States.EAT_TARGET)
        {
            // move until reaching current target
            if (Physics2D.IsTouching(GetComponent<BoxCollider2D>(), currentTarget.GetComponent<BoxCollider2D>() ))
            {
                currentTarget.GetComponent<Villager>().Kill();
                currentState = States.GET_TARGET;
            } else
            {
                transform.position += (currentTarget.transform.position - transform.position).normalized * speed * Time.fixedDeltaTime;
            }
        }
    }

    // Update is called once per frame
    void Update()
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
                    if (script.villagers.Capacity != 0)
                    {
                        currentTarget = script.villagers[Random.Range(0, script.villagers.Capacity - 1)];
                        if (currentTarget) currentState = States.EAT_TARGET;
                    }
                }
            }
        }
    }
}
