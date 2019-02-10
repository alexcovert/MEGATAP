using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWalls : MonoBehaviour {
    [SerializeField] private GameObject[] wallTriggers;
    [SerializeField] private GameObject wall;
    [SerializeField] CameraOneRotator camOneRotator;
    private int floor;

    private void OnTriggerEnter(Collider other)
    {
        floor = camOneRotator.GetFloor();
        Vector3 wallPos = wall.transform.position;
        Quaternion wallRot = wall.transform.rotation;
        float wallY = wall.transform.position.y + 20 * (floor - 1);

        switch (other.tag)
        {
            case "Wall1":
                wallPos = new Vector3(wallTriggers[0].transform.position.x, wallY, wallTriggers[0].transform.position.z - 4);
                Instantiate(wall, wallPos, wallRot);
                break;
            case "Wall2":
                wallPos = new Vector3(wallTriggers[1].transform.position.x + 3, wallY, wallTriggers[1].transform.position.z);
                wallRot = Quaternion.Euler(wall.transform.rotation.x, wall.transform.rotation.y, wall.transform.rotation.z);
                Instantiate(wall, wallPos, wallRot);
                break;
            case "Wall3":
                wallPos = new Vector3(wallTriggers[2].transform.position.x, wallY, wallTriggers[2].transform.position.z + 4);
                Instantiate(wall, wallPos, wallRot);
                break;
        }
    }
}
