using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nomalcarMnager : MonoBehaviour
{
    private float timer = 0f;
    private int interbal = 5;
    public GameObject nomalcar;
    public GameObject start;
    public GameObject finish;
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(nomalcar, start.transform.position, Quaternion.Euler(0,-90,0));
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (interbal < timer)
        {
            timer = 0f;
            Instantiate(nomalcar, start.transform.position, Quaternion.Euler(0, -90, 0));
        }
    }
}
