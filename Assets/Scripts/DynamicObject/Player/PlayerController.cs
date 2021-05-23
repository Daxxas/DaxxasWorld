using Cinemachine;
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
            FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;
            FindObjectOfType<CinemachineVirtualCamera>().LookAt = transform;
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
                Quaternion direction = Quaternion.identity;
                if (canControlWeapon)
                {
                    pointingDirection = mousePosition - (Vector2) camera.WorldToScreenPoint(transform.position);
                    float angle = Mathf.Atan2(pointingDirection.y, pointingDirection.x) * Mathf.Rad2Deg;
                    direction = Quaternion.AngleAxis(angle, Vector3.forward);
                    
                    UpdateCurrentDirection(angle);        
                    UpdateOrientation(direction);

                    InputMove(moveDirection);
                }
                
                if (momentum.magnitude > 0.2f)
                {
                    momentum = Vector2.Lerp(momentum, Vector3.zero, momentumCoef * Time.deltaTime);
                }
                else
                {
                    momentum = Vector2.zero;
                }

                if (canMove)
                {
                    Vector2 currentVelocity = Move(moveDirection) + momentum;
                    
                    Rigidbody.velocity = currentVelocity;
                }
                
                CmdUpdatePlayerInfo(lookDirection, direction, isWalking, pointingDirection);
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
    private void CmdUpdatePlayerInfo(int lookDirection, Quaternion lookOrientation, bool isWalking, Vector2 pointingDirection)
    {
        this.lookDirection = lookDirection;
        this.lookOrientation = lookOrientation;
        this.isWalking = isWalking;
        this.pointingDirection = pointingDirection;
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
