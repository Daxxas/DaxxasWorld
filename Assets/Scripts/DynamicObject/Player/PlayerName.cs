using System;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TextMeshPro nameText;

    [SyncVar(hook = nameof(OnChangeDisplayName))] public string displayName; 
    
    private void Start()
    {
        var networkManager = (CustomNetworkManager) NetworkManager.singleton;
        UpdateDisplayName(networkManager.PlayerName);
    }

    [Command]
    private void UpdateDisplayName(string newValue)
    {
        Debug.Log("new name for " + gameObject.name + " is " + newValue);
        
        displayName = newValue;
    }

    private void OnChangeDisplayName(string oldValue, string newValue)
    {
        nameText.text = displayName;
    }
}
