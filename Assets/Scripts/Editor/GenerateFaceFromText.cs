using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class GenerateFaceFromText
{

    // CHANGE TO USE
    // the name of the platform prefab you wish to use
    private static string horizontalPlatform = "HorizontalPlatform";
    private static string verticalPlatform = "VerticalPlatform";
    private static string stairsPlatform = "Stairs";

    static DirectoryInfo dir;
    static string state;

    [MenuItem("GameObject/Generate Faces From Textfiles", false, 12)]
    private static void Create()
    {
        // Loops through full directory and generates prefabs by name
        state = "normal";
        dir = new DirectoryInfo("Assets/TextLevels/");
        FileInfo[] info = dir.GetFiles("*.txt");
        foreach (FileInfo f in info)
        {
            Load(f.Name);
        }

        state = "4th";
        dir = new DirectoryInfo("Assets/TextLevels/4thFace");
        info = dir.GetFiles("*.txt");
        foreach (FileInfo f in info)
        {
            Load(f.Name);
        }
    }


    private static bool Load(string file)
    {
        // store transforms of all platforms to eventually place into the prefab
        List<Transform> transformList = new List<Transform>();
        // Handle any problems that might arise when reading the text
        try
        {
            string line;
            int lineNumber = 0; // the line we are on to get height correctly
                                // Create a new StreamReader, tell it which file to read and what encoding the file
                                // was saved as

            StreamReader theReader;
            if (state == "normal")
            {
                theReader = new StreamReader("Assets/TextLevels/" + file, Encoding.Default);
            } else
            {
                theReader = new StreamReader("Assets/TextLevels/4thFace/" + file, Encoding.Default);
            }
               
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();
                    if (line != null)
                    {
                        lineNumber++;
                        // for each line, read to 40 (length we have determined)
                        for (int i = 0; i < 40; i++)
                        {
                            // spawn a platform when appropriate
                            if (line[i].Equals('X'))
                            {
                                // load designated prefab, must be from proper folder
                                Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/" + horizontalPlatform + ".prefab", typeof(GameObject));
                                GameObject spawnedPlatform = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                spawnedPlatform.transform.position = new Vector3(i * 2, lineNumber * -2, 0);
                                spawnedPlatform.transform.eulerAngles = new Vector3(-90, 180, 0); // fix incorrect rotation on prefabs

                                //store transform
                                transformList.Add(spawnedPlatform.transform);

                            }
                            if (line[i].Equals('V'))
                            {
                                // load designated prefab, must be from proper folder
                                Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/" + verticalPlatform + ".prefab", typeof(GameObject));
                                GameObject spawnedPlatform = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                spawnedPlatform.transform.position = new Vector3(i * 2, lineNumber * -2, 0);
                                spawnedPlatform.transform.eulerAngles = new Vector3(-90, 180, 0);

                                //store transform
                                transformList.Add(spawnedPlatform.transform);
                            }
                            if (line[i].Equals('O'))
                            {
                                Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Pickups/Placeholder Pickup.prefab", typeof(GameObject));
                                GameObject spawnedPlaceholder = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                spawnedPlaceholder.transform.position = new Vector3(i * 2, lineNumber * -2, 0);

                                transformList.Add(spawnedPlaceholder.transform);
                            }
                            if (line[i].Equals('S'))
                            {
                                // load designated prefab, must be from proper folder
                                Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/" + stairsPlatform + ".prefab", typeof(GameObject));
                                GameObject spawnedPlatform = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                spawnedPlatform.transform.position = new Vector3(i * 2, lineNumber * -2, 0);
                                spawnedPlatform.transform.eulerAngles = new Vector3(-90, 180, 0);

                                //store transform
                                transformList.Add(spawnedPlatform.transform);
                            }
                        }

                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();

                // ADD CORNERS
                // load designated prefab, must be from proper folder
                Object corner1 = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/" + horizontalPlatform + ".prefab", typeof(GameObject));
                GameObject spawnedCorner1 = PrefabUtility.InstantiatePrefab(corner1) as GameObject;
                spawnedCorner1.transform.position = new Vector3(80, -20, 0);

                //store transform
                transformList.Add(spawnedCorner1.transform);

                Object corner2 = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/" + horizontalPlatform + ".prefab", typeof(GameObject));
                GameObject spawnedCorner2 = PrefabUtility.InstantiatePrefab(corner2) as GameObject;
                spawnedCorner2.transform.position = new Vector3(82, -20, 0);

                //store transform
                transformList.Add(spawnedCorner2.transform);


                // create an empty prefab that will hold our new prefab soon
                Object finalEmpty;
                if(state == "normal")
                {
                    finalEmpty = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Faces/" + file + ".prefab");
                } else
                {
                    finalEmpty = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Faces/4thFaces/" + file + ".prefab");
                }
                
                // empty game object to attatch all of the platforms too 
                GameObject finalFab = new GameObject();
                foreach (Transform t in transformList)
                {
                    t.transform.SetParent(finalFab.transform);
                }
                PrefabUtility.ReplacePrefab(finalFab.gameObject, finalEmpty, ReplacePrefabOptions.ConnectToPrefab);
                UnityEngine.Object.DestroyImmediate(finalFab);
                Debug.Log("Prefab " + file + ".prefab Created!");

                return true;
                
            }
           
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (System.Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }
}

