using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Buildings
{
    public enum HOUSE_STATE {
        FULL = 0,
        HALF_DAMAGED = 1,
        DESTROYED = 2
    }

    public Sprite FullLife;
    public Sprite HalfDamaged;
    public Sprite Damaged;

    public int Life = 1;

    public SpriteRenderer sr;

    public HOUSE_STATE state;

    public void Damage()
    {
        Life -= Constants.OGRE_DAMAGE_TO_HOUSE;
    }
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = FullLife;
        this.Life = Constants.HOUSE_MAX_HP;
    }

    void update_sprite()
    {
        if (state == HOUSE_STATE.FULL)
            sr.sprite = FullLife;
        if (state == HOUSE_STATE.DESTROYED)
            sr.sprite = Damaged;
        else if (state == HOUSE_STATE.HALF_DAMAGED)
            sr.sprite = HalfDamaged;
    }

    // Update is called once per frame
    void Update()
    {
        if (Life == 0)
            state = HOUSE_STATE.DESTROYED;
        else if ( this.Life <= Constants.HOUSE_DAMAGED_THR ) 
            state = HOUSE_STATE.HALF_DAMAGED;
        else
            state = HOUSE_STATE.FULL;
        update_sprite();
    }

    public override bool canWorkHere( jobs.Job iJob )
    {
        return string.Equals( iJob.getJobName(), Constants.builder_job_name); 
    }

}
