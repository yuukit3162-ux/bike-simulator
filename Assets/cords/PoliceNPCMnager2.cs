//一応書いたけどほぼAI製だし動くかわからんから試した時の結果だけ残してほしいかも...
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPCMnager2 : MonoBehaviour
{
    Rigidbody my_Rigidbody;
    public Rigidbody Player;
    public float speed = 10f;
    private float rotateSpeed = 3;
    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        my_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var control = InterceptController.CalculateControl(
            my_Rigidbody,
            Player,
            20f,
            speed,
            speed*0.5f,
            rotateSpeed);
        my_Rigidbody.AddForce(transform.forward * Mathf.Clamp(control.accel, -speed * 0.5f, speed));
        my_Rigidbody.AddTorque(transform.up * Mathf.Clamp(control.turnAccel, -rotateSpeed, rotateSpeed));
    }
}

public static class InterceptController//ありがとうAIくん！
{
    public struct Control
    {
        public float accel;      // +前進 -後退
        public float turnAccel;  // -左 +右
    }

    public static Control CalculateControl(
        Rigidbody self,
        Rigidbody target,
        float maxSpeed,
        float maxForwardAccel,
        float maxReverseAccel,
        float maxTurnAccel)
    {
        Control best = new Control();
        float bestCost = float.MaxValue;

        const float dt = 0.1f;
        const int horizon = 15; // 予測時間を少し短くして精度向上 (1.5秒先)

        // 候補を少し増やして滑らかに（または動的に数を変えても良い）
        float[] accelCandidates = { -maxReverseAccel, 0, maxForwardAccel * 0.5f, maxForwardAccel };
        float[] turnCandidates = { -maxTurnAccel, -maxTurnAccel * 0.5f, -maxTurnAccel * 0.2f, 0, maxTurnAccel * 0.2f, maxTurnAccel * 0.5f, maxTurnAccel };

        // 現在の物理状態を取得
        Vector3 currentPos = self.position;
        Vector3 currentVel = self.velocity;
        Quaternion currentRot = self.rotation;
        
        // ターゲットの予測位置（線形予測）
        float predictTime = horizon * dt;
        Vector3 targetPos = target.position + target.velocity * predictTime;

        foreach (float accel in accelCandidates)
        {
            foreach (float turn in turnCandidates)
            {
                Vector3 pos = currentPos;
                Vector3 vel = currentVel;
                Quaternion rot = currentRot;

                // 簡易的な慣性・摩擦のシミュレーション（オブジェクトの特性に合わせて調整）
                for (int i = 0; i < horizon; i++)
                {
                    // 旋回（現在の向きを更新）
                    rot *= Quaternion.Euler(0, turn * Mathf.Rad2Deg * dt, 0);

                    // 前進・後退の力を加える
                    Vector3 forward = rot * Vector3.forward;
                    vel += forward * accel * dt;

                    // 簡易的な空気抵抗・速度制限
                    if (vel.magnitude > maxSpeed)
                    {
                        vel = vel.normalized * maxSpeed;
                    }

                    pos += vel * dt;
                }

                // コスト計算
                float distance = Vector3.Distance(pos, targetPos);
                
                // .normalized を省略して高速化
                float headingError = Vector3.Angle(rot * Vector3.forward, targetPos - pos);

                float cost = distance 
                           + headingError * 0.08f // 向きの重要度を少しアップ
                           + Mathf.Abs(turn) * 0.05f; // 無駄な旋回をより抑制

                // 後退ペナルティ
                if (Vector3.Dot(vel, rot * Vector3.forward) < 0)
                    cost += 5.0f;

                if (cost < bestCost)
                {
                    bestCost = cost;
                    best.accel = accel;
                    best.turnAccel = turn;
                }
            }
        }

        return best;
    }
}
