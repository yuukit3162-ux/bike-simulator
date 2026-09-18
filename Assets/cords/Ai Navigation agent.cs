using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiNavigationagent : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public Transform target;
    private Rigidbody Rigidbody;
    [SerializeField] private float fixedspeed = 10f;
    [SerializeField] private float rotationspeed = 1f;

    [SerializeField] private float handle = 1f;
    private float Maxspeed = 30f;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("Agent有効: " + agent.isActiveAndEnabled);
        Debug.Log("車の位置: " + transform.position);
        Debug.Log("NavMesh上？ " + agent.isOnNavMesh);//NavMeshの上かどうか
        agent.updatePosition = false;
        agent.updateRotation = false;
        Rigidbody = transform.parent.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("車がNavMesh上にいません！");
            return;
        }
        //agent.SetDestination(target.position);



        // 1. 経路データを格納するコンテナ（NavMeshPath）を作成
        NavMeshPath path = new NavMeshPath();

        // 2. 経路を計算（スタート位置、ゴール位置、通行可能なエリア、格納先）
        // 経路が見つかると true を返します
        Vector3 start = transform.position;
        Vector3 end = target.position;
        if (NavMesh.SamplePosition(start, out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas))
        {
            start = hit.position;
        }
        if (NavMesh.SamplePosition(end, out NavMeshHit hit2, float.PositiveInfinity, NavMesh.AllAreas))
        {
            end = hit2.position;
        }
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            // 3. 経路のすべての曲がり角（座標）の配列を取得
            Vector3[] corners = path.corners;
            Vector3 dddd;
            //yukiの↓
            if (corners.Length < 2)
            {
                dddd = start - transform.position;
            }
            else
            {
                dddd = corners[1] - transform.position;
            }
            dddd.y = 0;
            dddd.Normalize();
            //yukiの↑



            float lookrotation_y = Quaternion.LookRotation(dddd).eulerAngles.y;
            float look_to = Mathf.DeltaAngle(transform.eulerAngles.y, lookrotation_y);
            float dist = Vector3.Distance(transform.position, target.position);
            

            Vector3 localVel = transform.InverseTransformDirection(Rigidbody.velocity);
            //Debug.Log(distance);
            float look_to_rotation_y = Mathf.Clamp(look_to * Mathf.PI / 180f  - Rigidbody.angularVelocity.y, -handle, +handle);//

            float speed = fixedspeed * dist;
            float movefored = speed - localVel.z;//* Mathf.Clamp(dist - localVel.z, 0f, 1f)
            Vector3 moveforedV3 = transform.forward * Mathf.Min(movefored, Maxspeed);
            Rigidbody.AddForce(moveforedV3 - transform.right * localVel.x * 2, ForceMode.Acceleration);
            //↓すべるの対策
            Rigidbody.AddTorque(new Vector3(0, look_to_rotation_y * Mathf.Clamp(localVel.z / 80, 0.1f, 1f) * rotationspeed, 0), ForceMode.Acceleration);

            // コンソールに取得した座標の数を表示
            Debug.Log($"経路のポイント数: {corners.Length}");

            // シーンビューに経路を線として描画（デバッグ用）
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(start, corners[1], Color.red);
            }
        }
    }
}
