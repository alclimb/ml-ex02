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
        // 位置を初期化
        this._rigidbody.position = Vector3.zero + this._basePosition;
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

        // 物理エンジン: 力を加える
        this._rigidbody.AddForce(new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]) * power);

        // ベース地点からの移動距離を算出
        var distance = Vector3.Distance(this._rigidbody.position, this._basePosition);

        // エリア外に移動した場合
        if (area < distance)
        {
            // 評価: 報酬を減らす
            base.AddReward(-1f);

            // リセットして次のエピソードを開始
            base.EndEpisode();
        }
        else
        {
            // 評価: 報酬を与える (ベース地点に近ければ高得点)
            base.AddReward(area - distance);
        }

        // UI更新
        // this.StatusText.text = $"distance: {distance}";
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this._rigidbody.position - this._basePosition);
        // sensor.AddObservation(this._rigidbody.velocity);
        // sensor.AddObservation((float)this._episodeTime.Elapsed.TotalSeconds);
    }
}
