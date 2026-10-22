using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowMnager : MonoBehaviour
{
    private Transform target;
    [SerializeField]private int speed=5;
    // Start is called before the first frame update
    /*矢印が目的地上空へ近づいたら移動するようにした...パラ調整とデバックはガンバへ*/
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            if((target.position-transform.position).sqrMagnitude <= 10000)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(0, Vector3.Distance(transform.parent.position, transform.position) / 3, 0), Time.deltaTime * speed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, transform.parent.position + new Vector3(0, 2.5f, 0), Time.deltaTime * speed);
            }
            transform.LookAt(target);
            transform.localScale= Vector3.one *Vector3.Distance(transform.parent.position , transform.position)/3;
        }
        else
        {
            
        }
    }
    public void Gettarget(Transform obj)
    {
        target = obj;
    }
}
