using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class 人物移動 : MonoBehaviour
{
    NavMeshAgent 導航;
    Animator 動畫控制器;
    public Transform 目標;

    // Start is called before the first frame update
    void Start()
    {
        導航 = GetComponent<NavMeshAgent>();
        動畫控制器 = GetComponent<Animator>();
        if(目標 != null)
        {
            導航.SetDestination(目標.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position, 目標.position) < 1.1f)
        {
            動畫控制器.SetBool("isStop", true);
        }
    }
}
