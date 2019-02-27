using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class GenerateFaceFromText {

    // CHANGE TO USE
    // the name of the platform prefab you wish to use
    private static string platform = "1x1Platform";
    private static string textfile1 = "Sample_File_2";

    [MenuItem("GameObject/Generate Face", false, 12)]
    private static void Create()
    {
        Debug.Log("Generating Faces");
        Load(textfile1);
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
            StreamReader theReader = new StreamReader("Assets/TextLevels/" + textfile1 + ".txt", Encoding.Default);
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
                        for(int i = 0; i < 40; i++)
                        {
                            // spawn a platform when appropriate
                            if(line[i].Equals('X'))
                            {
                                // load designated prefab, must be from proper folder
                                Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/" + platform + ".prefab", typeof(GameObject));
                                GameObject spawnedPlatform = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                spawnedPlatform.transform.position = new Vector3(i * 2, lineNumber * -2, 0);

                                //store transform
                                transformList.Add(spawnedPlatform.transform);

                            }
                        }
                        
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();

                // create an empty prefab that will hold our new prefab soon
                Object finalEmpty = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Faces/" + textfile1 + ".prefab");
                // empty game object to attatch all of the platforms too 
                GameObject finalFab = new GameObject();
                foreach (Transform t in transformList)
                {
                    t.transform.SetParent(finalFab.transform);
                }
                PrefabUtility.ReplacePrefab(finalFab.gameObject, finalEmpty, ReplacePrefabOptions.ConnectToPrefab);
                UnityEngine.Object.DestroyImmediate(finalFab);
                Debug.Log("Prefab Created!");
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

