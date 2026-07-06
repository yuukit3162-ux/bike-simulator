using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nomalcar : MonoBehaviour
{
    private float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += -transform.forward * speed * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position,-transform.forward,out hit,10f))
        {
            if (hit.collider.gameObject.CompareTag("finish"))
            {
                Destroy(gameObject);
            }
        }
    }
}
