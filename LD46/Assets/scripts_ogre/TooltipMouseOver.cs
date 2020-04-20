using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class TooltipMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler// required interface when using the OnPointerEnter method.
{
    public string tooltip;
     private bool mouse_over = false;
     void Update()
     {
         if (mouse_over)
         {
            GameObject pc_go = GameObject.Find(Constants.PLAYER_CONTROLLER_GO_NAME);
            if (!!pc_go)
            {
                PlayerController pc = pc_go.GetComponent<PlayerController>();
                if (!!pc)
                {
                    pc.updateTooltip(tooltip);
                }
            }
         }
     }
 
     public void OnPointerEnter(PointerEventData eventData)
     {
         mouse_over = true;
     }
 
     public void OnPointerExit(PointerEventData eventData)
     {
         mouse_over = false;
     }
}
