using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponDirectionHandler : MonoBehaviour
{
    private CharacterController characterController;
    
    private SpriteRenderer weaponChild;

    private bool isInitialized = false;
    
    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        weaponChild = GetComponentInChildren<SpriteRenderer>();

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInitialized)
        {
            if (characterController.lookDirection == 0)
            {
                weaponChild.flipY = false;
            }
            else
            {            
                weaponChild.flipY = true;
            }
            
        }
    }
}
