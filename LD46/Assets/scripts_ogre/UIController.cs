using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIController : MonoBehaviour
{
    private const bool ui_debug = false;

     private GraphicRaycaster raycaster;

    // Start is called before the first frame update
    void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
         //Check if the left Mouse button is clicked
         if (Input.GetKeyDown(KeyCode.Mouse0))
         {
             PointerEventData pointerData = new PointerEventData(EventSystem.current);
             List<RaycastResult> results = new List<RaycastResult>();
 
             pointerData.position = Input.mousePosition;
             this.raycaster.Raycast(pointerData, results);
 
            bool hits_villager_panel    = false;
            bool hits_close_panel       = false;
            foreach (RaycastResult result in results)
            {
                if (ui_debug)
                    Debug.Log("Hit " + result.gameObject.name);

                if ( result.gameObject.name == Constants.villager_panel_name )
                {
                    hits_villager_panel = true;
                }
                else if ( result.gameObject.name == Constants.close_panel_name )
                {
                    hits_close_panel = true;
                }
            }//! foreach raycast result
            if (!hits_villager_panel && hits_close_panel)
                gameObject.SetActive(false);
         }
    }//! update
}
