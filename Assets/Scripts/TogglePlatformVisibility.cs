using UnityEngine;

public class TogglePlatformVisibility : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public GameObject platform;
    public GameObject[] projectRooms;
    public bool zoomOut;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        if (!zoomOut)
        {
            //-----------------NOTE TO FUTURE DEVS-----------------
            //If the platform is not a zoomOut platform, it is set to false on start.
            platform.SetActive(false);
        }
    }

    public void platformOn()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Sets the platform to active in scene
        platform.SetActive(true);
    }

    public void platformOff()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Sets the platform to inactive in scene
        platform.SetActive(false);
    }

    public void disableRooms()
    {
        foreach(GameObject obj in projectRooms)
        {
            //-----------------NOTE TO FUTURE DEVS-----------------
            //Takes each object in the projectRooms array and sets them inactive upon teleporting so that the player gets a clear view when they zoom out in the Orbit Room. When you add more rooms / exhibits to the project, be sure to add their parent object to this array so there aren't any random floating rooms! 
            obj.SetActive(false);
        }
    }

    public void enableRooms()
    {
        foreach (GameObject obj in projectRooms)
        {
            //-----------------NOTE TO FUTURE DEVS-----------------
            //The opposite of the previous function, when you come back, this will enable all the rooms in the array again. If we didn't have this, we would be falling through space. Uh-oh!
            obj.SetActive(true);
        }
    }
}
