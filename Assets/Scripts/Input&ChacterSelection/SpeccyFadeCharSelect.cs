using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeccyFadeCharSelect : MonoBehaviour {
    [SerializeField] private float speccyFadeTime;

    private SkinnedMeshRenderer[] smrs;



    void Start()
    {
        smrs = GetComponentsInChildren<SkinnedMeshRenderer>();

    }

    public void FadeIn()
    {
        //StartCoroutine(FadeSpeccy(true));
        foreach (SkinnedMeshRenderer smr in smrs)
        {
            smr.enabled = true;
        }
    }

    public void FadeOut()
    {
        foreach (SkinnedMeshRenderer smr in smrs)
        {
            smr.enabled = false;
        }
        //StartCoroutine(FadeSpeccy(false));
    }
    
    private IEnumerator FadeSpeccy(bool fadeIn)
    {
        List<Material> mats = new List<Material>();

        //Looping over time
        for (float t = 0; t < speccyFadeTime; t += Time.deltaTime)
        {
            //Looping over SkinnedMeshRenderer materials to change transparency

            foreach (SkinnedMeshRenderer smr in smrs)
            {
                smr.GetMaterials(mats);
                foreach (Material mat in mats)
                {
                    Color color = mat.color;

                    if (fadeIn)
                    {
                        color.a = Mathf.Lerp(0, 1, t / speccyFadeTime);
                    }
                    else
                    {
                        color.a = Mathf.Lerp(1, 0, t / speccyFadeTime);
                    }


                    mat.color = color;

                    Debug.Log(mat.name);
                }
            }

            yield return null;
        }


        foreach (SkinnedMeshRenderer smr in smrs)
        {
            smr.GetMaterials(mats);
            foreach (Material mat in mats)
            {
                Color color = mat.color;

                if (fadeIn)
                {
                    color.a = 1;
                }
                else
                {
                    color.a = 0;
                }


                mat.color = color;

                Debug.Log(mat.name);
            }
        }


    }
}
