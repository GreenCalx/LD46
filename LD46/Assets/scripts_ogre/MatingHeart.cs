using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatingHeart : MonoBehaviour
{
    public GameObject parent_go;

    public void refreshParentGO()
    {
       if (parent_go == null) 
        parent_go = this.transform.parent.gameObject;
    }

    public void tryDisplayHeart()
    {
        refreshParentGO();
        if (!!parent_go)
        {
            Villager v = parent_go.GetComponent<Villager>();
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (!!v && !!sr)
            {
                sr.enabled = v.trying_to_mate ;
            }
        }   
    }

    // Start is called before the first frame update
    void Start()
    {
        tryDisplayHeart();
    }

    // Update is called once per frame
    void Update()
    {
        tryDisplayHeart();   
    }
}
