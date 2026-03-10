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

    //Check if needed
    [Header("Planet Zoom Locations")]
    public GameObject[] planetZoomLocations;
    public GameObject[] scaledPlanetZoomLocations;
    public GameObject mainTeleporter;
    public int currOrbit;

    [Header("Orbit Anchor")]
    public Transform orbitAnchor;

    [Header("Coordinate Frames")]
    public GameObject earthFrame;
    public GameObject scaledEarthFrame;
    // Sun
    public GameObject sunFrame;
    public GameObject scaledSunFrame;
    //also uses scaled bool
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
    //public Toggle orbitVisualToggle;

    public Slider monthSlider;
    public Slider daySlider;
    public Slider yearSlider;
    public TMP_Text monthLabel;
    public TMP_Text dayLabel;
    public TMP_Text yearLabel;
    public TMP_Dropdown zoomDropdown;

    //private bool usePrebakedOrbits = false;

    // UNITY

    void Start()
    {
        epochJD = JulianDate(2004, 4, 7);
        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        ResetSimulation();
        InitializeAxialTilts(planets);
        InitializeAxialTilts(scaledPlanets);

        setOrbitState(false);
        ApplyOrbitState();

        UpdateCoordinateFrames();
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

        //scaledPlanetsParent.SetActive(scaled);
        //planetsParent.SetActive(!scaled);

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

    // LINE RENDER STUFF
    public void setOrbitState(bool on)
    {
        TrailRenderer[] orbitTrail = scaled ? scaledOrbitTrails : orbitTrails;
        foreach (TrailRenderer orbit in orbitTrail)
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

    // TIME SCALE
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
        //StartCoroutine(ResetTrails());
    }

    /*IEnumerator ResetTrails()
    {
        yield return null;

        TrailRenderer[] trails = scaled ? scaledOrbitTrails : orbitTrails;

        foreach (var t in trails)
        {
            t.enabled = false;
            t.emitting = false;
            t.Clear();
        }

        yield return null;

        foreach (var t in trails)
        {
            if (orbitLinesVisible && useLiveTrails)
            {
                t.Clear();
                t.enabled = true;
                t.emitting = true;
            }
        }
    }
*/

    // UI

    /*public void SetScaleFromCheck()
    {
        scaled = !scaled;

        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        bakedOrbitParent.SetActive(!scaled);
        scaledBakedOrbitParent.SetActive(scaled);

        UpdateCoordinateFrames();
    }*/

    public void SetPlayingFromCheck(bool platform)
    {
        if (playSimulation)
        {
            playSimulation = false;
            if (platform)
            {
                playingToggle.isOn = false;
            }
        }
        else
        {
            playSimulation = true;
            if (platform)
            {
                playingToggle.isOn = true;
            }
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

        UpdateTrailModeFromTimeSpeed();
        ApplyOrbitState();

        //StartCoroutine(ResetTrails());
    }

    void UpdateTrailModeFromTimeSpeed()
    {
        switch (timeSpeed)
        {
            case TimeSpeed.OneDayPerSecond:
            case TimeSpeed.OneWeekPerSecond:
                //useLiveTrails = true;
                //useBakedTrails = false;
                useLiveTrails = false;
                useBakedTrails = true;
                break;

            default:
                useLiveTrails = false;
                useBakedTrails = true;
                break;
        }
    }

    /*void ApplyOrbitState()
    {
        TrailRenderer[] live = scaled ? scaledOrbitTrails : orbitTrails;
        TrailRenderer[] inactive = scaled ? orbitTrails : scaledOrbitTrails;

        GameObject bakedParent = scaled ? scaledBakedOrbitParent : bakedOrbitParent;

        // Planets always visible
        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        // If user turned off orbit lines entirely
        if (!orbitLinesVisible)
        {
            foreach (var t in live)
            {
                t.enabled = false;
                t.emitting = false;
            }
            bakedParent.SetActive(false);
            return;
        }

        // Live trails mode
        if (useLiveTrails)
        {/*
            // Active set
            foreach (var t in live)
            {
                t.enabled = true;
                t.emitting = true;
            }

            // Inactive set must NOT accumulate history
            foreach (var t in inactive)
            {
                t.enabled = false;
                t.emitting = false;
            }

            bakedParent.SetActive(false);
*//*
            bakedParent.SetActive(true);
            return;
        }

        // Prebaked mode
        if (useBakedTrails)
        {
            // Live trails off
            foreach (var t in live)
            {
                t.enabled = false;
                t.emitting = false;
            }

            // Inactive trails also off
            foreach (var t in inactive)
            {
                t.enabled = false;
                t.emitting = false;
            }

            bakedParent.SetActive(true);
        }
    }*/

    void ApplyOrbitState()
    {
        GameObject baked = bakedOrbitParent;
        GameObject scaledBaked = scaledBakedOrbitParent;

        // Planet sets
        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        if (!orbitLinesVisible)
        {
            baked.SetActive(false);
            scaledBaked.SetActive(false);
            return;
        }

        // Prebaked only
        if (useBakedTrails)
        {
            baked.SetActive(!scaled);
            scaledBaked.SetActive(scaled);
        }
    }


    /*public void OnOrbitToggleChanged(bool value)
    {
        orbitLinesVisible = value;

        ApplyOrbitState();

        //StartCoroutine(ResetTrails());
    }*/
// Called when the orbit lines toggle is changed
    public void OnOrbitToggleChanged(bool value)
    {
        // Update state
        orbitLinesVisible = value;

        // If toggle off, both prebaked sets off
        if (!orbitLinesVisible)
        {
            bakedOrbitParent.SetActive(false);
            scaledBakedOrbitParent.SetActive(false);
            return;
        }

        // Toggle on: show only the correct prebaked set based on scale
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

    // Called when switching between scaled and normal planets
    public void SetScaleFromCheck()
    {
        // Flip scale
        scaled = !scaled;

        // Activate correct planet parent
        planetsParent.SetActive(!scaled);
        scaledPlanetsParent.SetActive(scaled);

        // Show correct prebaked orbit if orbit lines are enabled
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

        // Update any coordinate frames if needed
        UpdateCoordinateFrames();
    }


    //chECK if needed
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

        mainTeleporter.SetActive(true);
    }

    //Check this needed
    void SetZoomActive(int value)
    {
        if (scaled)
        {
            foreach (GameObject platform in scaledPlanetZoomLocations)
            {
                platform.SetActive(false);
            }
            scaledPlanetZoomLocations[value].SetActive(true);
        }
        else
        {
            foreach (GameObject platform in planetZoomLocations)
            {
                platform.SetActive(false);
            }
            planetZoomLocations[value].SetActive(true);
        }
        currOrbit = value;
    }

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

    public void OnEarthToggleChanged(bool value)
    {
        showEarthFrame = value;
        UpdateCoordinateFrames();
    }

    public void OnSunToggleChanged(bool value)
    {
        showSunFrame = value;
        UpdateCoordinateFrames();
    }
}