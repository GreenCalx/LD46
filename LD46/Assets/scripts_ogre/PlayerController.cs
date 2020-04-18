using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // GameObjects
    public GameObject village_go;
    public GameObject UI_ref;
    private GameObject UI_go;
    private GameObject selected_villager;

    // Icons
    private Sprite male_ico     ;
    private Sprite female_ico   ;

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

    // Start is called before the first frame update
    void Start()
    {
        UI_go = Instantiate(UI_ref);

        // Set listeners on buttons
        Button[] buttons = UI_go.GetComponentsInChildren<Button>();
        foreach ( Button b in buttons )
        {
            if (b.gameObject.name == Constants.sacrifice_btn_name)
            {
                b.onClick.AddListener(sacrificeVillager);
            }
        }

        // load resources
        this.male_ico     = Resources.Load<Sprite>(Constants.male_ico);
        this.female_ico   = Resources.Load<Sprite>(Constants.female_ico);

        // start with ui deactivated
        UI_go.gameObject.SetActive(false);
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
