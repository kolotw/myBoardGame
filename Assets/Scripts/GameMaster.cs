using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public Transform[] 路線位置; //目標
    public GameObject[] 角色;
    public int 骰子點數;
    int 目前位置 = 0;
    public bool 可擲骰子 = false;
    public Text 點數文字;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        角色 = GameObject.FindGameObjectsWithTag("Player");
        點數文字.text = "點數：0";
    }

    // Update is called once per frame
    void Update()
    {
        if (!可擲骰子) return;
        if (Input.GetKeyUp(KeyCode.Mouse1)) //如果按下(放開)滑鼠右鍵
        {
            //進行擲骰子的動作
            骰子點數 = Random.Range(1, 4);
            print(骰子點數);
            點數文字.text = "點數： " + 骰子點數;

            int 前進點數 = (骰子點數 + 目前位置) ;
            if (前進點數 >= 路線位置.Length) {
                前進點數 = 前進點數 - 路線位置.Length;
            }
            目前位置 = 前進點數;

            // 路線位置[骰子點數] 
            角色[0].GetComponent<農夫移動>().下一個目標(路線位置[前進點數]);
        }
    }
}
