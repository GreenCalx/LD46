using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConveyor : MonoBehaviour
{

    public List<GameObject> objects_on_belt;

    private Transform loadingPoint;
    private Transform dischargePoint;

    public void putOnBelt(GameObject iGO)
    {
        objects_on_belt.Add(iGO);
    }

    public void removeOnBelt(GameObject iGO)
    {
        objects_on_belt.Remove(iGO);
    }

    public Transform getLoadingPoint()
    {
        return gameObject.transform.Find(Constants.BELT_LOADING_POINT);
    }

    public Transform getDischargePoint()
    {
        return gameObject.transform.Find(Constants.BELT_DISCHARGE_POINT);
    }

    void OnTriggerEnter2D( Collider2D other) 
    {
        Villager v = other.GetComponent<Villager>();
        if (!!v)
        {
            if (v.go_to_belt == false )
                return; // Wasn't suppose to step on the belt
            putOnBelt( v.gameObject );
            v.is_on_belt = true;
            v.go_to_belt = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        objects_on_belt = new List<GameObject>();

        loadingPoint    = gameObject.transform.Find(Constants.BELT_LOADING_POINT);
        dischargePoint  = gameObject.transform.Find(Constants.BELT_DISCHARGE_POINT);

    }

    // Update is called once per frame
    void Update()
    {
        foreach( GameObject go in objects_on_belt)
        {
            go.transform.position = Vector2.MoveTowards( go.transform.position, getDischargePoint().position, Constants.belt_speed * Time.deltaTime);
        }
    }

}
