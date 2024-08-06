using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kite : MonoBehaviour
{
    [SerializeField]
    private Rope rope;

    [SerializeField]
    [Range(0f, 1f)]
    private float windToForward = .95f;

    private void Start()
    {
        Vector3 dirToKite = (transform.position - rope.AnchorA.position).normalized;
        transform.position = rope.AnchorA.position + dirToKite * rope.ropeLength;
        
        //transform.up = dirToKite;
        //transform.forward = Vector3.up;
        transform.rotation = Quaternion.LookRotation(Vector3.up, dirToKite);
    }

    private void Update()
    {
        var wind = Wind.Instance.GetTotalWind(transform.position);
        float liftPercent = Vector3.Dot(wind.normalized, transform.up);

        transform.position += transform.forward * wind.magnitude * liftPercent * windToForward * Time.deltaTime;
        transform.position += wind * (1f - windToForward) * Time.deltaTime;

        Vector3 dirToKite = transform.position - rope.AnchorA.position;

        if (Mathf.Pow(rope.ropeLength, 2f) < dirToKite.sqrMagnitude) {    
            transform.position = rope.AnchorA.position + dirToKite.normalized * rope.ropeLength;
        }

        transform.rotation = LookRotationWithFixedUp(transform.forward, dirToKite.normalized);
    }

    Quaternion LookRotationWithFixedUp(Vector3 forward, Vector3 fixedUp)
    {
        // Ensure the forward vector is perpendicular to the fixed up vector
        Vector3 right = Vector3.Cross(fixedUp, forward).normalized;
        Vector3 adjustedForward = Vector3.Cross(right, fixedUp).normalized;

        // Create the rotation
        return Quaternion.LookRotation(adjustedForward, fixedUp);
    }
}
