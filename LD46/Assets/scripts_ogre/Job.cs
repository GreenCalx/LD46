using System.Collections;
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

        public GameObject targeted_building;
        public bool targeted_building_reached = false;
        public bool job_done = false;

        public bool need_building_to_operate = false;


        public int getLevelRequired() { return level_condition; }

        public virtual void applyJobEffect( Village iVillage ) {}

        public virtual void applyJobMove( Villager iVillager ) {}

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

        public bool hasABuildingTarget()
        {
            return ( !targeted_building_reached && need_building_to_operate && (targeted_building!=null) );
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
        public override void applyJobMove(Villager iVillager)
        {
            iVillager.moveRandom();
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
            this.need_building_to_operate = false;
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
            if (targeted_building == null)
            {
                List<GameObject> farms = iVillage.getFarms();
                if (farms.Count==0)
                    return;
                    int idx = (int) (Random.Range(0f, 1f) * farms.Count);
                targeted_building = farms[idx] ;
            }
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
            this.need_building_to_operate = true;
        }

        public override string getJobName()
        {
            return Constants.farmer_job_name;
        }
        public override void applyJobMove(Villager iVillager)
        {
            if (!!targeted_building)
            {
                if (targeted_building != null)
                {
                    iVillager.destination = targeted_building.transform;
                    iVillager.moveToDestination();
                    return;  
                }
            }

            iVillager.moveRandom();
        }
    }

    
    public class Builder : Job
    {
        
        public GameObject targeted_building;

        public override void applyJobEffect( Village iVillage )
        {
            // Is targeted house still broken?
            if (!!targeted_building)
            {
                House house = targeted_building.GetComponent<House>();
                if (!!house)
                {
                    if( house.Life == Constants.HOUSE_MAX_HP )
                    {
                        // JOB DONE
                        this.job_done = true;
                        //house.unsubscribe(  );
                        targeted_building = null;
                    }
                    else
                    {
                        house.Life = Mathf.Min( house.Life + Constants.BUILDER_REPAIR_SPEED, Constants.HOUSE_MAX_HP);
                    }
                }
                return;
            }

            //  Else find a broken house to repair
            foreach( GameObject house_go in iVillage.houses)
            {
                House house = house_go.GetComponent<House>();
                if(!house)
                    continue;
                if ( house.Life < Constants.HOUSE_MAX_HP )
                { 
                    house.Life = Mathf.Min( house.Life + Constants.BUILDER_REPAIR_SPEED, Constants.HOUSE_MAX_HP);
                    targeted_building = house_go;
                    //house.subscribe(  );
                    break;
                }
            }

        }

        public override void applyJobMove(Villager iVillager)
        {
            if (targeted_building != null)
            {
                iVillager.destination = targeted_building.transform;
                iVillager.moveToDestination();
                return;  
            }
            // default
            iVillager.moveRandom();
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
            this.targeted_building = null;
            this.need_building_to_operate = true;
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
        public override void applyJobMove(Villager iVillager)
        {
            iVillager.moveRandom();
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
            this.need_building_to_operate = false;
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

        public override void applyJobMove(Villager iVillager)
        {
            iVillager.moveRandom();
        }

        public Bard()
        {
            this.need_building_to_operate = false;
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
        public override void applyJobMove(Villager iVillager)
        {
            iVillager.moveRandom();
        }
        public override Sprite getJobSprite( Villager.SEX iSex )
        {
            return (iSex == Villager.SEX.Female) ? 
                Resources.Load<Sprite>( Constants.king_female_sprite ) :
                Resources.Load<Sprite>( Constants.king_male_sprite   ) ;
        }
        public King()
        {
            this.need_building_to_operate = false;
            this.level_condition = 3;
        }

        public override string getJobName()
        {
            return Constants.king_job_name;
        }
    }
}
