/**
I am planning on refactoring this, but it works for now.
**/
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
    public TextMeshProUGUI[] textObjects;
    public TextMeshProUGUI headerText;
    public OrbitSimulator orbitSimulator;


    //HAS TO MATCH DROPDOWN ORDER OR ELSE IT BREAKS
    public Transform[] planets;//Array of the planet objects (0-7 unscaled, 8-x scaled)

    public void planetChanged()
    {
        for (int i = 0; i < textObjects.Length; i++)
        {
            textObjects[i].gameObject.SetActive(false);
        }

        Debug.Log("Called" + orbitSimulator.scaled);

        switch (planetDropdown.value)
            {
                case 0:
                    Debug.Log("Mercury Selected");
                    headerText.text = "The Planet " + planets[0].name;
                    textObjects[0].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[0];
                        followPlanetPlatform.planet = planets[0];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[8];
                        followPlanetPlatform.planet = planets[8];
                    }
                    break;
                case 1:
                    Debug.Log("Venus Selected");
                    headerText.text = "The Planet " + planets[1].name;
                    textObjects[1].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[1];
                        followPlanetPlatform.planet = planets[1];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[9];
                        followPlanetPlatform.planet = planets[9];
                    }
                    break;
                case 2:
                    Debug.Log("Earth Selected");
                    headerText.text = "The Planet " + planets[2].name;
                    textObjects[2].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[2];
                        followPlanetPlatform.planet = planets[2];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[10];
                        followPlanetPlatform.planet = planets[10];
                    }
                    break;
                case 3:
                    Debug.Log("Mars Selected");
                    headerText.text = "The Planet " + planets[3].name;
                    textObjects[3].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[3];
                        followPlanetPlatform.planet = planets[3];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[11];
                        followPlanetPlatform.planet = planets[11];
                    }
                    break;
                case 4:
                    Debug.Log("Jupiter Selected");
                    headerText.text = "The Planet " + planets[4].name;
                    textObjects[4].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[4];
                        followPlanetPlatform.planet = planets[4];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[12];
                        followPlanetPlatform.planet = planets[12];
                    }
                    break;
                case 5:
                    Debug.Log("Saturn Selected");
                    headerText.text = "The Planet " + planets[5].name;
                    textObjects[5].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[5];
                        followPlanetPlatform.planet = planets[5];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[13];
                        followPlanetPlatform.planet = planets[13];
                    }
                    break;
                case 6:
                    Debug.Log("Neptune Selected");
                    headerText.text = "The Planet " + planets[6].name;
                    textObjects[6].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[6];
                        followPlanetPlatform.planet = planets[6];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[14];
                        followPlanetPlatform.planet = planets[14];
                    }
                    break;
                case 7:
                    Debug.Log("Uranus Selected");
                    headerText.text = "The Planet " + planets[7].name;
                    textObjects[7].gameObject.SetActive(true);

                    if (orbitSimulator.scaled == false)
                    {
                        followPlanetPlayer.planet = planets[7];
                        followPlanetPlatform.planet = planets[7];
                    }
                    else
                    {
                        followPlanetPlayer.planet = planets[15];
                        followPlanetPlatform.planet = planets[15];
                    }
                    break;
            }
    }
}
