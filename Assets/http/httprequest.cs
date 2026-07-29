using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JsonPostExample : MonoBehaviour
{

    // 送信するJSONデータの構造体
    [System.Serializable]
    public class PostData
    {
        public string model;
        public string prompt;
        public bool stream;
        public list<int> context;
    }

    [System.Serializable]
    public class GetData
    {
        public string model;
        public string created_at;
        public string response;
        public bool done;
        public string done_reason;
        public list<int> context;
        public int total_duration;
        public int load_duration;
        public int prompt_eval_count;
        public int prompt_eval_duration;
        public int eval_count;
        public int eval_duration;
    }

    private GetData getdata;
        
    void Start()
    {
        // データの準備
        PostData data = new PostData {model="LFM2.5-1.2B-JP", prompt="はろーわーるど", stream=False ,context =  new List<int>()};

        // コルーチンの開始
        StartCoroutine(PostJsonCoroutine("http://192.168.154.105:11434/api/generate", data));
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
