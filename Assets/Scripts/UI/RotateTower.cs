using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RotateTower : MonoBehaviour {

    [SerializeField] private EventSystem es;

    private int position = 17;
    private int newPosition = 17;
    private bool once = false;

    //-15.5, -3.7, 23.4, 0,0,0
    // Use this for initialization
    void Start() {


    }

    // Update is called once per frame
    void Update() {
        if (newPosition < position)
        {
            if (once == false)
            {
                once = true;
                if ((position - 1) % 4 == 0)
                {
                    StartCoroutine(SpinDownLeft());
                }
                else
                {
                    StartCoroutine(SpinLeft());
                }
            }

        }

        if (newPosition > position)
        {
            if (once == false)
            {
                once = true;
                if (position % 4 == 0)
                {
                    StartCoroutine(SpinUpRight());
                }
                else
                {
                    StartCoroutine(SpinRight());
                }
            }

        }
    }

    public void OnPointerEnterPlay()
    {
        newPosition = 17;
    }

    public void OnPointerEnterTutorial()
    {
        newPosition = 11;
    }

    public void OnPointerEnterCredits()
    {
        newPosition = 6;
    }

    public void OnPointerEnterQuit()
    {
        newPosition = 1;
    }

    private IEnumerator SpinLeft()
    {
        float time = 0;
        while (time <= 1f) {
            time += Time.deltaTime;
            transform.Rotate(new Vector3(0f, -90f, 0f) * Time.deltaTime);
            yield return null;
        }
        position--;
        if (position == 17 || position == 1)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (position == 11)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (position == 6)
        {
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        once = false;
    }

    private IEnumerator SpinDownLeft()
    {
        float time = 0;
        while (time <= 1f)
        {
            time += Time.deltaTime;
            transform.Rotate(new Vector3(0f, -90f, 0f) * Time.deltaTime);
            transform.position += new Vector3(0, 20f * Time.deltaTime, 0);
            yield return null;
        }
        position--;
        if (position == 17 || position == 1)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (position == 11)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (position == 6)
        {
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        once = false;
    }

    private IEnumerator SpinRight()
    {
        float time = 0;
        while (time <= 1f)
        {
            time += Time.deltaTime;
            transform.Rotate(new Vector3(0f, 90f, 0f) * Time.deltaTime);
            yield return null;
        }
        position++;
        if (position == 17 || position == 1)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (position == 11)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (position == 6)
        {
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        once = false;
    }

    private IEnumerator SpinUpRight()
    {
        float time = 0;
        while (time <= 1f)
        {
            time += Time.deltaTime;
            transform.Rotate(new Vector3(0f, 90f, 0f) * Time.deltaTime);
            transform.position += new Vector3(0, -20f * Time.deltaTime, 0);
            yield return null;
        }
        position++;
        if (position == 17 || position == 1)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (position == 11)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (position == 6)
        {
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        once = false;
    }
}
