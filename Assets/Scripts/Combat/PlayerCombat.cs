using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : CharacterCombat
{

    [SerializeField] private float moveWhileAttackReductionCoef = 0.5f;
    [SerializeField] private float moveWhileBlockReductionCoef = 0.8f;

    [SerializeField] private float dashForce = 10f;
    
    [SerializeField] private AnimationClip normalAttackClip;
    [SerializeField] private AnimationClip chargedAttackClip;
    [SerializeField] private AnimationClip raiseShieldClip;
    
    private float attackDuration = 0;
    private float chargedAttackDuration = 0;

    private PlayerController playerController;

    private bool isCacheBlocking = false;

    public new void Start()
    {
        base.Start();

        playerController = GetComponent<PlayerController>();
        
        chargedAttackDuration = chargedAttackClip.length;
        attackDuration = normalAttackClip.length;
        
        chargedHitWhileBlock += () => { InterruptBlock(); };
    }
    

    [Command]
    public void StartAttackCmd()
    {
        if (combatState != CombatState.Idle)
            return;

        combatState = CombatState.Charge;
        
        CharacterController.SlowStatus.AddSlow(-moveWhileAttackReductionCoef);
    }
    
    
    [Command]
    public void PerformAttackCmd()
    {
        if (combatState != CombatState.Charge)
            return;
        
        // Attack state set in EngageAttack()
        combatState = CombatState.Attack;

        attackIsCharged = true;
        
        StartCoroutine(Attack(chargedAttackDuration));
    }
    

    [Command]
    public void CancelAttackCmd()
    {
        if (combatState != CombatState.Charge)
            return;
        
        // Attack state set in EngageAttack()
        combatState = CombatState.Attack;

        
        attackIsCharged = false;
        
        StartCoroutine(Attack(attackDuration));
    }

    [Command]
    public void BlockCmd(bool block)
    {
        isCacheBlocking = block;

        if (combatState == CombatState.Attack)
        {
            return;
        }

        if (combatState == CombatState.Charge)
        {
            CharacterController.SlowStatus.AddSlow(moveWhileAttackReductionCoef);
        }
        
        isCacheBlocking = false;
        ChangeCombatState(block ? CombatState.ChargeBlock : CombatState.Idle);
        Block();
    }

    private void InterruptBlock()
    {
        ChangeCombatState(CombatState.Idle);
        CharacterController.SlowStatus.AddSlow(moveWhileBlockReductionCoef);
        RaiseWeaponClientRpc(true);
    }

    [Server]
    private void Block()
    {
        if (isServer)
        {
            if (combatState == CombatState.ChargeBlock)
            {
                StartCoroutine(EngageBlock());
                RaiseWeaponClientRpc(false);
                CharacterController.SlowStatus.AddSlow(-moveWhileBlockReductionCoef);
            }
            else
            {
                StopCoroutine(EngageBlock());
                RaiseWeaponClientRpc(true);
                CharacterController.SlowStatus.AddSlow(moveWhileBlockReductionCoef);
            }
        }
    }

    [ClientRpc]
    private void RaiseWeaponClientRpc(bool raising)
    {
        currentWeapon.gameObject.SetActive(raising);
    }

    [Server]
    private IEnumerator EngageBlock()
    {
        yield return new WaitForSeconds(raiseShieldClip.length);
        if (combatState == CombatState.ChargeBlock)
        {
            ChangeCombatState(CombatState.BlockReady);
        }
    }
    

    [Server]
    private IEnumerator Attack(float duration)
    {
        EngageAttack();
        
        var dashMomentum = playerController.PointingDirection.normalized * dashForce;
        
        playerController.AddMomentum(dashMomentum);
        
        yield return new WaitForSeconds(duration);

        ChangeCombatState(CombatState.Idle);
        
        attackIsCharged = false;
        CharacterController.SlowStatus.AddSlow(moveWhileAttackReductionCoef);
        
        if (isCacheBlocking)
        {
            ChangeCombatState(CombatState.ChargeBlock);
            Block();
        }
        
    }
}
