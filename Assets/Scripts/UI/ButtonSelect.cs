﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

//Alex
//Invoke on click when button is selected
public class ButtonSelect : MonoBehaviour, ISelectHandler, IDeselectHandler// required interface when using the OnSelect & OnDeselect methods.
{
    [SerializeField] private bool isThisTrap;
    
    private CastSpell cs;
    private PlaceTrap pt;
    private Image controllerCursor;
    private MoveControllerCursor cursorMove;
    private GameObject currentLastSpell;
    private GameObject currentFirstTrap;

    private AudioSource audioSource;
    private Vector3 buttonScale;

    private void Start()
    {
        GameObject player = GameObject.Find("Player 2");
        audioSource = GetComponentInParent<AudioSource>();
        cs = player.GetComponent<CastSpell>();
        pt = player.GetComponent<PlaceTrap>();
        controllerCursor = GameObject.Find("ControllerCursor").GetComponent<Image>();
        cursorMove = player.GetComponent<MoveControllerCursor>();
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if(audioSource != null) audioSource.Play();
        if (isThisTrap)
        {
            GetCurrentFirstTrap();
            if (Input.GetAxis("Horizontal_Menu") > 0 || IsSpellQueueNull())
            {
                if(cs != null) cs.DestroyTarget();
                if (currentFirstTrap != null && currentFirstTrap.gameObject == this.gameObject)
                {
                    currentFirstTrap = null;
                    controllerCursor.transform.localPosition = new Vector3(0, 130);
                    cursorMove.MovingTraps = true;

                }
            }
            if (cursorMove != null) cursorMove.MovingTraps = true;
        }
        else
        {
            GetCurrentLastSpell();
            if (Input.GetAxis("Horizontal_Menu") < 0 || IsTrapQueueNull())
            {
                if(pt != null) pt.DestroyGhost();
                if (currentLastSpell != null && currentLastSpell.gameObject == this.gameObject)
                {
                    controllerCursor.transform.localPosition = new Vector3(0, -100);
                    cursorMove.MovingTraps = false;
                }
            }
            if (cursorMove != null) cursorMove.MovingTraps = false;
        }

        //Button Scaling
        ScaleUp();

        GetComponent<Button>().onClick.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.transform.localScale = buttonScale;
    }

    public void ScaleUp()
    {
        buttonScale = this.transform.localScale;
        this.transform.localScale *= 1.35f;
    }

    private void GetCurrentFirstTrap()
    {
        if (pt != null)
        {
            foreach (GameObject t in pt.queue)
            {
                if (t != null && t.activeInHierarchy)
                {
                    currentFirstTrap = t;
                    return;
                }
            }
        }
    }

    private void GetCurrentLastSpell()
    {
        if (cs != null)
        {
            for (int s = cs.queue.Length - 1; s >= 0; s--)
            {
                if (cs.queue[s] != null && cs.queue[s].activeInHierarchy)
                {
                    currentLastSpell = cs.queue[s];
                    return;
                }
            }
        }
    }

    private bool IsTrapQueueNull()
    {
        if(pt!= null)
        {
            for (int s = cs.queue.Length - 1; s >= 0; s--)
            {
                if (cs.queue[s] != null && cs.queue[s].activeInHierarchy)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsSpellQueueNull()
    {
        if (cs != null)
        {
            for (int s = pt.queue.Count - 1; s >= 0; s--)
            {
                if (pt.queue[s] != null && pt.queue[s].activeInHierarchy)
                {
                    return false;
                }
            }
        }

        return true;
    }
}