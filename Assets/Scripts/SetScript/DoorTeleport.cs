using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    // Reference to the ScaleManager
    private ScaleManager scaleManager;

    private void Start()
    {
        // Find the ScaleManager in the scene
        scaleManager = Object.FindFirstObjectByType<ScaleManager>();
        if (scaleManager == null)
        {
            Debug.LogError("ScaleManager not found in the scene!");
        }
    }

    // Trigger event when the player enters the collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the door frame has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Get the current portal destination from the ScaleManager
            Transform portalDestination = scaleManager.GetPortalDestination();

            if (portalDestination != null)
            {
                // Teleport the player to the destination
                other.transform.position = portalDestination.position;
            }
            else
            {
                Debug.LogWarning("Portal destination is not set in the ScaleManager!");
            }
        }
    }
}