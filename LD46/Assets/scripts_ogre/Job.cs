using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jobs {
    public class Job
    {

        protected int level_condition;

        public virtual void applyJobEffect() {}

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
