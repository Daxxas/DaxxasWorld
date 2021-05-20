using System;
using Mirror;
using UnityEngine;


public enum CombatState
{
    Charge, Attack, ChargeBlock, Idle, BlockReady
}

public class CharacterCombat : NetworkBehaviour
{
    [SerializeField] private int damage = 1;

    [SerializeField] private LayerMask whatIsEnemy;

    [ShowInInspector] [SyncVar] protected CombatState combatState = CombatState.Idle;

    public CombatState CombatState => combatState;
    
    protected Weapon currentWeapon;
    
    private Health health;
    private CharacterController characterController;

    public CharacterController CharacterController => characterController;
    
    [SyncVar] protected bool attackIsCharged = false;
    public bool AttackIsCharged => attackIsCharged;
    
    public Action hitBlock;
    public Action<int> damagedEvent;
    public Action chargedHitWhileBlock;
    
    public void Start()
    {
        health = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        currentWeapon = GetComponentInChildren<Weapon>();
        
        damagedEvent += damage => health.ChangeHealth(-damage);
    }

    [Server]
    public void Damage(int damage, bool isCharged = false)
    {
        if (combatState == CombatState.BlockReady)
        {
            if (isCharged)
            {
                chargedHitWhileBlock?.Invoke();
                damagedEvent?.Invoke(damage);
            }
            else
            {
                hitBlock?.Invoke();
            }
        }
        else
        {
            damagedEvent?.Invoke(damage);
        }
    }
    
    [Server]
    public void WeaponHit(Collider2D hit)
    {
        // if collider hit is in layermask whatIsEnemy
        if (whatIsEnemy == (whatIsEnemy | (1 << hit.gameObject.layer)) && combatState == CombatState.Attack)
        {
            if(hit.TryGetComponent(out CharacterCombat hitCharacter))
            {
                hitCharacter.Damage(damage, attackIsCharged);
            }
        }
    }
}
