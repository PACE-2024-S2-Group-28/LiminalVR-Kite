using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private Transform
        anchorA,
        anchorB;

    public Transform AnchorA => anchorA;
    public Transform AnchorB => anchorB;

    [SerializeField]
    public float ropeLength = 10f;

    private Vector3 midPoint;
    private Vector3 fixedHangPoint;
    private Vector3 hangPoint;

    [SerializeField]
    private int numLinePoints = 10;
    [SerializeField]
    private LineRenderer lineR;

    // Start is called before the first frame update
    void Start()
    {
        lineR.positionCount = numLinePoints;
        
        Update();

        hangPoint = fixedHangPoint;
        springVel = Vector3.zero;
    }

    #region spring
    [Header("Spring")]
    [SerializeField]
    private float stiffness = 2f;
    [SerializeField]
    [Range(0f, 10f)]
    private float
        damping = .5f;
    [SerializeField]
    [Range(0f,1f)]
    private float gravMult = .5f;
    [SerializeField]
    [Range(0f, 6f)]
    private float windMult = 4f;

    private Vector3 springVel;

    void MoveRopeSpring(float taughtness = 0f)
    {
        Vector3 dist = fixedHangPoint - hangPoint;
        Vector3 springForce = dist * stiffness;
        Vector3 dampingForce = -damping * springVel;
        
        Vector3 gravForce = Physics.gravity * gravMult * (1f - taughtness);
        Vector3 windForce = Wind.Instance.GetTotalWind(hangPoint) * windMult;

        Vector3 totalForce = springForce + dampingForce + gravForce + windForce;

        springVel += totalForce * Time.deltaTime;
        hangPoint += springVel * Time.deltaTime;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //get hangpoint
        float dist = Vector3.Distance(anchorA.position, anchorB.position);
        midPoint = Vector3.Lerp(anchorA.position, anchorB.position, .5f);
        fixedHangPoint = midPoint + Vector3.down*(Mathf.Max(0, ropeLength-dist));

        //hangPoint = fixedHangPoint;
        MoveRopeSpring(Mathf.Min(1f, dist/ropeLength));

        EvalauteLine();
    }

    void EvalauteLine()
    {
        for(int i=0; i<lineR.positionCount; i++) {
            float t = (float)i / (lineR.positionCount - 1f);
            Vector3 point = GetBezierPoint(anchorA.position, hangPoint, anchorB.position, t);
            lineR.SetPosition(i, point);
        }
    }

    Vector3 GetBezierPoint(Vector3 A, Vector3 B, Vector3 C, float t)
    {
        Vector3 AB = Vector3.Lerp(A, B, t);
        Vector3 BC = Vector3.Lerp(B, C, t);
        return Vector3.Lerp(AB, BC, t);
    }

    #region Debug

    const float anchorSize = .25f;
    const float hangPointSize = .1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(anchorA.position, Vector3.one*anchorSize);
        Gizmos.DrawCube(anchorB.position, Vector3.one * anchorSize);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(fixedHangPoint, Vector3.one * hangPointSize);
        Gizmos.DrawLine(midPoint, fixedHangPoint);

        Gizmos.color = new Color(1f, .5f, 0f);
        Gizmos.DrawCube(hangPoint, Vector3.one*hangPointSize);
    }

    #endregion
}
