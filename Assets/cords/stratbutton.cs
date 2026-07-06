using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class stratbutton : MonoBehaviour
{
    public void Clickstart()
    {
        SceneManager.LoadScene("bike");
    }
}
