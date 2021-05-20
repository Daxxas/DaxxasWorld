using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : NetworkBehaviour
{
    private Rigidbody2D rigidbody;
    public Rigidbody2D Rigidbody => rigidbody;

    [SerializeField] private float movementSpeed = 1f;
    public float MovementSpeed => movementSpeed;

    [SyncVar] private SlowStatus slowStatus = new SlowStatus();
    public SlowStatus SlowStatus => slowStatus;
    
    public Vector2 moveDirection;
    public Vector2 stickDirection;

    [SyncVar] public int lookDirection = 0;
    
    [SyncVar] protected bool isWalking = false;
    public bool IsWalking => isWalking;

    [ClientRpc]
    private void UpdateSlowStatus(float newSlow)
    {
        slowStatus.SetSlow(newSlow);
    }
    
    public void Start()
    {
        if (isServer)
        {
            slowStatus.updated += newSlow => UpdateSlowStatus(newSlow);
        }
        
        rigidbody = GetComponent<Rigidbody2D>();
    }


}
