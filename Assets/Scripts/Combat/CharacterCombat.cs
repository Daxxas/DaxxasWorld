using System;
using System.Collections.Generic;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.UI;


public enum CombatState
{
    Charge, Attack, ChargeBlock, Idle, BlockReady, Dead
}

public class CharacterCombat : NetworkBehaviour, IHittable
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 3f;
    
    [SerializeField] private LayerMask whatIsEnemy;

    [SerializeField] [SyncVar] protected CombatState combatState = CombatState.Idle;
    public CombatState CombatState => combatState;
    
    protected Weapon currentWeapon;
    
    private Health health;

    private CharacterController characterController;
    public CharacterController CharacterController => characterController;
    
    [SyncVar] protected bool attackIsCharged = false;
    public bool AttackIsCharged => attackIsCharged;
    
    private List<Collider2D> hitCache = new List<Collider2D>();

    public Action onHitBlock;
    public Action<int> onDamaged;
    public Action onChargedHitWhileBlock;
    public Action onAttack;
    
    public void Start()
    {
        health = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        currentWeapon = GetComponentInChildren<Weapon>();
        
        onDamaged += damage => health.ChangeHealth(-damage);
        health.onDie += Die;

        if (isLocalPlayer)
        {
            var localPlayerUI = LocalPlayerUI.Instance;

            localPlayerUI.respawnButton.onClick.AddListener(ResurrectCmd);
        }

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
                    onChargedHitWhileBlock?.Invoke();
                    onDamaged?.Invoke(damage);
                }
                else
                {
                    onHitBlock?.Invoke();
                }
            }
            else
            {
                onDamaged?.Invoke(damage);
            }
            
            // Can die after first if, have to recheck here
            if (combatState != CombatState.Dead)
                Knockback(sourceDamage);
        }
    }

    [Server]
    private void Die()
    {
        ChangeCombatState(CombatState.Dead);
        characterController.canMove = false;
        characterController.canControlWeapon = false;
        characterController.SetMomentum(Vector2.zero);
        characterController.SlowStatus.SetSlow(1f);
        SetActiveWeapon(false);
    }

    [Command]
    private void ResurrectCmd()
    {
        health.ChangeHealth(health.MaxHealth);
        combatState = CombatState.Idle;
        characterController.canMove = true;
        characterController.canControlWeapon = true;
        characterController.SetMomentum(Vector2.zero);
        characterController.SlowStatus.SetSlow(1f);
        SetActiveWeapon(true);
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
