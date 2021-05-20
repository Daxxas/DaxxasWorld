using System;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Pong;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerSpriteAnimator : NetworkBehaviour
{
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private Animator animator;

    private NetworkAnimator networkAnimator;
    private Animator weaponAnimator;
    [SerializeField] private Animator shieldAnimator;
    [SerializeField] private NetworkAnimator shieldNetworkAnimator;

    
    private void Start()
    {
        weaponAnimator = transform.GetChild(0).GetComponentInChildren<Animator>();

        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();

        if (playerCombat.isServer)
        {
            playerCombat.hitBlock += StartBlockAnimation;
        }
    }

    private void Update()
    {
        animator.SetInteger("direction", playerController.lookDirection);
        shieldAnimator.SetInteger("direction", playerController.lookDirection);
        animator.SetBool("isWalking", playerController.IsWalking);
        animator.SetBool("isAttacking", playerCombat.IsAttacking);
        animator.SetBool("isBlocking", playerCombat.IsBlocking);
        
        shieldAnimator.SetBool("isBlocking", playerCombat.IsBlocking);

        weaponAnimator.SetBool("isCharging", playerCombat.IsCharging);
        weaponAnimator.SetBool("attackIsCharged", playerCombat.AttackIsCharged);
        
        if (playerController.Rigidbody != null)
        {
            animator.SetFloat("moveSpeed", playerController.Rigidbody.velocity.magnitude);
        }
    }

    [Server]
    private void StartBlockAnimation()
    {
        animator.SetTrigger("blockHit");
        shieldAnimator.SetTrigger("blockHit");
        StartBlockAnimationClientRpc();
    }

    [ClientRpc]
    private void StartBlockAnimationClientRpc()
    {
        animator.SetTrigger("blockHit");
        shieldAnimator.SetTrigger("blockHit");
    }

}
