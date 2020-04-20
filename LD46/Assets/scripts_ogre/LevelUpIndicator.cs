using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpIndicator : MonoBehaviour
{

    private float last_time_update;
    public bool show_me;
    public bool start_show;

    public void show(bool b)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (!!sr)
        {
            sr.enabled = b;
            this.show_me = b;
            this.last_time_update = Time.time;
            start_show = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        show_me = false;
        start_show = false;
        last_time_update = Time.time;
        show(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ( this.start_show )
            show(true);
        if ( this.show_me )
        {
            if (Time.time - last_time_update >= Constants.LEVEL_UP_INDICATOR_DURATION)
            {
                show(false);
            }
        }
    }//! Update
}
