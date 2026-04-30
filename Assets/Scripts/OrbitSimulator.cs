﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OrbitSimulator : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class Planet
    {
        public string name;
        public GameObject body;

        [Header("Orbital Elements (J2000)")]
        public double a;   // semi-major axis (km)
        public double e;   // eccentricity
        public double i;   // inclination (rad)
        public double w;   // argument of periapsis (rad)
        public double W;   // longitude of ascending node (rad)
        public double Mo;  // mean anomaly at epoch (rad)

        [Header("Axial Rotation")]
        public float axialTiltDeg;
        public float rotationPeriodSeconds;

    }

    [Header("Planets")]
    public Planet[] planets;
    public Planet[] scaledPlanets;
    public GameObject planetsParent;
    public GameObject scaledPlanetsParent;
    public bool scaled;

    [Header("Planet Zoom Locations")]
    public GameObject[] planetZoomLocations;
    public GameObject[] scaledPlanetZoomLocations;
    public GameObject mainTeleporter;
    public int currOrbit;

    public GameObject[] scaledLabels;
    public GameObject[] planetLabels;

    [Header("Orbit Anchor")]
    public Transform orbitAnchor;
    public Transform orbitAnchorDefault;

    [Header("Coordinate Frames")]
    public GameObject earthFrame;
    public GameObject scaledEarthFrame;
    public GameObject sunFrame;
    public GameObject scaledSunFrame;
    public bool showEarthFrame;
    public bool showSunFrame;

    [Header("Orbit Lines")]
    public bool orbitLinesVisible = true;  
    public bool useLiveTrails = true;  
    public bool useBakedTrails = false;
    public TrailRenderer[] orbitTrails;
    public TrailRenderer[] scaledOrbitTrails;
    public GameObject bakedOrbitParent;
    public GameObject scaledBakedOrbitParent;
    public OrbitLoader[] bakedLoaders;
    public OrbitLoader[] scaledBakedLoaders;

    [Header("Simulation Start Date")]
    public int startYear = 2004;
    public int startMonth = 4;
    public int startDay = 7;

    public enum TimeSpeed
    {
        OneSecondPerSecond,
        OneMinutePerSecond,
        OneHourPerSecond,
        OneDayPerSecond,
        OneWeekPerSecond,
        OneMonthPerSecond,
        OneYearPerSecond,
        OneDecadePerSecond
    }

    [Header("Playback")]
    public bool playSimulation = true;
    public TimeSpeed timeSpeed = TimeSpeed.OneDayPerSecond;

    [Header("Unity Scaling")]
    public float distanceScale = 1f / 1e8f;

    const double mu = 1.32712440018e11; // Sun GM (km^3 / s^2)

    double simulationTimeSeconds;
    double epochJD;

    [Header("UI")]
    public TMP_Dropdown timeDropdown;
    public Toggle scaleToggle;
    public Toggle earthToggle; //coordinate frame (not sure if needed)
    public Toggle sunToggle; //coordinate frame
    public Toggle playingToggle;
    public Toggle labelToggle;
    public Toggle playingTogglePlatform;

    public Slider monthSlider;
    public Slider daySlider;
    public Slider yearSlider;
    public TMP_Text monthLabel;
    public TMP_Text dayLabel;
    public TMP_Text yearLabel;
    public TMP_Text tiltLabel;
    public Slider tiltSlider;
    public TMP_Dropdown platformTimeDropdown;
    bool isSyncingDropdowns = false;

    //private bool usePrebakedOrbits = false;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //-----------------NOTE TO FUTURE DEVS-----------------
    //SCALE FOR ORBITANCHOR OBJECT: 3.8355. This is a number I messed with until it looked right. Be awayre that changing it may change the overall look of the orbit. The orbit lines will NOT change with this, so please be cautious


    void Start()
    {
        orbitAnchor.rotation = orbitAnchorDefault.rotation;
        scaledBakedLoaders = scaledBakedOrbitParent.GetComponents<OrbitLoader>();
        bakedLoaders = bakedOrbitParent.GetComponents<OrbitLoader>();
        epochJD = JulianDate(2004, 4, 7);
        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        ResetSimulation();
        InitializeAxialTilts(planets);
        InitializeAxialTilts(scaledPlanets);

        setOrbitState(false);
        ApplyOrbitState();

        UpdateCoordinateFrames();
        toggleLabels();

        playingToggle.isOn = true;
        playingTogglePlatform.isOn = true;
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //The update function keeps the simulation running. It keeps the planets moving based on the values given by the functions you will see afterwards.
    void Update()
    {
        double deltaSimSeconds = 0.0;

        if (playSimulation)
        {
            deltaSimSeconds = Time.deltaTime * GetTimeScaleSeconds();
            simulationTimeSeconds += deltaSimSeconds;
        }

        Planet[] activePlanets = scaled ? scaledPlanets : planets;

        ApplyOrbitState();

        foreach (var planet in activePlanets)
        {
            if (planet.body == null)
                continue;

            planet.body.transform.position =
                ComputeOrbitPosition(planet, simulationTimeSeconds);

            ApplyAxialRotation(planet, deltaSimSeconds);
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //LINE RENDER STUFF - MALIA ADD HERE
    public void setOrbitState(bool on)
    {
        TrailRenderer[] orbitTrail = scaled ? scaledOrbitTrails : orbitTrails;
        foreach (TrailRenderer orbit in orbitTrail)
        {
            if (on)
            {
                orbit.enabled = true;
            }
            else
            {
                orbit.enabled = false;
                orbit.Clear();
            }
        }

        TrailRenderer[] orbitTrail2 = !scaled ? scaledOrbitTrails : orbitTrails;
        foreach (TrailRenderer orbit in orbitTrail2)
        {
            if (on)
            {
                orbit.enabled = true;
            }
            else
            {
                orbit.enabled = false;
                orbit.Clear();
            }
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //This gets the time scale in seconds to be used in the set time scale function.
    double GetTimeScaleSeconds()
    {
        const double second = 1.0;
        const double minute = 60.0;
        const double hour = 3600.0;
        const double day = 86400.0;

        switch (timeSpeed)
        {
            case TimeSpeed.OneSecondPerSecond: return second;
            case TimeSpeed.OneMinutePerSecond: return minute;
            case TimeSpeed.OneHourPerSecond: return hour;
            case TimeSpeed.OneDayPerSecond: return day;
            case TimeSpeed.OneWeekPerSecond: return 7.0 * day;
            case TimeSpeed.OneMonthPerSecond: return 30.0 * day;
            case TimeSpeed.OneYearPerSecond: return 365.25 * day;
            case TimeSpeed.OneDecadePerSecond: return 10.0 * 365.25 * day;
            default: return day;
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Initializes the axis tilts of the planets (so they can be accurate to their real-life counterparts). This function focuses on the angle of rotation
    void InitializeAxialTilts(Planet[] planetArray)
    {
        if (planetArray == null) return;

        foreach (var planet in planetArray)
        {
            if (planet.body == null) continue;

            planet.body.transform.localRotation =
                Quaternion.Euler(planet.axialTiltDeg, 0f, 0f);
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Applies the correct axial rotation to the planets (so they can be accurate to their real-life counterparts). This function focuses on the direction and speed.
    void ApplyAxialRotation(Planet planet, double deltaSimSeconds)
    {
        if (planet.rotationPeriodSeconds <= 0.0)
            return;

        double degreesPerSecond = 360.0 / planet.rotationPeriodSeconds;
        float deltaDegrees = (float)(degreesPerSecond * deltaSimSeconds);

        planet.body.transform.Rotate(
            Vector3.up,
            deltaDegrees,
            Space.Self
        );
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Calculates the orbit according to all the variables we defined above. This was taken from Dr. Wall's MatLab code.
    public Vector3 ComputeOrbitPosition(Planet p, double t)
    {
        double n = System.Math.Sqrt(mu / (p.a * p.a * p.a));
        double M = p.Mo + n * t;

        double E = SolveKepler(p.e, M);

        double theta = 2.0 * System.Math.Atan(
            System.Math.Sqrt((1 + p.e) / (1 - p.e)) *
            System.Math.Tan(E / 2.0)
        );

        double r = p.a * (1 - p.e * p.e) /
                   (1 + p.e * System.Math.Cos(theta));

        double x =
            r * (System.Math.Cos(p.W) * System.Math.Cos(p.w + theta)
            - System.Math.Sin(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

        double y =
            r * (System.Math.Sin(p.W) * System.Math.Cos(p.w + theta)
            + System.Math.Cos(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

        double z =
            r * (System.Math.Sin(p.w + theta) * System.Math.Sin(p.i));

        Vector3 localPos = new Vector3(
            (float)x * distanceScale,
            (float)z * distanceScale,
            (float)y * distanceScale
        );

        return orbitAnchor != null
            ? orbitAnchor.TransformPoint(localPos)
            : localPos;
    }

    double SolveKepler(double e, double M)
    {
        M %= 2.0 * System.Math.PI;
        if (M < 0) M += 2.0 * System.Math.PI;

        double E = M;
        for (int i = 0; i < 30; i++)
        {
            E -= (E - e * System.Math.Sin(E) - M)
               / (1 - e * System.Math.Cos(E));
        }
        return E;
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Calculates the Julian Date.
    double JulianDate(int y, int m, int d)
    {
        return 367 * y
             - (7 * (y + (m + 9) / 12)) / 4
             + (275 * m) / 9
             + d + 1721013.5;
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //This function resets the simulation; takes it back to the start time and resets the positions.
    public void ResetSimulation()
    {
        double startJD = JulianDate(startYear, startMonth, startDay);
        simulationTimeSeconds = (startJD - epochJD) * 86400.0;
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Sets whether the simulation is playing ot not.
    public void SetPlayingFromCheck(bool platform)
    {
        if (playSimulation)
        {
            playSimulation = false;
        }
        else
        {
            playSimulation = true;
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //This function allows the user to change the date, but limits input depending on month. It resets the simulation as well, so the player doesn't have to reset it manually.
    public void DateChange()
    {
        int m = (int)monthSlider.value;
        int d = (int)daySlider.value;
        int y = (int)yearSlider.value;

        if (m > 12)
        {
            monthSlider.value = 12;
            m = 12;
        }
        else if (m < 1)
        {
            monthSlider.value = 1;
            m = 1;
        }

        if (m == 2)
        {
            if (y % 4 == 0)
            {
                if (d > 29)
                {
                    daySlider.value = 29;
                    d = 29;
                }
            }
            else
            {
                if (d > 28)
                {
                    daySlider.value = 28;
                    d = 28;
                }
            }
        }
        else if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
        {
            if (d > 31)
            {
                daySlider.value = 31;
                d = 31;
            }
        }
        else
        {
            if (d > 30)
            {
                daySlider.value = 30;
                d = 30;
            }
        }

        startMonth = m;
        startDay = d;
        startYear = y;
        ResetSimulation();
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Updates the slider label for the month, day, and year.
    public void UpdateSliderLabel()
    {
        monthLabel.text = monthSlider.value.ToString();
        dayLabel.text = daySlider.value.ToString();
        yearLabel.text = yearSlider.value.ToString();
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Updates the slider label for the tilt value
    public void UpdateTiltSliderLabel()
    {
        tiltLabel.text = tiltSlider.value.ToString();
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Updates the tilt of the planets themselves based on the slider value. The tilt of the orbit lines is done in OrbitLoader.
    public void UpdateTilt()
    {
        foreach(OrbitLoader loader in scaledBakedLoaders)
        {
            loader.changeAngle(tiltSlider.value);
        }
        foreach (OrbitLoader loader in bakedLoaders)
        {
            loader.changeAngle(tiltSlider.value);
        }

        orbitAnchor.rotation = orbitAnchorDefault.rotation;
        orbitAnchor.rotation = orbitAnchor.rotation * Quaternion.Euler(0f, 0f, tiltSlider.value);
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Sets the time scale based on the dropdown, taking into account whether it is a platform toggle or not.
    public void SetTimeSpeedFromDropdown(bool platform)
    {
        if (isSyncingDropdowns) return;

        isSyncingDropdowns = true;

        if (platform)
        {
            int value = platformTimeDropdown.value;
            timeSpeed = (TimeSpeed)value;

            if (timeDropdown.value != value)
                timeDropdown.value = value;
        }
        else
        {
            int value = timeDropdown.value;
            timeSpeed = (TimeSpeed)value;

            if (platformTimeDropdown.value != value)
                platformTimeDropdown.value = value;
        }

        isSyncingDropdowns = false;

        UpdateTrailModeFromTimeSpeed();
        ApplyOrbitState();
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    // MALIA ADD HERE
    void UpdateTrailModeFromTimeSpeed()
    {
        switch (timeSpeed)
        {
            case TimeSpeed.OneDayPerSecond:
            case TimeSpeed.OneWeekPerSecond:
                useLiveTrails = false;
                useBakedTrails = true;
                break;

            default:
                useLiveTrails = false;
                useBakedTrails = true;
                break;
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //MALIA ADD HERE
    void ApplyOrbitState()
    {
        GameObject baked = bakedOrbitParent;
        GameObject scaledBaked = scaledBakedOrbitParent;

        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        if (!orbitLinesVisible)
        {
            baked.SetActive(false);
            scaledBaked.SetActive(false);
            return;
        }

        if (useBakedTrails)
        {
            baked.SetActive(!scaled);
            scaledBaked.SetActive(scaled);
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Changes whether the orbit lines are visible. Also checks if we are currently looking at the scaled vs not scaled planets.
    public void OnOrbitToggleChanged(bool value)
    {
        orbitLinesVisible = value;

        if (!orbitLinesVisible)
        {
            bakedOrbitParent.SetActive(false);
            scaledBakedOrbitParent.SetActive(false);
            return;
        }

        if (scaled)
        {
            bakedOrbitParent.SetActive(false);
            scaledBakedOrbitParent.SetActive(true);
        }
        else
        {
            bakedOrbitParent.SetActive(true);
            scaledBakedOrbitParent.SetActive(false);
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Sets whether the scaled planets or the not scaled planets are active.
    public void SetScaleFromCheck()
    {
        scaled = !scaled;

        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);
        toggleLabels();

        if (!orbitLinesVisible)
        {
            bakedOrbitParent.SetActive(false);
            scaledBakedOrbitParent.SetActive(false);
        }
        else if (scaled)
        {
            bakedOrbitParent.SetActive(false);
            scaledBakedOrbitParent.SetActive(true);
        }
        else
        {
            bakedOrbitParent.SetActive(true);
            scaledBakedOrbitParent.SetActive(false);
        }

        UpdateCoordinateFrames();
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Enables the planet labels, once again, based on whether the planets are scaled or not.
    public void toggleLabels()
    {
        if (labelToggle.isOn == true)
        {
            if (scaled)
            {
                foreach (GameObject label in scaledLabels)
                {
                    label.SetActive(true);
                }
                foreach (GameObject label in planetLabels)
                {
                    label.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject label in planetLabels)
                {
                    label.SetActive(true);
                }
                foreach (GameObject label in scaledLabels)
                {
                    label.SetActive(false);
                }
            }
        } else
        {
            foreach (GameObject platform in scaledLabels)
            {
                platform.SetActive(false);
            }
            foreach (GameObject platform in planetLabels)
            {
                platform.SetActive(false);
            }
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //MALIA ADD HERE
    private void UpdateCoordinateFrames()
    {
        // EARTH FRAME
        bool earthShouldShow = showEarthFrame;
        earthFrame.SetActive(earthShouldShow && !scaled);
        scaledEarthFrame.SetActive(earthShouldShow && scaled);

        // SUN FRAME
        bool sunShouldShow = showSunFrame;
        sunFrame.SetActive(sunShouldShow && !scaled);
        scaledSunFrame.SetActive(sunShouldShow && scaled);
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //MALIA ADD HERE
    public void OnEarthToggleChanged(bool value)
    {
        showEarthFrame = value;
        UpdateCoordinateFrames();
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //MALIA ADD HERE
    public void OnSunToggleChanged(bool value)
    {
        showSunFrame = value;
        UpdateCoordinateFrames();
    }
}