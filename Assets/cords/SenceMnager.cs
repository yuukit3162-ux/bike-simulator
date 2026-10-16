using UnityEngine;
using UnityEngine.SceneManagement;
public static class SenceEx
{ 
    public static void next(int delta=1)
    {
        //保護機能はナイデスww
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + delta);
    }

}