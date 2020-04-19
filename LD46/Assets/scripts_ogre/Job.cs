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

        public virtual void applyJobEffect() {}

        public virtual Sprite getJobSprite( Villager.SEX iSex )
        {
            return Resources.Load<Sprite>(Constants.villager_template_sprite);
        }

        public bool canApplyToJob( Villager v)
        {
            return (v.level >= this.level_condition);
        }

        public virtual string getJobName()
        { return "unassigned"; }

        public Job() {}

    }

    public class Beggar : Job
    {
        public override void applyJobEffect()
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
            return "Beggar";
        }
    }

    public class Farmer : Job
    {
        public override void applyJobEffect()
        {
            // FOOD ++
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
            return "Farmer";
        }
    }

    
    public class Builder : Job
    {
        public override void applyJobEffect()
        {
            // Repair broken houses
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
            return "Builder";
        }
    }

    public class Cleric : Job
    {
        public override void applyJobEffect()
        {
            // Boost ogre moral
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
        }
        
        public override string getJobName()
        {
            return "Cleric";
        }
    }

    public class Bard : Job
    {
        public override void applyJobEffect()
        {
            // Boost ogre moral
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
            return "Bard";
        }
    }

    public class King : Job
    {
        public override void applyJobEffect()
        {
            // Boost ogre moral
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
            return "King";
        }
    }
}
