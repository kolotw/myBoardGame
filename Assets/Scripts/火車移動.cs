using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class 火車移動 : MonoBehaviour
{
    public Transform 目標;
    private NavMeshAgent 導航;
    // Start is called before the first frame update
    void Start()
    {
        導航 = GetComponent<NavMeshAgent>();
        if(目標 != null)
        {
            導航.SetDestination(目標.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(目標.name == "火車頭")
        {
            導航.SetDestination(目標.position);
        }
    }
}
