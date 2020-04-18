using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    // Game object names
    public const string BELT_LOADING_POINT      = "belt_loading_point";
    public const string BELT_DISCHARGE_POINT    = "belt_discharge_point";

    // Graphx
    public const string villager_male_sprite = "res_ogre/villager_male";
    public const string villager_female_sprite = "res_ogre/villager_female";

    // Time
    public const float job_time_step = 10f; // 10 seconds    
    public const float move_time_step = 0.5f; // 1 seconds    
    public const float belt_time_step = 1f;

    // Speeds
    public const float villager_speed = 0.5f; // 1 seconds    
    public const float belt_speed = 1f; // 1 seconds    


    // Tweak consts
    public const int START_POP  = 6;
    public const int MAX_FOOD   = 100;
    public const int MAX_MORAL  = 100;
    public const float villager_move_step = 0.5f;
    public const int BELT_CONVEYOR_CAPACITY = 10;
}
