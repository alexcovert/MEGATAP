using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeccySounds : MonoBehaviour {

    [SerializeField] [Range(0, 1)]
    private float jumpVolume;
    [SerializeField] [Range(0, 1)]
    private float landingVolume;
    [SerializeField] [Range(0, 1)]
    private float footstepVolume;
    [SerializeField] [Range(0, 1)]
    private float knockbackOofVolume;
    [SerializeField] [Range(0, 1)]
    private float sapStepVolume;
    [SerializeField] [Range(0, 1)]
    private float slipVolume;
    [SerializeField] [Range(0, 1)]
    private float sighVolume;

    [SerializeField]
    private AudioClip[] BodySFX;
    [SerializeField]
    private AudioClip[] JumpVoiceSFX;
    [SerializeField]
    private AudioClip[] OofVoiceSFX;
    [SerializeField]
    private AudioClip[] CrouchSFX;
    [SerializeField]
    private AudioClip[] sapSteps;

    int jumps = 0;
    int sighs = 0;
    int grunts = 0;
    int oofs = 0;

    private AudioSource audioSource;
    private PlayerOneMovement p1Movement;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        p1Movement = GetComponent<PlayerOneMovement>();
    }

    private void Start()
    {
        jumps = 0;
        oofs = 0;
    }

    // Body SFX
    // Footsteps
    private void  RightFootstep()
    {
        AudioClip clip = GetRightFootClip();
        audioSource.volume = footstepVolume;
        audioSource.PlayOneShot(clip);
    }

    private void LeftFootstep()
    {
        AudioClip clip = GetLeftFootClip();
        audioSource.volume = footstepVolume;
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRightFootClip()
    {
        return BodySFX[0];
    }

    private AudioClip GetLeftFootClip()
    {
        return BodySFX[1];
    }

    // Landing
    private void JumpLanding()
    {
        AudioClip clip = GetLandingClip();
        audioSource.volume = landingVolume;
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetLandingClip()
    {
        return BodySFX[2];
    }

    // Voice SFX
    // Jump

    private void JumpHyuh()
    {
        AudioClip clip = GetJumpingClip();
        audioSource.volume = jumpVolume;
        audioSource.PlayOneShot(clip);
    }

    private void TurnJumpHyah()
    {
        AudioClip clip = GetJumpingClip();
        audioSource.volume = jumpVolume;
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetJumpingClip()
    {
        int hyuh = jumps % 3;
        jumps++;
        return JumpVoiceSFX[hyuh];
    }    

    // Oof-Slip

    private void OofSlip()
    {
        AudioClip clip = GetSlip();
        audioSource.volume = slipVolume;
        audioSource.PlayOneShot(clip);
    }

    private void CrouchHMP()
    {
        int grunt = grunts % 4;
        grunts++;
        AudioClip clip = CrouchSFX[grunt];
        Debug.Log("CROUCHING");
        audioSource.volume = sighVolume;
        audioSource.PlayOneShot(clip);
    }

    private void GetUpSigh()
    {
        int sigh = sighs % 4 + 4;
        sighs++;
        AudioClip clip = CrouchSFX[sigh];
        Debug.Log("STANDING");
        audioSource.volume = sighVolume;
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetSlip()
    {
        int oof = oofs % 3;
        oofs++;
        return OofVoiceSFX[oof];
    }

    // Oof-Knockback
    private void OofKB()
    {
        AudioClip clip = GetKB();
        audioSource.volume = knockbackOofVolume;
        audioSource.PlayOneShot(clip);
    }

    private void OofStun()
    {
        AudioClip clip = GetKB();
        audioSource.volume = knockbackOofVolume;
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetKB()
    {
        int oof = oofs % 4 + 3;
        oofs++;
        return OofVoiceSFX[oof];
    }

    //Slime steps
    private void SapStepLeft()
    {
        audioSource.volume = sapStepVolume;
        audioSource.PlayOneShot(sapSteps[0]);
    }

    private void SapStepRight()
    {
        audioSource.volume = sapStepVolume;
        audioSource.PlayOneShot(sapSteps[1]);
    }


    //Crouch steps
    private void CrouchStepLeft()
    {
        if(p1Movement.GetSlowed())
        {
            audioSource.PlayOneShot(sapSteps[0]);
        }
        else
        {
            audioSource.PlayOneShot(BodySFX[0]);
        }
    }

    private void CrouchStepRight()
    {
        if (p1Movement.GetSlowed())
        {
            audioSource.PlayOneShot(sapSteps[1]);
        }
        else
        {
            audioSource.PlayOneShot(BodySFX[1]);
        }
    }
}
