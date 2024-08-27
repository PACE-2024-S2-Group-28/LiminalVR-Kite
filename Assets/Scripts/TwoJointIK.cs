using UnityEngine;
using NaughtyAttributes;

public class TwoJointIK : MonoBehaviour
{
    public Transform baseJoint;   // First joint (root)
    public Transform elbowJoint;  // Second joint (elbow)
    public Transform endEffector; // End effector (end of the second bone)
    public Transform target;      // Target position to reach

    public float upperArmLength = 1.0f; // Length of the first bone
    public float lowerArmLength = 1.0f; // Length of the second bone

    void Update()
    {
        SolveIK();
    }

    [Button]
    void SolveIK()
    {
        Vector3 baseToTarget = (target.position - baseJoint.position).normalized;
        baseToTarget = Vector3.Scale(baseToTarget, new Vector3(.25f, 1, .25f));
        baseJoint.forward = baseToTarget;

        elbowJoint.position = baseJoint.position + baseJoint.forward * upperArmLength;
        endEffector.position = elbowJoint.position;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(baseJoint.position, Vector3.one * .05f);
        Gizmos.DrawLine(baseJoint.position, elbowJoint.position);
        
        Gizmos.color = Color.white;
        Gizmos.DrawCube(elbowJoint.position, Vector3.one * .05f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(elbowJoint.position, endEffector.position);
        Gizmos.DrawCube(endEffector.position, Vector3.one * .05f);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(target.position, Vector3.one * .05f);
        Gizmos.DrawLine(elbowJoint.position, target.position);
    }
}
