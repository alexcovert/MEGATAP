using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TutorialFaceGeneration : MonoBehaviour {

    //Change to Use
    private static int NumFloors = 5;

    private static int state = 1;
    private static int floor = 1;
    private static bool loop = true;
    private static bool loop2 = true;
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

    [MenuItem("GameObject/Generate Tutorial Faces", false, 16)]

    private static void Create()
    {
        Debug.Log("Generating Tutorial Faces");
        Faces.Clear();
        Stairs.Clear();
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
                if (floor >= 3)
                {
                    loop = false;
                }
            }
        }

        while (loop2 == true && loop == false)
        {
            switch (state)
            {
                case 1:
                    Instantiate(Faces[Random.Range(7, Faces.Count)], new Vector3(-39.01f, (20.0f * floor) + 1, -41.99f), Quaternion.Euler(0, 0, 0));
                    state++;
                    index++;
                    break;
                case 2:
                    Instantiate(Faces[Random.Range(7, Faces.Count)], new Vector3(41.99f, (20.0f * floor) + 1, -39.01f), Quaternion.Euler(0, -90, 0));
                    state++;
                    index++;
                    break;
                case 3:
                    Instantiate(Faces[Random.Range(7, Faces.Count)], new Vector3(39.01f, (20.0f * floor) + 1, 41.99f), Quaternion.Euler(0, 180, 0));
                    state++;
                    index++;
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
            if (index >= NumFloors * 4)
            {
                loop2 = false;
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
        loop2 = true;
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

        for (int i = 0; i < result.Count; i++)
        {
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
            list.Add(result[i]);
        }
        return list;
    }
}
