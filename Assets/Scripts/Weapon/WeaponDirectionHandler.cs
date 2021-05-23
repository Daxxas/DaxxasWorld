using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponDirectionHandler : MonoBehaviour
{
    private CharacterController characterController;
    
    private SpriteRenderer weaponChild;

    public bool isBusy = false;
    
    private bool isInitialized = false;
    
    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        weaponChild = GetComponentInChildren<SpriteRenderer>();

        isInitialized = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isInitialized && !isBusy)
        {
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
