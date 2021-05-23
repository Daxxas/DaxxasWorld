using System;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SerializeField] [SyncVar] private int maxHealth = 1;
    [SyncVar(hook = nameof(SyncCurrentHealth))] private int currentHealth;

    public int CurrentHealth => currentHealth;
    
    public Action<int, int> healthUpdate;

    private PlayerInfoUI playerInfo;
    
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    public override void OnStartClient()
    {
        SyncCurrentHealth(currentHealth, currentHealth);
        
        base.OnStartClient();
    }
    
    private void Start()
    {
        if (isLocalPlayer)
        {
            playerInfo = FindObjectOfType<PlayerInfoUI>();

            healthUpdate += playerInfo.UpdateHealthBar;
            
            healthUpdate?.Invoke(currentHealth, maxHealth);
        }
    }

    [Server]
    public void ChangeHealth(int value)
    {
        currentHealth += value;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        healthUpdate?.Invoke(currentHealth, maxHealth);
    }

    private void SyncCurrentHealth(int oldValue, int newValue)
    {
        currentHealth = newValue;

        if (!isServer)
        {
            healthUpdate?.Invoke(currentHealth, maxHealth);
        }
    }
    
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + new Vector3(0, 1, 0), $"Health : {currentHealth} / {maxHealth}", new GUIStyle(){fontSize = 30});
    }
}
