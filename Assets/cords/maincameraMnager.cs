using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maincameraMnager : MonoBehaviour
{
    public Transform player;
    private Vector3 offset = new Vector3(0, 2, -4);
    private Vector3 parentAngles;
    private Vector3 childAngles;
    private Vector3 pos;
    private Vector3 localpos;
    private bool posbool;
    //private Quaternion pos;
    // Start is called before the first frame update
    void Start()
    {
        parentAngles = transform.parent.rotation.eulerAngles;

        childAngles = transform.rotation.eulerAngles;

        localpos = transform.InverseTransformPoint(transform.parent.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(posbool = !posbool)
            {
                posbool = true;
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 10);
            }
            else
            {
                posbool = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10);
            }
        }
        pos = player.transform.position - gameObject.transform.position + new Vector3(0, 2.7f, 0);

        //pos.x -= 0.2f;
        //pos.z = 0f;
        //transform.parent.eulerAngles = new Vector3(0,Input.mousePosition.x - Screen.width / 2,0);
        this.transform.rotation = Quaternion.LookRotation(pos);
        //childAngles.x = parentAngles.x + 10;
        //childAngles.y = parentAngles.y;
        //childAngles.z = parentAngles.z;
        //transform.rotation = Quaternion.Euler(childAngles);
        //transform.rotation = Quaternion.Euler(30f, transform.rotation.y, transform.rotation.z);
    }
}