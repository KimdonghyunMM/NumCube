using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _cubeExplosion;

    ParticleSystem.MainModule _cubeExMainModule;

    public static FXManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _cubeExMainModule = _cubeExplosion.main;
    }

    public void PlayCubeExplosionFX(Vector3 pos, Color color)
    {
        _cubeExMainModule.startColor = new ParticleSystem.MinMaxGradient(color);
        _cubeExplosion.transform.position = pos;
        _cubeExplosion.Play();
    }
}
