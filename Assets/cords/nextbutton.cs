using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextbutton : MonoBehaviour
{
    public void next()
    {
        Debug.Log("next");
        GameMnager.Insector.nextTo();
    }
}
