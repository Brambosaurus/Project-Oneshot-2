using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [SerializeField] private Image[] dashIcons; // sleep hier de twee UI icons in
    [SerializeField] private PlayerController player;

    private void Update()
    {
        int charges = player.GetRollsLeft();

        for (int i = 0; i < dashIcons.Length; i++)
        {
            dashIcons[i].enabled = i < charges;
        }
    }
}
