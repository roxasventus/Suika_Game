using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class JointLineRenderer2D : MonoBehaviour
{
    public DistanceJoint2D joint;   // 이 줄이 따라갈 조인트
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void LateUpdate() // 물리 연산 뒤에 갱신하려면 LateUpdate가 편함
    {
        if (!joint) return;

        Vector3 a = joint.transform.TransformPoint(joint.anchor);

        Vector3 b;
        if (joint.connectedBody != null)
            b = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
        else
            b = joint.connectedAnchor; // connectedBody가 없으면 월드 좌표로 취급

        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
    }
}
