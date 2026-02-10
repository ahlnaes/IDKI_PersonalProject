using Player.Movement;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DashCDUI : MonoBehaviour
{
    public PlayerController player;
    private TextMeshProUGUI txt;

    private void Awake() => txt = GetComponent<TextMeshProUGUI>();

    private void Update()
    {
        // Dash removed for VR — nothing to display
    }
}
