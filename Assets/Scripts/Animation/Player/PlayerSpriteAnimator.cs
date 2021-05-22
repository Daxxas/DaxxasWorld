﻿using System;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Pong;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerSpriteAnimator : NetworkBehaviour
{
    private PlayerController playerController;
    private PlayerCombat playerCombat;

    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private NetworkAnimator networkAnimator;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator shieldAnimator;
    [SerializeField] private NetworkAnimator shieldNetworkAnimator;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();

        if (playerCombat.isServer)
        {
            playerCombat.hitBlock += StartBlockAnimation;
            playerCombat.damagedEvent += GotHitAnimation;
        }
    }

    private void Update()
    {
        animator.SetInteger("direction", playerController.lookDirection);
        shieldAnimator.SetInteger("direction", playerController.lookDirection);
        weaponAnimator.SetInteger("direction", playerController.lookDirection);
        animator.SetBool("isWalking", playerController.IsWalking);
        animator.SetBool("isAttacking", playerCombat.CombatState == CombatState.Attack);
        animator.SetBool("isBlocking", playerCombat.CombatState == CombatState.ChargeBlock || playerCombat.CombatState == CombatState.BlockReady);
        shieldAnimator.SetBool("isBlocking", playerCombat.CombatState == CombatState.ChargeBlock || playerCombat.CombatState == CombatState.BlockReady);
        weaponAnimator.SetBool("isCharging", playerCombat.CombatState == CombatState.Charge);
        weaponAnimator.SetBool("attackIsCharged", playerCombat.AttackIsCharged);
        
        if (playerController.Rigidbody != null)
        {
            animator.SetFloat("moveSpeed", playerController.Rigidbody.velocity.magnitude);
        }
    }

    [Server]
    private void GotHitAnimation(int damage)
    {
        networkAnimator.SetTrigger(Animator.StringToHash("gotHit"));
    }
    

    [Server]
    private void StartBlockAnimation()
    {
        networkAnimator.SetTrigger("blockHit");
        shieldNetworkAnimator.SetTrigger("blockHit");
        // StartBlockAnimationClientRpc();
    }

    // [ClientRpc]
    // private void StartBlockAnimationClientRpc()
    // {
    //     animator.SetTrigger("blockHit");
    //     shieldAnimator.SetTrigger("blockHit");
    // }

}
