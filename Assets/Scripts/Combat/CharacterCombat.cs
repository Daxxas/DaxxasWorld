using System;
using System.Collections.Generic;
using Combat;
using Mirror;
using UnityEngine;


public enum CombatState
{
    Charge, Attack, ChargeBlock, Idle, BlockReady, Dead
}

public class CharacterCombat : NetworkBehaviour, IHittable
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 3f;
    
    [SerializeField] private LayerMask whatIsEnemy;

    [ShowInInspector] [SyncVar] protected CombatState combatState = CombatState.Idle;
    public CombatState CombatState => combatState;
    
    protected Weapon currentWeapon;
    
    private Health health;

    private CharacterController characterController;
    public CharacterController CharacterController => characterController;
    
    [SyncVar] protected bool attackIsCharged = false;
    public bool AttackIsCharged => attackIsCharged;
    
    private List<Collider2D> hitCache = new List<Collider2D>();

    public Action hitBlock;
    public Action<int> damagedEvent;
    public Action chargedHitWhileBlock;
    public Action onDie;
    
    public void Start()
    {
        health = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        currentWeapon = GetComponentInChildren<Weapon>();
        
        damagedEvent += damage => health.ChangeHealth(-damage);
    }

    [Server]
    public void Damage(int damage, Vector3 sourceDamage, bool isCharged = false)
    {
        if (combatState != CombatState.Dead)
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

            if (health.CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                Knockback(sourceDamage);
            }
        }
    }

    private void Die()
    {
        ChangeCombatState(CombatState.Dead);
        characterController.canMove = false;
        characterController.canControlWeapon = false;
        SetActiveWeapon(false);
        onDie?.Invoke();
    }

    [ClientRpc]
    private void SetActiveWeapon(bool active)
    {
        currentWeapon.gameObject.SetActive(active);
    }
     
    protected void EngageAttack()
    {
        
        ClearHitCache();
        FirstWeaponHit();
    }

    protected void ChangeCombatState(CombatState newState)
    {
        if (combatState != CombatState.Dead)
        {
            combatState = newState;
        }
    }
    
    [Server]
    protected void FirstWeaponHit()
    {
        Collider2D[] results = new Collider2D[10];
        Physics2D.OverlapCollider(currentWeapon.Collider, new ContactFilter2D(), results);

  
        Array.ForEach(results, result =>
        {
            if (result != null)
            {
                WeaponHit(result);
            }
        });
    }
    
    [Server]
    public void WeaponHit(Collider2D hit)
    {
        // if collider hit is in layermask whatIsEnemy
        if (whatIsEnemy == (whatIsEnemy | (1 << hit.gameObject.layer)) && combatState == CombatState.Attack)
        {
            if (!hitCache.Contains(hit))
            {
                if(hit.TryGetComponent(out IHittable hitCharacter))
                {
                    hitCharacter.Damage(damage, transform.position, attackIsCharged);
                }
            }
            
            hitCache.Add(hit);
        }
    }

    [Server]
    protected void ClearHitCache()
    {
        hitCache.Clear();
    }
    
    [Server]
    private void Knockback(Vector3 source)
    {
        characterController.AddMomentum((Vector2) (transform.position - source).normalized * knockback);
    }
}
