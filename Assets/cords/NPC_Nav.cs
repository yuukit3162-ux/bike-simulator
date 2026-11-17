using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Nav : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public Vector3 target;

    [SerializeField] private bool traficbool;
    private Rigidbody Rigidbody;
    [SerializeField] private float fixedspeed = 10f;
    [SerializeField] private float rotationspeed = 1f;
    private bool isTouching = true;
    private bool isTouching2 = true;
    [SerializeField] private float handle = 1f;
    [SerializeField] private float hoseikyoudo = 1f;
    [SerializeField]  private float Maxspeed = 100f;
    public LayerMask groundlayer;
    private bool useNavMesh;
    public carMnager mnager;
    public bool carway;
    public int CarCondition = 1;
    public traficlightMnager traficlightMnager;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("Agent有効: " + agent.isActiveAndEnabled);
        Debug.Log("車の位置: " + transform.position);
        Debug.Log("NavMesh上？ " + agent.isOnNavMesh);//NavMeshの上かどうか
        agent.updatePosition = false;
        agent.updateRotation = false;
        Rigidbody = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 Hosei = transform.position + new Vector3(0, 0, 0);
        Debug.DrawLine(Hosei, Hosei - transform.up*1.8f, Color.red);
        if (!Physics.Raycast(Hosei, -transform.up, 1.8f, groundlayer)) return;
        if (traficbool)
        {
            if (isTouching)
            {
                if (traficlightMnager.traficColor == 1)//赤
                {
                    CarCondition = 0;
                    //Debug.Log("red");
                }
                if (traficlightMnager.traficColor == 2)//黄色
                {
                    CarCondition = 0;
                    //Debug.Log("yellow");
                }
                if (traficlightMnager.traficColor == 3)//青色
                {
                    CarCondition = 1;
                    //Debug.Log("blue");
                }

            }
            if (isTouching2)
            {
                if (traficlightMnager.traficColor2 == 4)//赤
                {
                    CarCondition = 0;
                    //Debug.Log("red");
                }
                if (traficlightMnager.traficColor2 == 5)//黄色
                {
                    CarCondition = 0;
                    //Debug.Log("yellow");
                }
                if (traficlightMnager.traficColor2 == 6)//青色
                {
                    CarCondition = 1;
                    //Debug.Log("blue");
                }
            }
        }

        if (CarCondition == 0) { 
            return;
        }
        if ((transform.position - target).magnitude < 10)
        {
            
            mnager.cars.Add(gameObject);
            this.gameObject.SetActive(false);
            return;
        }
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
        
        float dist = Vector3.Distance(transform.position, target);
    
        Vector3 localVel = transform.InverseTransformDirection(Rigidbody.velocity);
        localVel.x = 0f;
        GetComponent<Rigidbody>().velocity = transform.TransformDirection(localVel);//ごり押しじゃぁあああああああぁあああああああ
        Vector3 start = transform.position;
        Vector3 end = target;
        if (float.IsNaN(end.x) || float.IsNaN(end.y) || float.IsNaN(end.z))
        {
            end = target;
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
        Debug.Log(agent.SetDestination(end));
        if (agent.SetDestination(end))
        {
            // 3. 経路のすべての曲がり角（座標）の配列を取得
            Vector3[] corners = agent.path.corners;
            Vector3 dddd;
            //if (corners.Length > 1)
            //{
            //    dist = Vector3.Distance(transform.position, corners[1]);
            //}
            float speed = fixedspeed * Mathf.Clamp(dist,100,float.PositiveInfinity);
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
            Debug.Log(moveforedV3);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("traficlight") && !isTouching2)
        {
            isTouching = true;
        }
        if (other.CompareTag("traficlight2") && !isTouching)
        {
            isTouching2 = true;
        }
        if (other.gameObject.tag == "carway")
        {
            carway = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("traficlight"))
        {
            isTouching = false;
            if(CarCondition == 0)
            {
                CarCondition = 1;
            }
        }
        if (other.CompareTag("traficlight2"))
        {
            isTouching2 = false;
            if (CarCondition == 0)
            {
                CarCondition = 1;
            }
        }
        if (other.gameObject.tag == "carway")
        {
            carway = false;//いったんフラグだけ作成、挙動はこれから考える。
        }
        
    }
}
