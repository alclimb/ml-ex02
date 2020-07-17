using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProgram : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (var z = 0; z < 3; z++)
        {
            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    // オブジェクトを生成
                    var areaObj = GameObject.Instantiate(
                        Resources.Load("Area") as GameObject,
                        new Vector3(x * 25f, y * 25f, z * 25f),
                        new Quaternion());

                    areaObj.GetComponentInChildren<UnityEngine.Canvas>().worldCamera = Camera.main;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickResetButton()
    {
        // 全てのファンネルを取得する
        var funnelAgents = GameObject.FindObjectsOfType<FunnelAgent>();

        // 全てのファンネルをリセットする
        foreach (var funnelAgent in funnelAgents)
        {
            funnelAgent.Reset();
        }
    }
}
