using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class PlacePickups : MonoBehaviour
{
    //Change to Use
    private static int NumFloors = 5;
    private static int NumPickUpsRegular = 2;
    private static int NumPickUpsExtra = 3;
    private static int NumPickUpsFloor = 2;
    private static int ChanceMorePickUps = 25;

    private static int NumFaces = 4;
    private static int index = 0;
    private static GameObject item = GetAtPath();

    private static int NumPickUpsTotal = 0;

    [MenuItem("GameObject/Generate Pickups", false, 15)]

    // Use this for initialization
    private static void Create()
    {
        Debug.Log("Placing Pickups");
        List<GameObject> faces = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(faces);
        int random = Random.Range(1, 100);
        if(random <= ChanceMorePickUps)
        {
            NumPickUpsFloor = NumPickUpsExtra;
        }
        else
        {
            NumPickUpsFloor = NumPickUpsRegular;
        }
        index = 0;
        Transform[] child;

        if (faces != null)
        {
            for (int i = 1; i <= NumFloors * NumFaces; i++)
            {
                if (faces[i - 1] != null)
                {
                    child = faces[i - 1].GetComponentsInChildren<Transform>();
                    foreach (Transform r in child)
                    {
                        if (r.name == "Placeholder Pickup")
                        {
                            if (r.transform.childCount == 0 && index != NumPickUpsFloor)
                            {
                                int num = Random.Range(1, 10);
                                if (num <= 3)
                                {
                                    GameObject pickup = Instantiate(item, new Vector3(r.position.x, r.position.y, r.position.z), Quaternion.identity);
                                    pickup.transform.parent = r.transform;
                                    index++;
                                    NumPickUpsTotal++;
                                }
                            }
                        }
                    }
                }
                if (i % 4 == 0 && index != NumPickUpsFloor)
                {
                    if (i != 0)
                    {
                        i -= 4;
                    }
                }
                if (i % 4 == 0 && index == NumPickUpsFloor)
                {
                    if (i != 0)
                    {
                        Debug.Log("PickUps this floor("+ i/4 + "): " + index);
                        index = 0;
                        random = Random.Range(1, 100);
                        if (random <= ChanceMorePickUps)
                        {
                            NumPickUpsFloor = NumPickUpsExtra;
                        }
                        else
                        {
                            NumPickUpsFloor = NumPickUpsRegular;
                        }
                    }
                }

            }
        }
        index = 0;
        faces.Clear();
        Debug.Log("Total Number of Pickups Generated: " + NumPickUpsTotal);
        NumPickUpsTotal = 0;
    }
    public static GameObject GetAtPath()
    {

        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + "Prefabs/Pickups");

        foreach (string fileName in fileEntries)
        {
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);

            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));

            if (t != null)
                al.Add(t);
        }
        GameObject temp = (GameObject)al[0];

        return temp;
    }
}
