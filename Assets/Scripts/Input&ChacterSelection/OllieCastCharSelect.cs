using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<ac>
//Script for Ollie to cast down traps on animation events in character select screen
public class OllieCastCharSelect : MonoBehaviour {

    [SerializeField] private GameObject trapPrefab1;
    [SerializeField] private Vector3 trapPos1;
    [SerializeField] private GameObject trapPrefab2;
    [SerializeField] private Vector3 trapPos2;

    private float trapStartHeight;

    [SerializeField] private float trapFallSpeed;

    [SerializeField] private float trapFadeTime;

    private GameObject trap1, trap2;

//    private float yVelocity;


    private void Start()
    {
        trapStartHeight = trapPos1.y + 5;
    }
    //Functions called from Ollie model animation events
    private void Cast1()
    {
        trap1 = Instantiate(trapPrefab1, new Vector3(trapPos1.x, trapStartHeight, trapPos1.z), trapPrefab1.transform.rotation);
        StartCoroutine(TrapFall(trap1, trapPos1));
    }

    private void Cast2()
    {
        trap2 = Instantiate(trapPrefab2, new Vector3(trapPos2.x, trapStartHeight, trapPos2.z), trapPrefab2.transform.rotation);
        StartCoroutine(TrapFall(trap2, trapPos2));
    }

    private void Restart()
    {
        if (trap1 != null) StartCoroutine(FadeTrap(trap1));
        if (trap2 != null) StartCoroutine(FadeTrap(trap2));
    }


    private IEnumerator TrapFall(GameObject trap, Vector3 targetPos)
    {
        float initHeight = trap.transform.position.y;

        for (float t = 0; t < trapFallSpeed; t += Time.deltaTime)
        {
            trap.transform.position = new Vector3(trap.transform.position.x,
                                                    Mathf.Lerp(initHeight, targetPos.y, t / trapFallSpeed),
                                                    trap.transform.position.z);

            yield return null;
        }

        trap.transform.position = new Vector3(trap.transform.position.x, targetPos.y, trap.transform.position.z);
    }

    private IEnumerator FadeTrap(GameObject trap)
    {
        MeshRenderer[] mrs = trap.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smrs = trap.GetComponentsInChildren<SkinnedMeshRenderer>();
        List<Material> mats = new List<Material>();
        
        //Looping over time
        for (float t = 0; t < trapFadeTime; t += Time.deltaTime)
        {
            //Looping over MeshRenderer & SkinnedMeshRenderer materials to change transparency
            foreach (MeshRenderer mr in mrs)
            {
                mr.GetMaterials(mats);
                foreach(Material mat in mats)
                {
                    Color color = mat.color;
                    color.a = Mathf.Lerp(1, 0, t / trapFadeTime);
                    mat.color = color;
                }
            }

            foreach(SkinnedMeshRenderer smr in smrs)
            {
                smr.GetMaterials(mats);
                foreach(Material mat in mats)
                {
                    Color color = mat.color;
                    color.a = Mathf.Lerp(1, 0, t / trapFadeTime);
                    mat.color = color;
                }
            }

            yield return null;
        }

        Destroy(trap);
    }
}
