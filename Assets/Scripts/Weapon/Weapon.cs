using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    private PlayerCombat playerCombat;
    private bool isActive = true;
    
    private void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();


        playerCombat.BlockEvent += ChangeActivation;
        
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D other)
    {
        playerCombat.WeaponHit(other);
    }

    private void ChangeActivation()
    {
        transform.gameObject.SetActive(isActive = !isActive);
    }
}
