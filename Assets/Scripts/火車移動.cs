using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class 火車移動 : MonoBehaviour
{
    public Transform 目標;
    private NavMeshAgent 導航;

    [Header("車廂設置")]
    public bool 是火車頭 = true;
    public Transform 前一節車廂;
    public float 跟隨距離 = 2.0f;
    public float 位置記錄間隔 = 0.05f; // 記錄位置的時間間隔（秒）

    // 位置歷史記錄
    private List<PositionRecord> 位置歷史 = new List<PositionRecord>();
    private float 記錄計時器 = 0f;
    public bool isStop = false;
    // 位置記錄類
    public class PositionRecord
    {
        public Vector3 位置;
        public Quaternion 旋轉;
        public float 時間戳;

        public PositionRecord(Vector3 pos, Quaternion rot, float time)
        {
            位置 = pos;
            旋轉 = rot;
            時間戳 = time;
        }
    }

    void Awake()
    {
        // 初始化位置歷史（避免出現null引用）
        //for (int i = 0; i < 10; i++)
        //{
        //    位置歷史.Add(new PositionRecord(transform.position, transform.rotation, -i * 位置記錄間隔));
        //}
    }

    void Start()
    {
        // 初始化火車頭
        if (是火車頭)
        {
            導航 = GetComponent<NavMeshAgent>();

            if (目標 != null)
            {
                導航.SetDestination(目標.position);
            }
        }
        else
        {
            // 確保車廂有前一節引用
            if (前一節車廂 == null)
            {
                Debug.LogError("車廂必須設置前一節車廂！");
                enabled = false;
                return;
            }

            // 初始化車廂位置
            StartCoroutine(初始化車廂位置());
        }
    }

    IEnumerator 初始化車廂位置()
    {
        // 等待一幀，確保前一節車廂已經初始化
        yield return null;

        if (前一節車廂 != null)
        {
            火車移動 前一節控制器 = 前一節車廂.GetComponent<火車移動>();

            if (前一節控制器 != null)
            {
                // 等待前一節車廂記錄足夠的位置歷史
                while (前一節控制器.位置歷史.Count < 10)
                {
                    yield return new WaitForSeconds(0.15f);
                }

                // 計算初始位置
                //Vector3 初始位置方向 = (前一節車廂.position - transform.position).normalized;
                //transform.position = 前一節車廂.position - 初始位置方向 * 跟隨距離;
                //transform.rotation = 前一節車廂.rotation;

                // 初始化自己的位置歷史
                位置歷史.Clear();
                for (int i = 0; i < 50; i++)
                {
                    位置歷史.Add(new PositionRecord(transform.position, transform.rotation, Time.time - i * 位置記錄間隔));
                }
            }
        }
    }

    void Update()
    {
        if (isStop) return;
        
        if (是火車頭)
        {
            // 火車頭使用NavMeshAgent前往目標
            if (目標 != null && 導航 != null)
            {
                導航.SetDestination(目標.position);
                if (Vector3.Distance(this.transform.position, 目標.position) < 1.1f)
                {
                    print(isStop);
                    isStop = true;
                }
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, 前一節車廂.position) > 1.0f)
            {
                // 車廂跟隨前一節的歷史軌跡
                跟隨前一節軌跡();
            }
                
        }

        // 無論是火車頭還是車廂，都記錄位置歷史        
        記錄位置();
    }

    void 記錄位置()
    {
        記錄計時器 += Time.deltaTime;

        // 每隔一段時間記錄一次位置
        if (記錄計時器 >= 位置記錄間隔)
        {
            位置歷史.Insert(0, new PositionRecord(transform.position, transform.rotation, Time.time));

            // 限制歷史記錄大小
            if (位置歷史.Count > 1000)
            {
                位置歷史.RemoveAt(位置歷史.Count - 1);
            }

            記錄計時器 = 0f;
        }
    }

    void 跟隨前一節軌跡()
    {
        if (前一節車廂 == null) return;

        // 獲取前一節車廂的控制器
        火車移動 前一節控制器 = 前一節車廂.GetComponent<火車移動>();

        if (前一節控制器 != null && 前一節控制器.位置歷史.Count > 0)
        {
            // 計算延遲時間（根據跟隨距離和速度）
            float 延遲時間 = 跟隨距離 / 5.0f; // 假設速度是5單位/秒，可以根據實際情況調整
            float 目標時間 = Time.time - 延遲時間;

            // 查找與目標時間最接近的兩個歷史點
            int 索引1 = 0;
            int 索引2 = 0;

            for (int i = 0; i < 前一節控制器.位置歷史.Count - 1; i++)
            {
                if (前一節控制器.位置歷史[i].時間戳 >= 目標時間 && 前一節控制器.位置歷史[i + 1].時間戳 <= 目標時間)
                {
                    索引1 = i;
                    索引2 = i + 1;
                    break;
                }
            }

            // 如果找到了合適的歷史點
            if (索引2 < 前一節控制器.位置歷史.Count)
            {
                // 計算兩點之間的插值因子
                float t = Mathf.InverseLerp(
                    前一節控制器.位置歷史[索引1].時間戳,
                    前一節控制器.位置歷史[索引2].時間戳,
                    目標時間
                );

                // 對位置和旋轉進行插值
                Vector3 插值位置 = Vector3.Lerp(
                    前一節控制器.位置歷史[索引1].位置,
                    前一節控制器.位置歷史[索引2].位置,
                    t
                );

                Quaternion 插值旋轉 = Quaternion.Slerp(
                    前一節控制器.位置歷史[索引1].旋轉,
                    前一節控制器.位置歷史[索引2].旋轉,
                    t
                );

                // 設置車廂位置和旋轉
                transform.position = 插值位置;
                transform.rotation = 插值旋轉;
            }
            else if (前一節控制器.位置歷史.Count > 0)
            {
                // 如果找不到合適的點，使用最後一個歷史點
                transform.position = 前一節控制器.位置歷史[前一節控制器.位置歷史.Count - 1].位置;
                transform.rotation = 前一節控制器.位置歷史[前一節控制器.位置歷史.Count - 1].旋轉;
            }
        }
    }

    // 可視化車廂歷史軌跡
    void OnDrawGizmos()
    {
        if (位置歷史 != null && 位置歷史.Count > 1)
        {
            Gizmos.color = 是火車頭 ? Color.red : Color.green;

            // 只顯示前20個點以避免過度繪製
            int 顯示點數 = Mathf.Min(位置歷史.Count - 1, 20);
            for (int i = 0; i < 顯示點數; i++)
            {
                if (i < 位置歷史.Count && i + 1 < 位置歷史.Count)
                {
                    Gizmos.DrawLine(位置歷史[i].位置, 位置歷史[i + 1].位置);
                }
            }
        }
    }
}