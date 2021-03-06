﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    //  Scenes names
    public const string MAIN_GAME_SCENE = "Schoenbr";
    public const string TITLE_SCENE     = "TitleScreen";
    public const string GAME_OVER_SCENE = "GameOver";
    public const string WIN_GAME_SCENE = "GGScene";

    // Game object names
    public const string BELT_LOADING_POINT      = "belt_loading_point";
    public const string BELT_DISCHARGE_POINT    = "belt_discharge_point";
    public const string VILLAGE_GO_NAME = "Village";
    public const string OGRE_GO_NAME = "ogre";
    public const string AUDIO_MANAGER_GO_NAME = "AudioManager";
    public const string PLAYER_CONTROLLER_GO_NAME = "PLAYER";
    public const string BELT = "conveyor_belt";
    public const string FARM_GO_NAME = "farm_zone";


    // Graphx
    public const string villager_template_sprite = "res_ogre/villager_template";
    public const string villager_male_sprite = "res_ogre/villager_male";
    public const string villager_female_sprite = "res_ogre/villager_female";
    public const string farmer_male_sprite = "res_ogre/villager_male_farmer";
    public const string farmer_female_sprite = "res_ogre/villager_female_farmer";
    public const string builder_male_sprite = "res_ogre/villager_male_builder";
    public const string builder_female_sprite = "res_ogre/villager_female_builder";
    public const string cleric_male_sprite = "res_ogre/villager_male_cleric";
    public const string cleric_female_sprite = "res_ogre/villager_female_cleric";
    public const string bard_male_sprite = "res_ogre/villager_male_bard";
    public const string bard_female_sprite = "res_ogre/villager_female_bard";
    public const string king_male_sprite = "res_ogre/villager_male_king";
    public const string king_female_sprite = "res_ogre/villager_female_king";
    
    public const string female_ico  = "res_ogre/FEMALE_ICO";
    public const string male_ico    = "res_ogre/MALE_ICO";


    // UI tags
    public const string villager_panel_name_field   = "Name_field";
    public const string villager_panel_job_field    = "job_field";
    public const string villager_panel_level_field  = "level_field";
    public const string villager_panel_name  = "VillagerPanel";
    public const string villager_panel_sex_ico  = "sex_ico";
    public const string ui_panel_name           = "UI";
    public const string close_panel_name        = "ClosePanel";
    public const string assign_panel_name        = "AssignPanel";
    public const string sacrifice_btn_name      = "sacrifice_btn";
    public const string assign_btn_name        = "assign_btn";

    // job assignement buttons
    public const string assign_farmer_btn_name    = "farmer_btn";
    public const string assign_builder_btn_name   = "builder_btn";
    public const string assign_cleric_btn_name    = "cleric_btn";
    public const string assign_bard_btn_name      = "bard_btn";
    public const string assign_king_btn_name      = "king_btn";
    public const string jobassign_tooltip_field = "jobassign_tooltip_field";

    // UI Bars
    public const string ogre_moral_bar  = "ogre_moral_bar";
    public const string ogre_food_bar   = "ogre_food_bar";
    public const string village_moral_bar  = "village_moral_bar";
    public const string village_food_bar   = "village_food_bar";

    // UI tooltips
    public const string FARMER_JOB_TOOLTIP = "harvest food";
    public const string BUILDER_JOB_TOOLTIP = "repair buildings";
    public const string CLERIC_JOB_TOOLTIP = "boost ogre moral";
    public const string BARD_JOB_TOOLTIP = "boost village moral";
    public const string KING_JOB_TOOLTIP = "boost village production";

    // UI Win cond
    public const string win_cond_panel = "win_cond_panel";
    public const string cur_pop_count_win_cond_field  = "cur_pop_count_win_cond_field";
    public const string n_pop_win_cond_field   = "n_pop_win_cond_field";
    public const string cur_male_king_pop_count_win_cond_field  = "cur_male_king_pop_count_win_cond_field";
    public const string king_pop_count_win_cond_field   = "king_pop_count_win_cond_field";
    public const string cur_female_king_pop_count_win_cond_field  = "cur_female_king_pop_count_win_cond_field";
    public const string female_king_pop_count_win_cond_field   = "female_king_pop_count_win_cond_field";


    // Time
    public const float job_time_step = 5f; // time between doJob in village  
    public const float village_moral_time_step = 5f; // time between moral update
    public const float village_mate_time_step = 5f; // time between try to mate in village
    public const float move_time_step = 1.5f; // .5 seconds    
    public const float belt_time_step = 1f;
    public const float villager_exp_gain_time_step = 5f; // 5 Seconds
    public const float villager_change_job_time = 3f; // 3 Seconds
    public const float villager_change_job_time_lvl2 = 4f; // 4 Seconds
    public const float villager_change_job_time_lvl3 = 5f; // 5 Seconds

    public const float villagers_hungry_time_step = 5f;
    


    // Speeds
    public const float villager_speed = 2f; // 1 seconds    
    public const float belt_speed = 3f; // 1 seconds   

    public const float HandSpeed = 3;
    public const float LittleOgreSpeed = 2;


    // Tweak consts
    public const int START_POP  = 6;
    public const int START_HOUSE = 4;
    public const int MAX_FOOD   = 100;
    public const int MAX_MORAL  = 100;
    public const float villager_move_step = 0.5f;
    public const int BELT_CONVEYOR_CAPACITY = 10;
    public const int VILLAGER_EXP_GAIN = 1;
    public const int VILLAGER_EXP_REQ_LEVEL1 = 4;
    public const int VILLAGER_EXP_REQ_LEVEL2 = 14;
    public const int VILLAGER_EXP_REQ_LEVEL3 = 30;
    public const int VILLAGER_FOOD_NEED = 2; // units of food eaten by a villager
    public const float FARMER_FOOD_INCOME = 8f; // food generated by a farmer
    public const int BUILDER_REPAIR_SPEED = 5; // building hr repaired by a builder
    public const float CLERIC_OGRE_MORAL = 5f; // moral generated to help ogre
    public const float BARD_VILLAGE_MORAL = 5f; // moral generated for village
    public const float KING_VILLAGE_BUFF_COEF = 1.3f; // general buff to village moral & food
    public const int HOUSE_MAX_HP = 50;
    public const int HOUSE_DAMAGED_THR = HOUSE_MAX_HP / 2;
    public const int MORAL_LOSS_HALF_BROKEN_HOUSE = 3; // moral loss when house is broko
    public const int MORAL_LOSS_BROKEN_HOUSE = 6; // moral loss when house is broko
    public const int OGRE_ANGER_THR = 50;
    public const int OGRE_GET_CALM_THR = 80;
    public const int OGRE_MORAL_GAIN_IN_ANGER = 10;
    public const int OGRE_WRONG_LVL_COMMAND_PENALTY = 5;
    public const int OGRE_WRONG_JOB_COMMAND_PENALTY = 15;
    public const int OGRE_WRONG_SEX_COMMAND_PENALTY = 20;
    public const int OGRE_DAMAGE_TO_HOUSE = 5;
    public const int VILLAGE_FAMINE_TRH = 50;
    public const int VILLAGE_FAMINE_MORAL_LOSS = 5;
    public const int VILLAGE_SACRIFICE_MORAL_LOSS = 6;
    public const int VILLAGE_MORAL_REQ_TO_MATE = 40;
    public const float LEVEL_UP_INDICATOR_DURATION = 1f; // 1sec
    public const float VILLAGER_COLLISION_REPULSE_FACTOR = -0.5f;

    public const float ogre_rampage_threshold_stop = 90;
    public const float ogre_rampage_threshold_start = 25;
    public const float ogre_hungry_threshold_start = 75;
    public const int Villager_food = 25;
    public const int Ogre_Food_Tick_Loss = -5;
    public const int Ogre_Food_Tick_Time = 3;
    public const float Ogre_Free_Food_Moral = 10;

    // win condition
    public const int WIN_COND_POP = 25;
    public const int WIN_COND_N_MALE_KING = 1;
    public const int WIN_COND_N_FEMALE_KING = 1;


    // MATE PROBA
    // Values of Log(y+1) func to 
    public static readonly double[] villager_mate_proba = { 0.298865,  0.477503, 0.606137, 0.699430, 0.779444, 0.845592};


    // Colors
    public const string UI_red     = "#CA1C18";
    public const string UI_orange  = "#CB8D18";
    public const string UI_green   = "#1EA427";

    // Jobs names
    public const string beggar_job_name = "Beggar";
    public const string farmer_job_name = "Farmer";
    public const string builder_job_name = "Builder";
    public const string cleric_job_name = "Cleric";
    public const string bard_job_name = "Bard";
    public const string king_job_name = "King";


    // Animator vars
    public const string villager_change_job = "change_job";
    public const string villager_change_job_lvl2 = "change_job_lvl2";
    public const string villager_change_job_lvl3 = "change_job_lvl3";


    public const int BloodTiming = 2;
    public const float max_time_in_rampage = 8f;
    // SFX
    public const string WHY_ME_VOICE = "why_me";
    public const string OH_NO_VOICE = "oh_no";
    public const string MY_LIFE_FOR_SCHNIBBLE_VOICE = "my_life_for_the_schnibble";
    public const string ALRIGHT_LETS_GO_VOICE = "alright_lets_go";
    public const string FRESH_MEAT = "ogre_rampage";
    public const string EATING = "ogre_eat";
    public const string MAIN_THEME_BGM = "main_theme";
    public const string TITLE_THEME_BGM = "title_theme";

}
