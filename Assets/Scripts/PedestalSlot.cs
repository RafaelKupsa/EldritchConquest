using System.Collections;
using System.Collections.Generic;
using Backend;
using UnityEngine;
using UnityEngine.UI;

public class PedestalSlot : MonoBehaviour
{
    public IconGameUI iconGameUI;
    
    public void Release()
    {
        if (!transform.GetComponent<Image>().raycastTarget) return;
        if (Global.SelectedID is null) return;

        ((IconGame)Global.MiniGame).SetShape(Global.SelectedID);
        Destroy(Global.Selected);
        Global.SelectedID = null;
        Global.Selected = null;

        iconGameUI.UpdateIcon();
    }
}
