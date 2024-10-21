using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

public class SpaceshipNoiseMovement : MonoBehaviour
{
    [SerializeField]
    private Transform
        rotParent,
        translateParent;

    [SerializeField]
    private float
        rotStrength,
        moveStrength;

    [SerializeField]
    private float perlinFrequency;

    [SerializeField]
    private float
        shakeStrength,
        shakeFreq = 3f,
        traumaDecayMult = 1f;

    [ShowNonSerializedField]
    private float trauma = 0f;

    private Vector3 startPos = Vector3.zero;

    //public UnityEvent OnShipHitByAsteroid;
    private void Start()
    {
        startPos = translateParent.position;
    }

    private void Update()
    {
        Vector3 translateNoise = Generate01Vec3Noise(0) * 2f - Vector3.one;
        translateParent.position = startPos + translateNoise * moveStrength;

        trauma -= Time.deltaTime * traumaDecayMult;
        trauma = Mathf.Clamp(trauma, 0f, 1f);
        Vector3 shakeNoise = (Generate01Vec3Noise(3, shakeFreq) * 2f - Vector3.one) * (trauma*trauma);

        Vector3 rotNoise = Generate01Vec3Noise(6) * 2f - Vector3.one;
        rotParent.localRotation = Quaternion.Euler(rotNoise * rotStrength + shakeNoise * shakeStrength);
    }

    [Button]
    private void SmallTrauma()
    {
        AddTrauma(.2f);
    }

    [Button]
    private void BigTrauma()
    {
        AddTrauma(.6f);
    }

    public void AddTrauma(float amount)
    {
        trauma += amount;
    }

    Vector3 Generate01Vec3Noise(int startIdx, float? freq = null)
    {
        if (freq == null) freq = perlinFrequency; 

        float x = Mathf.PerlinNoise(startIdx++, Time.time * freq.Value);
        float y = Mathf.PerlinNoise(startIdx++, Time.time * freq.Value);
        float z = Mathf.PerlinNoise(startIdx++, Time.time * freq.Value);

        return new Vector3(x, y, z);
    }
}
