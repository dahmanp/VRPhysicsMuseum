﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Unity Scaling")]
    public float distanceScale = 1f / 1e8f;

    const double mu = 1.32712440018e11; // Sun GM (km^3 / s^2)

    double simulationTimeSeconds;
    double epochJD;

    [Header("UI")]
    public TMP_Dropdown timeDropdown;
    public Toggle scaleToggle;
    public Toggle playingToggle;
    public Toggle zoomToggle;
    public TMP_InputField monthDropdown;
    public TMP_InputField dayDropdown;
    public TMP_InputField yearDropdown;

    // UNITY

    void Start()
    {
        epochJD = JulianDate(2004, 4, 7);
        ResetSimulation();
        InitializeAxialTilts(planets);
        InitializeAxialTilts(scaledPlanets);
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

        foreach (var planet in activePlanets)
        {
            if (planet.body == null)
                continue;

            planet.body.transform.position =
                ComputeOrbitPosition(planet, simulationTimeSeconds);

            ApplyAxialRotation(planet, deltaSimSeconds);
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
    }

    // UI

    public void SetScaleFromCheck()
    {
        if (scaled) {
            scaled = false;
        } else
        {
            scaled = true;
        }
    }

    public void SetPlayingFromCheck()
    {
        if (playSimulation) {
            playSimulation = false;
        }
        else
        {
            playSimulation = true;
        }
    }

    public void SetZoomFromCheck()
    {
        // ADD ZOOM FNCT CALL
        // tp player to the earth model
    }

    public void DateChange()
    {
        int m = int.Parse(monthDropdown.text);
        int d = int.Parse(dayDropdown.text);
        int y = int.Parse(yearDropdown.text);

        if (m > 12)
        {
            monthDropdown.text = 12.ToString();
            m = 12;
        }
        else if (m < 1)
        {
            monthDropdown.text = 1.ToString();
            m = 1;
        }

        if (m == 2)
        {
            if (y % 4 == 0)
            {
                if (d > 29)
                {
                    dayDropdown.text = 29.ToString();
                    d = 29;
                }
            }
            else
            {
                if (d > 28)
                {
                    dayDropdown.text = 28.ToString();
                    d = 28;
                }
            }
        }
        else if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
        {
            if (d > 31)
            {
                dayDropdown.text = 31.ToString();
                d = 31;
            }
        }
        else
        {
            if (d > 30)
            {
                dayDropdown.text = 30.ToString();
                d = 30;
            }
        }

        startMonth = m;
        startDay = d;
        startYear = y;
        ResetSimulation();
    }

    public void SetTimeSpeedFromDropdown()
    {
        int value = timeDropdown.value;
        timeSpeed = (TimeSpeed)value;
    }
}