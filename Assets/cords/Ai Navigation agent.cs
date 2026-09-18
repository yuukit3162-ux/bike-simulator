using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiNavigationagent : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public Transform target;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("Agent有効: " + agent.isActiveAndEnabled);
        Debug.Log("車の位置: " + transform.position);
        Debug.Log("NavMesh上？ " + agent.isOnNavMesh);//NavMeshの上かどうか
        //agent.updatePosition = false;
        //agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("車がNavMesh上にいません！");
            return;
        }
        agent.SetDestination(target.position);
    }
}
