using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC : MonoBehaviour
{

    public readonly Guid ID = Guid.NewGuid();
    protected GameObject _subscribedBuilding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void subscribe( Buildings iBuilding )
    {
        _subscribedBuilding = iBuilding.gameObject;
        iBuilding.addWorker( this.gameObject );
        onSubscribe();
    }

    public void unsubscribe(Buildings iBuilding)
    {
        NPC npc = this.gameObject.GetComponent<NPC>();
        if (null==npc)
            return;
        _subscribedBuilding = null;
        iBuilding.removeWorker( this.gameObject );
        onUnsubscribe();
    }

    public bool hasSubscribtion()
    {
        return ( _subscribedBuilding != null );
    }

    // Methods to add behaviors to subscription/unsub
    public virtual void onSubscribe() {}
    public virtual void onUnsubscribe() {}
}
