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
    private Vector3 _ModelPosition;
    private Vector3 ModelStartPosition;
    [SerializeField]
    private float LandingPower = 0;
    private bool reverseBool = false;
    private bool PlayLandAnimation;
    private float ClipLandingAnim = 0;
    private AnimationClip Landing;
    private Animator Anim;
    private AnimationClip Idle;
    private float SpeedMultiplyer = 1;
    private bool Jump = false;
    private bool IsGrounded = true;
    private IEnumerator _FallingInc;
    private float _fallingInc = 0;
    private IEnumerator _AnimHoldTimeDelay;
    private bool PlayerRollsOnLanding = false;

    private void Awake()
    {
        ModelStartPosition = transform.localPosition;
    }
    public Vector3 ModelPosition
    {
        get
        {
            _ModelPosition = transform.localPosition - ModelStartPosition;
            _ModelPosition.x = 0;
            transform.localPosition = ModelStartPosition;
            return _ModelPosition;
        }
    }
    void Start()
    {
        Anim = GetComponent<Animator>();
        Landing = AnimatorClips("Landing");
        Idle = AnimatorClips("Idle");
        if (Landing == null)
        {
            Debug.Log("There is no Clip name Landing");
        }
    }
    private void FixedUpdate()
    {
        if (Jump) // Sets Blend tree to Falling Idel when not grounded
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

        if(PlayLandAnimation) // this plays jump animation Forwards then Back to standing Idel based off fall hight
        {
            if (!reverseBool)
            {
                ClipLandingAnim += Time.deltaTime;
                if(ClipLandingAnim >= LandingPower)reverseBool = true;
            }
            if(PlayerRollsOnLanding || reverseBool)
            {
                ClipLandingAnim -= Time.deltaTime;
                if (PlayerRollsOnLanding || ClipLandingAnim <= 0)
                {
                    PlayerController.AllowMoveHorizontal();
                    ClipLandingAnim = 0;
                    reverseBool = false;
                    PlayLandAnimation = false;
                    if(!PlayerRollsOnLanding)Anim.SetBool("Landing", false);
                    PlayerRollsOnLanding = false;
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

    public void SetBoolJump(bool _IsState)
    {
        if (_IsState)
        {
            Debug.Log("idle State Ran ");
            Anim.Play(Idle.name, 0);
            PlayerController.IKModelTransformOffsetBool();
        }
        Anim.SetBool("Jump", _IsState);
    }
    public void CharactorAnimaition_Landing(float SoftLanding)  // this plays jump animation landing force at diffrent hights
    {
        PlayerRollsOnLanding = false;
        LandingPower = SoftLanding;
        if (LandingPower > 1 || LandingPower > 0.5f) LandingPower = 1;
        IsGrounded = true;
        _fallingInc = 0;
        PlayLandAnimation = true;
        Anim.SetBool("Landing", true);
    }

    private AnimationClip AnimatorClips(string _Name)
    {
        AnimationClip[] FindByName = Anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip AClip in FindByName)
        {
            if(AClip.name == _Name)
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
        Anim.SetBool("Climb", _IsState);
    }

    public void LadderClimb(bool _Climb)
    {
        Anim.SetBool("Climbing_Ladder", _Climb);
    }
    public void PlayerRolls(bool _isState)
    {
        PlayerRollsOnLanding = _isState; 
        Anim.SetBool("Roll", _isState);
    }
} 
