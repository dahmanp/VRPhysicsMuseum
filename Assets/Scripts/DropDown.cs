using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//-----------------NOTE TO FUTURE DEVS-----------------
//Note from Malia: I am sorry in advance! I am aware that this needs to be refactored, but it works and I don't want to change it before any important deadlines, so I left it as a fun little surprise for future devs!
//This script sets up "teleporting" to the planets, it changes dynamically based on which planet is selected from a dropdown, and if the planet is scaled or not as those are two different game objects.

public class DropDown : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public TMP_Dropdown planetDropdown;
    public FollowPlanet followPlanetPlayer;
    public FollowPlanet followPlanetPlatform;
    public TextMeshProUGUI[] textObjects;
    public TextMeshProUGUI headerText;
    public OrbitSimulator orbitSimulator;
    public GameObject teleporter;

    //-----------------NOTE TO FUTURE DEVS-----------------
    //HAS TO MATCH DROPDOWN ORDER OR ELSE IT BREAKS
    //Array of the planet objects (0-7 unscaled, 8-x scaled)
    public Transform[] planets;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void planetChanged()
    {
        teleporter.SetActive(true);

        //-----------------NOTE TO FUTURE DEVS-----------------
        //This is an array of game objects that holds the informational text displayed on the platform and cycles through it
        for (int i = 0; i < textObjects.Length; i++)
        {
            textObjects[i].gameObject.SetActive(false);
        }

        //-----------------NOTE TO FUTURE DEVS-----------------
        //Another debug log that may be useful in debugging in the future:
        //Debug.Log("Called" + orbitSimulator.scaled);

        switch (planetDropdown.value)
            {
            //-----------------NOTE TO FUTURE DEVS-----------------
            // Each case goes to a planet, and changes the header, cycles to the corresponding informational text, and sets follow planet to the correct game object depending on if scaled is enabled. Since some planets are significantly larger than others when scaled, we have positions hardcoded in to be a certain distance away so that the player is a good distance from the planet.
            case 0:
                    //Mercury Selected
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
                        followPlanetPlatform.offset = new Vector3(-1, -1, 0);
                    }
                    break;
                case 1:
                    //Venus Selected
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
                        followPlanetPlatform.offset = new Vector3(-1, -1, 0);
                    }
                    break;
                case 2:
                    //Earth Selected
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
                        followPlanetPlatform.offset = new Vector3(-1, -1, 0);
                    }
                    break;
                case 3:
                    //Mars Selected
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
                        followPlanetPlatform.offset = new Vector3(-1, -1, 0);
                    }
                    break;
                case 4:
                    //Jupiter Selected
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
                        followPlanetPlatform.offset = new Vector3(-3, -3, 0);
                    }
                    break;
                case 5:
                    //Saturn Selected
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
                        followPlanetPlatform.offset = new Vector3(-3, -3, 0);
                    }
                    break;
                case 6:
                    //Neptune Selected
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
                        followPlanetPlatform.offset = new Vector3(-2, -2, 0);
                    }
                    break;
                case 7:
                    //Uranus Selected
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
                        followPlanetPlatform.offset = new Vector3(-2, -2, 0);
                    }
                    break;
            }
    }
}
