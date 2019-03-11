using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;// Required when using Event data.

//Alex
//Invoke on click when button is selected
public class OnSelectButton : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    [SerializeField] private bool isThisTrap;

    private TextMeshProUGUI tooltip;
    private CastSpell cs;
    private PlaceTrap pt;
    private Image controllerCursor;
    private MoveControllerCursor cursorMove;
    private GameObject currentLastSpell;
    private GameObject currentFirstTrap;

    private AudioSource audioSource;

    private void Start()
    {
        GameObject player = GameObject.Find("Player 2");
        tooltip = GameObject.Find("Tooltip").GetComponent<TextMeshProUGUI>();
        audioSource = GetComponentInParent<AudioSource>();
        cs = player.GetComponent<CastSpell>();
        pt = player.GetComponent<PlaceTrap>();
        controllerCursor = GameObject.Find("ControllerCursor").GetComponent<Image>();
        cursorMove = player.GetComponent<MoveControllerCursor>();

        tooltip.transform.SetAsLastSibling();
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if(audioSource != null) audioSource.Play();
        if (tooltip != null) SetTooltip();

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

        GetComponent<Button>().onClick.Invoke();
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

    private void SetTooltip()
    {
        Debug.Log(this.name);
        switch(this.name)
        {
            //Traps
            case "ArrowButton(Clone)":
                tooltip.text = "Arrow Shooter (Stun)";
                break;
            case "BananaButton(Clone)":
                tooltip.text = "Banana (Slip)";
                break;
            case "SapButton(Clone)":
                tooltip.text = "Sap (Slow)";
                break;
            case "SpikesButton(Clone)":
                tooltip.text = "Spikes (Knockback)";
                break;
            //Spells
            case "Blur Spell Button(Clone)":
                tooltip.text = "Blur";
                break;
            case "Gust Spell Button(Clone)":
                tooltip.text = "Wind";
                break;
            case "Lightning Spell Button(Clone)":
                tooltip.text = "Lightning";
                break;
            case "NarrowPOV Spell Button(Clone)":
                tooltip.text = "Decrease Vision";
                break;
            case "Stun Spell Button(Clone)":
                tooltip.text = "Petrify";
                break;
            case "Slow Spell Button(Clone)":
                tooltip.text = "Slow";
                break;
        }
        tooltip.transform.SetAsLastSibling();
    }
}