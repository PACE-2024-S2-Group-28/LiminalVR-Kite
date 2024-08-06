using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kite : MonoBehaviour
{
    [SerializeField]
    private Rope rope;
    private float lastRopeLength;

    [SerializeField]
    [Range(0f, 1f)]
    private float windToForward = .95f;

    private void Start()
    {
        lastRopeLength = rope.ropeLength;

        Vector3 dirToKite = (transform.position - rope.AnchorA.position).normalized;
        transform.position = rope.AnchorA.position + dirToKite * rope.ropeLength;
        
        //transform.up = dirToKite;
        //transform.forward = Vector3.up;
        transform.rotation = Quaternion.LookRotation(Vector3.up, dirToKite);
        transform.rotation = LookRotationWithFixedUp(transform.forward, dirToKite.normalized);
    }

    private Vector3 velocity;
    [SerializeField]
    [Range(.5f, .99f)]
    private float damping = .98f;

    [SerializeField]
    private float tugForwardStrength = 1f;

    [SerializeField]
    private float gravMult = .1f;

    private void Update()
    {
        var wind = Wind.Instance.GetTotalWind(transform.position);
        float liftPercent = Vector3.Dot(wind.normalized, transform.up);

        Vector3 windForce = transform.forward * wind.magnitude * liftPercent * windToForward;
        Vector3 windPush = wind * (1f - windToForward);

        //tug
        float tugDist = Mathf.Max(0f, lastRopeLength - rope.ropeLength);
        lastRopeLength = rope.ropeLength;
        Vector3 tugForce = transform.forward * tugDist * tugForwardStrength;

        velocity += (windForce + windPush) * Time.deltaTime;
        velocity += Physics.gravity * Time.deltaTime * gravMult;
        velocity *= Mathf.Pow(damping, Time.deltaTime);
        velocity += tugForce;

        transform.position += velocity * Time.deltaTime;

        Vector3 dirToKite = transform.position - rope.AnchorA.position;
        Vector3 dirToKiteN = dirToKite.normalized;

        //if kite is further than rope
        if (dirToKite.sqrMagnitude > rope.ropeLength*rope.ropeLength) {    
            transform.position = rope.AnchorA.position + dirToKiteN * rope.ropeLength;

            //stop velocity from building up after snap, project orthaganally
            float velocityAlongRope = Mathf.Max(0f, Vector3.Dot(dirToKiteN, velocity.normalized));
            velocityAlongRope *= velocityAlongRope;
            velocity = Vector3.ProjectOnPlane(velocity, dirToKiteN) * (1f-velocityAlongRope);
        }

        Quaternion targetRotation = LookRotationWithFixedUp(velocity.normalized, dirToKiteN);
        float maxDegrees = velocity.magnitude * 5f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegrees);
    }

    Quaternion LookRotationWithFixedUp(Vector3 forward, Vector3 fixedUp)
    {
        // Ensure the forward vector is perpendicular to the fixed up vector
        Vector3 right = Vector3.Cross(fixedUp, forward).normalized;
        Vector3 adjustedForward = Vector3.Cross(right, fixedUp).normalized;

        // Create the rotation
        return Quaternion.LookRotation(adjustedForward, fixedUp);
    }

    private void OnDrawGizmos()
    {
        Vector3 colorVec = velocity / Mathf.Max(velocity.x, velocity.y, velocity.z);
        Gizmos.color = new Color(colorVec.x, colorVec.y, colorVec.z);
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }
}
