using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProgram : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (var y = 0; y < 3; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                // オブジェクトを生成
                var areaObj = GameObject.Instantiate(
                    Resources.Load("Area") as GameObject,
                    new Vector3(x * 10f, y * 10f, 0f),
                    new Quaternion());

                areaObj.GetComponentInChildren<UnityEngine.Canvas>().worldCamera = Camera.main;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
