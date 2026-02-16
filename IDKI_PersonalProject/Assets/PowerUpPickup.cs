using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Player.Movement;

[RequireComponent(typeof(XRGrabInteractable))]
public class PowerUpPickup : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private float healFraction = 0.5f; // 50% of max

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        var player = FindAnyObjectByType<PlayerController>();
        if (player != null)
            player.Heal(player.MaxHealth * healFraction);

        Destroy(gameObject);
    }
}
