using Mirror;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    private PlayerInput playerInput;

    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private PlayerSpriteAnimator playerSpriteAnimator;

    public PlayerInput PlayerInput => playerInput;


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
        playerSpriteAnimator = GetComponent<PlayerSpriteAnimator>();

        playerInput = new PlayerInput();
    }

    void Start()
    {
        if (hasAuthority)
        {
            InitInputs();
        } 
    }
    
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Disable();
        }
    }

    private bool block = false;
    
    private void InitInputs()
    {
        PlayerInput.Gameplay.Move.performed += context => playerController.moveDirection = context.ReadValue<Vector2>();
        PlayerInput.Gameplay.Move.canceled += context => playerController.moveDirection = context.ReadValue<Vector2>();
        PlayerInput.Gameplay.Direction.performed += context => playerController.stickDirection = context.ReadValue<Vector2>();
        PlayerInput.Gameplay.Direction.canceled += context => playerController.stickDirection = context.ReadValue<Vector2>();
        
        PlayerInput.Gameplay.MousePosition.performed += context => playerController.mousePosition = context.ReadValue<Vector2>();
        PlayerInput.Gameplay.MousePosition.canceled += context => playerController.mousePosition = context.ReadValue<Vector2>();

        PlayerInput.Gameplay.Attack.started += context => playerCombat.StartAttackCmd();
        PlayerInput.Gameplay.Attack.performed += context => playerCombat.PerformAttackCmd();
        PlayerInput.Gameplay.Attack.canceled += context => playerCombat.CancelAttackCmd();

        // PlayerInput.Gameplay.Block.performed += context => playerCombat.BlockCmd(true);
        // PlayerInput.Gameplay.Block.canceled += context => playerCombat.BlockCmd(false);
        
#if UNITY_EDITOR
        PlayerInput.Gameplay.Block.performed += context => playerCombat.BlockCmd(block = !block);
#else
        PlayerInput.Gameplay.Block.performed += context => playerCombat.BlockCmd(true);
        PlayerInput.Gameplay.Block.canceled += context => playerCombat.BlockCmd(false);
#endif
        
    }
}
