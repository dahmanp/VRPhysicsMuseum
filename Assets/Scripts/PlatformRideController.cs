using UnityEngine;

public class PlatformRideController : MonoBehaviour
{
    private FollowPlanet followPlanet;

    void Start()
    {
        // Get the FollowPlanet component on this object and disable it initially
        followPlanet = GetComponent<FollowPlanet>();
        if (followPlanet != null)
            followPlanet.enabled = false;
        else
            Debug.LogError("FollowPlanet component not found on XR Base!");
    }

    // This function will be called by the UI button
    public void ToggleRidePlatform()
    {
        if (followPlanet == null) return;

        // Toggle FollowPlanet on/off
        followPlanet.enabled = !followPlanet.enabled;
        Debug.Log("FollowPlanet enabled: " + followPlanet.enabled);
    }
}
