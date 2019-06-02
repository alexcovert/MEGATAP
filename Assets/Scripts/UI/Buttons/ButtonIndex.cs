using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonIndex : MonoBehaviour {

    public int buttonIndex;

    public void ButtonIndexing(int index)
    {
        buttonIndex = index;
    }

    public int GetIndex()
    {
        return buttonIndex;
    }
}
