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
        const float limitTime = 100f; // リミット時間 (単位: 秒)

        var episodeTime = (float)this._episodeTime.Elapsed.TotalSeconds;

        var force = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]) * power;

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
