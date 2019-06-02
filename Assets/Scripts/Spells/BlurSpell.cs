using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BlurSpell : MonoBehaviour {
    [SerializeField] private float spellTime;
    [SerializeField] [Range (0, 2)] private float blurIntensity;
    [SerializeField] private float fadeTime;
    [SerializeField] private AudioClip inSound;
    [SerializeField] private AudioClip outSound;

    private Camera bottomCam;

    private PostProcessVolume postProcess;
    private DepthOfField blur = null;

    private SpellBase sb;
    private bool spellCast = true;
    private bool started;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sb = GetComponent<SpellBase>();
    }

    private void Update()
    {
        if(sb.SpellCast && spellCast)
        {
            bottomCam = GameObject.Find("Player 1 Camera").GetComponent<Camera>();
            postProcess = bottomCam.GetComponent<PostProcessVolume>();
            postProcess.profile.TryGetSettings(out blur);


            if (!started && !blur.active)
            {
                blur.active = true;
                StartCoroutine(Blur());
                started = true;
            }
            //For now, make them not stack
            else if (blur.active)
            {
                spellCast = false;
            }
        }
    }

    private IEnumerator Blur()
    {
        spellCast = false;
        float targetBlur = 2 - blurIntensity;
        //Fade in
        audioSource.PlayOneShot(inSound);
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            blur.aperture.value = Mathf.Lerp(2f, targetBlur, t / fadeTime);
            yield return null;
        }

        blur.aperture.value = targetBlur;
        //Spell duration
        for (float t = 0; t < spellTime; t += Time.deltaTime)
        {
            yield return null;
        }

        //Fade out
        audioSource.PlayOneShot(outSound);
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            blur.aperture.value = Mathf.Lerp(targetBlur, 2f, t / fadeTime);
            yield return null;
        }
        blur.aperture.value = 2;

        blur.active = false;

        Destroy(this.gameObject, 5);

    }
}
