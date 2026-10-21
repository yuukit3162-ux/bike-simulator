using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow2Mnager : MonoBehaviour
{
    private Transform target2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target2 != null)
        {
            transform.LookAt(target2);
        }
        else
        {

        }
    }
    public void Gettarget2(Transform obj)
    {
        target2 = obj;
    }
}