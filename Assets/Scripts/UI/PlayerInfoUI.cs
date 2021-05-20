using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    public Slider healthBar;

    public void UpdateHealthBar(int value)
    {
        healthBar.value = value;
    }

    public void UpdateMaxHealthBar(int value)
    {
        healthBar.maxValue = value;
    }
    
}
