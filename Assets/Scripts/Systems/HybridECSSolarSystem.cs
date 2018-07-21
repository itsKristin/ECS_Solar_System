using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class HybridECSSolarSystem : ComponentSystem
{
    struct Stars
    {
        public StarComponent _starComponent;
        public MeshRenderer _meshRenderer;
    }

    struct Planets
    {
        public PlanetComponent _planetComponent;
        public Transform _transform;
    }

    protected override void OnUpdate()
    {
        foreach(var starEntity in GetEntities<Stars>())
        {
            int timeAsInt = (int)Time.time;
            if(Random.Range(1f, 100f) < starEntity._starComponent.twinkleFrequency)
            {
                starEntity._meshRenderer.enabled = (timeAsInt % 2 == 0) ? true : false;
            }
        }

     
        foreach (var planetEntity in GetEntities<Planets>())
        {
            //Planet Rotation
            planetEntity._transform.RotateAround(planetEntity._transform.position, planetEntity._transform.up, Time.deltaTime * planetEntity._planetComponent.rotationSpeed);

            //PlanetMovement
            if(planetEntity._planetComponent.orbitalElipseToFollow.GetPosition(planetEntity._planetComponent.currentPathpoint) != planetEntity._transform.position)
            {
                planetEntity._transform.position = Vector3.Lerp(planetEntity._transform.position, planetEntity._planetComponent.orbitalElipseToFollow.GetPosition(planetEntity._planetComponent.currentPathpoint), /*planetEntity._planetComponent.movementSpeed*/ Time.deltaTime);
            }
           else if (planetEntity._planetComponent.orbitalElipseToFollow.GetPosition(planetEntity._planetComponent.currentPathpoint) == planetEntity._transform.position)
            {
                if(planetEntity._planetComponent.currentPathpoint == 37)
                {
                    planetEntity._planetComponent.currentPathpoint = 0;
                    planetEntity._transform.position = Vector3.Lerp(planetEntity._transform.position, planetEntity._planetComponent.orbitalElipseToFollow.GetPosition(planetEntity._planetComponent.currentPathpoint), /*planetEntity._planetComponent.movementSpeed*/ Time.deltaTime);
                }
                else
                {
                    planetEntity._planetComponent.currentPathpoint++;
                    planetEntity._transform.position = Vector3.Lerp(planetEntity._transform.position, planetEntity._planetComponent.orbitalElipseToFollow.GetPosition(planetEntity._planetComponent.currentPathpoint), /*planetEntity._planetComponent.movementSpeed*/ Time.deltaTime);
                }
            }
        }
        
    }
    
}
