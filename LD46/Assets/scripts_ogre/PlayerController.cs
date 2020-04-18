using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    public GameObject village_go;

    public GameObject UI_ref;
    private GameObject UI_go;

    private GameObject selected_villager;

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

        // start ith ui deactivated
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
