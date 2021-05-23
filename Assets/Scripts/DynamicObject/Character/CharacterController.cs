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
    
    protected Vector2 pointingDirection;
    public Vector2 PointingDirection => pointingDirection;

    [SyncVar] public int lookDirection = 0;
    
    [SyncVar] protected bool isWalking = false;
    public bool IsWalking => isWalking;

    [SyncVar] public bool canMove = true;
    [SyncVar] public bool canControlWeapon = true;
    
    public Vector2 momentum = Vector2.zero;
    protected float momentumCoef = 5f;

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

    [TargetRpc]
    public void AddMomentum(Vector2 momentum)
    {
        this.momentum += momentum;
    }
    
    [TargetRpc]
    public void SetMomentum(Vector2 momentum)
    {
        this.momentum = momentum;
    }
}
