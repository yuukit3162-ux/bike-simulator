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
    public GameObject UI;
    public Text title;
    public Text explain;
    public GameObject startbutton;
    public GameObject nextbutton;
    private int nextint = 2;
    private int nextCount = 0;

    public GameObject penalty;//罰金
    private Text penaltytext;
    private int penaltyint = 0;

    public CanvasGroup Danger;//違反時の表示
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
        //canvas.enabled = true;//仮置き
        UI.SetActive(true);
        startbutton.SetActive(false);

        penalty.SetActive(false);
        penaltytext = penalty.GetComponent<Text>();

        Danger.alpha = 0f;
        Danger.interactable = false;
        Danger.blocksRaycasts = false;

        stage = 0;
    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("信号無視 6000円");
            //guilty(6000);
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
            explain.text = "自転車の色はspaceキーでの変更ができます\n" +
                "リスポーン時は青色になります\n" +
                "それではstartボタンを教えてください";
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
        UI.SetActive(false);
        GameStatus = "play";
        Debug.Log(GameStatus);
        penalty.SetActive(true);
        penaltytext.text = "罰金:0円";
        penaltyint = 0;

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
        UI.SetActive(true);
        penalty.SetActive(false);
    }
}
