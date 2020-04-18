using System.Collections;
using System.Collections.Generic;

using System.Text;
using System;
using UnityEngine;

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

    // villager weak attributes
    public string name;
    public string job_str;


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
        this.job   = new jobs.Beggar(); // starts beggar regardless of level
        this.job_str = this.job.getJobName();
    }

    public void assignJob( jobs.Job iJob )
    {
        if ( iJob.canApplyToJob(this) )
            this.job = iJob;
    }

    public void doJob()
    {
        this.job.applyJobEffect();
    }

    public void update_graphics()
    {
        SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
        if (!!sr)
        {
            Sprite sprite = ( this.sex == SEX.Female ) ?
                Resources.Load("villager_female", typeof(Sprite)) as Sprite :
                Resources.Load("villager_male", typeof(Sprite)) as Sprite   ;
            sr.sprite = sprite;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       randomize();
       update_graphics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
