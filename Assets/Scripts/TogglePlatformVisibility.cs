using UnityEngine;

public class TogglePlatformVisibility : MonoBehaviour
{
    public GameObject platform;
    public GameObject[] projectRooms;
    public bool zoomOut;

    void Start()
    {
        if (!zoomOut)
        {
            platform.SetActive(false);
        }
    }

    public void platformOn()
    {
        platform.SetActive(true);
    }

    public void platformOff()
    {
        platform.SetActive(false);
    }

    public void disableRooms()
    {
        Debug.Log("called");
        foreach(GameObject obj in projectRooms)
        {
            Debug.Log("called 2");
            obj.SetActive(false);
        }
    }

    public void enableRooms()
    {
        foreach (GameObject obj in projectRooms)
        {
            obj.SetActive(true);
        }
    }
}
