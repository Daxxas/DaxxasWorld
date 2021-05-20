using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBootManager : MonoBehaviour
{
    #region Singleton
    public static ServerBootManager Singleton { get; private set; }
    
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
    [SerializeField] private Rect buttonPos;
    
    void Start()
    {
#if UNITY_SERVER
        NetworkManager.Singleton.StartServer();
#endif
    }
    


    

}
