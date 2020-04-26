using System.Collections;
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

    private GameObject info_text;
    private Text info_text_villager;

    private GameObject ogreError;

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

        info_text = GameObject.Find("Info_Text");
        info_text_villager = GameObject.Find("Info_Text_Villager").GetComponent<Text>();
        ogreError = GameObject.Find("Info_Text_Ogre");
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

    private void updateWinCondFields()
    {
        if (village_go==null)
            return;
        Village v = village_go.GetComponent<Village>();
        if (v==null)
            return;

        Text[] txts = GetComponentsInChildren<Text>();
        foreach (Text t in txts)
        {
            if (t.name == Constants.cur_pop_count_win_cond_field)
                t.text = "" + v.villagers.Count;
            if (t.name == Constants.n_pop_win_cond_field)
                t.text = "" + Constants.WIN_COND_POP;
            if (t.name == Constants.cur_male_king_pop_count_win_cond_field)
                t.text = "" + v.countMaleKings();
            if (t.name == Constants.king_pop_count_win_cond_field)
                t.text = "" + Constants.WIN_COND_N_MALE_KING;
            if (t.name == Constants.cur_female_king_pop_count_win_cond_field)
                t.text = "" + v.countFemaleKings();
            if (t.name == Constants.female_king_pop_count_win_cond_field)
                t.text = "" + Constants.WIN_COND_N_FEMALE_KING;
        }
    }

    // Update is called once per frame
    void Update()
    {
        info_text.SetActive(false);
        info_text_villager.text = "";
        info_text.GetComponent<RectTransform>().sizeDelta = new Vector2 ( info_text.GetComponent<RectTransform>().sizeDelta.x , 0);

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

                if ( v.food < Constants.VILLAGE_FAMINE_TRH )
                {
                    info_text.SetActive(true);
                    info_text.GetComponent<RectTransform>().sizeDelta += new Vector2 ( 0 , 40);
                    info_text_villager.text = "Famine is killing villagers!\n You need more farmers!\n";
                }
                if ( v.moral < Constants.VILLAGE_MORAL_REQ_TO_MATE )
                {
                    info_text.SetActive(true);
                    info_text.GetComponent<RectTransform>().sizeDelta += new Vector2 ( 0 , 40);
                    info_text_villager.text += "Moral is too low to mate!\n You need more bards!";
                }
            }
        }

        //update village bars
        if (!!ogre_go)  
        {
            OgreBehaviour ogre = ogre_go.GetComponent<OgreBehaviour>();
            if (!!ogre)
            {
                this.updateBarWidth( this.ogre_food_bar, 
                                     new Vector2( ((ogre.Food * this.bar_max_width) / Constants.MAX_FOOD), this.bar_max_height), 
                                     true );
                this.updateBarWidth( this.ogre_moral_bar, 
                                     new Vector2( ((ogre.Moral * this.bar_max_width) / Constants.MAX_MORAL), this.bar_max_height), 
                                     true );

                decimal food_ratio_percentage  = ( (decimal)ogre.Food / Constants.MAX_FOOD) * 100m;
                decimal moral_ratio_percentage = ( (decimal)ogre.Moral / Constants.MAX_MORAL) * 100m;

                this.updateBarColor( this.ogre_food_bar , food_ratio_percentage);
                this.updateBarColor( this.ogre_moral_bar, moral_ratio_percentage);

                if (ogre.needDisplayCommandError)
                {
                    ogreError.SetActive(true);
                }else
                {
                    ogreError.SetActive(false);
                }
            }
        }

        // Update win cond
        updateWinCondFields();

    }
}
