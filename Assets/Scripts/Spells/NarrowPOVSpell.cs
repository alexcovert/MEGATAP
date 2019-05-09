using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NarrowPOVSpell : MonoBehaviour {
    [SerializeField] private float spellTime;
    [SerializeField] private float vignetteIntensity;
    [SerializeField] private float fadeTime;
    [SerializeField] private AudioClip inSound;
    [SerializeField] private AudioClip outSound;

    private bool started;

    private Camera bottomCam;

    private PostProcessVolume postProcess;
    private Vignette vignette = null;

    private SpellBase sb;
    private bool spellCast = true;

	private AudioSource audioSource;
    private void Start()
    {
    	audioSource = GetComponent<AudioSource>();
        sb = GetComponent<SpellBase>();
    }

    private void Update()
    {
        if (sb.SpellCast && spellCast)
        {
            bottomCam = GameObject.Find("Player 1 Camera").GetComponent<Camera>();
            postProcess = bottomCam.GetComponent<PostProcessVolume>();
            postProcess.profile.TryGetSettings(out vignette);

            if (!started && !vignette.active)
            {
                vignette.active = true;
                StartCoroutine(Blur());
                started = true;
            }
            //For now, make them not stack
            else if(vignette.active)
            {
                spellCast = false;
            }
            //StartCoroutine(StopBlur());
        }
    }

    private IEnumerator Blur()
    {

        //Fade in
        audioSource.PlayOneShot(inSound);
        for(float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            vignette.intensity.value = Mathf.Lerp(0f, vignetteIntensity, t / fadeTime);
            yield return null;
        }

        vignette.intensity.value = vignetteIntensity;
        //Spell duration
        for(float t = 0; t < spellTime; t += Time.deltaTime)
        {
            yield return null;
        }

        //Fade out
        audioSource.PlayOneShot(outSound);
        Debug.Log("Sound Started");
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            vignette.intensity.value = Mathf.Lerp(vignetteIntensity, 0f, t / fadeTime);
            yield return null;
        }
        vignette.intensity.value = 0;

        vignette.active = false;

    }
}
