
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    public MMProgressBar healthBar;

    public void UpdateHealthBar(int value, int maxValue)
    {
        healthBar.UpdateBar(value, 0f, (float) maxValue);
    }
}
