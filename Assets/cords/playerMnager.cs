using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class playerMnager : MonoBehaviour
{
    public float moveForce = 20f;   // 加える力の強さ
    public float maxSpeed = 10000f;    // 最高速度の制限
    public float turnSpeed = 180f;  // 回転速度
    private Rigidbody rb;
    public LayerMask groundlayer;
    private bool Jokou = false;
    public Material Mcolor;
    public Transform people;
    public traficlightMnager traficlightMnager;
    private bool traficlightIn = false;
    private bool traficlightIn2 = false;
    private bool carwayright;
    public bool carway;
    private Vector3 nowflem;
    private Vector3 beforeflem;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Mcolor.color = Color.green;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        beforeflem = transform.position;
    }

    void Update()
    {
        WebSocketClient.webC.road_type = carway;
        WebSocketClient.webC.move_inRight = carwayright;
        // Debug.Log(carway);
        if (GameMnager.Insector.GameStatus == "play")
        {
            rb.constraints = RigidbodyConstraints.None;
        }
        if(GameMnager.Insector.GameStatus == "finish")
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            playerReset();//初期配置
        }
        if((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Jokou == false)
        {
            moveForce = 15f;
        }
        else if (!Jokou)
        {
            moveForce = 30f;
        }
        if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Jokou == true)
        {
            moveForce = 10f;
        }
        else if (Jokou)
        {
            moveForce = 20f;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerReset();
        }
        if (Jokou && Input.GetKeyDown(KeyCode.Space))
        {//下のは自転車
            JokouUnFreezeRotation();
            Jokou = false;
        }
        else if(!Jokou && Input.GetKeyDown(KeyCode.Space))
        {//下のは歩行
            JokouFreezeRotation();
            Jokou = true;
        }
        if(gameObject.transform.position.y < 80f)
        {
            Debug.Log("落ちないためにリセット");
            playerReset();
        }

        if (GameMnager.Insector.PlayerReset)//GameMnager
        {
            playerReset();
            GameMnager.Insector.PlayerReset = false;
        }
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;
        nowflem = transform.position;
        Vector3 velocity = (nowflem - beforeflem) / Time.deltaTime;
        float velocityZ = velocity.z;
        
       
    }
    void playerReset()
    {
        transform.position = new Vector3(22f, 103f, -10f);
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rb.velocity = new Vector3(0f, 0f, 0f); ;
        rb.rotation = Quaternion.Euler(0f, 0f, 0f);
        JokouFreezeRotation();
        Jokou = true;
    }
    void JokouFreezeRotation()//歩き
    {
        
        //rb.rotation = Quaternion.Euler(0f, gameObject.transform.rotation.y, 0f);
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        Mcolor.color = Color.blue;
        people.localPosition = new Vector3(0, -0.3f, 0);
    }
    void JokouUnFreezeRotation()//自転車
    {
        rb.constraints = RigidbodyConstraints.None;
        Mcolor.color = Color.green;
        people.localPosition = new Vector3(0, 0, 0);
    }
    void FixedUpdate()
    {
        if (GameMnager.Insector.GameStatus != "play") return;
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        WebSocketClient.webC.speedKel = localVel.z;
        string a = localVel.z.ToString("F14");
        Debug.Log("valo " + a + "  " + localVel.z);
        //Debug.DrawRay(transform.position, -transform.up,Color.red,0.8f);
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        float rollAngle = transform.localEulerAngles.z;
        if (rollAngle > 180) rollAngle -= 360;
        if (Physics.Raycast(transform.position, -transform.up, 0.8f, groundlayer))
            if (Jokou)
            {
                //Debug.Log(transform.localEulerAngles.x);

                float rollAnglex = transform.localEulerAngles.x;
                if (rollAnglex > 180) rollAnglex -= 360;
                //rb.AddRelativeTorque(-transform.localEulerAngles.x, 0f, -rollAngle * 100);
                Vector3 tt2 = transform.forward;
                tt2.y = 0;
                if (maxSpeed > localVel.z)
                {
                    tt2 *= moveInput * 50;
                }
                tt2 -= transform.right * localVel.x * 2;
                rb.AddForce(tt2, ForceMode.Acceleration);
                Quaternion turnRotation = Quaternion.Euler(-rollAnglex, turnInput * turnSpeed * Time.fixedDeltaTime, -rollAngle);
                rb.MoveRotation(rb.rotation * turnRotation);
                //yロール加速度
                //Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime * moveInput / 2, 0f);
                //rb.MoveRotation();
            }
            else
            {
                // 1. 回転（A/Dキー）

                // 2. 向いている方向に力を加える（W/Sキー）


                // 傾きを打ち消す方向の加速度
                float targetRollAcc = -rollAngle * Math.Abs(localVel.z / 2) / 1 + turnInput * turnSpeed / -2;
                //Debug.Log(targetRollAcc);
                Vector3 sideSpeed = transform.right * localVel.x * 2;
                rb.AddForce(-sideSpeed, ForceMode.VelocityChange);//滑らないよう加速度適用
                

                // 相対座標のZ軸（forward）に対して加速度を適用
                rb.AddRelativeTorque(-localVel.z * 0.7f, 0f, Vector3.forward.z * targetRollAcc, ForceMode.Acceleration);//加速度適用
                Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime * moveInput / 2, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);//yロール加速度
                Vector3 force = transform.forward * moveInput * moveForce;
                // 現在の速度が最高速度を超えていない時だけ力を加える
                rb.AddForce(force, ForceMode.Acceleration);//加速
                
                //else
                //{
                //    Vector3 sideSpeed = transform.right * localVel.x * 2;
                //    rb.AddForce(-sideSpeed, ForceMode.VelocityChange);//滑らないよう加速度適用
                //    Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime * moveInput / 2, 0f);
                //    rb.MoveRotation(rb.rotation * turnRotation);//yロール加速度
                //    Vector3 force = transform.forward * moveInput * moveForce;
                //    // 現在の速度が最高速度を超えていない時だけ力を加える
                //    rb.AddForce(force, ForceMode.Acceleration);//加速
                //}



            }
        else
        {
            //Debug.Log("noray");

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "traficlight" && !traficlightIn2)
        {
            traficlightIn = true;
            if (traficlightMnager.traficColor == 1)//赤
            {
                WebSocketClient.webC.lights_on = 0;
                Debug.Log("red");
                GameMnager.whatSin = GameMnager.violationType.IgnoringTrafficLights;
            }
            if (traficlightMnager.traficColor == 2)//黄色
            {
                WebSocketClient.webC.lights_on = 1;
                Debug.Log("yellow");
            }
            if (traficlightMnager.traficColor == 3)//青
            {
                WebSocketClient.webC.lights_on = 2;
                Debug.Log("bule");
            }
        }
        if (other.gameObject.tag == "traficlight2" && !traficlightIn)
        {
            traficlightIn2 = true;
            if (traficlightMnager.traficColor2 == 4)//赤
            {
                WebSocketClient.webC.lights_on = 0;
                Debug.Log("red2222");
                GameMnager.whatSin = GameMnager.violationType.IgnoringTrafficLights;
            }
            if (traficlightMnager.traficColor2 == 5)//黄色
            {
                WebSocketClient.webC.lights_on = 1;
                Debug.Log("yellow2222");
            }
            if (traficlightMnager.traficColor2 == 6)//青
            {
                WebSocketClient.webC.lights_on = 2;
                Debug.Log("bule2222");
            }
        }
        if(other.gameObject.tag == "bikewaypoint")
        {
            GameMnager.Insector.Countpls = true;
            Debug.Log("count+");
        }

        if (other.gameObject.tag == "carwayright")
        {
            carwayright = true;
        }
        if (other.gameObject.tag == "carwayleft")
        {
            carwayright = false;
        }
        if (other.gameObject.tag == "carway")
        {
            carway = true;
        }
        if (other.gameObject.tag == "carNPC")
        {
            //if (other.gameObject.GetComponent<Rigidbody>() == null)
            //    return;
            //Rigidbody r = other.gameObject.GetComponent<Rigidbody>();
            Debug.Log("衝突今は切っている");
            Rigidbody r = gameObject.GetComponent<Rigidbody>();
            //r.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 10000, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "carwayright")
        {
            carwayright = true;
        }
        if (other.gameObject.tag == "carwayleft")
        {
            carwayright = false;
        }
        if (other.gameObject.tag == "carway")
        {
            carway = false;
        }
        if (other.gameObject.tag == "traficlight")
        {
            WebSocketClient.webC.lights_on = 4;
            traficlightIn = false;
        }
        if (other.gameObject.tag == "traficlight2")
        {
            WebSocketClient.webC.lights_on = 4;
            traficlightIn2 = false;
        }
    }
}

