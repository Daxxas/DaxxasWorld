using System;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SerializeField] [SyncVar] private int maxHealth = 1;
    [SyncVar(hook = nameof(OnSyncCurrentHealth))] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    
    public Action<int, int> healthUpdate;
    public Action onDie;

    private LocalPlayerUI localPlayerUI;
    
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    public override void OnStartClient()
    {
        OnSyncCurrentHealth(currentHealth, currentHealth);
        
        base.OnStartClient();
    }
    
    private void Start()
    {
        if (isLocalPlayer)
        {
            localPlayerUI = LocalPlayerUI.Instance;

            healthUpdate += localPlayerUI.UpdateHealthBar;
            healthUpdate += localPlayerUI.UpdateDeathMenu;
            
            healthUpdate?.Invoke(currentHealth, maxHealth);
        }
    }

    [Server]
    public void ChangeHealth(int value)
    {
        currentHealth += value;
        if (currentHealth <= 0)
        {
            onDie?.Invoke();
            currentHealth = 0;
        }

        healthUpdate?.Invoke(currentHealth, maxHealth);
    }

    private void OnSyncCurrentHealth(int oldValue, int newValue)
    {
        currentHealth = newValue;

        if (!isServer)
        {
            healthUpdate?.Invoke(currentHealth, maxHealth);
            
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + new Vector3(0, 1, 0), $"Health : {currentHealth} / {maxHealth}", new GUIStyle(){fontSize = 30});
    }
    
    #endif
}
