using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    private KcpTransport kcp;
    private String playerName;

    public string PlayerName => playerName;
    
    [SerializeField] private GameObject connectMenu;

    public override void Start()
    {
        base.Start();

        kcp = GetComponent<KcpTransport>();

#if UNITY_SERVER
        StartServer();
#endif
        
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        connectMenu.SetActive(false);
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        connectMenu.SetActive(false);
        
        
        base.OnClientConnect(conn);
    }

    
    

    public void ChangeAddress(string newAddress)
    {
        networkAddress = newAddress;
    }
    
    public void ChangePlayerName(string newName)
    {
        playerName = newName;
    }

    public void ChangePort(string newPort)
    {
        kcp.Port = ushort.Parse(newPort);
    }
}
