using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Munition : MonoBehaviour
{

    public TMP_Text canvasAmmoText1;
    public TMP_Text canvasAmmoText2;
    private void Start()
    {
        
    }

    public void UIAmmo1(int _value)
    {
        canvasAmmoText1.text = "" + _value;
    }
    public void UIAmmo2(int _value)
    {
        canvasAmmoText2.text = "" + _value;
    }
}

