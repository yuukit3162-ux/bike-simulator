using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carNPC2 : MonoBehaviour
{
    public Transform[] waypoints;
    public int[] conditions;
    private float speed = 50f;
    private float MAXspeed = 50f;
    private int currecount = 0;
    public int CarCondition = 1;//0:停止 1:直進　2:右回転　3:左回転
    private bool isTouching = false;
    private bool isTouching2 = false;
    public traficlightMnager traficlightMnager;
    Rigidbody Rigidbody;
    private Camera dummyCamera;
    private Camera dummyCameraR;
    private Camera dummyCameraL;
    string turnback = "";
    public bool traficbool;
    public carMnager carMnager;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
        // 1. コードから動的にダミーカメラを作成
        GameObject cameraObj = new GameObject("DetectionDummyCamera");
        dummyCamera = cameraObj.AddComponent<Camera>();

        // 2. このオブジェクトの子要素にして前方を向かせる
        cameraObj.transform.SetParent(this.transform);
        cameraObj.transform.localPosition = new Vector3(0f,-1f, 1f);
        cameraObj.transform.localRotation = Quaternion.identity;


        // 4. 四角錐の形を設定
        dummyCamera.fieldOfView = 80f;//視野（横）
        dummyCamera.farClipPlane = 40f;//カメラの奥行
        dummyCamera.nearClipPlane = 3f;//カメラに映る近さ
        dummyCamera.aspect = 1.33f;//カメラの比

        // 3. 描画を無効にして負荷をゼロにする
        dummyCamera.enabled = false;
        dummyCamera.cullingMask = 0;

        GameObject cameraObjR = new GameObject("DetectionDummyCamera");
        dummyCameraR = cameraObjR.AddComponent<Camera>();
        // 2. このオブジェクトの子要素にして前方を向かせる
        cameraObjR.transform.SetParent(this.transform);
        cameraObjR.transform.localPosition = new Vector3(2.0f,-0.2f, 3.5f);
        cameraObjR.transform.localRotation = Quaternion.Euler(0f, 168f, 0f);


        // 4. 四角錐の形を設定
        dummyCameraR.fieldOfView = 40f;//視野（横）
        dummyCameraR.farClipPlane = 40f;//カメラの奥行
        dummyCameraR.nearClipPlane = 0.01f;//カメラに映る近さ
        dummyCameraR.aspect = 0.5f;//カメラの比


        // 3. 描画を無効にして負荷をゼロにする
        dummyCameraR.enabled = false;
        dummyCameraR.cullingMask = 0;

        GameObject cameraObjL = new GameObject("DetectionDummyCamera");
        dummyCameraL = cameraObjL.AddComponent<Camera>();

        // 2. このオブジェクトの子要素にして前方を向かせる
        cameraObjL.transform.SetParent(this.transform);
        cameraObjL.transform.localPosition = new Vector3(-2.8f, -0.2f, 3.5f);
        cameraObjL.transform.localRotation = Quaternion.Euler(0f, -168f, 0f);


        // 4. 四角錐の形を設定
        dummyCameraL.fieldOfView = 40f;//視野（横）
        dummyCameraL.farClipPlane = 40f;//カメラの奥行
        dummyCameraL.nearClipPlane = 0.01f;//カメラに映る近さ
        dummyCameraL.aspect = 0.5f;//カメラの比


        // 3. 描画を無効にして負荷をゼロにする
        dummyCameraL.enabled = false;
        dummyCameraL.cullingMask = 0;
    }
    // Update is called once per frame
    private bool IsPointInsidePlanes(Plane[] planes, Vector3 point)
    {
        for (int i = 0; i < planes.Length; i++)
        {
            // 点が平面の裏側（範囲外）にあれば、その時点でfalse
            if (!planes[i].GetSide(point))
            {
                return false;
            }
        }
        return true; // すべての平面の表側にあればtrue
    }
    void Update()
    {
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
        

        if (waypoints.Length == 0) return;
        if (CarCondition == 0) return;
        Transform target = waypoints[currecount];
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 localVel = transform.InverseTransformDirection(Rigidbody.velocity);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(dummyCamera);
        Plane[] planesR = GeometryUtility.CalculateFrustumPlanes(dummyCameraR);
        Plane[] planesL = GeometryUtility.CalculateFrustumPlanes(dummyCameraL);
        List<GameObject> cansee = new List<GameObject>();
        float distance = float.PositiveInfinity;
        float distances = float.PositiveInfinity;
        float rotatedist = float.PositiveInfinity;
        // すべての車をループ処理
        foreach (GameObject car in carMnager.games)
        {
            if (car == null) continue;
            //Debug.Log(car.transform.position);
            // 車の Collider を取得（なければスキップ）
            //Collider carCollider = car.GetComponent<Collider>();
            //if (carCollider == null) continue;

            // 車が四角錐（ダミーカメラの視界）の内側にあるか判定
            if (IsPointInsidePlanes(planes, car.transform.position)|| IsPointInsidePlanes(planesR, car.transform.position)|| IsPointInsidePlanes(planesL, car.transform.position))
            {
                if (IsPointInsidePlanes(planesR, car.transform.position) || IsPointInsidePlanes(planesL, car.transform.position))
                {
                    //Debug.Log("ess");
                }
                if (Mathf.Abs(transform.InverseTransformPoint(car.transform.position).z) < distances)
                {
                    distances = transform.InverseTransformPoint(car.transform.position).z;
                    //Debug.Log(distances);
                    if (Mathf.Abs(transform.InverseTransformPoint(car.transform.position).x) < 3.5f && distances>0)
                    {
                        //Debug.Log("!");
                        distance = distances;

                    }
                    else
                    {
                        if (distances > 0)
                        {
                            distances = 0;
                        }
                        rotatedist = Mathf.MoveTowards(transform.InverseTransformPoint(car.transform.position).x, 0f, 3.5f+ distances/5);
                    }

                }

                cansee.Add(car);
            }
        }

        //dir.y=Math.Clamp(dir.y,Vector3.forward.y-30,Vector3.forward.y+30);
        float back = 1;
        float lookrotation_y = Quaternion.LookRotation(dir).eulerAngles.y + Mathf.Clamp(-90/rotatedist, -90, 90);
        float look_to = Mathf.DeltaAngle(transform.eulerAngles.y, lookrotation_y);
        float dist = Vector3.Distance(transform.position, target.position);
        //if (distance < 15f)
        //{
        //    if (look_to > 0)
        //    {
        //        turnback = "rite";
        //    }
        //    else
        //    {
        //        turnback = "left";
        //    }
        //    distance = (15f - distance) * 10;
        //}
        if (look_to > 80 || turnback == "rite")
        {
            back = -1;

            turnback = "rite";
            if (look_to < 40)
            {
                turnback = "";
            }
            //look_to -= 180;
        }
        else if (look_to < -80 || turnback == "left")
        {
            back = -1;
            turnback = "left";
            if (look_to > -40)
            {
                turnback = "";
            }

            //look_to += 180;
        }





        distance = Mathf.Clamp((distance - 20) / 10 ,-1,1);
        
        //Debug.Log(distance);
        float look_to_rotation_y = Mathf.Clamp(look_to * Mathf.PI / 180f/ (Time.fixedDeltaTime*20) - Rigidbody.angularVelocity.y, -5, +5);//
        float movefored = speed * back  * distance - localVel.z;//* Mathf.Clamp(dist - localVel.z, 0f, 1f)
        Vector3 moveforedV3 = transform.forward * Mathf.Min(movefored, MAXspeed);
        Rigidbody.AddForce(moveforedV3 - transform.right * localVel.x * 2, ForceMode.Acceleration);
        //↓すべるの対策
        Rigidbody.AddTorque(new Vector3(0, look_to_rotation_y / (Time.fixedDeltaTime) * Mathf.Clamp(localVel.z/80, 0.1f, 1f), 0), ForceMode.Acceleration);
        //Debug.Log((look_to_rotation_y, Time.fixedDeltaTime, look_to_rotation_y / (Time.fixedDeltaTime), look_to_rotation_y / (Time.fixedDeltaTime*Time.fixedDeltaTime), localVel.z / 50));
        if (dist < 10f)
        {
            currecount++;
            if (currecount >= waypoints.Length)
            {
                currecount = 0;
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("traficlight"))
        {
            isTouching = false;
        }
        if (other.CompareTag("traficlight2"))
        {
            isTouching2 = false;
        }
    }

}