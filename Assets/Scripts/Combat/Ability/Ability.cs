using Mirror;
using UnityEngine;

[RequireComponent(typeof(CooldownSystem))]
public abstract class Ability : NetworkBehaviour, IHasCooldown
{
    [SerializeField] private int id = 1;
    [SerializeField] private float cooldownDuration = 1f;

    public int Id => id;
    public float CooldownDuration => cooldownDuration;

    private void WarmUp()
    {
        
    }

    private void Perform()
    {

    }

    private void Interrupt()
    {

    }
}