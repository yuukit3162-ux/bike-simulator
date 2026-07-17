using UnityEngine;
using NativeWebSocket;
using System.Text;
using System.Threading.Tasks;

public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    public static WebSocketClient webC;
    public float speedKel;
    public bool road_type;
    private string carwaytext;

    public bool move_inRight;
    private string move_inText;

    public int lights_on = 4;
    private string lights_onText;

    public int Webhand_sine = 4;
    private string handsineText;

    public bool usingSmartPhone;
    private string usingSmartPhoneText;

    public bool bikelight;
    private string bikeligthText;

    public bool morning = true;
    private string morningText;

    public int drunkint;
    public string drunkText;

    public int Countviolations;
    private void Awake()
    {
        webC = this;
    }
    async void Start()
    {
        websocket = new WebSocket("ws://127.0.0.1:8001");
        Debug.Log("move1");
        websocket.OnOpen += () =>
        {
            Debug.Log("接続成功");
            
        };
        Debug.Log("move2");
        websocket.OnMessage += (bytes) =>
        {
            var message = Encoding.UTF8.GetString(bytes);
            if(message != "")
            {
                Debug.LogWarning("受信: " + message);
                Countviolations++;
            }
        };
        websocket.OnError += (e) =>
        {
            Debug.Log("エラー: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("切断");
        };
        await Task.Yield(); // ←追加
        try
        {
            Debug.Log("Connect前");

            await websocket.Connect();

            Debug.Log("Connect後");

            Debug.Log("接続完了");
            Debug.Log("sousinn");

            await websocket.SendText("iiyokoiyo");
            Debug.Log("iiyokoiyo");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        Debug.Log("move3");
    }

    async void Update()
    {
        if (websocket != null &&
       websocket.State == WebSocketState.Open)
        {
            websocket.DispatchMessageQueue();
        }
        string speedtext = speedKel.ToString("F14");
        if (road_type)
            carwaytext = "0.0";
        else
            carwaytext = "1.0";

        if (move_inRight)
            move_inText = "2.0";
        else
            move_inText = "1.0";

        if (lights_on == 0)
            lights_onText = "0.0";
        if (lights_on == 1)
            lights_onText = "1.0";
        if (lights_on == 2)
            lights_onText = "2.0";
        if (lights_on == 4)
            lights_onText = "4.0";

        if (morning)
            morningText = "1.0";
        else
            morningText = "0.0";
        Debug.Log("web " + Webhand_sine);
        if (Webhand_sine == 1)
            handsineText = "1.0";
        else if (Webhand_sine == 2)
            handsineText = "2.0";
        else if (Webhand_sine == 4)
            handsineText = "4.0";
        else
            handsineText = "3.0";

        if (usingSmartPhone)
            usingSmartPhoneText = "1.0";
        else
            usingSmartPhoneText = "0.0";
        if (bikelight)
            bikeligthText = "1.0";
        else
            bikeligthText = "0.0";

        if (drunkint == 0)
            drunkText = "0.0";
        else if(drunkint > 0)
            drunkText = "1.0";
        else
        {
            drunkText = "2.0";
            Debug.Log("不明な酔いレベル");
        }
        Debug.Log("statesBefore");
        string csvPayload = string.Join("|",
            speedtext,                     // スピード
            carwaytext,                    // 道路タイプ (0.0: roadwork, 1.0: sidewalk, etc.)
            move_inText,                   // 移動方向 (1.0: left, 2.0: right)
            handsineText,                  // hand_sine (lest: 1.0, right: 2.0, none: 4.0)
            usingSmartPhoneText,           // using_phone (false: 0.0 true: 1.0)
            bikeligthText,                 // lights_on (false: 0.0 true: 1.0)
            "0.0",                         // breaks_functional (false: 0.0)
            "0.0",                         // passenger (false: 0.0)歩行者
            drunkText,                     // alcohol (false: 0.0 ture:1.0)アルコール
            lights_onText,                 // signal_state (0.0: red, 1.0: yellow, 2.0: blue, 4.0: null)
            morningText,                   // time_of_day (morning: 0.0)
            "0.0",                         // stop_sign_present (false: 0.0)
            "4.0",                         // crossing_gate (none: 4.0)
            "0.0"                          // pedestrians_nearby (false: 0.0)
        );
        Debug.Log(csvPayload);
        // 2. 正しい非同期メソッド（SendTextAsyncなど）で送信します
        // ※お使いのライブラリの仕様に合わせてメソッド名を変更してください
        await websocket.SendText(csvPayload);
        //"road_type": random.choice(["roadway", "sidewalk", "bike_lane", "crosswalk"]),
        //"move_in": random.choice(["right", "left"]),
        //"hand_sine": random.choice(["right", "left", "none"]),
        //"using_phone": bool(random.getrandbits(1)),
        //"lights_on": bool(random.getrandbits(1)),
        //"brakes_functional": bool(random.getrandbits(1)),
        //"passenger": bool(random.getrandbits(1)),
        //"alcohol": bool(random.getrandbits(1)),
        //"signal_state": random.choice(["red", "yellow", "blue", "none"]),
        //"time_of_day": random.choice(["night", "morning"]),
        //"stop_sign_present": bool(random.getrandbits(1)),
        //"crossing_gate": "none",
        //"pedestrians_nearby": bool(random.getrandbits(1))
        Debug.Log("statesAfter");
    }
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}