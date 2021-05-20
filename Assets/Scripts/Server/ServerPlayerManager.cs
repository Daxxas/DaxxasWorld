using System;
using UnityEngine;

public class ServerPlayerManager : MonoBehaviour
{
    #region Singleton
    public static ServerPlayerManager Singleton { get; private set; }
    
    public void SetSingleton()
    {
        Singleton = this;
    }
    
    private void OnEnable()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        SetSingleton();

        DontDestroyOnLoad(gameObject);
    }
    #endregion


}
