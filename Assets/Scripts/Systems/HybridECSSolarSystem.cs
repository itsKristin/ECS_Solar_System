using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class HybridECSSolarSystem : ComponentSystem
{
    struct Stars
    {
        public StarComponent starComponent;
        public MeshRenderer renderer;
    }

    struct Planets
    {
        public PlanetComponent planetComponent;
        public Transform transform;
    }

    protected override void OnUpdate()
    {
        foreach(var starEntity in GetEntities<Stars>())
        {
            int timeAsInt = (int)Time.time;
            if(Random.Range(1f, 100f) < starEntity.starComponent.twinkleFrequency)
            {
                starEntity.renderer.enabled = timeAsInt % 2 == 0;
            }
        }


        foreach (var planetEntity in GetEntities<Planets>())
        {
            //Planet Rotation
            planetEntity.transform.Rotate(Vector3.up * Time.deltaTime * planetEntity.planetComponent.rotationSpeed, Space.Self);
            //PlanetMovement
            planetEntity.transform.position = planetEntity.planetComponent.orbit.Evaluate(Time.time / planetEntity.planetComponent.orbitDuration);
        }
        
    }
    
}
