using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockParticleManager : MonoBehaviour, GenericObjPoolUserI
{
    public static RockParticleManager Instance => instance;
    private static RockParticleManager instance;

    [SerializeField]
    private ParticleSystem rockPSysFab;

    public GenericObjPool ObjPool => throw new System.NotImplementedException();
    [SerializeField]
    private GenericObjPool objPool;

    private void Awake()
    {
        if(instance!=null) {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (objPool == null) objPool = GetComponent<GenericObjPool>();
    }

    public static void PlayRockParticlesAt(Vector3 worldPos)
    {
        var pSys = instance.GetRockBreakParticleFromPool();
        pSys.transform.parent.position = worldPos;
        pSys.Play();
    }

    public ParticleSystem GetRockBreakParticleFromPool()
    {
        return objPool.SupplyObject<ParticleSystem>();
    }

    public Transform GetObjectTransformFromPool()
    {
        throw new System.NotImplementedException();
    }

    public void ObjSetup(GameObject obj)
    {
        GameObject.Instantiate(rockPSysFab, obj.transform);
    }
}
