using UnityEngine;

public static class DebugEx
{
    public enum ShapeType
    {
        Circle,   // 丸
        Star,     // 星
        Square,    // 正方形
        Intersect
    }
    // どこからでも呼べる「点（十字）を描画する」関数
    public static void DrawPoint(Vector3 position, Color color, float size = 0.3f, float duration = 0f, ShapeType shape = ShapeType.Intersect)
    {
        switch (shape)
        {
            case ShapeType.Intersect:
                Debug.DrawLine(position - Vector3.right * size, position + Vector3.right * size, color, duration);
                Debug.DrawLine(position - Vector3.up * size, position + Vector3.up * size, color, duration);
                Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color, duration);
                break;
            case ShapeType.Star:
                float r = size * 0.5f;       // 中心から基準点までの距離
                float s = size * 0.75f;      // トゲの先端の長さ調整

                // 1. 中心の基準点 (6個)
                Vector3 top = position + Vector3.up * r;
                Vector3 bottom = position + Vector3.down * r;
                Vector3 left = position + Vector3.left * r;
                Vector3 right = position + Vector3.right * r;
                Vector3 forward = position + Vector3.forward * r;
                Vector3 back = position + Vector3.back * r;

                // 2. トゲの先端 (8個)
                Vector3 t1 = position + new Vector3(1, 1, 1).normalized * s;
                Vector3 t2 = position + new Vector3(-1, 1, 1).normalized * s;
                Vector3 t3 = position + new Vector3(1, 1, -1).normalized * s;
                Vector3 t4 = position + new Vector3(-1, 1, -1).normalized * s;
                Vector3 t5 = position + new Vector3(1, -1, 1).normalized * s;
                Vector3 t6 = position + new Vector3(-1, -1, 1).normalized * s;
                Vector3 t7 = position + new Vector3(1, -1, -1).normalized * s;
                Vector3 t8 = position + new Vector3(-1, -1, -1).normalized * s;

                // 3. トゲの先端から各基準点へ線を引く
                // 上半分のトゲ
                Debug.DrawLine(t1, right, color, duration); Debug.DrawLine(t1, top, color , duration); Debug.DrawLine(t1, forward, color, duration);
                Debug.DrawLine(t2, left, color ,duration); Debug.DrawLine(t2, top, color ,duration); Debug.DrawLine(t2, forward, color ,duration);
                Debug.DrawLine(t3, right, color ,duration); Debug.DrawLine(t3, top, color ,duration); Debug.DrawLine(t3, back, color ,duration);
                Debug.DrawLine(t4, left, color ,duration); Debug.DrawLine(t4, top, color ,duration); Debug.DrawLine(t4, back, color ,duration);

                // 下半分のトゲ
                Debug.DrawLine(t5, right, color ,duration); Debug.DrawLine(t5, bottom, color ,duration); Debug.DrawLine(t5, forward, color ,duration);
                Debug.DrawLine(t6, left, color ,duration); Debug.DrawLine(t6, bottom, color ,duration); Debug.DrawLine(t6, forward, color ,duration);
                Debug.DrawLine(t7, right, color ,duration); Debug.DrawLine(t7, bottom, color ,duration); Debug.DrawLine(t7, back, color ,duration);
                Debug.DrawLine(t8, left, color ,duration); Debug.DrawLine(t8, bottom, color ,duration); Debug.DrawLine(t8, back, color ,duration);
                break;
            case ShapeType.Circle:
                int segments = 16;
                for (int i = 0; i < segments; i++)
                {
                    float angle1 = (float)i / segments * Mathf.PI * 2;
                    float angle2 = (float)(i + 1) / segments * Mathf.PI * 2;

                    // XZ平面（横方向の円）
                    Vector3 p1_xz = position + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * size;
                    Vector3 p2_xz = position + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * size;
                    Debug.DrawLine(p1_xz, p2_xz, color, duration);

                    // XY平面（正面の円）
                    Vector3 p1_xy = position + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * size;
                    Vector3 p2_xy = position + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * size;
                    Debug.DrawLine(p1_xy, p2_xy, color, duration);

                    // YZ平面（側面の円）
                    Vector3 p1_yz = position + new Vector3(0, Mathf.Sin(angle1), Mathf.Cos(angle1)) * size;
                    Vector3 p2_yz = position + new Vector3(0, Mathf.Sin(angle2), Mathf.Cos(angle2)) * size;
                    Debug.DrawLine(p1_yz, p2_yz, color, duration);
                }

                break;
            case ShapeType.Square:
                float half = size / 2f;

                // 4つの頂点を計算 (XY平面の場合)
                Vector3 topLeft = position + new Vector3(-half, half, 0);
                Vector3 topRight = position + new Vector3(half, half, 0);
                Vector3 bottomLeft = position + new Vector3(-half, -half, 0);
                Vector3 bottomRight = position + new Vector3(half, -half, 0);

                // 4つの辺を描画
                Debug.DrawLine(topLeft, topRight, color ,duration);
                Debug.DrawLine(topRight, bottomRight, color ,duration);
                Debug.DrawLine(bottomRight, bottomLeft, color ,duration);
                Debug.DrawLine(bottomLeft, topLeft, color ,duration);
                break;

        }
    }
    
}

