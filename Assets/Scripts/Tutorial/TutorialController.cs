using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{

    // two serparate lists for player tips
    [SerializeField] private int timeBetweenTips; // in seconds
    [SerializeField] private List<GameObject> p1tips;
    [SerializeField] private List<GameObject> p2tips;


    // Use this for initialization
    void Start()
    {
        // set all of them to invisible, for both lists
        foreach (GameObject tip in p1tips)
        {
            //tip.GetComponent<CanvasRenderer>().SetAlpha(0f);
            //tip.transform.GetChild(0).gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
            tip.SetActive(false);
        }
        foreach (GameObject tip in p2tips)
        {
            //tip.GetComponent<CanvasRenderer>().SetAlpha(0f);
            //tip.transform.GetChild(0).gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
            tip.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TOP PLAYER
        // spawn a tip every timeBetweenTips seconds
        if ((int)Time.timeSinceLevelLoad % timeBetweenTips == 0 && (int)Time.timeSinceLevelLoad / timeBetweenTips < p1tips.Count)
        {
            GameObject tip = p1tips[(int)Time.timeSinceLevelLoad / timeBetweenTips];
            //tip.GetComponent<CanvasRenderer>().SetAlpha(1f);
            //tip.transform.GetChild(0).gameObject.GetComponent<CanvasRenderer>().SetAlpha(1f);
            tip.SetActive(true);
        }
        // remove the previous tip every timeBetweenTips seconds
        if ((int)Time.timeSinceLevelLoad % timeBetweenTips == 0 && (int)Time.timeSinceLevelLoad / timeBetweenTips > 0 && (int)Time.timeSinceLevelLoad / timeBetweenTips < p1tips.Count + 1)
        {
            GameObject tip = p1tips[((int)Time.timeSinceLevelLoad / timeBetweenTips) - 1];
            //tip.GetComponent<CanvasRenderer>().SetAlpha(0f);
            //tip.transform.GetChild(0).gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
            tip.SetActive(false);
        }

        // BOTTOM PLAYER
        // spawn a tip ever timeBetweenTips seconds
        if ((int)Time.timeSinceLevelLoad % timeBetweenTips == 0 && (int)Time.timeSinceLevelLoad / timeBetweenTips < p2tips.Count)
        {
            GameObject tip = p2tips[(int)Time.timeSinceLevelLoad / timeBetweenTips];
            //tip.GetComponent<CanvasRenderer>().SetAlpha(1f);
            //tip.transform.GetChild(0).gameObject.GetComponent<CanvasRenderer>().SetAlpha(1f);
            tip.SetActive(true);
        }
        // remove the previous tip every timeBetweenTips seconds
        if ((int)Time.timeSinceLevelLoad % timeBetweenTips == 0 && (int)Time.timeSinceLevelLoad / timeBetweenTips > 0 && (int)Time.timeSinceLevelLoad / timeBetweenTips < p2tips.Count + 1)
        {
            GameObject tip = p2tips[((int)Time.timeSinceLevelLoad / timeBetweenTips) - 1];
            //tip.GetComponent<CanvasRenderer>().SetAlpha(0f);
            //tip.transform.GetChild(0).gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
            tip.SetActive(false);
        }


    }
}
