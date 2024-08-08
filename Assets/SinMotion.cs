using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMotion : MonoBehaviour
{
    [SerializeField]
    private Vector3 dir = Vector3.right;
    [SerializeField]
    private float distance = 2f;
    [SerializeField]
    private float timeScale = 1f;

    Vector3? startPos = null;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos.Value + Mathf.Sin(Time.time * timeScale * Mathf.PI) * dir.normalized * distance;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos;
        if(startPos.HasValue) {
            pos = startPos.Value;
        } else {
            pos = transform.position;
        }

        var dirN = dir.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos + dirN * distance, pos - dirN * distance);
    }
}
