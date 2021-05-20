using Mirror;
using UnityEngine;

public class PlayerController : CharacterController
{
    public Vector2 mousePosition;

    private Camera camera;
    private WeaponDirectionHandler weaponDirectionHandler;
    private PlayerCombat playerCombat;

    private bool isInitialized = false;

    [SyncVar] private Quaternion lookOrientation;
    
    private void Start()
    {
        base.Start();
        
        if (hasAuthority)
        {
            camera = Camera.main;
        }
        
        weaponDirectionHandler = GetComponentInChildren<WeaponDirectionHandler>();
        playerCombat = GetComponent<PlayerCombat>();

        isInitialized = true;
    }

    private void Update()
    {
        if (hasAuthority)
        {
            if (!isServerOnly)
            {
                var dir = mousePosition - (Vector2) camera.WorldToScreenPoint(transform.position);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                var direction = Quaternion.AngleAxis(angle, Vector3.forward);
                
                UpdateCurrentDirection(angle);        
                UpdateOrientation(direction);

                InputMove(moveDirection);
                Rigidbody.velocity = Move(moveDirection);
                
                CmdUpdatePlayerInfo(lookDirection, direction, isWalking);
            }
        }
        else
        {
            UpdateOrientation(lookOrientation);
        }
        
        if (isServer)
        {
            
            // TODO : Check if velocity is ok here from server point of view to avoid cheat
        }
        
    }
    
    [Command]
    private void CmdUpdatePlayerInfo(int lookDirection, Quaternion lookOrientation, bool isWalking)
    {
        this.lookDirection = lookDirection;
        this.lookOrientation = lookOrientation;
        this.isWalking = isWalking;
    }

    private void UpdateCurrentDirection(float angle)
    {
        if (angle > 90 || angle < -90)
        {
            lookDirection = 1;
        }
        else
        {
            lookDirection = 0;
        }
    }

    private void UpdateOrientation(Quaternion orientation)
    {
        weaponDirectionHandler.transform.rotation = orientation;
    }
    
    private void InputMove(Vector2 inputDirection)
    {
        if (inputDirection.magnitude > 0.01f && playerCombat.CombatState != CombatState.Attack)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        moveDirection = inputDirection;
    }

    private Vector2 Move(Vector2 input)
    {
        return input * MovementSpeed * SlowStatus.GetSlowAmount();
    }
}
