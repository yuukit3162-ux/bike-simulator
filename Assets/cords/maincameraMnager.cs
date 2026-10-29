using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maincameraMnager : MonoBehaviour
{
    public Transform player;
    private Vector3 offset = new Vector3(0, 2, -4);
    private Vector3 parentAngles;
    [SerializeField]
    private GameObject[] cam;
    private Vector3 pos;
    private Vector3 localpos;
    private int posbool;
    //private Quaternion pos;
    // Start is called before the first frame update
    void Start()
    {

        localpos = transform.InverseTransformPoint(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cam[posbool].SetActive(false);
            posbool++;
            posbool %= cam.Length;
            cam[posbool].SetActive(true);
            //switch (posbool) {
            //    case 0:
            //        transform.position = transform.TransformPoint( new Vector3(localpos.x, localpos.y, localpos.z + 10));
            //        break;
            //    case 1:
            //        transform.position = transform.TransformPoint(new Vector3(localpos.x, localpos.y, localpos.z - 10));
            //        break;
            //    case 2:
            //        transform.position = transform.TransformPoint(new Vector3(localpos.x, localpos.y+10, localpos.z));
            //        break;
            //}
        }
        pos = player.transform.position + new Vector3(0, 2.7f, 0);
        cam[posbool].transform.LookAt(pos);
        //pos.x -= 0.2f;
        //pos.z = 0f;
        //transform.parent.eulerAngles = new Vector3(0,Input.mousePosition.x - Screen.width / 2,0);
        //this.transform.rotation = Quaternion.LookRotation(pos);
        //childAngles.x = parentAngles.x + 10;
        //childAngles.y = parentAngles.y;
        //childAngles.z = parentAngles.z;
        //transform.rotation = Quaternion.Euler(childAngles);
        //transform.rotation = Quaternion.Euler(30f, transform.rotation.y, transform.rotation.z);
    }
}