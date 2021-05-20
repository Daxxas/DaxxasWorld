using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCombat : CharacterCombat
{

    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private int damage = 1;
    [SerializeField] private float moveWhileAttackReductionCoef = 0.5f;
    [SerializeField] private float moveWhileBlockReductionCoef = 0.8f;
    
    
    [SerializeField] private AnimationClip normalAttackClip;
    [SerializeField] private AnimationClip chargedAttackClip;
    [SerializeField] private AnimationClip raiseShieldClip;
    
    private float attackDuration = 0;
    private float chargedAttackDuration = 0;

    public Action BlockEvent;

    private bool isCacheBlocking = false;
    

    [SyncVar] private bool isCharging = false;
    public bool IsCharging => isCharging;
    [SyncVar] private bool attackIsCharged = false;
    public bool AttackIsCharged => attackIsCharged;
    [SyncVar] private bool blockHit = false;
    public bool BlockHit => blockHit;
    
    public new void Start()
    {
        base.Start();
        
        chargedAttackDuration = chargedAttackClip.length;
        attackDuration = normalAttackClip.length;
        
        chargedHitWhileBlock += () => {CharacterController.SlowStatus.AddSlow(moveWhileBlockReductionCoef);};
    }
    
    
    [Command]
    public void StartAttackCmd()
    {
        if (isBlocking)
            return;
        
        isCharging = true;
        CharacterController.SlowStatus.AddSlow(-moveWhileAttackReductionCoef);
    }
    
    
    [Command]
    public void PerformAttackCmd()
    {
        if (isBlocking || !isCharging)
            return;
        
        isAttacking = true;
        isCharging = false;
        attackIsCharged = true;
        StartCoroutine(EndAttack(chargedAttackDuration));
    }
    

    [Command]
    public void CancelAttackCmd()
    {
        if (isBlocking || !isCharging)
            return;
        
        isAttacking = true;
        isCharging = false;
        attackIsCharged = false;
        StartCoroutine(EndAttack(attackDuration));
    }

    [Command]
    public void SetBlockCmd(bool block)
    {
        isCacheBlocking = block;
        
        if (isCharging || isAttacking || isBlocking == block)
        {
            return;
        }

        isCacheBlocking = false;
        isCharging = false;
        isAttacking = false;
        isBlocking = block;
        
        RaiseBlockEvent();
    }

    [ClientRpc]
    private void RaiseBlockEventClientRpc()
    {
        BlockEvent?.Invoke();
    }

    [Server]
    private void RaiseBlockEvent()
    {
        if (isServerOnly)
        {
            BlockEvent?.Invoke();
        }

        if (isServer)
        {
            if (isBlocking)
            {
                EngageBlock(true);
                CharacterController.SlowStatus.AddSlow(-moveWhileBlockReductionCoef);
            }
            else
            {
                EngageBlock(false);
                CharacterController.SlowStatus.AddSlow(moveWhileBlockReductionCoef);
            }
        }
        
        RaiseBlockEventClientRpc();
    }
    
    [Server]
    private void EngageBlock(bool engaging)
    {
        if (engaging)
        {
            StartCoroutine(EngageBlock());
        }
        else
        {
            StopCoroutine(EngageBlock());
            canBlock = false;
        }
        
    }
    
    [Server]
    private IEnumerator EngageBlock()
    {
        yield return new WaitForSeconds(raiseShieldClip.length);

        canBlock = true;
    }
    

    [Server]
    private IEnumerator EndAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        isAttacking = false;
        attackIsCharged = false;
        
        CharacterController.SlowStatus.AddSlow(moveWhileAttackReductionCoef);
        
        if (isCacheBlocking)
        {
            isBlocking = true;
            RaiseBlockEvent();
        }
        
    }
    

    [Server]
    public void WeaponHit(Collider2D hit)
    {
        // if collider hit is in layermask whatIsEnemy
        if (whatIsEnemy == (whatIsEnemy | (1 << hit.gameObject.layer)) && isAttacking)
        {
            if(hit.TryGetComponent(out CharacterCombat hitCharacter))
            {
                hitCharacter.Damage(damage, attackIsCharged);
            }
        }
    }
}
