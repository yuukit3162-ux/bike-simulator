using UnityEngine;

public class traficlightMnager : MonoBehaviour
{
    public Material[] material;
    public int traficColor = 1;//1 = 赤　2 = 黄色　3 = 青
    public int traficColor2 = 6;//4 = 赤 5 = 黄色 6 = 青
    //private float timered = 30f;
    private float timerred = 14;//0~14赤
    private float timerblue = 22;//14~22青
    private float timeryellow = 24;//22~24黄色
    //private float timerred2 = 24;//12~24赤
    private float timerredshrot2 = 2;//0~2赤
    private float timerblue2 = 10;//2~10青
    private float timeryellow2 = 12;//10~12黄色
    private float counter = 0;
    public bool first;
    // Start is called before the first frame update
    void Start()
    {
        material[0].color = Color.red;//発光もここで管理したい
        material[0].SetColor("_EmissionColor", Color.red);
        material[1].color = Color.gray;
        material[1].SetColor("_EmissionColor", Color.black);
        material[2].color = Color.gray;
        material[2].SetColor("_EmissionColor", Color.black);

        material[3].color = Color.gray;
        material[3].SetColor("_EmissionColor", Color.black);
        material[4].color = Color.gray;
        material[4].SetColor("_EmissionColor", Color.black);
        material[5].color = Color.blue;
        material[5].SetColor("_EmissionColor", Color.blue);
    }
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        //信号のサイクルは1~3分のことが多い
        if (timerblue < counter)
            yellowlight();//3
        else if (timerred < counter)
            bulelight();//2
        else
            redlight();//1
        if (timeryellow < counter)
            counter = 0;
        //反対の信号の作り途中
        if (timeryellow2 < counter)
            redlight2();
        else if (timerblue2 < counter)
            yellowlight2();
        else if (timerredshrot2 < counter)
            bulelight2();
        else
            redlight2();

    }
    void redlight()
    {
        material[traficColor - 1].color = Color.gray;
        material[traficColor - 1].SetColor("_EmissionColor", Color.black);
        traficColor = 1;
        material[traficColor - 1].color = Color.red;
        material[traficColor - 1].SetColor("_EmissionColor", Color.red);

    }
    void yellowlight()
    {
        material[traficColor - 1].color = Color.gray;
        material[traficColor - 1].SetColor("_EmissionColor", Color.black);
        traficColor = 2;
        material[traficColor - 1].color = Color.yellow;
        material[traficColor - 1].SetColor("_EmissionColor", Color.yellow);
    }
    void bulelight()
    {
        material[traficColor - 1].color = Color.gray;
        material[traficColor - 1].SetColor("_EmissionColor", Color.black);
        traficColor = 3;
        material[traficColor - 1].color = Color.blue;
        material[traficColor - 1].SetColor("_EmissionColor", Color.blue);
    }
    void redlight2()
    {
        material[traficColor2 - 1].color = Color.gray;
        material[traficColor2 - 1].SetColor("_EmissionColor", Color.black);
        traficColor2 = 4;
        material[traficColor2 - 1].color = Color.red;
        material[traficColor2 - 1].SetColor("_EmissionColor", Color.red);

    }
    void yellowlight2()
    {
        material[traficColor2 - 1].color = Color.gray;
        material[traficColor2 - 1].SetColor("_EmissionColor", Color.black);
        traficColor2 = 5;
        material[traficColor2 - 1].color = Color.yellow;
        material[traficColor2 - 1].SetColor("_EmissionColor", Color.yellow);
    }
    void bulelight2()
    {
        material[traficColor2 - 1].color = Color.gray;
        material[traficColor2 - 1].SetColor("_EmissionColor", Color.black);
        traficColor2 = 6;
        material[traficColor2 - 1].color = Color.blue;
        material[traficColor2 - 1].SetColor("_EmissionColor", Color.blue);
    }
}
