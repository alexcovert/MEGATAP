using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreBoundaries : MonoBehaviour {
    [SerializeField] private float timeToDie = 4f;

    // Use this for initialization
    void Start () {
        Physics.IgnoreLayerCollision(2, 19);
	}
	
	// Update is called once per frame
	void Update () {
		
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(this.transform.parent.gameObject);
    }
}
