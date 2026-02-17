﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


//SCALE FOR ORBITANCHOR??? 3.8355

public class OrbitSimulator : MonoBehaviour
{
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

    [Header("Orbit Anchor")]
    public Transform orbitAnchor;

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
    //public bool orbitVisual = false;
    public TrailRenderer[] orbitTrails;
    public TrailRenderer[] scaledOrbitTrails;
    public bool coordinateFrame = false;

    [Header("Unity Scaling")]
    public float distanceScale = 1f / 1e8f;

    const double mu = 1.32712440018e11; // Sun GM (km^3 / s^2)

    double simulationTimeSeconds;
    double epochJD;

    [Header("UI")]
    public TMP_Dropdown timeDropdown;
    public Toggle scaleToggle;
    public Toggle playingToggle;
    //public Toggle zoomToggle;
    public Toggle coordinateFrameToggle;
    public Toggle orbitVisualToggle;
    public Slider monthSlider;
    public Slider daySlider;
    public Slider yearSlider;
    public TMP_Text monthLabel;
    public TMP_Text dayLabel;
    public TMP_Text yearLabel;
    public TMP_Dropdown zoomDropdown;
    public GameObject CoordinateFrameObject;

    private bool usePrebakedOrbits = false;


    // UNITY

    void Start()
    {
        epochJD = JulianDate(2004, 4, 7);
        ResetSimulation();
        InitializeAxialTilts(planets);
        InitializeAxialTilts(scaledPlanets);
        setOrbitState(false);
    }

    void setOrbitState(bool on)
    {
        TrailRenderer[] orbitTrail = scaled ? scaledOrbitTrails : orbitTrails;
        foreach(TrailRenderer orbit in orbitTrail)
        {
            //Debug.Log("test2");
            if (on)
            {
                orbit.enabled = true;
                //Debug.Log("test");
            } else
            {
                orbit.enabled = false;
                orbit.Clear();
            }
        }

        TrailRenderer[] orbitTrail2 = !scaled ? scaledOrbitTrails : orbitTrails;
        foreach (TrailRenderer orbit in orbitTrail2)
        {
            //Debug.Log("test2");
            if (on)
            {
                orbit.enabled = true;
                //Debug.Log("test");
            }
            else
            {
                orbit.enabled = false;
                orbit.Clear();
            }
        }
    }

    void Update()
    {
        double deltaSimSeconds = 0.0;

        if (playSimulation)
        {
            deltaSimSeconds = Time.deltaTime * GetTimeScaleSeconds();
            simulationTimeSeconds += deltaSimSeconds;
        }

        Planet[] activePlanets = scaled ? scaledPlanets : planets;

        scaledPlanetsParent.SetActive(scaled);
        planetsParent.SetActive(!scaled);

        // Orbit line stuff
        TrailRenderer[] trails = scaled ? scaledOrbitTrails : orbitTrails;

        if (usePrebakedOrbits)
        {
            foreach (var t in trails)
            {
                t.emitting = false; // get rid of live line
                t.enabled = true; // show pre-baked line
            }
        }
        else
        {
            // Live trail update
            foreach (var t in trails)
            {
                t.emitting = true;
                t.enabled = true;
            }
        }

        UpdateOrbitTrails();

        foreach (var planet in activePlanets)
        {
            if (planet.body == null)
                continue;

            planet.body.transform.position =
                ComputeOrbitPosition(planet, simulationTimeSeconds);

            ApplyAxialRotation(planet, deltaSimSeconds);
        }
    }

    // TIME SCALE and prebaked orbits if neccessary

