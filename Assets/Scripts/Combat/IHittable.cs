

using UnityEngine;

namespace Combat
{
    public interface IHittable
    {
        public void Damage(int damage, Vector3 sourceDamage, bool isCharged = false);
    }
}