using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private static Wind instance = null;
    public static Wind Instance => instance;

    [SerializeField]
    private Vector3 windDir = Vector3.right;
    public Vector3 WindDir => windDir;
    [SerializeField]
    private float windStrength = 3f;
    public float WindStrength => windStrength;
    [SerializeField]
    private float localWindStrength = 2f;

    public Vector3 GetGlobalWind => windDir.normalized * windStrength;
    public Vector3 GetTotalWind(Vector3 pos) => GetGlobalWind + GetLocalWind(pos);

    void Awake()
    {
        if(instance=null) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    static Vector2 noiseTimeCoord = Vector2.zero;

    private void Update()
    {
        noiseTimeCoord -= new Vector2(windDir.x, windDir.z) * Time.deltaTime;
    }


    public static Vector3 GetLocalWind(Vector3 pos)
    {
        Vector2 coord = new Vector2(pos.x, pos.z) + noiseTimeCoord;
        float xAmount = Mathf.PerlinNoise(coord.x, coord.y)*2f-1f;
        float zAmount = Mathf.PerlinNoise(1f + coord.x, 1f + coord.y)*2f-1f;

        return (new Vector3(xAmount, 0, zAmount) * instance.localWindStrength * .5f);
    }
}
