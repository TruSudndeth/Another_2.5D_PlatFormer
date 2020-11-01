using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class Player_ControlAnimation : MonoBehaviour
{
    [SerializeField]
    private Player_Controller PlayerController;
    [SerializeField]
    public Vector3 ModelPosition;
    private bool Climb = false;
    [SerializeField]
    private float LandingPower = 0;
    private bool reverseBool = false;
    private bool PlayLandAnimation;
    private float ClipLandingAnim = 0;
    private AnimationClip Landing;
    private Animator Anim;
    private float SpeedMultiplyer = 1;
    private bool Jump = false;
    private bool IsGrounded = true;
    private IEnumerator _FallingInc;
    private float _fallingInc = 0;
    private IEnumerator _AnimHoldTimeDelay;
    void Start()
    {
        Anim = GetComponent<Animator>();
        Landing = AnimatorClips();
        if (Landing == null)
        {
            Debug.Log("There is no Clip name Landing");
        }
    }
    private void FixedUpdate()
    {
        if(Climb)
        {
            ModelPosition = transform.localPosition * 3;
            transform.localPosition = Vector3.zero;
        }

        if (Jump)
        {
            IsGrounded = false;
            _fallingInc -= Time.fixedDeltaTime * 3;
            if (_fallingInc <= -1)
            {
                Jump = false;
                _fallingInc = -1;
            }
            if(!PlayLandAnimation)Anim.SetFloat("Vertical", _fallingInc);
        }

        if(PlayLandAnimation)
        {
            if (!reverseBool)
            {
                ClipLandingAnim += Time.deltaTime;
                if(ClipLandingAnim >= LandingPower)reverseBool = true;
            }
            if(reverseBool)
            {
                ClipLandingAnim -= Time.deltaTime;
                if (ClipLandingAnim <= 0)
                {
                    PlayerController.AllowMoveHorizontal();
                    ClipLandingAnim = 0;
                    reverseBool = false;
                    PlayLandAnimation = false;
                    Anim.SetBool("Landing", false);
                }
            }
            Anim.Play(Landing.name, 0, ClipLandingAnim);
        }
    }
    public void CharacterAnimations(Vector3 _PlayerInput)
    {
        if (_PlayerInput.y < -1)
        {
            if (_fallingInc == 0) Jump = true;
        }
        if (_PlayerInput.y == -1) _fallingInc = 0;
        if (_PlayerInput.y == 0)
        {
            Jump = false;
            _fallingInc = 0;
            Anim.SetFloat("Vertical", 0);
        }
        else Anim.SetFloat("Vertical", _fallingInc);
        SpeedMultiplyer = Mathf.Lerp( 1, 2,_PlayerInput.z);
        Anim.SetFloat("Speed", SpeedMultiplyer);
        Anim.SetFloat("Horizontal", _PlayerInput.z);
    }
    public void CharactorAnimations_Jump()
    {
        Jump = true;
    }
    public void CharactorAnimaition_Landing(float SoftLanding)
    {
        LandingPower = SoftLanding;
        if (LandingPower > 1 || LandingPower > 0.5f) LandingPower = 1;
        IsGrounded = true;
        _fallingInc = 0;
        PlayLandAnimation = true;
        Anim.SetBool("Landing", true);
    }

    private AnimationClip AnimatorClips()
    {
        AnimationClip[] FindByName = Anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip AClip in FindByName)
        {
            if(AClip.name == "Landing")
            {
                return AClip;
            }
        }
        return null;
    }

    public void LedgeGrab(bool _ISState)
    {
        _fallingInc = 0;
        Anim.SetBool("LedgeGrab" , _ISState);
    }

    public void ClimbLedge(bool _IsState)
    {
        Climb = _IsState;
        Anim.SetBool("Climb", _IsState);
    }
} 
