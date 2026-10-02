using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiNavigationagent : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public Transform target;
    private Rigidbody targetRB;
    private Rigidbody Rigidbody;
    [SerializeField] private float fixedspeed = 10f;
    [SerializeField] private float rotationspeed = 1f;

    [SerializeField] private float handle = 1f;
    [SerializeField] private float hoseikyoudo = 1f;
    private float Maxspeed = 20f;
    public LayerMask groundlayer;
    private bool useNavMesh;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("Agent有効: " + agent.isActiveAndEnabled);
        Debug.Log("車の位置: " + transform.position);
        Debug.Log("NavMesh上？ " + agent.isOnNavMesh);//NavMeshの上かどうか
        agent.updatePosition = false;
        agent.updateRotation = false;
        Rigidbody = transform.GetComponent<Rigidbody>();
        targetRB = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 Hosei = transform.position + new Vector3(0, 0.8f, 0);
        Debug.DrawLine(Hosei, Hosei - transform.up*0.8f, Color.red);
        if (!Physics.Raycast(Hosei, -transform.up, 0.8f, groundlayer)) return;
        //if (!agent.isOnNavMesh)
        //{
        //    Debug.LogWarning("車がNavMesh上にいません！");
        //    return;
        //}
        //agent.SetDestination(target.position);



        // 1. 経路データを格納するコンテナ（NavMeshPath）を作成
        //NavMeshPath path = new NavMeshPath();

        // 2. 経路を計算（スタート位置、ゴール位置、通行可能なエリア、格納先）
        // 経路が見つかると true を返します
        
        float dist = Vector3.Distance(transform.position, target.position);
    
        Vector3 localVel = transform.InverseTransformDirection(Rigidbody.velocity);
        localVel.x = 0f;
        GetComponent<Rigidbody>().velocity = transform.TransformDirection(localVel);//ごり押しじゃぁあああああああぁあああああああ
        Vector3 start = transform.position;
        Vector3 end = target.position + targetRB.velocity*dist/Vector3.Dot(GetComponent<Rigidbody>().velocity,(target.position - transform.position).normalized);
        if (float.IsNaN(end.x) || float.IsNaN(end.y) || float.IsNaN(end.z))
        {
            end = target.position;
        }

        if (NavMesh.SamplePosition(end, out NavMeshHit hit2, float.PositiveInfinity, NavMesh.AllAreas))
        {
            end = hit2.position;
        }
        if (NavMesh.SamplePosition(start, out NavMeshHit hit, 10, NavMesh.AllAreas))
        {
            start = hit.position;
            useNavMesh=true;
        }else if (NavMesh.SamplePosition(start, out NavMeshHit hit3, float.PositiveInfinity, NavMesh.AllAreas)){
            useNavMesh=false;
            end = hit3.position;
        }
        
        if (agent.SetDestination(end))
        {
            // 3. 経路のすべての曲がり角（座標）の配列を取得
            Vector3[] corners = agent.path.corners;
            Vector3 dddd;

            float speed = fixedspeed * dist;
            float movefored = Mathf.Min( speed - localVel.z, Maxspeed);//* Mathf.Clamp(dist - localVel.z, 0f, 1f)
            //yukiの↓
            // if (corners.Length < 2)
            // {
            //    dddd = start - transform.position;
            // }
            // else
            // {
            //    dddd = corners[1] - transform.position;
            // }
            if (useNavMesh){
                agent.nextPosition = start;
                dddd=agent.desiredVelocity;
                Debug.DrawLine(start, start+dddd*10, Color.magenta);
                Vector3 startx = start + dddd.normalized * movefored + Rigidbody.velocity;
                Debug.DrawLine(startx, startx - transform.right * 80, Color.blue);
                if (NavMesh.Raycast(startx, startx - transform.right*80, out NavMeshHit hit4, NavMesh.AllAreas)){
                    float leftlong = Vector3.Distance(startx, hit4.position);
                    Debug.DrawLine(startx, startx + transform.right * 80, Color.green);
                    if (NavMesh.Raycast(startx, startx + transform.right * 80, out NavMeshHit hit5, NavMesh.AllAreas)){
                        float alllong = Vector3.Distance(hit5.position,hit4.position);
                        float leftpa_sent = leftlong/alllong;
                        float mokuhyouti = 0.3f-leftpa_sent;
                        Debug.Log("目標"+mokuhyouti);
                        //dddd = Vector3.ProjectOnPlane(dddd, transform.right);
                        dddd += transform.right * mokuhyouti * hoseikyoudo * alllong;
                        Debug.DrawLine(start, start + dddd*10, Color.cyan);
                        //if (NavMesh.FindClosestEdge(transform.position,out NavMeshHit edge,NavMesh.AllAreas))
                        //{
                        //    Vector3 toEdge = edge.position - transform.position;

                        //    // Y方向を無視
                        //    toEdge.y = 0;

                        //    float distance = toEdge.magnitude;
                        //    float targetDistance = 2.0f;
                        //    if (distance < targetDistance)
                        //    {
                        //        Vector3 awayFromEdge = -toEdge.normalized;

                        //        // 端に近いほど強くする
                        //        float strength = Mathf.Clamp(targetDistance - distance,0f,0.5f);

                        //        dddd += awayFromEdge * strength;

                        //    }
                        //}
                    }
                }
            }
            else{
                dddd=end-start;
            }
            
            dddd.y = 0;
            dddd.Normalize();
            //yukiの↑
            


            float lookrotation_y = Quaternion.LookRotation(dddd).eulerAngles.y;
            float look_to = Mathf.DeltaAngle(transform.eulerAngles.y, lookrotation_y);
            

            //Debug.Log(distance);
            float look_to_rotation_y = Mathf.Clamp(look_to*5f - (Rigidbody.angularVelocity.y * 180f / Mathf.PI ), -handle, +handle);//

            Vector3 moveforedV3 = transform.forward *movefored;
            Rigidbody.AddForce(moveforedV3, ForceMode.Acceleration);// - transform.right * localVel.x 
            //↓すべるの対策
            Rigidbody.AddTorque(new Vector3(0, look_to_rotation_y * Mathf.Clamp(localVel.z, 0.1f, 1f), 0), ForceMode.Acceleration);
            Debug.Log(agent.desiredVelocity);
            // コンソールに取得した座標の数を表示
            //Debug.Log($"経路のポイント数: {corners.Length}");

            // シーンビューに経路を線として描画（デバッグ用）
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(start, corners[1], Color.red);
            }
        }
    }
}
