using UnityEngine;

public class PlatformRideController : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public GameObject platform;
    private FollowPlanet followPlanet;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //This gets the FollowPlanet component
        followPlanet = GetComponent<FollowPlanet>();

        //-----------------NOTE TO FUTURE DEVS-----------------
        //Follow planet script is always updating so that the platform can always be right under it
        //However, we do not always want the player to be following the planets too, so it is set to inactive at start
        if (followPlanet != null)
            followPlanet.enabled = false;
        else
            Debug.LogError("FollowPlanet component not found on XR Base!");
    }

    public void ToggleRidePlatform()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //When the player clicks the blue button on the teleport anchor, this is set to enabled so the player starts following the planet, moving in tandem with the platform and unable to move position
        if (followPlanet == null) return;

        followPlanet.enabled = !followPlanet.enabled;
        platform.SetActive(followPlanet.enabled);

        //-----------------NOTE TO FUTURE DEVS-----------------
        // This is a debug log that can be useful in debugging, so we left it here in case it is needed
        //Debug.Log("FollowPlanet enabled: " + followPlanet.enabled);
    }

}
