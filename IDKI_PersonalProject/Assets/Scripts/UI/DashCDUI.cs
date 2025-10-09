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
        if (!player) return;
        
        var t = player.GetRemainingCooldown();
        txt.color = new Color(1, 1, 1, t > 0 ? 0.5f : 1f); // wow you can do this inside the parameters
        var arrows = Mathf.Clamp(Mathf.CeilToInt(2f - t), 0, 3);
        txt.text = new string('>', arrows);
    }
    
}