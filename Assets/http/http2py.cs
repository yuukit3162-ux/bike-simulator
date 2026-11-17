// これはテスト的な内容です、実際にこれが採用されるかどうかは不明です

using System;
using System.Threading.Tasks;
using UnityEngine;
using LLama.Common;
using LLama;
using LLama.Batched;
using LLama.Native;
using System.Runtime.InteropServices;

public class LlamaChatManager : MonoBehaviour
{
    // 💡 ダウンロードしたGGUFモデルファイルのフルパスを指定してください
    [SerializeField] private string modelPath = @"C:\Users\g254303\AI-Models\llm-jp-3-1.8b-instruct-Q4_K_M.gguf";
    [SerializeField] private int contextSize = 2048;
    [SerializeField] private int gpuLayers = 0; // CPUのみの場合は0。GPUを使う場合は枚数やレイヤー数（例: 20）を指定

    private LLamaWeights _model;
    private LLamaContext _context;
    private BatchedExecutor executor;
    private DefaultSamplingPipeline sampler;
    private StreamingTokenDecoder decoder;
    private LLama.Batched.Conversation conversation;
    private bool canuseAI = false;
    private LLamaToken[] baseTokens;
    private int start;
    public List<LLamaToken> DATA;

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
                executor = new BatchedExecutor(_model, modelParams);
                sampler = new DefaultSamplingPipeline();
                decoder = new StreamingTokenDecoder(executor.Context);
                conversation = executor.Create();
                baseTokens = executor.Context.Tokenize("あなたは優秀なアシスタントです。\n", addBos: true, special: true);
                conversation.Prompt(baseTokens);
                await executor.Infer()
                start = baseTokens.Length;


            }
            catch (Exception e)
            {
                Debug.LogError($"[LLama] 初期化エラー: {e.Message}");
            }
        });

        if (_model != null)
        {
            Debug.Log("[LLama] モデルの読み込みが完了しました！");
            // テスト質問の実行
            canuseAI=true;
        }
    }

    public async Task<List<LLamaToken>> putdata()
    {
        if(!canuseAI) return null;
        LLamaToken nextToken;
        int back;
        List<LLamaToken> tokenHistory = new List<LLamaToken>();
        do{
            List<LLamaToken> data=Volatile.Read(ref DATA);
            back = data.Count;
            conversation.Prompt(CollectionsMarshal.AsSpan(data));
            await executor.Infer();
            nextToken = sampler.Sample(executor.Context.NativeHandle, conversation.Sample(), Array.Empty<LLamaToken>());
            conversation.Rewind(back);
            conversation.Prompt(nextToken);
            tokenHistory.Add(nextToken);
        }while(!model.Vocab.IsEOG(nextToken));
        await executor.Infer();
        Conversation.Rewind(conversation.TokenCount-start);
        return tokenHistory;
    }

    private void OnDestroy()
    {
        // メモリリークを防ぐため、ゲーム終了時にC++側のメモリを確実に解放する
        _model?.Dispose();
        Debug.Log("[LLama] リソースを解放しました。");
    }
}
