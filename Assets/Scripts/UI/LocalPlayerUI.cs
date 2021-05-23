
using System;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerUI : MonoBehaviour
{
    private static LocalPlayerUI _instance;

    
    public static LocalPlayerUI Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("No LocalPlayerUI existing !");
                GameObject container = new GameObject("LevelManager");
                _instance = container.AddComponent<LocalPlayerUI>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;

    }


    public MMProgressBar healthBar;

    public GameObject deathMenu;
    public Button respawnButton;
    
    public void UpdateHealthBar(int value, int maxValue)
    {
        healthBar.UpdateBar(value, 0f, (float) maxValue);
    }

    public void UpdateDeathMenu(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            deathMenu.SetActive(true);
        }
    }
}
