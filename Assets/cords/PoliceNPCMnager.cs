using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPCMnager : MonoBehaviour
{
    public Transform Player;
    public float speed = 0;
    private float rotateSpeed = 100;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (WebSocketClient.webC.Countviolations > 0)
        {
            Vector3 dir = (Player.position - transform.position).normalized;
            Quaternion lookrotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.Euler(0, lookrotation.eulerAngles.y, 0), rotateSpeed * Time.deltaTime);
            //transform.position += transform.forward * speed * Time.deltaTime;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if (WebSocketClient.webC.Countviolations == 0)
        {

        }
        else if (WebSocketClient.webC.Countviolations < 3)//1.2
        {
            speed = 10;//speed‚Í‚æ‚è‘å‚«‚­‚È‚¢‚ÆŒã‚ë‚É‰º‚ª‚é
            //’¼‚µ‚½‚¢‚È‚çƒR[ƒh‚ð‘‚«Š·‚¦‚È‚¢‚Æ‚¢‚¯‚È‚¢‚æw
        }
        else if (WebSocketClient.webC.Countviolations < 5)//3.4
        {
            speed = 15;
        }
        else if (WebSocketClient.webC.Countviolations < 7)//5.6
        {
            speed = 20;
        }
        else if (WebSocketClient.webC.Countviolations < 8)//7.8
        {
            speed = 25;
        }
        else
        {
            speed = 30;
        }
    }
}
