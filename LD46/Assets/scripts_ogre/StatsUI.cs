﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public GameObject village_go;
    public GameObject ogre_go;

    Image ogre_moral_bar, ogre_food_bar;
    Image village_moral_bar, village_food_bar;

    // all bars share same max width
    private float bar_max_width;
    private float bar_max_height;

    // Start is called before the first frame update
    void Start()
    {

        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.name == Constants.ogre_food_bar)
                this.ogre_food_bar = image;
            if (image.name == Constants.ogre_moral_bar)
                this.ogre_moral_bar = image;
            if (image.name == Constants.village_food_bar)
                this.village_food_bar = image;
            if (image.name == Constants.village_moral_bar)
                this.village_moral_bar = image;
        }

        // all bars share same max width && height
        this.bar_max_width = this.ogre_food_bar.rectTransform.sizeDelta.x;
        this.bar_max_height = this.ogre_food_bar.rectTransform.sizeDelta.y;
    }

    private void updateBarWidth(Image iBar, Vector2 iNewSize, bool iInvertBarProgression)
    {
        // get current left coordinates
        float original_coordinates = (iInvertBarProgression) ? iBar.rectTransform.offsetMax.x : iBar.rectTransform.offsetMin.x;
        // scale the bar
        iBar.rectTransform.sizeDelta  = iNewSize;
        // translate to keep bar on left using original coordinates
        float new_coordinates = (iInvertBarProgression) ? iBar.rectTransform.offsetMax.x : iBar.rectTransform.offsetMin.x;
        float delta = new_coordinates - original_coordinates;
        iBar.rectTransform.transform.Translate( new Vector2( -delta, 0f) );
    }

    private void updateBarColor(Image iBar, decimal iResourcePercentage)
    {
        Color new_color;
        if ( iResourcePercentage <= 25 )
        { // RED
            if (!ColorUtility.TryParseHtmlString( Constants.UI_red, out new_color))
                return;
        }
        else if ( iResourcePercentage <= 50 )
        { // ORANGE
            if (!ColorUtility.TryParseHtmlString( Constants.UI_orange, out new_color))
                return;
        }
        else
        { // GREEN
            if (!ColorUtility.TryParseHtmlString( Constants.UI_green, out new_color))
                return;        
        }
        iBar.color = new_color;
    }

    // Update is called once per frame
    void Update()
    {
        //update ogre bars
        if (!!village_go)
        {
            Village v = village_go.GetComponent<Village>();
            if (!!v)
            {
                this.updateBarWidth( this.village_food_bar, 
                                     new Vector2( ((v.food * this.bar_max_width) / Constants.MAX_FOOD), this.bar_max_height), 
                                     false );
                this.updateBarWidth( this.village_moral_bar, 
                                     new Vector2( ((v.moral * this.bar_max_width) / Constants.MAX_MORAL), this.bar_max_height), 
                                     false );

                decimal food_ratio_percentage  = ( (decimal)v.food / Constants.MAX_FOOD) * 100m;
                decimal moral_ratio_percentage = ( (decimal)v.moral / Constants.MAX_MORAL) * 100m;

                this.updateBarColor( this.village_food_bar , food_ratio_percentage);
                this.updateBarColor( this.village_moral_bar, moral_ratio_percentage);
            }
        }

        //update village bars
        if (!!ogre_go)  
        {
            OgreBehaviour ogre = ogre_go.GetComponent<OgreBehaviour>();
            if (!!ogre)
            {
                this.updateBarWidth( this.ogre_food_bar, 
                                     new Vector2( ((ogre.food * this.bar_max_width) / Constants.MAX_FOOD), this.bar_max_height), 
                                     true );
                this.updateBarWidth( this.ogre_moral_bar, 
                                     new Vector2( ((ogre.moral * this.bar_max_width) / Constants.MAX_MORAL), this.bar_max_height), 
                                     true );

                decimal food_ratio_percentage  = ( (decimal)ogre.food / Constants.MAX_FOOD) * 100m;
                decimal moral_ratio_percentage = ( (decimal)ogre.moral / Constants.MAX_MORAL) * 100m;

                this.updateBarColor( this.ogre_food_bar , food_ratio_percentage);
                this.updateBarColor( this.ogre_moral_bar, moral_ratio_percentage);
            }
        }
    }
}