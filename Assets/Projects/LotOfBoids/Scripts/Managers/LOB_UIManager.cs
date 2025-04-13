using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LOB_UIManager : MonoBehaviour
{
    public WaypointAgent wA;
    public TMP_Text staminaText; 
    
    void Update()
    {
        staminaText.text = wA.Stamina.ToString();
    }
}
