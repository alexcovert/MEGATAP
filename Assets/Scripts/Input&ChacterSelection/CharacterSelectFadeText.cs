using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectFadeText : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI p1Text;
    [SerializeField] private Image p1SelectImage;
    [SerializeField] private Image p1CancelImage;

    [SerializeField] private TextMeshProUGUI p2Text;
    [SerializeField] private Image p2SelectImage;
    [SerializeField] private Image p2CancelImage;

    [SerializeField] private float fadeTime = 1.5f;
    private bool textFaded = false;
    private bool spriteFaded = false;

	// Update is called once per frame
	void Update ()
    {
        if (!textFaded)
        {
            StartCoroutine(FadeText());
            textFaded = true;
        }
        if (!spriteFaded)
        {
            StartCoroutine(FadeSprite());
            spriteFaded = true;
        }
    }

    private IEnumerator FadeSprite()
    {
        //Fade out select
        for(float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1SelectImage.color = new Color(p1SelectImage.color.r, p1SelectImage.color.g, p1SelectImage.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            p2SelectImage.color = new Color(p2SelectImage.color.r, p2SelectImage.color.g, p2SelectImage.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            yield return null;
        }

        //Fade in cancel
        for(float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1CancelImage.color = new Color(p1CancelImage.color.r, p1CancelImage.color.g, p1CancelImage.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            p2CancelImage.color = new Color(p2CancelImage.color.r, p2CancelImage.color.g, p2CancelImage.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            yield return null;
        }

        //Fade out cancel
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1CancelImage.color = new Color(p1CancelImage.color.r, p1CancelImage.color.g, p1CancelImage.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            p2CancelImage.color = new Color(p2CancelImage.color.r, p2CancelImage.color.g, p2CancelImage.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            yield return null;
        }

        //Fade in select
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1SelectImage.color = new Color(p1SelectImage.color.r, p1SelectImage.color.g, p1SelectImage.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            p2SelectImage.color = new Color(p2SelectImage.color.r, p2SelectImage.color.g, p2SelectImage.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            yield return null;
        }

        spriteFaded = false;
    }

    private IEnumerator FadeText()
    {
        //Fade Out
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1Text.color = new Color(p1Text.color.r, p1Text.color.g, p1Text.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            p2Text.color = new Color(p2Text.color.r, p2Text.color.g, p2Text.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            yield return null;
        }
        p1Text.text = " to cancel!";
        p2Text.text = " to cancel!";
        
        //Fade In
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1Text.color = new Color(p1Text.color.r, p1Text.color.g, p1Text.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            p2Text.color = new Color(p2Text.color.r, p2Text.color.g, p2Text.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            yield return null;
        }

        //Fade Out
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1Text.color = new Color(p1Text.color.r, p1Text.color.g, p1Text.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            p2Text.color = new Color(p2Text.color.r, p2Text.color.g, p2Text.color.b, Mathf.Lerp(1, 0, t / fadeTime));
            yield return null;
        }
        p1Text.text = "to select!";
        p2Text.text = "to select!";

        //Fade In
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            p1Text.color = new Color(p1Text.color.r, p1Text.color.g, p1Text.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            p2Text.color = new Color(p2Text.color.r, p2Text.color.g, p2Text.color.b, Mathf.Lerp(0, 1, t / fadeTime));
            yield return null;
        }

        textFaded = false;
    }
}
