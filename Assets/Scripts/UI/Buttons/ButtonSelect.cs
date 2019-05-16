using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;// Required when using Event data.
using TMPro;

//Alex
//Invoke on click when button is selected
public class ButtonSelect : MonoBehaviour, ISelectHandler, IDeselectHandler// required interface when using the OnSelect & OnDeselect methods.
{
    [SerializeField] private bool isThisTrap;
    
    private CastSpell cs;
    private PlaceTrap pt;
    private Image controllerCursor;
    private TextMeshProUGUI tooltipText;
    private GameObject tooltipBox;
    private MoveControllerCursor cursorMove;
    private GameObject currentFirstSpell;
    private GameObject currentLastTrap;

    private AudioSource audioSource;
    private Vector3 buttonScale;
    EventSystem es;

    private InputManager inputManager;

    private CheckControllers cc;
    private IEnumerator fader;
    Color tooltipColor;
    Color textColor;


    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        cc = inputManager.GetComponent<CheckControllers>();

    }

    private void Start()
    {
        GameObject player = GameObject.Find("Player 2");
        audioSource = GetComponentInParent<AudioSource>();
        buttonScale = new Vector3(0.75f, 0.75f, 0.75f);
        TextMeshProUGUI[] tooltips = transform.parent.parent.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI t in tooltips)
        {
            if(t.name == "Tooltip")
            {
                tooltipText = t;
                tooltipBox = tooltipText.transform.parent.gameObject;
                tooltipText.transform.SetAsLastSibling();


                if(!inputManager.GetComponent<CheckControllers>().topPlayersController)
                {
                    tooltipBox.GetComponent<Image>().enabled = false;
                }
            }
        }
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        cs = player.GetComponent<CastSpell>();
        pt = player.GetComponent<PlaceTrap>();
        controllerCursor = GameObject.Find("ControllerCursor").GetComponent<Image>();
        cursorMove = player.GetComponent<MoveControllerCursor>();

        if (es.currentSelectedGameObject != null && this.gameObject.Equals(pt.queue[0].gameObject))
        {
            ChangeTooltip(es.currentSelectedGameObject.name, es.currentSelectedGameObject);
        }

        tooltipColor = tooltipBox.GetComponent<Image>().color;
        textColor = new Color(1, 1, 1, 1);
    }

    public void Update()
    {
        if (es.currentSelectedGameObject == null && cc.topPlayersController)
        {
            tooltipBox.GetComponent<Image>().enabled = false;
            tooltipText.text = "";
        }
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if (audioSource != null) audioSource.Play();
        if (isThisTrap)
        {
            GetCurrentLastTrap();
            if (inputManager.GetAxis(InputCommand.TopPlayerMenu) < 0 || IsSpellQueueNull())
            {
                if(cs != null) cs.DestroyTarget();
                if (currentLastTrap != null && currentLastTrap.gameObject == this.gameObject && currentLastTrap.gameObject.GetComponent<Button>().interactable)
                {
                    currentLastTrap = null;
                    controllerCursor.transform.localPosition = new Vector3(0, 130);
                    cursorMove.MovingTraps = true;

                }
            }
            if (cursorMove != null) cursorMove.MovingTraps = true;
        }
        else
        {
            GetCurrentFirstSpell();
            if (inputManager.GetAxis(InputCommand.TopPlayerMenu) > 0 || IsTrapQueueNull())
            {
                if (pt != null) pt.DestroyGhost();
                if (currentFirstSpell != null && currentFirstSpell.gameObject == this.gameObject)
                {
                    controllerCursor.transform.localPosition = new Vector3(0, -130);
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
        if (!inputManager.GetComponent<CheckControllers>().topPlayersController)
        {
            tooltipBox.GetComponent<Image>().enabled = false;
            tooltipText.text = "";
        }
        else
        {
            if (fader != null)
            {
                StopCoroutine(fader);
            }
        }
    }

    public void ScaleUp()
    {
        if(this.GetComponent<Button>().interactable) this.transform.localScale *= 1.35f;
        ChangeTooltip(this.name, this.gameObject);
    }


    private void ChangeTooltip(string toCheck, GameObject button)
    {
        
        if(tooltipText != null)
        {

            //Debug.Log(fader);
            if (fader != null)
            {
                StopCoroutine(fader);
                fader = null;
            }
            tooltipBox.GetComponent<Image>().color = new Color(tooltipColor.r, tooltipColor.g, tooltipColor.b, 1);
            tooltipText.color = new Color(textColor.r, textColor.g, textColor.b, 1);

            tooltipBox.GetComponent<Image>().enabled = true;
            Vector3 tooltipPosition = tooltipBox.transform.position;

            tooltipBox.transform.position = new Vector3(button.transform.position.x, tooltipPosition.y, 0);
            RectTransform tooltipTransform = tooltipBox.GetComponent<RectTransform>();
            switch (toCheck)
            {
                //Spells
                case "Blur Spell Button(Clone)":
                    tooltipText.text = "Blur";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                case "Gust Spell Button(Clone)":
                    tooltipText.text = "Wind";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                case "Lightning Spell Button(Clone)":
                    tooltipText.text = "Lightning";
                    tooltipTransform.sizeDelta = new Vector2(100, tooltipTransform.rect.height);
                    break;
                case "NarrowPOV Spell Button(Clone)":
                    tooltipText.text = "Blind";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                case "Slow Spell Button(Clone)":
                    tooltipText.text = "Slow";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                case "Stun Spell Button(Clone)":
                    tooltipText.text = "Petrify";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                //Traps
                case "ArrowButton(Clone)":
                    tooltipText.text = "Crossbow";
                    tooltipTransform.sizeDelta = new Vector2(100, tooltipTransform.rect.height);
                    break;
                case "BananaButton(Clone)":
                    tooltipText.text = "Banana";
                    tooltipTransform.sizeDelta = new Vector2(85, tooltipTransform.rect.height);
                    break;
                case "SapButton(Clone)":
                    tooltipText.text = "Sap";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                case "SpikeButton(Clone)":
                    tooltipText.text = "Spike";
                    tooltipTransform.sizeDelta = new Vector2(75, tooltipTransform.rect.height);
                    break;
                case "LogRollerButton(Clone)":
                    tooltipText.text = "Log Roller";
                    tooltipTransform.sizeDelta = new Vector2(120, tooltipTransform.rect.height);
                    break;

            }
            if (cc.topPlayersController)
            {
                fader = Fade();
                StartCoroutine(fader);
            }
        }
    }

    private IEnumerator Fade()
    {
        float time = 3;

        for(float t = 0; t < time; t += Time.deltaTime)
        {
            tooltipBox.GetComponent<Image>().color = new Color(tooltipColor.r, tooltipColor.g, tooltipColor.b, Mathf.Lerp(1, 0, t / time));
            tooltipText.color = new Color(textColor.r, textColor.g, textColor.b, Mathf.Lerp(1, 0, t / time));
            yield return null;
        }

        tooltipBox.GetComponent<Image>().color = new Color(tooltipColor.r, tooltipColor.g, tooltipColor.b, 0);
        tooltipText.color = new Color(textColor.r, textColor.g, textColor.b, 0);
        fader = null;
    }

    private void GetCurrentLastTrap()
    {
        if (pt != null)
        {
            //foreach (GameObject t in pt.queue)
            //{
            //    if (t != null && t.activeInHierarchy && t.GetComponent<Button>().interactable)
            //    {
            //        currentFirstTrap = t;
            //        return;
            //    }
            //}

            for(int t = pt.queue.Count - 1; t >= 0; t--)
            {
                if (pt.queue[t] != null && pt.queue[t].activeInHierarchy && pt.queue[t].GetComponent<Button>().interactable)
                {
                    currentLastTrap = pt.queue[t];
                    return;
                }
            }
        }
    }

    private void GetCurrentFirstSpell()
    {
        if (cs != null)
        {
            //for (int s = cs.queue.Length - 1; s >= 0; s--)
            //{
            //    if (cs.queue[s] != null && cs.queue[s].activeInHierarchy && cs.queue[s].GetComponent<Button>().interactable)
            //    {
            //        currentLastSpell = cs.queue[s];
            //        return;
            //    }
            //}

            for(int s = 0; s < cs.queue.Length; s++)
            {
                if (cs.queue[s] != null && cs.queue[s].activeInHierarchy && cs.queue[s].GetComponent<Button>().interactable)
                {
                    currentFirstSpell = cs.queue[s];
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
                if (cs.queue[s] != null && cs.queue[s].activeInHierarchy && cs.queue[s].GetComponent<Button>().interactable) 
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
                if (pt.queue[s] != null && pt.queue[s].activeInHierarchy && pt.queue[s].GetComponent<Button>().interactable)
                {
                    return false;
                }
            }
        }

        return true;
    }
}