using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropDown : MonoBehaviour
{
    public TMP_Dropdown planetDropdown;

    public FollowPlanet followPlanetPlayer;
    public FollowPlanet followPlanetPlatform;

    //HAS TO MATCH DROPDOWN ORDER OR ELSE IT BREAKS
    public Transform[] planets;//Array of the planet objects (not scaled only for now)

    public void planetChanged()
    {
        switch (planetDropdown.value)
        {
            case 0:
                Debug.Log("Mercury Selected");
                followPlanetPlayer.planet = planets[0];
                followPlanetPlatform.planet = planets[0];
                break;
            case 1:
                Debug.Log("Venus Selected");
                followPlanetPlayer.planet = planets[1];
                followPlanetPlatform.planet = planets[1];
                break;
            case 2:
                Debug.Log("Earth Selected");
                followPlanetPlayer.planet = planets[2];
                followPlanetPlatform.planet = planets[2];
                break;
            case 3:
                Debug.Log("Mars Selected");
                followPlanetPlayer.planet = planets[3];
                followPlanetPlatform.planet = planets[3];
                break;
            case 4:
                Debug.Log("Jupiter Selected");
                followPlanetPlayer.planet = planets[4];
                followPlanetPlatform.planet = planets[4];                  
                break;
            case 5:
                Debug.Log("Saturn Selected");
                followPlanetPlayer.planet = planets[5];
                followPlanetPlatform.planet = planets[5];
                break;
            case 6:
                Debug.Log("Neptune Selected");
                followPlanetPlayer.planet = planets[6];
                followPlanetPlatform.planet = planets[6];
                break;
            case 7:
                Debug.Log("Uranus Selected");
                followPlanetPlayer.planet = planets[7];
                followPlanetPlatform.planet = planets[7];
                break;
        }
    }
}
