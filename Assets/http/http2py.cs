// これはテスト的な内容です、実際にこれが採用されるかどうかは不明です

using System;
using System.Threading.Tasks;
using UnityEngine;
using LLama.Common;
using LLama;

public class LlamaChatManager : MonoBehaviour
{
    // 💡 ダウンロードしたGGUFモデルファイルのフルパスを指定してください
    [SerializeField] private string modelPath = @"C:\Users\g254303\AI-Models\llm-jp-3-1.8b-instruct-Q4_K_M.gguf";
    [SerializeField] private int contextSize = 2048;
    [SerializeField] private int gpuLayers = 0; // CPUのみの場合は0。GPUを使う場合は枚数やレイヤー数（例: 20）を指定

    private LLamaWeights _model;
    private LLamaContext _context;
    private InteractiveExecutor _executor;
    private ChatSession _session;
    private bool canuseAI = false;
    async void Start()
    {
        Debug.Log("[LLama] モデルの読み込みを開始します...（数秒かかります）");

        // バックグラウンドスレッドで重いモデル読み込み処理を実行
        await Task.Run(() =>
        {
            try
            {
                var modelParams = new ModelParams(modelPath)
                {
                    ContextSize = (uint)contextSize,
                    GpuLayerCount = gpuLayers
                };

                _model = LLamaWeights.LoadFromFile(modelParams);
                _context = _model.CreateContext(modelParams);
                _executor = new InteractiveExecutor(_context);
                _session = new ChatSession(_executor);
            }
            catch (Exception e)
            {
                Debug.LogError($"[LLama] 初期化エラー: {e.Message}");
            }
        });

        if (_session != null)
        {
            Debug.Log("[LLama] モデルの読み込みが完了しました！");
            // テスト質問の実行
            canuseAI=true;
        }
    }

    public bool putdata(string data)
    {
        //今日はここまで
        return true;
    }

    private void OnDestroy()
    {
        // メモリリークを防ぐため、ゲーム終了時にC++側のメモリを確実に解放する
        _context?.Dispose();
        _model?.Dispose();
        Debug.Log("[LLama] リソースを解放しました。");
    }
}
