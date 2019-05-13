using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTrees : MonoBehaviour {
    private Transform[] trees;


	private void Start ()
    {
        trees = GetComponentsInChildren<Transform>();

        for(int t = 1; t < trees.Length; t++)
        {
            float randomHeight = Random.Range(1, 1.2f);
            float randomRot = Random.Range(0, 360);

            trees[t].eulerAngles = new Vector3(trees[t].eulerAngles.x, randomRot, trees[t].eulerAngles.z);
            trees[t].localScale = new Vector3(trees[t].localScale.x, randomHeight, trees[t].localScale.z);
            //TODO: Randomize "Group Seed" of branches? 
        }
    }

}
