using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature : MonoBehaviour
{
    public string id;
    public int index;
    private IconGameUI _iconGameUI;

    public void Start()
    {
        _iconGameUI = FindObjectOfType<IconGameUI>();
    }

    public void Select()
    {
        Destroy(gameObject);
        _iconGameUI.SelectFromIcon(id, index);
    }
}
