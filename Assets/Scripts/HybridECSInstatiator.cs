﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridECSInstatiator : MonoBehaviour
{
    [Header("General Settings:")]
    [SerializeField] float universeRadius;

    [Header("Sun:")]
    [SerializeField] GameObject sunPrefab;
    [SerializeField] Vector3 sunPosition;

    [Header("Moon:")]
    [SerializeField] GameObject moonPrefab;
    [SerializeField] float minMoonMovementSpeed;
    [SerializeField] float maxMoonMovementSpeed;

    [Header("Stars:")]
    [SerializeField] GameObject starPrefab;
    [SerializeField] float minStarsize;
    [SerializeField] float maxStarsize;
    [SerializeField] int starsAmount;
    [SerializeField] [Range(0, 100)] float minTwinkleFrequency;
    [SerializeField] [Range(0, 100)] float maxTwinkleFrequency;

    [Header("Orbital Elipses:")]
    [SerializeField] int elipseSegments;
    [SerializeField] float elipseWidth;
    [SerializeField] GameObject orbitalElipsePrefab;

    [Header("Planets:")]
    [SerializeField] List<Planet> planets = new List<Planet>();

    static HybridECSInstatiator instance;
    public static HybridECSInstatiator Instance { get { return instance; } }

    GameObject sun;

    void Awake()
    {
        instance = this;
        PlaceSun();
        PlaceStars();
        PlacePlanets();
    }

    #region Sun
    void PlaceSun()
    {
        sun = Instantiate(sunPrefab, sunPosition, Quaternion.identity);
        GameObject sunParent = new GameObject();

        sunParent.name = "Sun";

        sun.transform.parent = sunParent.transform;
    }
    #endregion

    #region Stars
    void PlaceStars()
    {
        GameObject starParent = new GameObject();
        starParent.name = "Stars";

        for (int i = 0; i < starsAmount; i++)
        {
            GameObject currentStar = Instantiate(starPrefab);
            currentStar.transform.parent = starParent.transform;

            currentStar.GetComponent<StarComponent>().twinkleFrequency = Random.Range(minTwinkleFrequency, maxTwinkleFrequency);

            float randomStarScale = Random.Range(minStarsize, maxStarsize);
            currentStar.transform.localScale = new Vector3(randomStarScale, randomStarScale, randomStarScale);
            currentStar.transform.position = Random.onUnitSphere * universeRadius;
            currentStar.SetActive(true);
        }
    }
    #endregion

    #region OrbitalElipses
    void DrawOrbitalElipse(LineRenderer line, OrbitalEllipse ellipse)
    {
        Vector3[] drawPoints = new Vector3[elipseSegments + 1];

        for (int i = 0; i < elipseSegments; i++)
        {
            drawPoints[i] = ellipse.Evaluate(i / (elipseSegments - 1f));
        }
        drawPoints[elipseSegments] = drawPoints[0];

        line.useWorldSpace = false;
        line.positionCount = elipseSegments + 1;
        line.startWidth = elipseWidth;
        line.SetPositions(drawPoints);
    }

    #endregion

    #region Planets
    void PlacePlanets()
    {
        GameObject planetParent = new GameObject();
        planetParent.name = "Planets";

        for (int i = 0; i < planets.Count; i++)
        {
            GameObject currentPlanet = Instantiate(planets[i].planetPrefab);
            currentPlanet.transform.parent = planetParent.transform;

            currentPlanet.GetComponent<PlanetComponent>().rotationSpeed = planets[i].rotationSpeed;
            currentPlanet.GetComponent<PlanetComponent>().orbitDuration = planets[i].orbitDuration;
            currentPlanet.GetComponent<PlanetComponent>().orbit = planets[i].orbit;

            GameObject currentElipse = Instantiate(orbitalElipsePrefab, sunPosition, Quaternion.identity);
            currentElipse.transform.parent = sun.transform;
            DrawOrbitalElipse(currentElipse.GetComponent<LineRenderer>(), planets[i].orbit);

            if(planets[i].hasMoon)
            {
                GenerateMoon(currentPlanet);
            }
        }
    }
    #endregion

    #region Moons
    void GenerateMoon(GameObject planet)
    {
        GameObject moonParent = new GameObject();
        moonParent.name = "Moons";

        GameObject currentMoon = Instantiate(moonPrefab);
        currentMoon.transform.parent = moonParent.transform;

        currentMoon.GetComponent<MoonComponent>().movementSpeed = Random.Range(minMoonMovementSpeed, maxMoonMovementSpeed);
        currentMoon.GetComponent<MoonComponent>().parentPlanet = planet;
    }
    #endregion
}

[System.Serializable]
public class OrbitalEllipse
{
    public float xExtent;
    public float yExtent;
    public float tilt;

    public Vector3 Evaluate(float _t)
    {
        Vector3 up = new Vector3(0, Mathf.Cos(tilt * Mathf.Deg2Rad), -Mathf.Sin(tilt * Mathf.Deg2Rad));

        float angle = Mathf.Deg2Rad * 360f * _t;

        float x = Mathf.Sin(angle) * xExtent;
        float y = Mathf.Cos(angle) * yExtent;

        return up * y + Vector3.right * x;
    }
}
[System.Serializable]
public class Planet
{
    public GameObject planetPrefab;
    public OrbitalEllipse orbit;

    public bool hasMoon;

    [Header("Movement Settings:")]
    public float rotationSpeed;
    public float orbitDuration;
}
