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
    public const string female_ico  = "res_ogre/FEMALE_ICO";
    public const string male_ico    = "res_ogre/MALE_ICO";


    // UI tags
    public const string villager_panel_name_field   = "Name_field";
    public const string villager_panel_job_field    = "job_field";
    public const string villager_panel_level_field  = "level_field";
    public const string villager_panel_name  = "VillagerPanel";
    public const string villager_panel_sex_ico  = "sex_ico";

    public const string close_panel_name     = "ClosePanel";
    public const string sacrifice_btn_name     = "sacrifice_btn";

    public const string ogre_moral_bar  = "ogre_moral_bar";
    public const string ogre_food_bar   = "ogre_food_bar";
    public const string village_moral_bar  = "village_moral_bar";
    public const string village_food_bar   = "village_food_bar";

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

    // Colors
    public const string UI_red     = "#CA1C18";
    public const string UI_orange  = "#CB8D18";
    public const string UI_green   = "#1EA427";
}
