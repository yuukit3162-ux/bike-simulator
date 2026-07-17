using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private int hand_sine = 4;//left:1 rigth:2 none:4
    public GameObject handleft;
    public GameObject handright;
    private float handinclination = 0;//傾き
    private float handdismiter = 1f;
    private bool usingSmartPhone;
    public Camera maincamera;
    public CanvasGroup CanvasGroup;
    public GameObject bikelight;
    private int DrunkLevel;//酔い度　0~3
    private float DrunkNoise = 0;
    public Text DrunkText;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Mcolor.color = Color.green;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition;
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
        if (GameMnager.Insector.GameStatus == "finish")
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            playerReset();//初期配置
        }
        if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Jokou == false)
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerReset();
        }
        if (Jokou && Input.GetKeyDown(KeyCode.Space))
        {//下のは自転車
            JokouUnFreezeRotation();
            Jokou = false;
        }
        else if (!Jokou && Input.GetKeyDown(KeyCode.Space))
        {//下のは歩行
            JokouFreezeRotation();
            Jokou = true;
        }
        if (gameObject.transform.position.y < 80f)
        {
            Debug.Log("落ちないためにリセット");
            playerReset();
        }

        if (GameMnager.Insector.PlayerReset)//GameMnager
        {
            playerReset();
            GameMnager.Insector.PlayerReset = false;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (handleft.activeSelf == true)//すでに左手を出していたら直す
            {
                handleft.SetActive(false);
            }
            else
            {
                handleft.SetActive(true);
            }
            WebSocketClient.webC.Webhand_sine = hand_sine;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (handright.activeSelf == true)//すでに右手を出していたら直す
            {
                handright.SetActive(false);
            }
            else
            {
                handright.SetActive(true);
            }
            WebSocketClient.webC.Webhand_sine = hand_sine;
        }
        if (handleft.activeSelf == true && handright.activeSelf == true)
        {
            handinclination = 0;
            handdismiter = 100f;
            hand_sine = 4;
        }
        else if (handright.activeSelf == true)
        {
            handinclination = -10f;
            handdismiter = 10f;
            hand_sine = 2;
        }
        else if (handleft.activeSelf == true)
        {
            handinclination = 10f;
            handdismiter = 10f;
            hand_sine = 1;
        }
        else
        {
            handinclination = 0f;
            handdismiter = 1f;
            hand_sine = 4;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (usingSmartPhone)
            {
                usingSmartPhone = false;
                maincamera.farClipPlane = 300f;
                CanvasGroup.alpha = 0;
            }
            else
            {
                usingSmartPhone = true;
                maincamera.farClipPlane = 50f;
                CanvasGroup.alpha = 1;
            }
            Debug.Log(usingSmartPhone);
            WebSocketClient.webC.usingSmartPhone = usingSmartPhone;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (bikelight.activeSelf)
            {
                bikelight.SetActive(false);
            }
            else
            {
                bikelight.SetActive(true);
            }
            WebSocketClient.webC.bikelight = bikelight.activeSelf;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (DrunkLevel < 3)
            {
                DrunkLevel++;
                DrunkText.text = "酔い度:" + DrunkLevel;
            }
            else
            {
                Debug.Log("drunklevel is Max" + DrunkLevel);
            }
            WebSocketClient.webC.drunkint = DrunkLevel;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (0 < DrunkLevel)
            {
                DrunkLevel--;
                DrunkText.text = "酔い度:" + DrunkLevel;
            }
            else
            {
                Debug.Log("drunklevel is Min" + DrunkLevel);
            }
            WebSocketClient.webC.drunkint = DrunkLevel;
        }
        StartCoroutine(Noise());
    }
    void playerReset()
    {
        transform.position = new Vector3(22f, 103f, -10f);
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rb.velocity = new Vector3(0f, 0f, 0f); ;
        rb.rotation = Quaternion.Euler(0f, 0f, 0f);
        handleft.SetActive(false);
        handright.SetActive(false);
        hand_sine = 4;
        usingSmartPhone = false;
        maincamera.farClipPlane = 300f;
        CanvasGroup.alpha = 0;
        DrunkLevel = 0;
        DrunkText.text = "酔い度:" + DrunkLevel;
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
    IEnumerator Noise()
    {
        DrunkNoise = Random.Range(-1f, 1f) * DrunkLevel;
        yield return new WaitForSeconds(0.2f);
    }
    void FixedUpdate()
    {
        if (GameMnager.Insector.GameStatus != "play") return;
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        WebSocketClient.webC.speedKel = localVel.z;
        string a = localVel.z.ToString("F14");
        //Debug.Log("valo " + a + "  " + localVel.z);
        //Debug.DrawRay(transform.position, -transform.up,Color.red,0.8f);
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        float rollAngle = transform.localEulerAngles.z;
        if (rollAngle > 180) rollAngle -= 360;
        if (Physics.Raycast(transform.position, -transform.up, 0.8f, groundlayer))
            if (Jokou)//歩き
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
            else//↓自転車
            {
                //酔っているときのnoise
                turnInput += DrunkNoise;
                // 1. 回転（A/Dキー）
                // 2. 向いている方向に力を加える（W/Sキー）
                // 傾きを打ち消す方向の加速度
                float targetRollAcc = -rollAngle * Mathf.Abs(localVel.z / 2) / handdismiter + turnInput * turnSpeed / -2 + handinclination;
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
                GameMnager.whatSin = GameMnager.violationType.IgnoringTrafficLights;
            }
            if (traficlightMnager.traficColor == 2)//黄色
            {
                WebSocketClient.webC.lights_on = 1;
            }
            if (traficlightMnager.traficColor == 3)//青
            {
                WebSocketClient.webC.lights_on = 2;
            }
        }
        if (other.gameObject.tag == "traficlight2" && !traficlightIn)
        {
            traficlightIn2 = true;
            if (traficlightMnager.traficColor2 == 4)//赤
            {
                WebSocketClient.webC.lights_on = 0;
                GameMnager.whatSin = GameMnager.violationType.IgnoringTrafficLights;
            }
            if (traficlightMnager.traficColor2 == 5)//黄色
            {
                WebSocketClient.webC.lights_on = 1;
            }
            if (traficlightMnager.traficColor2 == 6)//青
            {
                WebSocketClient.webC.lights_on = 2;
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

