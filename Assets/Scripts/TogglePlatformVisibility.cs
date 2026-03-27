using UnityEngine;

public class TogglePlatformVisibility : MonoBehaviour
{
    public GameObject platform;

    void Start()
    {
        platform.SetActive(false);
    }

    public void platformOn()
    {
        platform.SetActive(true);
    }

    public void platformOff()
    {
        platform.SetActive(false);
    }

}
