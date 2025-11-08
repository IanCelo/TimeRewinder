using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRewind : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ParticleSystem.MainModule particle_system_main = _particleSystem.main;
            particle_system_main.simulationSpeed = 0f;
        }
    }
}
