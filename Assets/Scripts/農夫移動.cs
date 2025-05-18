using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class 農夫移動 : MonoBehaviour
{
    private NavMeshAgent 導航;
    private Animator 動畫控制器;
    public Transform 目標;
    // Start is called before the first frame update
    void Start()
    {
        導航 = GetComponent<NavMeshAgent>();
        動畫控制器 = GetComponent<Animator>();
        if (目標 != null)
        {
            //導航.SetDestination(目標.position);
            //動畫控制器.SetBool("isWalking", true);
        }
    }
    public void 下一個目標(Transform nextPos)
    {
        目標 = nextPos;
        導航.SetDestination(目標.position);
        動畫控制器.SetBool("isWalking", true);
    }
    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position, 目標.position) < 0.2f)
        {
            動畫控制器.SetBool("isWalking", false);
        }
        AnimatorStateInfo stateInfo = 動畫控制器.GetCurrentAnimatorStateInfo(0);

        // 檢查特定動畫狀態是否正在播放並且已完成至少一個循環
        if (stateInfo.IsName("BasicMotions@Idle02 - Idle01") && stateInfo.normalizedTime >= 1.0f)
        {
            Debug.Log("動畫已完成!");
            // 在這裡執行動畫完成後的邏輯
            GameObject.Find("/00GameMaster").GetComponent<GameManager>().可擲骰子 = true;
        }
        else
        {
            GameObject.Find("/00GameMaster").GetComponent<GameManager>().可擲骰子 = false;
        }
    }
}
