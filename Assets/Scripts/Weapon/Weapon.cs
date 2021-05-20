using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    private CharacterCombat characterCombat;
    private bool isActive = true;

    public Action triggerEnter2D;
    
    private void Start()
    {
        characterCombat = GetComponentInParent<CharacterCombat>();
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D other)
    {
        characterCombat.WeaponHit(other);
    }

    private void ChangeActivation()
    {
        transform.gameObject.SetActive(isActive = !isActive);
    }
}
