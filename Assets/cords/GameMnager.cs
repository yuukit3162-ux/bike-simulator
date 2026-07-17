using System.Collections;
using System.Collections.Generic;
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
    public CanvasGroup UIsgroup;
    public Text title;
    public Text explain;
    public GameObject startbutton;
    public GameObject nextbutton;
    private int nextint = 3;
    private int nextCount = 0;

    public GameObject penalty;//罰金
    private Text penaltytext;
    private int penaltyint = 0;

    public CanvasGroup Danger;//違反時の表示

    private float daytime = 50;//時間 太陽の角度 50:朝 210:夜 0~360
    public GameObject Sun;//明るさ

    public CanvasGroup UIcanvas;

    public CanvasGroup Risultgroup;
    public Text violationsNumber;
    public Text resultpenlty;
    public Text ClearTimeText;
    private float ClearTime;
    public Text Rank;//A,B,Cなどのやつ
    public Text RankText;//そのA,B,Cに対する説明
    public Text evaluation;//評価
    // UIを表示
    private void ShowUI()
    {
        Debug.Log("showUI");
        Danger.alpha = 1f;
        Danger.interactable = true;
        Danger.blocksRaycasts = true;
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
        RunningBackwards//逆走

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
        Danger.interactable = false;
        Danger.blocksRaycasts = false;

        stage = 0;
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
        if (whatSin == violationType.IgnoringTrafficLights)//信号無視 6000円
        {  
            //Debug.Log("信号無視 6000円 今は切っている");
            whatSin = violationType.none;
            guilty(6000);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Sun.transform.Rotate(Vector3.right * 180f);
            DynamicGI.UpdateEnvironment();
            if(Sun.transform.eulerAngles.x == 50f)
            {
                WebSocketClient.webC.morning = true;
            }
            else
            {
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
        //罰金を増やす
        penaltyint += value;
        penaltytext.text = "罰金:" + penaltyint + "円";
        whatSin = violationType.none;
        StartCoroutine(Fadeout());
    }
    IEnumerator Fadeout()
    {
        yield return new WaitForSeconds(1);
        float counttime = 0f;
        while (Danger.alpha > 0)
        {
            counttime += Time.deltaTime;
            float percentage = counttime / fadeoutTime;
            Danger.alpha = Mathf.Lerp(1f, 0f, percentage);
            yield return null;
        }
        Debug.Log("非表示");
        Danger.alpha = 0f;
        Danger.interactable = false;
        Danger.blocksRaycasts = false;
    }
    public void nextTo()
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
    public void startgame()
    {
        //canvas.enabled = false;
        UIsgroup.alpha = 0;
        GameStatus = "play";
        Debug.Log(GameStatus);
        UIcanvas.alpha = 1;
        penaltytext.text = "罰金:0円";
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
        UIsgroup.alpha = 1;
        UIcanvas.alpha = 0;
        int V = WebSocketClient.webC.Countviolations;
        if (V == 0)
        {
            Rank.text = "A";
            RankText.text = "最高ランク";
            evaluation.text = "清廉潔白";
        }
        else if (V < 3)
        {
            Rank.text = "B";
            RankText.text = "ランク";
            evaluation.text = "微犯罪者";
        }
        else if (V < 5)
        {
            Rank.text = "C";
            RankText.text = "ランク";
            evaluation.text = "犯罪者予備軍";
        }
        else if (V < 7)
        {
            Rank.text = "D";
            RankText.text = "ランク";
            evaluation.text = "犯罪者";
        }
        else if (V < 9)
        {
            Rank.text = "E";
            RankText.text = "最低ランク";
            evaluation.text = "重罪人";
        }
        violationsNumber.text = "違反回数:" + V;
        WebSocketClient.webC.Countviolations = 0;
        resultpenlty.text = "罰金" + penaltyint;
        ClearTimeText.text = "クリア時間:" + ClearTime;
        ClearTime = 0;
        Risultgroup.alpha = 1;
    }
}
