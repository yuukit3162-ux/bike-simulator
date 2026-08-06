using UnityEngine;

public class CollisionPointDetector : MonoBehaviour
{
  MeshFilter meshFilter;
  Vector3[] vertices;
  float toosa=2f;
  void Start()
  {
    meshFilter=GetComponent<MeshFilter>();
    vertices = meshFilter.mesh.vertices;
  }
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contactCount > 0)
    {
      // 最初の衝突点の情報を取得
      ContactPoint contact = collision.GetContact(0);
      float impactForce = collision.relativeVelocity.magnitude;
      // 衝突位置（ワールド座標）
      Vector3 collisionPoint = contact.point;
      
      // 衝突面の法線ベクトル（当たった方向）
      Vector3 collisionNormal = contact.normal;
      Vector3 worldPosition;
      float distance
      for (int i = 0; i < vertices.Length; i++)
      {
        worldPosition = vertices[i] + transform.position;
        distance = Vector3.Distance(worldPosition, editPoint);

        if (distance < editDistance)
        {
          vertices[i] += new Vector3(0, 0.1f * (toosa - distance), 0);
        }
        
      }
      meshFilter.mesh.vertices = vertices;

      
    }
  }
