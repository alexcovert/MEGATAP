using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
/// </summary>
//[ExecuteInEditMode]
//[SaveDuringPlay]
//[AddComponentMenu("")] // Hide in menu
public class LockCameraY : CinemachineExtension
{
    [Tooltip("Lock the camera's Y position to this value")]
    public float m_YPosition = 10;
    public float offsetAbove = 0;
    

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            //pos.y = m_YPosition;
            state.RawPosition = new Vector3(pos.x, Mathf.Clamp(pos.y, m_YPosition, m_YPosition + offsetAbove), pos.z);
        }
    }
}
