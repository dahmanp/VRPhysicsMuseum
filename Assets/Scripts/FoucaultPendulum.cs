using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FoucaultPendulum : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Anchor")]
    public Transform anchor;

    [Header("Pendulum Objects")]
    public Transform bob;
    public Transform rod;
    float rodThickness = 0.05f;

    [Range(0f, 45f)]
    float initialAngleDegrees = 10f;

    float length = 10f;
    float gravity = 9.81f;

    [Header("Earth Variables")]
    [Range(-90f, 90f)]
    public float latitude = 90f;

    public float precessionScale = 100f;

    [Header("UI")]
    public TMP_Text latitudeLabel;
    public Slider latitudeSlider;
    public TMP_Dropdown timeScaleDropdown;
    float timeMultiplier = 1f;

    float theta0;
    float wEarth;
    float w;
    float time;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        if (anchor == null)
            anchor = transform;

        //-----------------NOTE TO FUTURE DEVS-----------------
        //Calculates the theta and wEarth values, then calls the RecalculateAngularVelocity fct.
        theta0 = initialAngleDegrees * Mathf.Deg2Rad;
        wEarth = 2f * Mathf.PI / (23f * 3600f + 56f * 60f + 4.0905f);
        RecalculateAngularVelocity();

        //-----------------NOTE TO FUTURE DEVS-----------------
        //Set the minimum latitude to -90, and the max to 90. This makes sure you can't go over or under.
        //Also, the bit at the end is called to make sure the label on the UI is accurate. Sometimes with testing the value changes so its important to just set it here just in case.
        if (latitudeSlider != null)
        {
            latitudeSlider.minValue = -90f;
            latitudeSlider.maxValue = 90f;
            latitudeSlider.value = latitude;
        }
        UpdateLatitudeLabel();

        //-----------------NOTE TO FUTURE DEVS-----------------
        //Sets the dropdown and timescale value to 0 to start. Same reasoning as the slider above.
        if (timeScaleDropdown != null)
        {
            timeScaleDropdown.value = 0;
            timeScaleDropdown.onValueChanged.AddListener(UpdateTimeScale);
        }
    }

    void Update()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //This chunk of code is the meat of the pendulum simulation and visualization. Take caution in editing this, as it may break the simulation.
        //The "* timeMultiplier" was added so that we could change the time scale of the pendulum's movement.
        time += Time.deltaTime * timeMultiplier;

        //-----------------NOTE TO FUTURE DEVS-----------------
        //Precession angle and swing angle. Unfortunately, programming doesn't have mathfont or equation formats so it looks a little...ugly.
        float phi = w * time;
        float theta = theta0 * Mathf.Cos(Mathf.Sqrt(gravity / length) * time);

        //-----------------NOTE TO FUTURE DEVS-----------------
        //This will give us our x,y,z values for the simulation. We take the angles and convert them into positions in 3D space.
        float x = length * Mathf.Cos(phi) * Mathf.Sin(theta);
        float y = length * Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = -length * Mathf.Cos(theta);

        //-----------------NOTE TO FUTURE DEVS-----------------
        //This will place the bob in the right position so that it looks like it is attached to the rod.
        Vector3 anchorPos = anchor.position;
        Vector3 bobPos = anchorPos + new Vector3(x, z, y);
        bob.position = bobPos;

        //-----------------NOTE TO FUTURE DEVS-----------------
        //This chunk positions and scales the rod properly, also rotating it to look at the bob. This is primarily a visual part of the code.
        Vector3 direction = bobPos - anchorPos;
        float rodLength = direction.magnitude;
        rod.position = anchorPos + direction * 0.5f;
        rod.rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);
        rod.localScale = new Vector3(rodThickness, rodThickness, rodLength);

        //-----------------NOTE TO FUTURE DEVS-----------------
        //This section makes sure the bob actually rotates with the string, looking like an actual pendulum. If we didn't have this, the bob would not look as realistic.
        //1st: upDirection is the vector pointint from the bob to the anchor.
        //2nd: We get the tan values for the z and y direction and x and y direction to see how much the bob should tilt in either direction(since z and x are what would be tilting) - trigonometry, yay!
        //     This gives us the rotation for our tilt.
        //3rd: Since we don't want it to snap to the rotation (choppy), we use Slerp. This will take our current rotation and gradually move it to the target rotation. You can change the 5f if desired,
        //     that value just looked good.
        Vector3 upDirection = (anchorPos - bobPos).normalized;
        Quaternion targetRotation = Quaternion.Euler((Mathf.Atan2(upDirection.z, upDirection.y) * Mathf.Rad2Deg), 0f, (-Mathf.Atan2(upDirection.x, upDirection.y) * Mathf.Rad2Deg));
        bob.rotation = Quaternion.Slerp(bob.rotation, targetRotation, Time.deltaTime * 5f);
    }

    void RecalculateAngularVelocity()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Will recalculate the w value so that it reflects a changed latitude value.
        w = wEarth * Mathf.Sin(latitude * Mathf.Deg2Rad);
    }

    public void ResetSimulation()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Resets the sim to 0: sets the pendulum back to its original state at the start of the sim so its ready to start again.
        time = 0f;
        float x = length * Mathf.Sin(theta0);
        float y = 0f;
        float z = -length * Mathf.Cos(theta0);
        Vector3 anchorPos = anchor.position;
        Vector3 bobPos = anchorPos + new Vector3(x, z, y);
        bob.position = bobPos;
    }

    // UI----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Brief note from Paige: it is best practice to move this section to a dedicated UI script, but for now this was the quickest and easiest way to prototype this project.

    public void UpdateLatitudeUI()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //When the user updates the latitude UI, they must also change the actual latitude in the simulation (hence calling RecalculateAngularVelocity). Additionally, the label should be changed,
        //so we call that here too. Finally, I wanted the simulation to reset right when you change the latitude so the player wouldn't have to press another button. UX!
        latitude = latitudeSlider.value;
        UpdateLatitudeLabel();
        RecalculateAngularVelocity();
        ResetSimulation();
    }

    void UpdateLatitudeLabel()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Formats the latitude label properly such that it reads "Latitude: [value]°". It looks nicer for the viewer and reinforces the value type.
        latitudeLabel.text = $"{latitude:F1}°";
    }

    public void UpdateTimeScale(int index)
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Changes the time scale based on the selection. Multiplier is in seconds, so you must multiply each by the desired second amount. 60f=1min, 3600f-1hr, etc.
        switch (index)
        {
            case 0:
                timeMultiplier = 1f;
                break;

            case 1:
                timeMultiplier = 60f;
                break;

            case 2:
                timeMultiplier = 3600f;
                break;
        }
    }
}
