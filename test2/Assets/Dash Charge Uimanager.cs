using UnityEngine;
using UnityEngine.UI;

public class DashChargeUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image[] chargeIcons; // Sleep hier je cirkels naartoe in volgorde
    [SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.2f);

    private void Update()
    {
        if (playerController == null) return;

        int charges = playerController.CurrentDashCharges;

        for (int i = 0; i < chargeIcons.Length; i++)
        {
            chargeIcons[i].color = (i < charges) ? fullColor : emptyColor;
        }
    }
}
