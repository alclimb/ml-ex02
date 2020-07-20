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
        const float range = 5f;

        // 位置を初期化
        this._rigidbody.position = this._basePosition + new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
        this._rigidbody.velocity = Vector3.zero;
        this._rigidbody.rotation = Quaternion.identity;
        this._rigidbody.angularVelocity = Vector3.zero;

        // ストップウォッチをリセット
        this._episodeTime.Reset();
        this._episodeTime.Start();
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        const float power = 300f;
        const float area = 10f;
        const float limitTime = 5f; // リミット時間 (単位: 秒)

        float actionX = vectorAction[0];
        float actionY = vectorAction[1];
        float actionZ = vectorAction[2];

        var episodeTime = (float)this._episodeTime.Elapsed.TotalSeconds;

        var force = new Vector3(actionX, actionY, actionZ) * power;

        // 物理エンジン: 力を加える
        this._rigidbody.AddForce(force);

        // ベース地点からの移動距離を算出
        var distance = Vector3.Distance(this._rigidbody.position, this._basePosition);

        // // 評価: 報酬を与える (少ないエネルギー程高得点)
        // base.AddReward((power - Vector3.Distance(force, Vector3.zero)) / power);

        // リミット時間を判定
        if (limitTime < episodeTime)
        {
            // リセットして次のエピソードを開始
            base.EndEpisode();
        }

        // エリア外に移動した場合
        if (area < distance)
        {
            // // 評価: 報酬を減らす
            // base.AddReward(-1f);

            // リセットして次のエピソードを開始
            base.EndEpisode();
        }

        // ベース地点の範囲内に入った場合
        else if (distance < 0.5f)
        {
            // MEMO: 評価ポイント
            // 中心に位置に、素早く、少ないエネルギーで

            var score = (limitTime - episodeTime) * 0.1f;

            // 評価: 報酬を与える (ベース地点に近ければ高得点)
            base.AddReward(score);
        }

        // アフターバーナー表現
        {
            const float afterburnerSize = 4f;
            const float afterburnerOffset = 1f;

            var afterburnerFront = base.transform.Find("AfterburnerFront").GetComponent<ParticleSystem>().main;
            var afterburnerBack = base.transform.Find("AfterburnerBack").GetComponent<ParticleSystem>().main;
            var afterburnerTop = base.transform.Find("AfterburnerTop").GetComponent<ParticleSystem>().main;
            var afterburnerDown = base.transform.Find("AfterburnerDown").GetComponent<ParticleSystem>().main;
            var afterburnerRight = base.transform.Find("AfterburnerRight").GetComponent<ParticleSystem>().main;
            var afterburnerLeft = base.transform.Find("AfterburnerLeft").GetComponent<ParticleSystem>().main;

            if (0 < actionX)
            {
                afterburnerFront.startSize = afterburnerOffset;
                afterburnerBack.startSize = actionX * afterburnerSize + afterburnerOffset;
            }
            else
            {
                afterburnerFront.startSize = -actionX * afterburnerSize + afterburnerOffset;
                afterburnerBack.startSize = afterburnerOffset;
            }

            if (0 < actionY)
            {
                afterburnerTop.startSize = afterburnerOffset;
                afterburnerDown.startSize = actionY * afterburnerSize + afterburnerOffset;
            }
            else
            {
                afterburnerTop.startSize = -actionY * afterburnerSize + afterburnerOffset;
                afterburnerDown.startSize = afterburnerOffset;
            }

            if (0 < actionZ)
            {
                afterburnerRight.startSize = actionZ * afterburnerSize + afterburnerOffset;
                afterburnerLeft.startSize = afterburnerOffset;
            }
            else
            {
                afterburnerRight.startSize = afterburnerOffset;
                afterburnerLeft.startSize = -actionZ * afterburnerSize + afterburnerOffset;
            }
        }

        // UI更新
        // this.StatusText.text = $"distance: {distance}";
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this._rigidbody.position - this._basePosition);
        sensor.AddObservation(this._rigidbody.velocity);
        sensor.AddObservation((float)this._episodeTime.Elapsed.TotalSeconds);
    }

    public void Reset()
    {
        // リセットして次のエピソードを開始
        base.EndEpisode();
    }
}
