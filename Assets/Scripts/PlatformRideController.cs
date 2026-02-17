using UnityEngine;

public class PlatformRideController : MonoBehaviour
{
    public GameObject platform;

    private FollowPlanet followPlanet;

    void Start()
    {
        // Get the FollowPlanet component
        followPlanet = GetComponent<FollowPlanet>();
        if (followPlanet != null)
            followPlanet.enabled = false; // so that player doesn't auto start following
        else
            Debug.LogError("FollowPlanet component not found on XR Base!");
    }

    //Called when player clicks the button on the teleporter
    public void ToggleRidePlatform()
    {
        if (followPlanet == null) return;

        // Toggle FollowPlanet on/off
        followPlanet.enabled = !followPlanet.enabled;
        Debug.Log("FollowPlanet enabled: " + followPlanet.enabled);

        // Toggle the platform itself on/off
        if (platform.activeSelf == true)
        {
            platform.SetActive(false);
        }
        else
        {
            platform.SetActive(true);
        }
    }
}
