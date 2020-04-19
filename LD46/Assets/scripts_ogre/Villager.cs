using System.Collections;
using System.Collections.Generic;

using System.Text;
using System;
using UnityEngine;
//using UnityEngine.Physics2DModule;
using jobs;


public class Villager : MonoBehaviour
{

    public enum SEX {
        Female = 0,
        Male = 1
    }

    // villager strong attributes
    public int level;
    public Job job;
    public SEX sex;
    public int exp;

    // villager weak attributes
    public string name;
    public string job_str;

    // time
    private float last_move_update;
    private float last_exp_update;

    // constraints
    public bool is_on_belt;
    public bool go_to_belt;
    public Transform destination;
 

    public string generateName()
    {
      StringBuilder str_builder = new StringBuilder();  
      char          letter;  

      int name_length = 8;
      for (int i = 0; i < name_length; i++)
      {
        double flt = UnityEngine.Random.Range(0f, 1f);
        int shift = Convert.ToInt32(Math.Floor(25 * flt)); // 26 letter in alphabet
        letter = Convert.ToChar(shift + 65); // 65 is ASCII val of 'a'
        str_builder.Append(letter);  
      }  
      return str_builder.ToString();

    }//! generateName

    public SEX generateSex()
    {
        //System.Random random      = new System.Random();  
        return (UnityEngine.Random.Range(0f, 100f) <= 50 ) ? SEX.Male : SEX.Female ; // 50% either male or female
    }

    public int generateLevel()
    {
        //System.Random random      = new System.Random();  
        return ( UnityEngine.Random.Range(0f, 100f) <= 50 ) ? 0 : 1 ; // 50% chance of having a beggar
    }

    public void randomize()
    {
        this.name  = this.generateName();
        this.sex   = this.generateSex();
        this.level = this.generateLevel();
        assignJob( new jobs.Beggar() ); // starts beggar regardless of level
    }

    public void assignJob( jobs.Job iJob )
    {
        if (iJob.canApplyToJob(this))
        {
            this.job = iJob;
            this.job_str = iJob.getJobName();
            update_graphics();
        }
    }

    public void doJob()
    {
        if (this.job != null)
            this.job.applyJobEffect();
    }

    public void update_graphics()
    {
        SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
        if (!!sr)
        {
            sr.sprite = this.job.getJobSprite(this.sex);
        }
    }

    public void moveRandom()
    {
        if (is_on_belt)
            return;

        Rigidbody2D rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        if (!rb2d)
            return;

        if (Time.time - last_move_update >= Constants.move_time_step)
        {
            float min = Constants.villager_move_step * (-1);
            float max = Constants.villager_move_step ;
            var x = UnityEngine.Random.Range( min, max);
            var y = UnityEngine.Random.Range( min, max);
            rb2d.velocity = new Vector2( x, y);
            last_move_update = Time.time;
        }
    }

    public void moveToDestination()
    {
        Rigidbody2D rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        if (!rb2d)
            return;

        rb2d.velocity = new Vector2(0, 0);
        transform.position = Vector2.MoveTowards( transform.position, destination.position, Constants.villager_speed * Time.deltaTime);

    }

    public void revertDirection()
    {
        Rigidbody2D rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        if (!rb2d)
            return;
        rb2d.velocity = -rb2d.velocity;
    }


    public void updateExperience()
    {
        if (Time.time - last_exp_update >= Constants.villager_exp_gain_time_step)
        {
            this.exp += Constants.VILLAGER_EXP_GAIN;

            if ( this.exp > Constants.VILLAGER_EXP_REQ_LEVEL3  && (level == 3) )
            { /* level 3 is last level, no more to do */}
            if ( (this.exp >= Constants.VILLAGER_EXP_REQ_LEVEL3) && (level==2) )
            {
                this.level = 3;
            }
            else if ( (this.exp >= Constants.VILLAGER_EXP_REQ_LEVEL2) && (level==1) )
            {
                this.level = 2;
            }
            else if ( (this.exp >= Constants.VILLAGER_EXP_REQ_LEVEL1) && (level==0) )
            {
                this.level = 1;
            }

            last_exp_update = Time.time;
        }
    }

    public void Kill()
    {
        // here is animation of killing if needed
        // for now just remove gameobject from game
        GameObject.Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
       randomize();
       update_graphics();
       last_move_update = Time.time;
       last_exp_update  = Time.time;
       this.is_on_belt = false;
       this.go_to_belt = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Update position
        if( is_on_belt)
            {}
        else if (go_to_belt && destination)
            this.moveToDestination();
        else
            this.moveRandom();

        // Update exp
        this.updateExperience();
    }
}
