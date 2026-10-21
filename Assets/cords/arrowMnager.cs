using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowMnager : MonoBehaviour
{
    private Transform target;
    privete Vector3 pos;
    [SerializeField]private int speed=5;
    // Start is called before the first frame update
    void Start()
    {
        pos=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            if((target.position-transform.position).sqrMagnitude <= 2500){
                transform.position = Vector3.Lerp(transform.position, target.position+new Vector3(0,20,0), Time.deltaTime * speed):
                
            }else{
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);
                
            }
            transform.LookAt(target);
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
