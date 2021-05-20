using System;
using Mirror;
using UnityEngine;


public class CharacterCombat : NetworkBehaviour
{
    private Health health;
    private CharacterController characterController;

    public CharacterController CharacterController => characterController;
    
    [SyncVar] protected bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    [SyncVar] protected bool canBlock = false;
    public bool CanBlock => canBlock;
    
    [SyncVar] protected bool isBlocking = false;
    public bool IsBlocking => isBlocking;

    public Action hitBlock;
    public Action<int> damagedEvent;
    public Action chargedHitWhileBlock;
    
    public void Start()
    {
        health = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        
        damagedEvent += damage => health.ChangeHealth(-damage);
    }

    [Server]
    public void Damage(int damage, bool isCharged = false)
    {
        if (canBlock && isCharged)
        {
            isBlocking = false;
            canBlock = false;
            chargedHitWhileBlock?.Invoke();
            damagedEvent?.Invoke(damage);
            return;
        }
        
        if (canBlock)
        {
            hitBlock?.Invoke();
        }
        else
        {
            damagedEvent?.Invoke(damage);
        }
    }
}
