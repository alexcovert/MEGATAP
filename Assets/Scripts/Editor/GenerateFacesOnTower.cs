using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateFacesOnTower : MonoBehaviour {

    //Change to Use
    //private static int NumFloors = 7;

    private static int state = 1;
    private static int floor = 1;
    private static bool loop = true;
    //Index for every face on tower
    private static int index = 0;
    //Index for Faces List
    private static int FaceIndex = 0;
    //Index for 4th Faces List
    private static int StairsIndex = 0;

    //Regular faces on tower
    private static List<GameObject> Faces = new List<GameObject>();
    //4th face on tower, stairs to go up to the next floor
    private static List<GameObject> Stairs = new List<GameObject>();

    [MenuItem("GameObject/Generate Face on Tower in Sequence", false, 13)]

    private static void Create()
    {
        Debug.Log("Generating Faces on Tower in Sequence");
        GetAtPath(Faces);
        GetStairs(Stairs);
        if (Faces != null && Stairs != null)
        {
            while (loop == true)
            {
                switch (state)
                {
                    case 1:
                        Instantiate(Faces[FaceIndex], new Vector3(-39.01f, (20.0f * floor) + 1, -41.99f), Quaternion.Euler(0, 0, 0));
                        state++;
                        index++;
                        FaceIndex++;
                        break;
                    case 2:
                        Instantiate(Faces[FaceIndex], new Vector3(41.99f, (20.0f * floor) + 1, -39.01f), Quaternion.Euler(0, -90, 0));
                        state++;
                        index++;
                        FaceIndex++;
                        break;
                    case 3:
                        Instantiate(Faces[FaceIndex], new Vector3(39.01f, (20.0f * floor) + 1, 41.99f), Quaternion.Euler(0, 180, 0));
                        state++;
                        index++;
                        FaceIndex++;
                        break;
                    case 4:
                        Instantiate(Stairs[StairsIndex], new Vector3(-41.99f, (20.0f * floor) + 1, 39.01f), Quaternion.Euler(0, -270, 0));
                        state = 1;
                        floor++;
                        index++;
                        if (Stairs.Count > 2)
                        {
                            StairsIndex++;
                        }
                        break;
                }
                if (index >= 20)
                {
                    loop = false;
                }
            }
        }
        index = 0;
        floor = 1;
        state = 1;
        FaceIndex = 0;
        StairsIndex = 0;
        Faces.Clear();
        Stairs.Clear();
        loop = true;
    }


    public static List<GameObject> GetAtPath(List<GameObject> list)
    {

        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + "Prefabs/Faces");

        foreach (string fileName in fileEntries)
        {
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);

            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));

            if (t != null)
                al.Add(t);
        }
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < al.Count; i++)
        {
            result.Add((GameObject)al[i]);
        }
        
        for(int i = 0; i < result.Count; i++)
        {
            //int random = Random.Range(0, result.Count);
            list.Add(result[i]);
        }
        return list;
    }
    public static List<GameObject> GetStairs(List<GameObject> list)
    {

        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + "Prefabs/Faces/4thFaces");

        foreach (string fileName in fileEntries)
        {
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);

            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));

            if (t != null)
                al.Add(t);
        }
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < al.Count; i++)
        {
            result.Add((GameObject)al[i]);
        }

        for (int i = 0; i < result.Count; i++)
        {
            //int random = Random.Range(0, result.Count);
            list.Add(result[i]);
        }
        return list;
    }
}
