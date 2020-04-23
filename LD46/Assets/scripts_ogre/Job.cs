﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jobs {
    public class Job
    {
        public enum JOB_LIST {
            BEGGAR = 0,
            FARMER = 1,
            BUILDER = 2,
            CLERIC = 3,
            BARD = 4,
            KING = 5
        }

        protected int level_condition;

        public int getLevelRequired() { return level_condition; }

        public virtual void applyJobEffect( Village iVillage ) {}

        public virtual Sprite getJobSprite( Villager.SEX iSex )
        {
            return Resources.Load<Sprite>(Constants.villager_template_sprite);
        }

        public bool canApplyToJob( Villager v)
        {
            return (v.level >= this.level_condition);
        }

        public static int availableJobsForLevel( int iLevel )
        {
            int n_jobs = 1; // beggar always available
            switch (iLevel)
            {
                case 0:
                    n_jobs = 1;
                    break;
                case 1:
                    n_jobs = 3;
                    break;
                case 2:
                    n_jobs = 5;
                    break;
                case 3:
                    n_jobs = 6;
                    break;
                default:
                    n_jobs = 1;
                    break;
            }
            return n_jobs;
        }

        public virtual string getJobName()
        { return "unassigned"; }

        public Job() {}

    }

    public class Beggar : Job
    {
        public override void applyJobEffect(Village iVillage)
        {
            // does nothing but eat food :(
        }

        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return ( iSex == Villager.SEX.Female ) ? 
                Resources.Load<Sprite>( Constants.villager_female_sprite ) :
                Resources.Load<Sprite>( Constants.villager_male_sprite   ) ;
        }

        public Beggar()
        {
            this.level_condition = 0;
        }

        public override string getJobName()
        {
            return Constants.beggar_job_name;
        }
    }

    public class Farmer : Job
    {
        public override void applyJobEffect( Village iVillage )
        {
            // FOOD ++
            iVillage.food = (int) Mathf.Min( iVillage.food + Constants.FARMER_FOOD_INCOME, Constants.MAX_FOOD);
        }

        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return (iSex == Villager.SEX.Female) ? 
                Resources.Load<Sprite>( Constants.farmer_female_sprite ) :
                Resources.Load<Sprite>( Constants.farmer_male_sprite   ) ;
        }

        public Farmer()
        {
            this.level_condition = 1;
        }

        public override string getJobName()
        {
            return Constants.farmer_job_name;
        }
    }

    
    public class Builder : Job
    {
        // TODO : Make me move the hiouse i'm repairing physcially
        public override void applyJobEffect( Village iVillage )
        {
            // Repair broken houses
            foreach( GameObject house_go in iVillage.houses)
            {
                House house = house_go.GetComponent<House>();
                if(!house)
                    continue;
                if ( house.Life < Constants.HOUSE_MAX_HP )
                { 
                    house.Life = Mathf.Min( house.Life + Constants.BUILDER_REPAIR_SPEED, Constants.HOUSE_MAX_HP);
                    break;
                }
            }

        }
        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return (iSex == Villager.SEX.Female) ? 
                Resources.Load<Sprite>( Constants.builder_female_sprite ) :
                Resources.Load<Sprite>( Constants.builder_male_sprite   ) ;
        }
        public Builder()
        {
            this.level_condition = 1;
        }

        public override string getJobName()
        {
            return Constants.builder_job_name;
        }
    }

    public class Cleric : Job
    {
        private GameObject ogre_ref;

        public override void applyJobEffect( Village iVillage )
        {
            // Boost ogre moral
            if (ogre_ref == null)
                ogre_ref = GameObject.Find( Constants.OGRE_GO_NAME );
            
            OgreBehaviour ob = ogre_ref.GetComponent<OgreBehaviour>();
            if (!!ob)
                ob.AddMoral((int)Constants.CLERIC_OGRE_MORAL);

        }
        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return (iSex == Villager.SEX.Female) ? 
                Resources.Load<Sprite>( Constants.cleric_female_sprite ) :
                Resources.Load<Sprite>( Constants.cleric_male_sprite   ) ;
        }
        public Cleric()
        {
            this.level_condition = 2;
            ogre_ref = GameObject.Find( Constants.OGRE_GO_NAME );
        }
        
        public override string getJobName()
        {
            return Constants.cleric_job_name;
        }
    }

    public class Bard : Job
    {
        public override void applyJobEffect( Village iVillage )
        {
            // Boost village moral
            iVillage.moral = (int) Mathf.Min( iVillage.moral + Constants.BARD_VILLAGE_MORAL, Constants.MAX_MORAL);
        }
        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return (iSex == Villager.SEX.Female) ? 
                Resources.Load<Sprite>( Constants.bard_female_sprite ) :
                Resources.Load<Sprite>( Constants.bard_male_sprite   ) ;
        }
        public Bard()
        {
            this.level_condition = 2;
        }

        public override string getJobName()
        {
            return Constants.bard_job_name;
        }
    }

    public class King : Job
    {
        public override void applyJobEffect( Village iVillage )
        {
            iVillage.moral = (int) Mathf.Min( iVillage.moral * Constants.KING_VILLAGE_BUFF_COEF, Constants.MAX_MORAL);
            iVillage.food = (int) Mathf.Min( iVillage.food * Constants.KING_VILLAGE_BUFF_COEF , Constants.MAX_FOOD);
        }
        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return (iSex == Villager.SEX.Female) ? 
                Resources.Load<Sprite>( Constants.king_female_sprite ) :
                Resources.Load<Sprite>( Constants.king_male_sprite   ) ;
        }
        public King()
        {
            this.level_condition = 3;
        }

        public override string getJobName()
        {
            return Constants.king_job_name;
        }
    }
}
