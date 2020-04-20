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

    // Held GO references
    private GameObject village_go = null;
    public GameObject BloodSplash;
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
    private float last_changejob_update;

    // constraints && actions
    public bool is_on_belt;
    public bool go_to_belt;
    public bool changing_job;
    public bool trying_to_mate;
    public Transform destination;

    public void stop_movement()
    {
        Rigidbody2D rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        if (!rb2d)
            return;
        rb2d.velocity = new Vector2( 0f, 0f);
    }

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
        return (UnityEngine.Random.Range(0f, 100f) <= 50 ) ? SEX.Male : SEX.Female ; // 50% either male or female
    }

    public int generateLevel()
    {
        return ( UnityEngine.Random.Range(0f, 100f) <= 50 ) ? 0 : 1 ; // 50% chance of having a beggar
    }

    public void randomize()
    {
        this.name  = this.generateName();
        this.sex   = this.generateSex();
        this.level = this.generateLevel();
        assignJob( new jobs.Beggar() ); // starts beggar regardless of level
    }

    public string getJobChangeAnimationParam( int iJobLevel )
    {
        string anim_trig_parm = "";
        switch (iJobLevel)
        {
            case 1:
                anim_trig_parm = Constants.villager_change_job;
                break;
            case 2:
                anim_trig_parm = Constants.villager_change_job_lvl2;
                break;
            case 3:
                anim_trig_parm = Constants.villager_change_job_lvl3;
                break;
            default:
                anim_trig_parm = Constants.villager_change_job;
                break;
        }
        return anim_trig_parm;
    }

    public void assignJob( jobs.Job iJob )
    {
        if (iJob.canApplyToJob(this))
        {
            // freeze during transformation
            stop_movement();

            // update job & villager sprite
            this.job = iJob;
            this.job_str = iJob.getJobName();
            update_graphics();

            // time offset to change job
            last_changejob_update = Time.time;
            changing_job = true;
            
            // Start changing job animation ( unless beggar since its starting job )
            if ( !String.Equals( iJob.getJobName(), Constants.beggar_job_name) )
            {
                Animator animator = GetComponent<Animator>();
                if (!!animator)
                {
                    int job_level = iJob.getLevelRequired();
                    animator.SetBool( getJobChangeAnimationParam(job_level), true);
                    animator.enabled = true;
                }
            }//! job change animation
        }
    }//! assignJob

    public void doJob()
    {
        if (this.job != null)
        {
            refreshVillageRef(); // useless? safety..
            Village v = village_go.GetComponent<Village>();
            if (!!v)
                this.job.applyJobEffect(v);
        }
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


    public void refreshVillageRef()
    {
        if (this.village_go == null)
            this.village_go =  GameObject.Find( Constants.VILLAGE_GO_NAME );
    }

    public void Kill()
    {
        // here is animation of killing if needed
        // for now just remove gameobject from game
        refreshVillageRef();
        if (village_go)
        {
            var script = village_go.GetComponent<Village>();

            script.removeVillager(this.gameObject);

            var go = Instantiate( BloodSplash, this.transform.position, Quaternion.Euler(new Vector3(
                                                                                   0,
                                                                                   0,
                                                                                   UnityEngine.Random.Range(0,360))) );
            Destroy(go, Constants.BloodTiming);

            GameObject.Destroy(this.gameObject);
        }
        
    }

    public void updateExperience()
    {
        if (Time.time - last_exp_update >= Constants.villager_exp_gain_time_step)
        {
            this.exp += Constants.VILLAGER_EXP_GAIN;
            bool leveled_up = false;
            if ( this.exp > Constants.VILLAGER_EXP_REQ_LEVEL3  && (level == 3) )
            { /* level 3 is last level, no more to do */}
            if ( (this.exp >= Constants.VILLAGER_EXP_REQ_LEVEL3) && (level==2) )
            {
                this.level = 3;
                leveled_up = true;
            }
            else if ( (this.exp >= Constants.VILLAGER_EXP_REQ_LEVEL2) && (level==1) )
            {
                this.level = 2;
                leveled_up = true;
            }
            else if ( (this.exp >= Constants.VILLAGER_EXP_REQ_LEVEL1) && (level==0) )
            {
                this.level = 1;
                leveled_up = true;
            }
            if (leveled_up)
            {
                LevelUpIndicator lui = GetComponentInChildren<LevelUpIndicator>();
                if (!!lui)
                {
                    lui.start_show = true;
                }
            }

            last_exp_update = Time.time;
        }
    }

    public void isChangeJobDone()
    {
        bool job_change_done = false;
        int job_level = this.job.getLevelRequired();
        switch(job_level)
        {
            case 1:
                job_change_done = (Time.time - last_changejob_update >= Constants.villager_change_job_time);
                break;
            case 2:
                job_change_done = (Time.time - last_changejob_update >= Constants.villager_change_job_time_lvl2);
                break;
            case 3:
                job_change_done = (Time.time - last_changejob_update >= Constants.villager_change_job_time_lvl3);
                break;
            default:
                job_change_done = (Time.time - last_changejob_update >= Constants.villager_change_job_time);
                break;
        }//! swi

        if (job_change_done)
        {
            // done changing job
            //  > Start changing job animation
            Animator animator = GetComponent<Animator>();
            if (!!animator)
            {
                animator.SetBool(getJobChangeAnimationParam(job_level), false);
                animator.enabled = false;
            }
            this.changing_job = false;
            this.update_graphics();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       randomize();
       update_graphics();
       last_move_update      = Time.time;
       last_exp_update       = Time.time;
       last_changejob_update = Time.time; // just to init
       this.is_on_belt = false;
       this.go_to_belt = false;
       this.changing_job = false;
       this.trying_to_mate = false;

        // init animator and deactivate it to prevent sprite overloading
        Animator animator = GetComponent<Animator>();
        if (!!animator)
        {
            animator.SetBool( Constants.villager_change_job, false);
            animator.SetBool(Constants.villager_change_job_lvl2, false);
            animator.SetBool(Constants.villager_change_job_lvl3, false);

            animator.enabled = false;
        }

        refreshVillageRef();
    }

    // Update is called once per frame
    void Update()
    {

        if (this.changing_job)
        {
            this.isChangeJobDone();
        }
        else 
        {
            // Update position
            if( is_on_belt)
                {}
            else if (go_to_belt && destination)
                this.moveToDestination();

            else if ( trying_to_mate && destination )
                this.moveToDestination();
            else
                this.moveRandom();

            // Update exp
            this.updateExperience();
        }
    } //! Update


    public void repulse( Collision2D other )
    {
        Rigidbody2D rb2d_this = GetComponent<Rigidbody2D>();
        Rigidbody2D rb2d_other = other.gameObject.GetComponent<Rigidbody2D>();
        if ( !!rb2d_this && !!rb2d_other )
        {
            rb2d_this.velocity  *= Constants.VILLAGER_COLLISION_REPULSE_FACTOR;
            rb2d_other.velocity *= Constants.VILLAGER_COLLISION_REPULSE_FACTOR;
        }
    }

    public void checkMatingCollision( Collision2D other )
    {
         // self wants mate
        if (!trying_to_mate)
            return;

        Villager v = other.gameObject.GetComponent<Villager>();
        if (!!v) 
        {
            // other wants mate
            if (!v.trying_to_mate)
                return;
            
            // repulse to prevent bbox overlap at hi speed
            this.repulse(other);

            // go mate and prevent double firing by reset trying_to_mate
            trying_to_mate = false;
            v.trying_to_mate = false;
            this.destination = null;
            refreshVillageRef();
            if (village_go)
            {
                Village village = village_go.GetComponent<Village>();
                if (!!village)
                {
                    Vector2 birth_location = Vector2.zero;
                    birth_location = Vector2.Lerp( this.transform.position, other.transform.position, 0.5f);
                    village.birth(birth_location);
                }
            }
        }
    }

    void OnCollisionEnter2D( Collision2D other) 
    {
        checkMatingCollision(other);
    }

    void OnCollisionStay2D( Collision2D other) 
    {
        checkMatingCollision(other);
    }

}
