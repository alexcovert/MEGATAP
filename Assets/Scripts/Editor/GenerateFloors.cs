using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//<alex>
public class GenerateFloors {
    //Change these to generate a different # of floors.
    private static int floorHeight = 20;
    public static int NumFloors = 5;


    [MenuItem("GameObject/Instantiate Floors", false, 11)]
    private static void Create()
    {
        GameObject previouslyInstantiated = GameObject.Find("ProceduralFloorsPrefab");
        if (previouslyInstantiated == null)
        {
            GameObject tower = GameObject.Find("Tower");
            tower.GetComponent<NumberOfFloors>().NumFloors = NumFloors;
            for (int floor = 0; floor < NumFloors; floor++)
            {
                GameObject newFloor = PrefabUtility.InstantiatePrefab(Resources.Load("ProceduralFloorsPrefab")) as GameObject;
                newFloor.transform.position = new Vector3(0, floorHeight * floor, 0);
                newFloor.transform.SetParent(tower.transform);

                if(floor == NumFloors - 1)
                {
                    //Generate Win Trigger
                    GameObject winTrigger = PrefabUtility.InstantiatePrefab(Resources.Load("WinTrigger")) as GameObject;
                    winTrigger.transform.position = new Vector3(winTrigger.transform.position.x, floorHeight * floor + 30, winTrigger.transform.position.z);
                    winTrigger.transform.SetParent(tower.transform);

                    //Make boundaries taller
                    Transform[] transforms = newFloor.GetComponentsInChildren<Transform>();
                    List<GameObject> boundaries = new List<GameObject>();
                    foreach(Transform t in transforms)
                    {
                        for(int i = 1; i <= 4; i++)
                        {
                            if(t.name == ("Boundary" + i))
                            {
                                boundaries.Add(t.gameObject);
                            }
                        }
                    }

                    for(int i = 0; i < boundaries.Count; i++)
                    {
                        boundaries[i].transform.localScale = new Vector3(boundaries[i].transform.localScale.x, 100, boundaries[i].transform.localScale.z);
                        boundaries[i].transform.localPosition = boundaries[i].transform.localPosition + new Vector3(0, 50, 0);

                    }
                }
            }
        }
        else
        {
            Debug.Log("Delete previously instantiated floors & previously instantiated win trigger from Tower parent game object first.");
        }
    }
}
