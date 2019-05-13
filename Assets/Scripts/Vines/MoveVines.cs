using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVines : MonoBehaviour {
    [HideInInspector] public bool Started = false; //Whether the vines have started moving

    [SerializeField] private float vineStartSpeed;
    [SerializeField] private float speedIncreasePerFace;
    [SerializeField] private float vineMoveUpSpeed;
    [SerializeField] private float curvinessUpperLimit;

    //Positions of each individual vine in children
    private List<GameObject> vines;

    //Current position along tower
    private int face = 1, floor = 1;
    private int targetHeight;
    //Bools to help with moving the vines up
    private bool moveUp;                 //true while the vines are moving up
    private bool movedUpThisFloor;       //true if the vines have already moved up this floor (so they don't do it repeatedly)


    //floats to add randomness to vine movement
    private List<float> offset, multiplier;


    //speed that will increase as the vines progress
    private float speed;

	void Start () {
        //Get all of the children w/ trail renderers (each individual vine object)
        vines = new List<GameObject>();
        offset = new List<float>();
        multiplier = new List<float>();
		foreach(TrailRenderer t in GetComponentsInChildren<TrailRenderer>())
        {
            vines.Add(t.gameObject);
            offset.Add(Random.Range(0, 2 * Mathf.PI));
            multiplier.Add(Random.Range(1, curvinessUpperLimit));
        }

        //Initialize variables
        speed = vineStartSpeed;
        Started = false;
	}
	

	void Update () {
		if(Started)
        {
            for(int p = 0; p < vines.Count; p++)
            {
                MoveVine(vines[p], p);
                CheckRotate(vines[p]);
                CheckMoveUp(vines[p]);
            }
        }
	}


    //Move passed in vine position from left to right according to randomness variables & sine wave
    private void MoveVine(GameObject p, int num)
    {
        switch (face)
        {
            case 1:
                p.transform.position += new Vector3(speed, multiplier[num] * Mathf.Sin(p.transform.position.x + offset[num]), 0) * Time.deltaTime;
                break;
            case 2:
                p.transform.position += new Vector3(0, multiplier[num] * Mathf.Sin(p.transform.position.z + offset[num]), speed) * Time.deltaTime;
                break;
            case 3:
                p.transform.position -= new Vector3(speed, multiplier[num] * Mathf.Sin(p.transform.position.x + offset[num]), 0) * Time.deltaTime;
                break;
            case 4:
                p.transform.position -= new Vector3(0, multiplier[num] * Mathf.Sin(p.transform.position.z + offset[num]), speed) * Time.deltaTime;
                break;
        }

    }

    //Check if the vines should rotate to wrap around the tower
    private void CheckRotate(GameObject p)
    {
        //First to Second Face
        if (p.transform.position.x >= 43 && p.transform.position.z <= -40 && face == 1)
        {
            speed += speedIncreasePerFace;
            Rotate();
            face = 2;
            movedUpThisFloor = false;
        }
        //Second to Third Face
        if (p.transform.position.x >= 40 && p.transform.position.z >= 43 && face == 2)
        {
            speed += speedIncreasePerFace;
            Rotate();
            face = 3;
        }
        //Third to Fourth Face
        if (p.transform.position.x <= -43 && p.transform.position.z >= 40 && face == 3)
        {
            speed += speedIncreasePerFace;
            Rotate();
            face = 4;
        }
        //Fourth to First face -- Rotating
        if (p.transform.position.x <= -40 && p.transform.position.z <= -43 && face == 4 && floor < 5)
        {
            speed += speedIncreasePerFace;
            Rotate();
            floor++;
            face = 1;
        }
    }

    private void Rotate()
    {
        for(int v = 0; v < vines.Count; v++)
        {
            vines[v].transform.Rotate(Vector3.up, 90);
        }
    }

    //Check if the vines should start moving upwards
    private void CheckMoveUp(GameObject p)
    {
        if (p.transform.position.x <= -40 && p.transform.position.z <= -10 && p.transform.position.z >= -11 && face == 4 && floor < 5 && !movedUpThisFloor && !moveUp)
        {
            moveUp = true;
            targetHeight = (20 * floor) + 10;
        }
        if(p.transform.position.x <= -40 && p.transform.position.z <= -40 && face == 4 && floor == 5 && !movedUpThisFloor && !moveUp)
        {
            Started = false;
        }
        if (moveUp && !movedUpThisFloor)
        {
            transform.position += new Vector3(0, vineMoveUpSpeed, 0) * Time.deltaTime;
            
            if (transform.position.y >= targetHeight)
            {
                moveUp = false;
                movedUpThisFloor = true;
                vineMoveUpSpeed += 0.075f;
            }
        }
    }

    public int GetVineFace()
    {
        return face;
    }

    public int GetVineFloor()
    {
        return floor;
    }
}
