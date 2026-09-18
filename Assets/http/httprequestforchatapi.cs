using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class httprequestforchatapi : MonoBehaviour
{
    //https://qiita.com/g0e/items/9a4f886897fd46f107a8
    [Serializable]
    public class JsonSchema{
        }
    
    [Serializable]
    public class LlmMessage
    {
        public string role;
        public string content;
    }
    // 送信するJSONデータの構造体
    [System.Serializable]
    public class PostData
    {
        public string model;
        public List<LlmMessage> message;
        public bool stream;
        public JsonSchema format;
    }

    [System.Serializable]
    public class GetData
    {
        public LlmMessage message;
    }

    private GetData getdata;
        
    void Start()
    {
        // データの準備
        PostData data = new PostData {
            model="LFM2.5-1.2B-JP",
            message= new List<LlmMessage> 
                {
                    new LlmMessage { role = "system", content = "あなたのIDは1357です。" },
                    new LlmMessage { role = "assistant", content = "私はAIアシスタントです。" },
                    new LlmMessage { role = "user", content = "こんにちは！" }
                },
            stream = false
            };

        // コルーチンの開始
        StartCoroutine(PostJsonCoroutine("http://192.168.154.105:11434/api/chat", data));
    }

    IEnumerator PostJsonCoroutine(string url, PostData data)
    {
        string jsonString = JsonUtility.ToJson(data);
        // 1. リクエストの作成（空のWebRequestを作成し、設定を流し込む）
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            //byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonString));
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // 2. ヘッダーの追加（JSON送信に必須）
            request.SetRequestHeader("Content-Type", "application/json");

            // 3. 送信と待機
            yield return request.SendWebRequest();

            // 4. 結果の処理
            if (request.result == UnityWebRequest.Result.Success)
            {
                getdata = JsonUtility.FromJson<GetData>(request.downloadHandler.text);
                Debug.Log(getdata);
            }
            else
            {
                Debug.LogError("エラー発生: " + request.error);
            }
        }
    }
}
