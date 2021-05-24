using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponDirectionHandler : MonoBehaviour
{
    private CharacterController characterController;
    
    private SpriteRenderer weaponChild;

    private Animator weaponAnimator;
    
    private bool isInitialized = false;
    
    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        weaponChild = GetComponentInChildren<SpriteRenderer>();
        weaponAnimator = weaponChild.GetComponent<Animator>();
        isInitialized = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isInitialized && weaponAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            Debug.Log("Animator is in idle state");
            if (characterController.lookDirection == 0)
            {
                weaponChild.transform.localRotation = new Quaternion(0, 0, 0, 0);
            }
            else
            {            
                weaponChild.transform.localRotation = new Quaternion(180, 0, 0, 0);
            }
            
        }
    }
}
