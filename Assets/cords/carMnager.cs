using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class carMnager : MonoBehaviour
{
    public GameObject[] games;
    [SerializeField] private GameObject[] spownpoint;
    [SerializeField] private GameObject car;
    public HashSet<GameObject> cars = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }
    IEnumerator RandomActionRoutine()
    {
        while (true)
        {
            // 例：1秒〜5秒の間でランダムな時間を待つ
            float waitTime = Random.Range(1f, 10f);
            yield return new WaitForSeconds(waitTime);

            // ここに実行したい処理を書く
            spown();
        }
    }
    void spown()
    {
        Vector3 pos=spownpoint[Random.Range(0, spownpoint.Length)].position;
        Vector3 target=spownpoint[Random.Range(0, spownpoint.Length)].position;
        GameObject NPC_Object;
        if (cars.Count <= 0)
        {
            NPC_Object = Instantiate(car,pos,Quaternion.identity);
        }
        else
        {
            NPC_Object = items.First();
            items.Remove(NPC_Object);
            NPC_Object.position=pos;
            NPC_Object.SetActive=true;
        }
        NPC_Nav code=NPC_Object.GetComponent<NPC_Nav>();
        code.target=target;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
