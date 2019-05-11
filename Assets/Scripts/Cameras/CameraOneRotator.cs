using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//<alexc> This class rotates the Player 1 (left side) camera when the player runs into triggers at
//        the edge of each face of the tower. 
public class CameraOneRotator : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] [Tooltip("Audio Source on Game Manager")] private AudioSource audioSource;
    [SerializeField] private float windVolIncreasePerLevel;
    [SerializeField] private Camera playerOneCam;
    [SerializeField] private GameObject cinemachineSpeccy;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveUpSlowMultiplier;
    [SerializeField] private float zoomOutAmount;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private LockCameraY vcamLock;
    [SerializeField] private GameObject cameraTarget;
    //[SerializeField] private GameObject wall;
    [SerializeField] private GameObject[] rotateTriggers;    //Triggers that cause tower to rotate
    //[SerializeField] private GameObject[] wallTriggers;     //Triggers that pop up invisible wall behind player
    [SerializeField] private Transform floorSpawn;

    //Change these if the tower is scaled
    private static int camPosHorizontal = 20;
    private static int camPosVertical = 13;
    private static int camRotationX = 0;
    private static int camRotationY = 0;
    private static int numFloors;

    
    private Vector3[] basePositions = new[] {  new Vector3(0,                 camPosVertical, -camPosHorizontal),
                                               new Vector3(camPosHorizontal,  camPosVertical, 0),
                                               new Vector3(0,                 camPosVertical, camPosHorizontal),
                                               new Vector3(-camPosHorizontal, camPosVertical, 0)};

    private Quaternion[] rotations = new[] { Quaternion.Euler(camRotationX, camRotationY, 0),
                                             Quaternion.Euler(camRotationX, camRotationY - 90, 0),
                                             Quaternion.Euler(camRotationX, camRotationY - 180, 0),
                                             Quaternion.Euler(camRotationX, camRotationY - 270, 0)};

    private IEnumerator camTween;

    private int cameraState, floor;
    private Rigidbody rb;
    private CinemachineVirtualCamera cinemachineCam;
    private CinemachineFramingTransposer transposer;
    private void Start()
    {
        numFloors = tower.GetComponent<NumberOfFloors>().NumFloors;
        playerOneCam.transform.localPosition = basePositions[0];
        playerOneCam.transform.rotation = rotations[0];
        cameraState = 1;
        floor = 1;
        rb = GetComponent<Rigidbody>();

        //Camera
        cinemachineCam = cinemachineSpeccy.GetComponent<CinemachineVirtualCamera>();
        transposer = cinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        SetCullingMask();
    }

    private void Update()
    {
        //Don't delete this comment block - in case we switch back from Cinemachine
        //=========================================================================
        //switch (cameraState)
        //{
        //    case 1:
        //        playerOneCam.transform.position = new Vector3(Mathf.Clamp(playerModel.transform.position.x, -10, 10), playerOneCam.transform.position.y, playerModel.transform.position.z - camPosHorizontal);
        //        break;
        //    case 2:
        //        playerOneCam.transform.position = new Vector3(playerModel.transform.position.x + camPosHorizontal, playerOneCam.transform.position.y, Mathf.Clamp(playerModel.transform.position.z, -10, 10));
        //        break;
        //    case 3:
        //        playerOneCam.transform.position = new Vector3(Mathf.Clamp(playerModel.transform.position.x, -10, 10), playerOneCam.transform.position.y, playerModel.transform.position.z + camPosHorizontal);
        //        break;
        //    case 4:
        //        playerOneCam.transform.position = new Vector3(playerModel.transform.position.x - camPosHorizontal, playerOneCam.transform.position.y, Mathf.Clamp(playerModel.transform.position.z, -10, 10));
        //        break;

        //}
    }

    //4 triggers for rotating camera
    private void OnTriggerEnter(Collider other)
    {

        switch (other.tag)
        {
            case "Trigger1":
                StartMove(new Vector3(playerModel.transform.position.x + camPosHorizontal, playerOneCam.transform.position.y, playerModel.transform.position.z), rotations[1], 2);
                Destroy(other.gameObject);
                break;
            case "Trigger2":
                StartMove(new Vector3(playerModel.transform.position.x, playerOneCam.transform.position.y, playerModel.transform.position.z + camPosHorizontal), rotations[2], 3);
                Destroy(other.gameObject);
                break;
            case "Trigger3":
                StartMove(new Vector3(playerModel.transform.position.x - camPosHorizontal, playerOneCam.transform.position.y, playerModel.transform.position.z), rotations[3], 4);
                Destroy(other.gameObject);
                StartCoroutine(IncreaseOffset());

                break;
            case "Trigger4":
                if (cameraState == 4)
                {
                    Destroy(other.gameObject);
                    if (floor < numFloors)
                    {
                        floor++;
                        audioSource.volume += windVolIncreasePerLevel;


                        StartCoroutine(ReduceOffset(0));

                        StartMove(new Vector3(playerModel.transform.position.x, playerOneCam.transform.position.y + 20, playerModel.transform.position.z - camPosHorizontal), rotations[0], 1);

                        //Commented out bc it looks bad
                        //StartCoroutine(ChangeFOV(moveSpeed * moveUpSlowMultiplier));

                        break;
                    }
                    else
                    {
                        StartCoroutine(ReduceOffset(0));
                        StartMove(new Vector3(playerModel.transform.position.x, playerOneCam.transform.position.y + 20, playerModel.transform.position.z - camPosHorizontal), rotations[0], 1);
                        break;
                    }
                }
                break;
        }
    }

    //Initialize camera movement variables and start movement coroutine
    private void StartMove(Vector3 goalPos, Quaternion goalRot, int camState)
    {
       
        rb.velocity = Vector3.zero;
        RotatePlayer();
        cameraState = camState;

        if (camTween != null)
        {
            StopCoroutine(camTween);
        }
        //Tween the vcam rotation
        camTween = TweenToPosition(goalPos, goalRot, moveSpeed);
        StartCoroutine(camTween);
        SetCullingMask();
    }


    //src: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
    public void SetCullingMask()
    {
        switch (cameraState)
        {
            case 1:
                //Hide
                playerOneCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Trees1"));
                //Show
                playerOneCam.cullingMask |= 1 << LayerMask.NameToLayer("Trees4");
                break;
            case 2:
                //Hide
                playerOneCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Trees2"));
                //Show
                playerOneCam.cullingMask |= 1 << LayerMask.NameToLayer("Trees1");
                break;
            case 3:
                //Hide
                playerOneCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Trees3"));
                //Show
                playerOneCam.cullingMask |= 1 << LayerMask.NameToLayer("Trees2");
                break;
            case 4:
                //Hide
                playerOneCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Trees4"));
                //Show
                playerOneCam.cullingMask |= 1 << LayerMask.NameToLayer("Trees3");
                break;
        }
    }

    private IEnumerator ChangeFOV(float time)
    {
        float normalFOV = cinemachineCam.m_Lens.FieldOfView;
        float zoomedFOV = cinemachineCam.m_Lens.FieldOfView + zoomOutAmount;

        cinemachineCam.m_Lens.FieldOfView = zoomedFOV;

        for (float t = 0; t < time / 2; t += Time.deltaTime)
        {
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(normalFOV, zoomedFOV, t / time);
            yield return null;
        }

        for (float t = time / 2; t < time; t += Time.deltaTime)
        {
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(zoomedFOV, normalFOV, t / time);
            yield return null;
        }
        
    }

    private IEnumerator IncreaseOffset()
    {
        float time = 10;
        
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            vcamLock.offsetAbove = Mathf.Lerp(0, 20, t / time);
            transposer.m_YDamping = Mathf.Lerp(0, 1, t / time);
            yield return null;
        }
    }

    private IEnumerator ReduceOffset(float amount)
    {
        float time = 10;
        
        float prevLock = vcamLock.m_YPosition;
        for(float t = 0; t < time; t += Time.deltaTime)
        {
            Debug.Log("locking...");
            transposer.m_YDamping = Mathf.Lerp(1, 0, t / time);
            vcamLock.offsetAbove = Mathf.Lerp(20, 0, t / time);
            vcamLock.m_YPosition = Mathf.Lerp(prevLock, prevLock + 20, t / time);
            yield return null;
        }
    }

    //Camera movement coroutine
    private IEnumerator TweenToPosition(Vector3 targetPos, Quaternion targetRot, float time)
    {
        Vector3 currentPos = playerOneCam.transform.localPosition;
        Quaternion currentRot = playerOneCam.transform.rotation;

        targetPos.y *= floor;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            //cinemachineSpeccy.transform.position = Vector3.Lerp(currentPos, targetPos, t / time);
            cinemachineSpeccy.transform.rotation = Quaternion.Slerp(currentRot, targetRot, t / time);
            yield return null;
        }

        //playerOneCam.transform.po
        cinemachineSpeccy.transform.rotation = targetRot;
        camTween = null;
    }

    //TODO: Change how we do this once we get the more final player model
    //Rotate the player model when you move around the tower
    private void RotatePlayer()
    {
        float rotY = playerModel.transform.localRotation.eulerAngles.y;
        playerModel.transform.localRotation = Quaternion.Euler(0, rotY - 90, 0);
    }

    public int GetFloor()
    {
        return floor;
    }

    public int GetState()
    {
        return cameraState;
    }
}
