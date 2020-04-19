using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // GameObjects
    public GameObject village_go;
    public GameObject ogre_go;
    public GameObject UI_ref;
    private GameObject UI_go;
    private GameObject selected_villager;
    public GameObject  UIStats_ref;
    private GameObject UIStats_go;

    // Icons
    private Sprite male_ico     ;
    private Sprite female_ico   ;

    // Panels
    public Image iAssignPanel;

    public void spawnVillagerPanel(Villager v)
    {
        if (UI_go == null)
            return;
        UI_go.gameObject.SetActive(true);

        string name     = v.name;
        string job_name = v.job_str;
        string level    = "" + v.level;

        Text[] text_fields = UI_go.GetComponentsInChildren<Text>();
        foreach (Text t in text_fields)
        {
            if(t.name == Constants.villager_panel_name_field)
            { t.text = name; }
            else if(t.name == Constants.villager_panel_job_field)
            { t.text = job_name; }
            else if(t.name == Constants.villager_panel_level_field)
            { t.text = level; }
        }

        Image[] images = UI_go.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if( image.name == Constants.villager_panel_sex_ico )
            { 
                image.sprite = (v.sex == Villager.SEX.Female) ? female_ico : male_ico;
            }
        }

    }

    // /!\ only used to close panel when selected villager is sacrificed, 
    // otherwise we close the panel with ClosePanel (cf. UIController.cs )
    public void unselectVillager()
    {
        if (UI_go == null)
            return;
        UI_go.gameObject.SetActive(false);
        selected_villager = null;
    }

    public void sacrificeVillager()
    {
        if (selected_villager == null)
            return;

        Village  village    = village_go.GetComponent<Village>();
        Villager villager   = selected_villager.GetComponent<Villager>();

        if (!!village)
        {
            village.sendVillagerToBelt(villager);
            unselectVillager();
        }
    }

    public void toggleAssignPanel()
    {
        if (iAssignPanel == null)
        {
            Image[] child_panels = UI_go.GetComponentsInChildren<Image>();
            foreach (Image im in child_panels)
            {
                if (im.name == Constants.assign_panel_name)
                {
                    //im.gameObject.SetActive(!im.gameObject.activeInHierarchy);
                    iAssignPanel = im;
                }
            }
        }
        iAssignPanel.gameObject.SetActive(!iAssignPanel.gameObject.activeInHierarchy);
    }

    // Start is called before the first frame update
    void Start()
    {
        // load resources
        this.male_ico     = Resources.Load<Sprite>(Constants.male_ico);
        this.female_ico   = Resources.Load<Sprite>(Constants.female_ico);

        // Villager Panel UI
        UI_go = Instantiate(UI_ref);

        // Set listeners on buttons
        Button[] buttons = UI_go.GetComponentsInChildren<Button>();
        foreach ( Button b in buttons )
        {
            if (b.gameObject.name == Constants.sacrifice_btn_name)
            {
                b.onClick.AddListener(sacrificeVillager);
            }
            if (b.gameObject.name == Constants.assign_btn_name)
            {
                toggleAssignPanel(); // deactivate panel at start
                b.onClick.AddListener( toggleAssignPanel );
            }

        }
        // start with ui deactivated
        UI_go.gameObject.SetActive(false);

        // Stats Panel UI
        UIStats_go = Instantiate(UIStats_ref);
        StatsUI ui_stats = UIStats_go.GetComponent<StatsUI>();
        if (!!ui_stats)
        {
            ui_stats.village_go   = this.village_go.gameObject;
            ui_stats.ogre_go      = this.ogre_go.gameObject;
        }

    }


    // Update is called once per frame
    void Update()
    {
        // Open villager panel on click
        if ( Input.GetMouseButtonDown(0) ) 
        {  
            Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouse_position_2d = new Vector2( mouse_position.x, mouse_position.y);

            RaycastHit2D hit = Physics2D.Raycast( mouse_position_2d, Vector2.zero);
            Collider2D collider = hit.collider;
            if( !!collider )
            {
                Villager v      = collider.GetComponent<Villager>();
                if ( !!v && !v.is_on_belt )
                {
                    spawnVillagerPanel( v );
                    selected_villager = v.gameObject;
                }
            }
        }
    }
}
