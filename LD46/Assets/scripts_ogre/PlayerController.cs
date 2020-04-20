using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using jobs;

public class PlayerController : MonoBehaviour
{
    // GameObjects
    public GameObject village_go;
    public GameObject ogre_go;
    public GameObject UI_ref;
    private GameObject UI_go;
    private GameObject selected_villager;
    public GameObject UIStats_ref;
    private GameObject UIStats_go;

    // Icons
    private Sprite male_ico;
    private Sprite female_ico;

    // Panels
    public Image assignPanel;

    public void spawnVillagerPanel(Villager v)
    {
        if (UI_go == null)
            return;
        UI_go.gameObject.SetActive(true);

        string name = v.name;
        string job_name = v.job_str;
        string level = "" + v.level;

        Text[] text_fields = UI_go.GetComponentsInChildren<Text>();
        foreach (Text t in text_fields)
        {
            if (t.name == Constants.villager_panel_name_field)
            { t.text = name; }
            else if (t.name == Constants.villager_panel_job_field)
            { t.text = job_name; }
            else if (t.name == Constants.villager_panel_level_field)
            { t.text = level; }
        }

        Image[] images = UI_go.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.name == Constants.villager_panel_sex_ico)
            {
                image.sprite = (v.sex == Villager.SEX.Female) ? female_ico : male_ico;
            }
        }

        // grey out unavailable job
        disableUnavailableJobs(v.level);

    }

    public void disableUnavailableJobs( int villager_lvl)
    {
        initAssignPanelRef(); // just in case

        Button[] btns = assignPanel.gameObject.GetComponentsInChildren<Button>();
        foreach (Button b in btns)
        {
            if (b.name == Constants.assign_farmer_btn_name)
            {
                b.interactable = (villager_lvl >= 1);
            }
            else if (b.name == Constants.assign_builder_btn_name)
            {
                b.interactable = (villager_lvl >= 1);
            }
            else if (b.name == Constants.assign_cleric_btn_name)
            {
                b.interactable = (villager_lvl >= 2);
            }
            else if (b.name == Constants.assign_bard_btn_name)
            {
                b.interactable = (villager_lvl >= 2);
            }
            else if (b.name == Constants.assign_king_btn_name)
            {
                b.interactable = (villager_lvl >= 3);
            }
        }//! foreach
    }


    // Refresh panel for selected villager
    public void refreshVillagerPanel()
    {
        if ((UI_go == null) || (selected_villager == null))
            return;

        Villager v = selected_villager.GetComponent<Villager>();
        if (v == null)
            return;

        string name = v.name;
        string job_name = v.job_str;
        string level = "" + v.level;
        Text[] text_fields = UI_go.GetComponentsInChildren<Text>();
        foreach (Text t in text_fields)
        {
            if (t.name == Constants.villager_panel_name_field)
            { t.text = name; }
            else if (t.name == Constants.villager_panel_job_field)
            { t.text = job_name; }
            else if (t.name == Constants.villager_panel_level_field)
            { t.text = level; }
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

        Village village = village_go.GetComponent<Village>();
        Villager villager = selected_villager.GetComponent<Villager>();

        if (!!village)
        {
            village.sendVillagerToBelt(villager);
            unselectVillager();
        }
    }

    public void initAssignPanelRef()
    {
        if (assignPanel == null)
        {
            Image[] child_panels = UI_go.GetComponentsInChildren<Image>();
            foreach (Image im in child_panels)
            {
                if (im.name == Constants.assign_panel_name)
                {
                    assignPanel = im;
                }
            }
        }
    }
    public void toggleAssignPanel()
    {
        initAssignPanelRef();
        assignPanel.gameObject.SetActive(!assignPanel.gameObject.activeInHierarchy);
    }

    void initJobAssignementListener()
    {
        initAssignPanelRef(); // just in case
        Button[] btns = assignPanel.gameObject.GetComponentsInChildren<Button>();
        foreach (Button b in btns)
        {
            if (b.name == Constants.assign_farmer_btn_name)
            {
                //b.onClick.AddListener(tryAssignSelectedToFarm);
                b.onClick.AddListener(delegate { tryAssignToJob(new Farmer()); });
            }
            else if (b.name == Constants.assign_builder_btn_name)
            {
                //b.onClick.AddListener(tryAssignSelectedToBuild);
                b.onClick.AddListener(delegate { tryAssignToJob(new Builder()); });
            }
            else if (b.name == Constants.assign_cleric_btn_name)
            {
                //b.onClick.AddListener(tryAssignSelectedToCleric);
                b.onClick.AddListener(delegate { tryAssignToJob(new Cleric()); });
            }
            else if (b.name == Constants.assign_bard_btn_name)
            {
                //b.onClick.AddListener(tryAssignSelectedToBard);
                b.onClick.AddListener(delegate { tryAssignToJob(new Bard()); });
            }
            else if (b.name == Constants.assign_king_btn_name)
            {
                //b.onClick.AddListener(tryAssignSelectedToKing);
                b.onClick.AddListener(delegate { tryAssignToJob(new King()); });
            }
        }
    }

    public void tryAssignToJob(Job iJob)
    {
        if (selected_villager == null)
            return;
        Villager v = selected_villager.GetComponent<Villager>();
        if (!!v)
        {
            if (iJob.canApplyToJob(v))
            {
                v.assignJob(iJob);
                refreshVillagerPanel();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // load resources
        this.male_ico = Resources.Load<Sprite>(Constants.male_ico);
        this.female_ico = Resources.Load<Sprite>(Constants.female_ico);

        // Villager Panel UI
        UI_go = Instantiate(UI_ref);

        // Set listeners on buttons
        Button[] buttons = UI_go.GetComponentsInChildren<Button>();
        foreach (Button b in buttons)
        {
            if (b.gameObject.name == Constants.sacrifice_btn_name)
            {
                b.onClick.AddListener(sacrificeVillager);
            }
            if (b.gameObject.name == Constants.assign_btn_name)
            {
                toggleAssignPanel(); // deactivate panel at start
                b.onClick.AddListener(toggleAssignPanel);
            }
        }
        initJobAssignementListener();

        // start with ui deactivated
        UI_go.gameObject.SetActive(false);

        // Stats Panel UI
        UIStats_go = Instantiate(UIStats_ref);
        StatsUI ui_stats = UIStats_go.GetComponent<StatsUI>();
        if (!!ui_stats)
        {
            ui_stats.village_go = this.village_go.gameObject;
            ui_stats.ogre_go = this.ogre_go.gameObject;
        }

    }


    // Update is called once per frame
    void Update()
    {
        // Open villager panel on click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouse_position_2d = new Vector2(mouse_position.x, mouse_position.y);

            RaycastHit2D[] hit = Physics2D.RaycastAll(mouse_position_2d, Vector2.zero);
            foreach (RaycastHit2D h in hit)
            {
                Collider2D collider = h.collider;
                if (!!collider)
                {
                    Villager v = collider.GetComponent<Villager>();
                    if (!!v && !v.is_on_belt)
                    {
                        spawnVillagerPanel(v);
                        selected_villager = v.gameObject;
                    }
                }

            }

        }
    }
}
