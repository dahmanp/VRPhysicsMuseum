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

    public void ToggleRidePlatform()
    {
        if (followPlanet == null) return;

        followPlanet.enabled = !followPlanet.enabled;
        platform.SetActive(followPlanet.enabled);

        Debug.Log("FollowPlanet enabled: " + followPlanet.enabled);
    }

}