    double GetTimeScaleSeconds()
    {
        const double second = 1.0;
        const double minute = 60.0;
        const double hour = 3600.0;
        const double day = 86400.0;

        // Decide if we need prebaked orbits
        switch (timeSpeed)
        {
            case TimeSpeed.OneSecondPerSecond:
            case TimeSpeed.OneMinutePerSecond:
            case TimeSpeed.OneHourPerSecond:
            case TimeSpeed.OneMonthPerSecond:
            case TimeSpeed.OneYearPerSecond:
            case TimeSpeed.OneDecadePerSecond:
                usePrebakedOrbits = true;
                break;

            case TimeSpeed.OneDayPerSecond:
            case TimeSpeed.OneWeekPerSecond:
            default:
                usePrebakedOrbits = false;
                break;
        }

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


    // AXIAL ROTATION

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

    // ORBIT CALCULATION 

    Vector3 ComputeOrbitPosition(Planet p, double t)
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

    // DATE

    double JulianDate(int y, int m, int d)
    {
        return 367 * y
             - (7 * (y + (m + 9) / 12)) / 4
             + (275 * m) / 9
             + d + 1721013.5;
    }

    // RESET

    public void ResetSimulation()
    {
        double startJD = JulianDate(startYear, startMonth, startDay);
        simulationTimeSeconds = (startJD - epochJD) * 86400.0;
        StartCoroutine(ResetLines());
    }

    //Gets rid of tangent lines after reset
    //Reset moves planets, then this clears it and turns it back on
    //SHould work with the toggle
    IEnumerator ResetLines()
    {
        // Let Update() move planets to the reset position
        yield return null;

        TrailRenderer[] trails = scaled ? scaledOrbitTrails : orbitTrails;

        foreach (var t in trails)
            t.emitting = false; //Pause

        foreach (var t in trails)
            t.Clear(); //Clear

        yield return null; //Wait

        foreach (var t in trails)
            t.emitting = true; //Resume
    }


    // UI

    public void SetScaleFromCheck()
    {
        if (scaled)
        {
            scaled = false;
            StartCoroutine(ResetLines());
        }
        else
        {
            scaled = true;
            StartCoroutine(ResetLines());

        }
    }

    public void SetPlayingFromCheck(bool platform)
    {
        if (playSimulation) {
            playSimulation = false;
            if (platform)
            {
                playingToggle.isOn = false;
            }
            //playingToggle.isOn = false;
        }
        else
        {
            playSimulation = true;
            if (platform)
            {
                playingToggle.isOn = true;
            }
            //playingToggle.isOn = true;
        }
    }

    public void DateChange()
    {
        int m = (int)monthSlider.value;// int.Parse(monthDropdown.text);
        int d = (int)daySlider.value;// int.Parse(dayDropdown.text);
        int y = (int)yearSlider.value;// int.Parse(yearDropdown.text);

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

    public void UpdateSliderLabel()
    {
        monthLabel.text = monthSlider.value.ToString();
        dayLabel.text = daySlider.value.ToString();
        yearLabel.text = yearSlider.value.ToString();
    }

    public void SetTimeSpeedFromDropdown()
    {
        int value = timeDropdown.value;
        timeSpeed = (TimeSpeed)value;
        zoomDropdown.value = value;
    }

    public void SetZoomLocationFromDropdown()
    {
        int value = zoomDropdown.value;
        SetZoomActive(value);

        //reset time speed
        timeDropdown.value = 4;
        SetTimeSpeedFromDropdown();

        //reset playback
        playingToggle.isOn = true;
        SetPlayingFromCheck(true);

        //reset scaled
        scaleToggle.isOn = false;
        scaled = false;
        //SetScaleFromCheck();

        mainTeleporter.SetActive(true);

        //teleport user to zoom location
        // TELEPORT PLAYER TO planetZoomLocation[value];
    }

    void SetZoomActive(int value)
    {
        if (scaled) {
            //Changed "planetLocation" to "platform"
            foreach (GameObject platform in scaledPlanetZoomLocations)
            {
                platform.SetActive(false);
            }
            scaledPlanetZoomLocations[value].SetActive(true);
        } else
        {  
            foreach (GameObject platform in planetZoomLocations)
            {
                platform.SetActive(false);
            }
            planetZoomLocations[value].SetActive(true);
        }
        currOrbit = value;
    }

    public void SetOrbitCircleVisualFromCheck()
    {
        /*if (orbitVisual)
        {
            orbitVisual = false;
        }
        else
        {
            orbitVisual = true;
        }*/
        setOrbitState(orbitVisualToggle.isOn);
        Debug.Log("Set Orbit Visual From Check"); //Toggle
    }

    void UpdateOrbitTrails()
    {
        TrailRenderer[] trails = scaled ? scaledOrbitTrails : orbitTrails;
        foreach (var t in trails)
        {
            t.enabled = true;         // always visible
            t.emitting = !usePrebakedOrbits; // live trails only if not prebaked
        }
    }


    public void SetCoordinateFrameVisualFromCheck()
    {
        if (coordinateFrame)
        {
            coordinateFrame = false;
            CoordinateFrameObject.SetActive(false);
        }
        else
        {
            coordinateFrame = true;
            CoordinateFrameObject.SetActive(true);
        }
    }
}