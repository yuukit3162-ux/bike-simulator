using UnityEngine;

public class hekomaseru : MonoBehaviour
{
    MeshFilter meshFilter;
    Vector3[] vertices;
    float toosa = 2f;
    void Start()
    {
        meshFilter = transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("衝突");
        if (collision.contactCount > 0)
        {
            Debug.Log("衝突2");

            // 最初の衝突点の情報を取得
            ContactPoint contact = collision.GetContact(0);
            float impactForce = collision.relativeVelocity.magnitude;
            // 衝突位置（ワールド座標）
            Vector3 collisionPoint = contact.point;

            // 衝突面の法線ベクトル（当たった方向）
            Vector3 collisionNormal = contact.normal;
            Vector3 worldPosition;
            float distance;
            for (int i = 0; i < vertices.Length; i++)
            {
                worldPosition = vertices[i] + transform.position;
                distance = Vector3.Distance(worldPosition, collisionPoint);

                if (distance < toosa)
                {
                    vertices[i] += collisionNormal* impactForce*0.1f;
                }
                

            }
            Debug.Log(collisionNormal * impactForce);
            meshFilter.mesh.vertices = vertices;


        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("出力");
    }
}