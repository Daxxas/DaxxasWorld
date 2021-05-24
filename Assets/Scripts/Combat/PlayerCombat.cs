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
    
    [SerializeField] private float attackRecoverTime = 0.5f;
    [SerializeField] private float chargedAttackRecoverTime = 0.5f;
    [SerializeField] private float raiseShieldTime = 0.5f;

    private PlayerController playerController;

    private bool isCacheBlocking = false;

    public new void Start()
    {
        base.Start();

        playerController = GetComponent<PlayerController>();

        onChargedHitWhileBlock += () => { InterruptBlock(); };
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
        onAttack?.Invoke();

        attackIsCharged = true;
        
        StartCoroutine(Attack(chargedAttackRecoverTime));
    }
    

    [Command]
    public void CancelAttackCmd()
    {
        if (combatState != CombatState.Charge)
            return;
        
        // Attack state set in EngageAttack()
        combatState = CombatState.Attack;
        onAttack?.Invoke();

        
        attackIsCharged = false;
        
        StartCoroutine(Attack(attackRecoverTime));
    }

    [Command]
    public void BlockCmd(bool block)
    {
        Debug.Log("BlockCmd Called with " + block);
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
        Debug.Log("Calling Block()");
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
        yield return new WaitForSeconds(raiseShieldTime);
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
