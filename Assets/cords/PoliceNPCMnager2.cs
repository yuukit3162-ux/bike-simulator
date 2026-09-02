//試作中...
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPCMnager2 : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public Transform Player;
    public float speed = 100;
    private float rotateSpeed = 100;
    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        m_Rigidbody.Add
    }
}
