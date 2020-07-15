using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class FunnelAgent : Agent
{
    /// <summary>ファンネルのリジッドボディ</summary>
    private Rigidbody _rigidbody;

    /// <summary>基準になる位置</summary>
    private Vector3 _basePosition;

    /// <summary>エピソード毎のストップウォッチ</summary>
    private System.Diagnostics.Stopwatch _episodeTime = new System.Diagnostics.Stopwatch();

    [Header("ステータス表示用テキスト")]
    public UnityEngine.UI.Text StatusText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void Initialize()
    {
        this._rigidbody = base.GetComponent<Rigidbody>();
        this._basePosition = this._rigidbody.position;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log($"OnEpisodeBegin");

        // 位置を初期化
        this._rigidbody.position = Vector3.zero + this._basePosition;
        this._rigidbody.velocity = Vector3.zero;
        this._rigidbody.rotation = Quaternion.identity;
        this._rigidbody.angularVelocity = Vector3.zero;

        // ストップウォッチをリセット
        this._episodeTime.Reset();
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log($"OnActionReceived >> {vectorAction.Length}");

        const float power = 20f;
        var actionX = Mathf.Clamp(vectorAction[0], -1f, 1f) * power;
        var actionZ = Mathf.Clamp(vectorAction[1], -1f, 1f) * power;

        // 移動
        this._rigidbody.AddForce(new Vector3(actionX, 0, actionZ));

        // // ターゲットの位置 (ボールからの相対位置)
        // var targetPosition = this.Target.position - this.transform.position;

        // if ((0 < targetPosition.x && 0 < this._rigidbody.velocity.x) ||
        //     (targetPosition.x < 0 && this._rigidbody.velocity.x < 0))
        // {
        //     // 評価: 報酬を与える
        //     base.AddReward(+0.01f);
        // }
        // else
        // {
        //     // 評価: 報酬を減らす
        //     base.AddReward(-0.01f);
        // }

        // if ((0 < targetPosition.z && 0 < this._rigidbody.velocity.z) ||
        //     (targetPosition.z < 0 && this._rigidbody.velocity.z < 0))
        // {
        //     // 評価: 報酬を与える
        //     base.AddReward(+0.01f);
        // }
        // else
        // {
        //     // 評価: 報酬を減らす
        //     base.AddReward(-0.01f);
        // }

        // 落下判定
        if (this.transform.position.y < -1.0f)
        {
            // UI更新
            this.StatusText.text = "落下";

            // // 評価: 報酬を減らす
            // base.AddReward(-1f);

            // リセットして次のエピソードを開始
            base.EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this._rigidbody.position - this._basePosition);
        sensor.AddObservation(this._rigidbody.velocity);
        sensor.AddObservation((float)this._episodeTime.Elapsed.TotalSeconds);
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("MY_TARGET"))
    //     {
    //         // UI更新
    //         this.StatusText.text = "クリア";

    //         var delta = (Time.time - this._episodeTime);

    //         var score = (100f - delta) * 0.1f;

    //         // 評価: 報酬を与える
    //         base.AddReward(1.0f + score);

    //         // リセットして次のエピソードを開始
    //         base.EndEpisode();
    //     }
    // }
}
