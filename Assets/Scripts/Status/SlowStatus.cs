using System;
using System.Collections.Generic;
using UnityEngine;

public class SlowStatus
{

    public SlowStatus(float currentSlow = 1f)
    {
        this.currentSlow = currentSlow;
    }
    
    private float currentSlow;
    public Action<float> updated;
    
    public void AddSlow(float value)
    {
        currentSlow += value;
        updated?.Invoke(currentSlow);
    }

    public void SetSlow(float value)
    {
        currentSlow = value;
    }
    
    public float GetSlowAmount()
    { 
        return Mathf.Clamp(currentSlow, 0f, 1f); ;
    }
        
}
