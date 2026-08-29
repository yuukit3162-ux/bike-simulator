using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class GameMnager : MonoBehaviour
{
    public static GameMnager Insector;
    public static violationType whatSin;//罪の種類
   
    public int stage = 0;
    public int Countwaypoints = 0;
    
    private float fadeoutTime = 2f;

    public bool Countpls = false;
    public bool PlayerReset = false;

    public string GameStatus = "start";
    
    
    public CanvasGroup canvas;//uiを非表示に
    public CanvasGroup UIsgroup;//UIs
    public Text title;
    public Text explain;
    public GameObject startbutton;
    public GameObject nextbutton;
    private int nextint = 3;
    private int nextCount = 0;

    public GameObject penalty;//反則金
    private Text penaltytext;
    private int penaltyint = 0;

    public CanvasGroup Danger;//違反時の表示
    public Text DangerText;
    private Dictionary<int, string> DangerDictionary;
    private Dictionary<violationType, int> violatDictionary;

    public GameObject Sun;//明るさ
    private float lightAngle = 50f;//時間 太陽の角度 50:朝 210:夜 0~360

    public CanvasGroup UIcanvas;//アルコール　反則金

    public CanvasGroup Risultgroup;
    public Text violationsNumber;
    public Text resultpenlty;
    public Text ClearTimeText;
    private float ClearTime;
    public Text Rank;//A,B,Cなどのやつ
    public Text RankText;//そのA,B,Cに対する説明
    public Text evaluation;//評価

    private bool FadeoutB = false;
    private float counttime = 0;
    public bool night = false;
    // UIを表示
    private void ShowUI()
    {
        Debug.Log("showUI");
        counttime = 0;
        Danger.alpha = 1f;
        //Danger.interactable = true;
        //Danger.blocksRaycasts = true;//いるか分からない
    }
    public GameObject[] waypoints;
    private void Awake()
    {
        Insector = this;
    }
    public enum violationType
    {
        none,//無し
        IgnoringTrafficLights,//信号無視
        RunningBackwards,//逆走(右側通行)
        smartphoneUse,//ながらスマホ
        OnTheSidewalk,//歩道走行
        FailStop, //一時不停止
        drunkDriving,//酒気帯び運転
        offLights //夜間無灯火
    }
    // Start is called before the first frame update
    void Start()
    {
        UIsgroup.alpha = 1;
        startbutton.SetActive(false);

        UIcanvas.alpha = 0;
        penaltytext = penalty.GetComponent<Text>();

        Risultgroup.alpha = 0;
        Danger.alpha = 0f;
        //Danger.interactable = false;
        //Danger.blocksRaycasts = false;//いるか分からない

        stage = 0;
        DangerDictionary = new Dictionary<int, string>
        {
            {0, "違反なし" },
            {1, "信号無視により\r\n反則金:6000円" },
            {2 ,"逆走により\r\n反則金:6000円" },
            {3 ,"ながらスマホにより\r\n反則金:12000円" },
            {4 ,"歩道走行により\r\n反則金:6000円" },
            {5 ,"一時不停止により\r\n反則金:5000円" },
            {6 ,"酒気帯び運転により\r\n罰金:500000円" },
            {7 ,"夜間無灯火により\r\n反則金:5000円" }
        };

        violatDictionary = new Dictionary<violationType, int>
        {
            {violationType.none, 0},
            {violationType.IgnoringTrafficLights, 6000},//反則金 できた
            {violationType.RunningBackwards, 6000},//反則金
            {violationType.smartphoneUse, 12000},//反則金　できた
            {violationType.OnTheSidewalk, 6000},//反則金
            {violationType.FailStop, 5000},//反則金
            {violationType.drunkDriving, 500000},//罰金 できた
            {violationType.offLights, 5000},//反則金　できた

        };
    }

    // Update is called once per frame
    void Update()
    {
        if(GameStatus == "play")
        {
            ClearTime += Time.deltaTime;
        }
        //Debug.Log(Countpls);
        if (stage == 0 && Countpls)
        {
            waypoints[Countwaypoints].SetActive(false);
            Countwaypoints++;
            //waypointの表示
            if (waypoints.Length - 1 < Countwaypoints)
            {
                finishgame();
            }
            else
            {
                waypoints[Countwaypoints].SetActive(true);
            }
            Countpls = false;
        }
        if(whatSin != violationType.none)
        {
            int Dnumber = (int)whatSin;
            int penalty = violatDictionary[whatSin];
            DangerText.text = DangerDictionary[Dnumber];
            guilty(penalty);
        }
        


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (lightAngle == 50f)
                lightAngle = 310f;
            else
                lightAngle = 50f;
            Sun.transform.rotation = Quaternion.Euler(lightAngle, -30f, 0f);
            DynamicGI.UpdateEnvironment();
            if (lightAngle == 50f)
            {
                night = false;
                WebSocketClient.webC.morning = true;
            }
            else
            {
                night = true;
                WebSocketClient.webC.morning = false;
            }
        }
    }
    private void guilty(int value)
    {
        //具体的な罪状
        ShowUI();

        //playerの位置をリセット
        PlayerReset = true;
        //総額を増やす
        penaltyint += value;
        penaltytext.text = "総額:" + penaltyint + "円";
        WebSocketClient.webC.Countviolations++;
        whatSin = violationType.none;
        if(FadeoutB == false)
        {
            StartCoroutine(Fadeout());
        }
    }
    IEnumerator Fadeout()
    {
        FadeoutB = true;
        yield return new WaitForSeconds(1);
        counttime = 0f;
        while (Danger.alpha > 0)
        {
            counttime += Time.deltaTime;
            float percentage = counttime / fadeoutTime;
            Danger.alpha = Mathf.Lerp(1f, 0f, percentage);
            yield return null;
        }
        FadeoutB = false;
        Debug.Log("非表示");
        Danger.alpha = 0f;
        //Danger.interactable = false;
        //Danger.blocksRaycasts = false;//いるか分からない
    }
    public void nextTo()//nextbutton
    {
        if(stage == 1)
        {
            Risultgroup.alpha = 0;
            nextbutton.SetActive(false);
            UIsgroup.alpha = 1;
            startbutton.SetActive(true);
        }
        if(stage == 0)
        {
            if (nextCount == 0)
            {
                explain.text = "自転車には青色と緑色があって\n" +
                    "青色:動きが遅いが倒れない、歩いているイメージ\n" +
                    "緑色:動きが早いが倒れる";
            }
            if (nextCount == 1)
            {
                explain.text = "自転車の色はspaceキーでの変更ができる\n" +
                    "リスポーン時は青色にできるよ\n" +
                    "それではstartボタンを\n" +
                    "押してね";
            }
            if (nextCount == 2)
            {
                title.text = "チュートリアル";
                explain.text = "みどりのチェックポイントを\n" +
                    "通っていってね\n" +
                    "赤色がゴールだよ\n" +
                    "信号に注意しよう";
            }
            nextCount++;
            if (nextint == nextCount)
            {
                nextbutton.SetActive(false);
                startbutton.SetActive(true);
            }
        }
    }
    public void startgame()//startbutton
    {
        //canvas.enabled = false;
        UIsgroup.alpha = 0;
        startbutton.SetActive(false);
        GameStatus = "play";
        Debug.Log(GameStatus);
        UIcanvas.alpha = 1;
        penaltytext.text = "総額:0円";
        Debug.Log("総額リセット");
        penaltyint = 0;
        Risultgroup.alpha = 0;
        if(stage == 0)
        {
            Countwaypoints = 0;
            waypoints[Countwaypoints].SetActive(true);
        }
    }
    void finishgame()
    {
        //できたリスト
        //player位置変更

        //やることリスト
        //stageの変化、地形や物体の移動
        //NPCの配置
        GameStatus = "finish";
        stage++;
        Debug.Log("finish");
        title.text = "stage" + stage;
        if (stage == 1)
        {
            explain.text = "車に注意しよう\n*車はすべて直進で進むよ*";

        }
        UIcanvas.alpha = 0;
        int V = WebSocketClient.webC.Countviolations;
        if (V == 0)
        {
            Rank.text = "A";
            RankText.text = "最高ランク";
            evaluation.text = "清廉潔白";
        }
        else if (V < 3)//1 2
        {
            Rank.text = "B";
            RankText.text = "ランク";
            evaluation.text = "微犯罪者";
        }
        else if (V < 5)//3 4
        {
            Rank.text = "C";
            RankText.text = "ランク";
            evaluation.text = "犯罪者予備軍";
        }
        else if (V < 7)//5 6
        {
            Rank.text = "D";
            RankText.text = "ランク";
            evaluation.text = "犯罪者";
        }
        else 
        {
            Rank.text = "E";
            RankText.text = "最低ランク";
            evaluation.text = "重罪人";
        }
        violationsNumber.text = "違反回数:" + V;
        WebSocketClient.webC.Countviolations = 0;
        resultpenlty.text = "総額:" + penaltyint + "円";
        ClearTimeText.text = "クリア時間:" + ClearTime;
        ClearTime = 0;
        Risultgroup.alpha = 1;
        nextbutton.SetActive(true);
    }
}
