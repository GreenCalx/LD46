using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public List<GameObject> workers;


    // Start is called before the first frame update
    void Start()
    {
        workers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D( Collider2D other) 
    {
        Villager v = other.GetComponent<Villager>();
        if (!!v)
        {
            jobs.Job job = v.job;
            if ( null == job )
                return;

            if( !job.hasABuildingTarget() )
                return;

            if ( !canWorkHere(job) )
                return;

            Buildings b = gameObject.GetComponent<Buildings>();
            v.subscribe( b );
        }
    }

    void OnTriggerExit2D( Collider2D other) 
    {
        Villager v = other.GetComponent<Villager>();
        if (!!v)
        {
            if ( workers.Contains( other.gameObject ) )
            {
                v.revertDirection();
            }
        }
    }

    public virtual bool canWorkHere( jobs.Job iJob )
    { return false; }

    public void addWorker( GameObject iNPC )
    {
        if ( iNPC == null )
            return; // Raise error ?
        if ( workers.Contains(iNPC ) )
            return;
        workers.Add(iNPC);
    }

    public void removeWorker( GameObject iNPC )
    {
        if ( iNPC == null )
            return; // Raise error ?
        workers.Remove(iNPC);
    }
}
