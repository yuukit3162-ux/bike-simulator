using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carNPC : MonoBehaviour
{
    public Transform[] waypoints;
    public int[] conditions;
    private float speed = 10f;
    private float rotateSpeed = 100f;
    private int currecount = 0;
    private int CarCondition = 1;//0:停止 1:直進　2:右回転　3:左回転
    private bool isTouching = false; 
    private bool isTouching2 = false;
    public traficlightMnager traficlightMnager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
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
        if (isTouching2)//交差点のバグ
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
        if (waypoints.Length == 0) return;
        if (CarCondition == 0) return;
        Transform target = waypoints[currecount];
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookrotation = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookrotation, rotateSpeed * Time.deltaTime);

        //transform.position += transform.forward * speed * Time.deltaTime;
        transform.Translate(-transform.forward * speed * Time.deltaTime);
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 2f)
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
            Debug.Log("isTouching");
        }
        if (other.CompareTag("traficlight2") && !isTouching)
        {
            isTouching2 = true;
            Debug.Log("isTouching2");
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
