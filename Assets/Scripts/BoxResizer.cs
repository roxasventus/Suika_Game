using UnityEngine;

public class BoxResizer : MonoBehaviour
{
    public Transform floor;
    public Transform leftWall;
    public Transform rightWall;
    public JointLineRenderer2D leftJoint;
    public JointLineRenderer2D rightJoint;

    public float wallOffset = 0.5f; // 벽 두께 보정

    public void Resize(float floorWidth, float wallHeight)
    {
        float preHalfWidth = floor.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;
        float preHeight = leftWall.GetComponent<SpriteRenderer>().bounds.size.y;

        // 1️⃣ 바닥 크기 조절, 좌우 벽 높이 조절
        floor.localScale = new Vector3(floorWidth, 1f, 1f);
        leftWall.localScale = new Vector3(0.54f, wallHeight, 1f);
        rightWall.localScale = new Vector3(0.54f, wallHeight, 1f);

        // 2️⃣ 바닥 반폭 계산
        float halfWidth = floor.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;
        float Height = leftWall.GetComponent<SpriteRenderer>().bounds.size.y;

        // 3️⃣ 좌우 벽 위치 이동, joint anchor 이동
        float diffX = halfWidth - preHalfWidth;
        float diffY = Height - preHeight;

        float leftWallHalfWidth = leftWall.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;
        float rightWallHalfWidth = rightWall.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;

        float leftWallHalfHeight = leftWall.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
        float rightWallHalfHeight = rightWall.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;

        leftWall.position = new Vector3(-halfWidth - leftWallHalfWidth, leftWall.position.y, leftWall.position.z);
        rightWall.position = new Vector3(halfWidth + rightWallHalfWidth, rightWall.position.y, rightWall.position.z);
        leftJoint.joint.connectedAnchor = new Vector3(leftJoint.joint.connectedAnchor.x - leftWallHalfWidth - diffX, leftJoint.joint.connectedAnchor.y + leftWallHalfHeight - diffY);
        rightJoint.joint.connectedAnchor = new Vector3(rightJoint.joint.connectedAnchor.x + rightWallHalfWidth + diffX, rightJoint.joint.connectedAnchor.y + rightWallHalfHeight - diffY);

    }

}
