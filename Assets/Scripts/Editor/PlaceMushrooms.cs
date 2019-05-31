using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class PlaceMushrooms : MonoBehaviour {

    //Change to Use
    private static int NumFloors = 5;

    private static int NumFaces = 4;
    private static List<GameObject> Mushrooms = new List<GameObject>();

    private static int NumMushroomsTotal = 0;

    [MenuItem("GameObject/Generate Mushrooms", false, 17)]

    // Use this for initialization
    private static void Create()
    {
        Debug.Log("Placing Mushrooms");
        List<GameObject> faces = new List<GameObject>();
        GetAtPath(Mushrooms);
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(faces);

        Transform[] child;

        GameObject mushrooms = new GameObject("Mushrooms");

        if (faces != null)
        {
            for (int i = 1; i <= NumFloors * NumFaces; i++)
            {
                if (faces[i - 1] != null)
                {
                    child = faces[i - 1].GetComponentsInChildren<Transform>();
                    foreach (Transform r in child)
                    {
                        if (r.name == "HorizontalPlatform")
                        {
                            if (r.transform.childCount == 0)
                            {
                                int num = Random.Range(1, 100);
                                if (num <= 10)
                                {
                                    int random = Random.Range(0, Mushrooms.Count);
                                    if (random == 3 || random == 1 || random == 2)
                                    {
                                        if (random == 1)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 1.1f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.identity);
                                            mushroom.transform.Rotate(0, Random.Range(0, 360), 0);
                                            mushroom.transform.parent = mushrooms.transform;

                                        }
                                        else if(random == 2)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 1.4f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.identity);
                                            mushroom.transform.Rotate(0, Random.Range(0, 360), 0);
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        else if(random == 3)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 1.15f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.identity);
                                            mushroom.transform.Rotate(0, Random.Range(0, 360), 0);
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        GameObject empty = new GameObject("Empty");
                                        empty.transform.parent = r.transform;
                                    }
                                    else
                                    {
                                        if (random == 0)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 1.15f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.Euler(new Vector3(-90f, 0, 0)));
                                            mushroom.transform.Rotate(0, 0, Random.Range(0, 360));
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        else if(random == 4)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 0.95f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.Euler(new Vector3(-90f, 0, 0)));
                                            mushroom.transform.Rotate(0, 0, Random.Range(0, 360));
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        else if(random == 5)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 1.2f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.Euler(new Vector3(-90f, 0, 0)));
                                            mushroom.transform.Rotate(0, 0, Random.Range(0, 360));
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        else if(random == 6)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-0.9f, 0.9f), r.position.y + 1f, r.position.z + Random.Range(-0.9f, 0.9f)), Quaternion.Euler(new Vector3(-90f, 0, 0)));
                                            mushroom.transform.Rotate(0, 0, Random.Range(0, 360));
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        else if(random == 7)
                                        {
                                            GameObject mushroom = Instantiate(Mushrooms[random], new Vector3(r.position.x + Random.Range(-1, 1), r.position.y + 1.105f, r.position.z + Random.Range(-1, 1)), Quaternion.Euler(new Vector3(-90f, 0, 0)));
                                            mushroom.transform.Rotate(0, 0, Random.Range(0, 360));
                                            mushroom.transform.parent = mushrooms.transform;
                                        }
                                        GameObject empty = new GameObject("Empty");
                                        empty.transform.parent = r.transform;
                                    }
                                    NumMushroomsTotal++;
                                }
                            }
                        }
                    }
                }
            }
        }
        faces.Clear();
        Mushrooms.Clear();
        Debug.Log("Total Number of Mushrooms Generated: " + NumMushroomsTotal);
        NumMushroomsTotal = 0;
    }
    public static List<GameObject> GetAtPath(List<GameObject> list)
    {

        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + "Prefabs/Mushrooms");

        foreach (string fileName in fileEntries)
        {
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);

            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));

            if (t != null)
                al.Add(t);
        }
        for (int i = 0; i < al.Count; i++)
        {
            list.Add((GameObject)al[i]);
        }
        return list;
    }
}
