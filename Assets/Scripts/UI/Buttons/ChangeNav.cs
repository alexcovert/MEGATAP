using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNav : MonoBehaviour {
    private CastSpell cs;
    private PlaceTrap pt;

    private GameObject currentFirstButton;
    private GameObject currentSecondButton;
    private GameObject currentLastButton;
    private GameObject currentSecondLastButton;

	void Start ()
    {
        GameObject player = GameObject.Find("Player 2");
        cs = player.GetComponent<CastSpell>();
        pt = player.GetComponent<PlaceTrap>();
    }
	
    public void ResetNav()
    {
        GetCurrentFirstButtons();
        GetCurrentLastButtons();

        //Debug.Log("\n" + currentFirstButton);
        //Debug.Log("\n" + currentSecondButton);
        //Debug.Log("\n" + currentSecondLastButton);
        //Debug.Log("\n" + currentLastButton);

        //Change navigation of first and last buttons to wrap around

        if (currentFirstButton != null)
        {
            Navigation navFirst = currentFirstButton.GetComponent<Button>().navigation;
            navFirst.mode = Navigation.Mode.Explicit;
            if (currentLastButton != null) navFirst.selectOnLeft = currentLastButton.GetComponent<Button>();
            if (currentSecondButton != null) navFirst.selectOnRight = currentSecondButton.GetComponent<Button>();
            currentFirstButton.GetComponent<Button>().navigation = navFirst;
        }

        if (currentLastButton != null)
        {
            Navigation navLast = currentLastButton.GetComponent<Button>().navigation;
            navLast.mode = Navigation.Mode.Explicit;
            if (currentSecondLastButton != null) navLast.selectOnLeft = currentSecondLastButton.GetComponent<Button>();
            if (currentFirstButton != null) navLast.selectOnRight = currentFirstButton.GetComponent<Button>();
            currentLastButton.GetComponent<Button>().navigation = navLast;
        }

        //Reset to null
        currentFirstButton = null;
        currentSecondButton = null;
        currentSecondLastButton = null;
        currentLastButton = null;
    }


    private void GetCurrentFirstButtons()
    {

            foreach (GameObject t in pt.queue)
            {
                if (t != null && t.activeInHierarchy && t.GetComponent<Button>().interactable)
                {
                    if (currentFirstButton == null)
                    {
                        currentFirstButton = t;
                    }
                    else
                    {
                        currentSecondButton = t;
                        return;
                    }
                }
            }

            for (int s = 0; s < cs.queue.Length; s++)
            {
                if (cs.queue[s] != null && cs.queue[s].activeInHierarchy && cs.queue[s].GetComponent<Button>().interactable)
                {
                    if (currentFirstButton == null)
                    {
                        currentFirstButton = cs.queue[s];
                    }
                    else
                    {
                        currentSecondButton = cs.queue[s];
                        return;
                    }
                }
            }
        
    }

    private void GetCurrentLastButtons()
    {
        for (int s = cs.queue.Length - 1; s >= 0; s--)
        {
            if (cs.queue[s] != null && cs.queue[s].activeInHierarchy && cs.queue[s].GetComponent<Button>().interactable)
            {
                if(currentLastButton == null)
                {
                    currentLastButton = cs.queue[s];

                    if(s > 0 && cs.queue[s - 1] != null)
                    {
                        currentSecondLastButton = cs.queue[s - 1];
                        return;
                    }
                }
            }
        }

        for(int t = pt.queue.Count - 1; t >= 0; t--)
        {
            if (pt.queue[t] != null && pt.queue[t].activeInHierarchy && pt.queue[t].GetComponent<Button>().interactable)
            {
                if(currentLastButton == null)
                {
                    currentLastButton = pt.queue[t];
                    if(t > 0 && pt.queue[t - 1] != null)
                    {
                        currentSecondLastButton = pt.queue[t - 1];
                        return;
                    }
                }
            }
        }
    }



}
