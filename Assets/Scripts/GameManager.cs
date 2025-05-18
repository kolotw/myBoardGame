using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //先做骰子 1-3
    int 步數;
    //站點
    public Transform[] 站點;
    [SerializeField] GameObject[] 角色;
    int 前進 = 0;
    public Text 顯示步數;
    public bool 可擲骰子 = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        角色 = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && 可擲骰子)
        {
            步數 = Random.Range(1, 4);
            print(步數);
            顯示步數.text = "步數: " + 步數.ToString();
            前進 = 前進 + 步數;
            if(前進 > 站點.Length)
            {
                前進 = 前進 - 站點.Length;
            }
            角色[0].GetComponent<農夫移動>().下一個目標(站點[前進]);
            可擲骰子 = false;
        }
    }
}
