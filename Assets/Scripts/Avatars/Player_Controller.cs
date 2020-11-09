using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    //ToDo's
    // Landing ON Moving Platform cuts threw platform
    // Must Fix LedgePlatformPosition to update player transform to where the ledge is !IMPORTANT!
    // Fix Player Ledge LetGo Falls (Animation Scrip)
    // On landing choose to roll forwards
    // on moving Backwards jump landing automaticly rolls backwards. with an option to back power flip.
    private float WorldRecordHangTime = 185.9f;
    [HideInInspector]
    public Transform MovingPlatform;
    [HideInInspector]
    public Transform LedgePlatformPosition;
    private Vector3 _MovingPlatform;
    [SerializeField]
    private float OffSet_Y;
    [SerializeField]
    private float GrabSpeed;
    [SerializeField]
    private Vector3 LedgeStandUpPosition;
    [SerializeField]
    private bool Climbing = false;
    [SerializeField]
    private bool OnLedge = false;
    private Player_LedgeGrab LedgeDetection_Offset;
    [SerializeField]
    private bool IsOnLedge = false;
    [SerializeField]
    private Vector3 ArmsReachPosition;
    private LedgeGrab _GrabableLedge;
    private Vector3 PlayerRotation;
    private int InverseDirection = 1;
    private float LandingDamage = 0;
    private Player_ControlAnimation TriggerAnim;
    private CharacterController PlayerController;
    [SerializeField]
    private float Speed;
    [SerializeField]
    private float MaxSpeed;
    [SerializeField]
    private float JumpForce;
    private float gravity = 9.8f;
    private float HangeTime = 0.05f;
    private Vector3 MoveHorizontal = Vector3.zero;
    private Vector3 MoveVertical = Vector3.zero;
    [HideInInspector]
    public bool Jumped = false;
    [SerializeField]
    private bool Jumped_FixedUpdate = false;
    private bool SuperLanding = false;
    private IEnumerator _Climbed;
    private bool GravityT = true;
    private bool Grounded = false;
    private bool LandPosition = false;
    private float PlayerPlatformOffset = 0;
    private bool ZeroGravityOnLanding_thisFrame = false;
    [SerializeField]
    private bool _AllowMoveHorizontal = true;
    [SerializeField]
    private Vector3 _LedgePlatformPosition;
    private Vector3 CurrentPosition;
    private Vector3 _ArmsReach;
    [SerializeField]
    private Vector3 TestingFloat;
    private float LedgeGrabAnimationSpeedInc;
    private Vector3 ClimbAnimationPosition;
    [SerializeField]
    private bool ClimbingLadders = false;
    [SerializeField]
    private float MySpeedFloat;
    [SerializeField]
    private bool StepOverAnim = false;
    [SerializeField]
    private bool LadderHandsOffSet = false;
    private Vector3 IKModelTransformOffset;
    private Vector3 IKModelTransformOffset_Var;
    private bool _IKModelTransformOffset = false;
    private bool _IKModelExitAnim = false;

    // Start is called before the first frame update
    void Start()
    {
        LedgeDetection_Offset = GetComponentInChildren<Player_LedgeGrab>();
        PlayerRotation = transform.localEulerAngles;
        SuperLanding = true;
        PlayerController = GetComponent<CharacterController>();
        TriggerAnim = GetComponentInChildren<Player_ControlAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && LedgePlatformPosition.tag == "LatterStep")
        {
            IKModelTransformOffset = Vector3.zero;
            IKModelTransformOffset_Var = Vector3.zero;
            LedgeGrabAnimationSpeedInc = 0;
            ClimbingLadders = !ClimbingLadders;
            TriggerAnim.LadderClimb(ClimbingLadders);
            if (ClimbingLadders)
            {
                LadderHandsOffSet = false;
                ClimbingLadders = true;
                _AllowMoveHorizontal = false;
            }
        }
        if (LedgePlatformPosition && LedgePlatformPosition.tag == "MovingPlatform")
        {
            Vector3 _LedgeOffset = _GrabableLedge.ReverseLedgeGrab(InverseDirection);
            Vector3 _LedgeDetection_Offset = LedgeDetection_Offset.ReverseLedgeGrab(InverseDirection);

            _LedgePlatformPosition = _LedgeOffset + (transform.position - _LedgeDetection_Offset);
            _LedgePlatformPosition.x = 0;
        }
        if(Input.GetKeyDown(KeyCode.X) && IsOnLedge)
        {
            ResetAllControls();
        }
        if(MovingPlatform == null && !IsOnLedge)
        {
            if (PlayerPlatformOffset != 0) PlayerPlatformOffset = 0;
            if(LandPosition)LandPosition = false;
            Grounded = false;
            if (_MovingPlatform != Vector3.zero)_MovingPlatform = Vector3.zero;
            MoveHorizontal.z = Input.GetAxis("Horizontal");
        }
        if(MovingPlatform != null && !IsOnLedge)
        {
            
            Grounded = true;
            if(!LandPosition)
            {
                LandPosition = true;
                PlayerPlatformOffset = (MovingPlatform.position.z - transform.position.z);
            }
            if (Input.GetAxis("Horizontal") != 0 && _AllowMoveHorizontal)
            {
                PlayerPlatformOffset -= Input.GetAxis("Horizontal") * SpeedWMultiplier(Input.GetAxis("Horizontal")) * Time.fixedDeltaTime;
            }
            _MovingPlatform = MovingPlatform.position - new Vector3(transform.position.x, transform.position.y + OffSet_Y, transform.position.z + PlayerPlatformOffset);
            if (MovingPlatform.tag == "MovingPlatform") _MovingPlatform.y -= 2.31f;
        }
        if((Input.GetKeyDown(KeyCode.Space)) && !Jumped && !Jumped_FixedUpdate) Jumped = true;
        if (Input.GetAxisRaw("Horizontal") != 0 && (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) && _AllowMoveHorizontal)
        {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    PlayerRotation.y = 0;
                    InverseDirection = 1;
                }
                else
                {
                    PlayerRotation.y = 180;
                    InverseDirection = -1;
                }
            
        }
    }

    private void FixedUpdate()
    {
        if(PlayerController.isGrounded || Grounded)
        {
            if(SuperLanding)
            {
                _AllowMoveHorizontal = false;
                ZeroGravityOnLanding_thisFrame = true;
                LandingDamage = Mathf.Abs(MoveVertical.y / 300);
                SuperLanding = false;
                TriggerAnim.CharactorAnimaition_Landing(LandingDamage);
                LandingDamage = 0;
                MoveVertical = Vector3.zero;
            }
            if(Jumped_FixedUpdate)Jumped_FixedUpdate = false;
            if(Jumped && !Jumped_FixedUpdate && !IsOnLedge)
            {
                LandPosition = false;
                MovingPlatform = null;
                Grounded = false;
                GravityT = true;
                SuperLanding = true;
                TriggerAnim.CharactorAnimations_Jump();
                Jumped_FixedUpdate = true;
                Jumped = false;
                MoveVertical.y = 0;
                MoveVertical.y = JumpForce;
            }
            if (PlayerController.isGrounded && !Jumped_FixedUpdate && !IsOnLedge && !Grounded)
            {
                if(!ZeroGravityOnLanding_thisFrame)MoveVertical.y = -1;
                ZeroGravityOnLanding_thisFrame = false;
            }
        }
        else
        {
            if((Input.GetKey(KeyCode.Space)) && MoveVertical.y > JumpForce - 1 && Jumped_FixedUpdate)
            {
                gravity = HangeTime;
            }
            else
            {
                gravity = 9.8f;
            }
            if(MoveVertical.y > -300 && !IsOnLedge && GravityT && !Grounded) MoveVertical.y -= gravity;
        }
        if (!IsOnLedge && _AllowMoveHorizontal)
        {
            transform.rotation = Quaternion.Euler(PlayerRotation);
            if (Grounded) 
                MoveVertical.y = 0;
            PlayerController.Move(_MovingPlatform + (MoveVertical + (MoveHorizontal * SpeedWMultiplier(MoveHorizontal.z))) * Time.fixedDeltaTime);
        }
        Vector3 HorizontalAnimation = new Vector3(0, 0 , Input.GetAxis("Horizontal"));
        if (!IsOnLedge)TriggerAnim.CharacterAnimations((MoveVertical + HorizontalAnimation * InverseDirection));
        ClifHanger();
        ClimbLadderAnimation();
    }

    public void StepOverLadder(bool _state)
    {
        StepOverAnim = _state;
        if (StepOverAnim)
        {
            IKModelTransformOffset = Vector3.zero;
            IKModelTransformOffset_Var = Vector3.zero;
            ClimbingLadders = false;
            TriggerAnim.LadderClimb(false);
        }
    }
    public void IKModelTransformOffsetBool()
    {
        _IKModelExitAnim = true;
    }

    public void ClimbLadderAnimation()
    {
        if (ClimbingLadders)
        {
            ClimbAnimationPosition += ModelToControllerTransformOffset(new Vector3(0, 2.20178f, -0.6133f), MySpeedFloat);
            transform.position = ClimbAnimationPosition + _LedgePlatformPosition;
            if (_IKModelTransformOffset) _IKModelTransformOffset = false;
        }
        if (StepOverAnim)
        {
            ClimbAnimationPosition += ModelToControllerTransformOffset(Vector3.zero, 0);
            transform.position = ClimbAnimationPosition + _LedgePlatformPosition;
            if(_IKModelExitAnim)
            {
                _IKModelExitAnim = false;
                SuperLanding = false;
                _IKModelTransformOffset = false;
                StepOverAnim = false;
                _AllowMoveHorizontal = true;
                IsOnLedge = false;
            }
        }
    }
    private Vector3 ModelToControllerTransformOffset(Vector3 _ControllerAnimationOffset, float _Speed)
    {
        Vector3 _IKModelPosition = ScaleModelOffset(TriggerAnim.ModelPosition);

        IKModelTransformOffset_Var = Vector3.MoveTowards(Vector3.zero, _ControllerAnimationOffset, LedgeGrabAnimationSpeedInc);
        Vector3 IKModelInc = IKModelTransformOffset_Var - IKModelTransformOffset;
        IKModelTransformOffset = IKModelTransformOffset_Var;
        if (IKModelTransformOffset_Var != _ControllerAnimationOffset)
        {
            LedgeGrabAnimationSpeedInc += Time.deltaTime * _Speed;
        }
        if(IKModelTransformOffset_Var == _ControllerAnimationOffset)
        {
            _IKModelTransformOffset = true;
        }
        IKModelInc.z *= InverseDirection;
        _IKModelPosition.z *= InverseDirection;
        return IKModelInc + _IKModelPosition;
    }
    private Vector3 ScaleModelOffset(Vector3 _Scaled)
    {
        _Scaled.x *= transform.localScale.x;
        _Scaled.y *= transform.localScale.y;
        _Scaled.z *= transform.localScale.z;
        return _Scaled;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(!PlayerController.isGrounded && hit.normal == (Vector3.back * InverseDirection) && !IsOnLedge)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.black);
            _GrabableLedge = hit.gameObject.GetComponent<LedgeGrab>();
            if(_GrabableLedge != null)
            {
                IKModelTransformOffset = Vector3.zero;
                IKModelTransformOffset_Var = Vector3.zero;
                LadderHandsOffSet = false;
                ClimbAnimationPosition = new Vector3();
                LedgePlatformPosition = hit.gameObject.transform;
                IsOnLedge = true;
                _AllowMoveHorizontal = false;
                Vector3 _LedgeOffset = _GrabableLedge.ReverseLedgeGrab(InverseDirection);
                Vector3 _LedgeDetection_Offset = LedgeDetection_Offset.ReverseLedgeGrab(InverseDirection);
                 ArmsReachPosition = _LedgeOffset - _LedgeDetection_Offset;
                _LedgePlatformPosition = _LedgeOffset + (transform.position - _LedgeDetection_Offset);
                _LedgePlatformPosition.x = 0;
                CalculateAnimationGrabSpeed();
                TriggerAnim.LedgeGrab(true);
                Jumped_FixedUpdate = false;
                MoveVertical = Vector3.zero;
                TriggerAnim.CharacterAnimations(MoveVertical + MoveVertical);
                // trigger animation
                // trigger charactor move position
                // stop all input other then move over ledge.
            }
        }
        
    }

    private void CalculateAnimationGrabSpeed()
    {
        GrabSpeed = Vector3.Distance(LedgeDetection_Offset.transform.position, transform.position + ArmsReachPosition);
        float DistanceDelta = Mathf.Clamp((GrabSpeed - 4) / 5, 0, 1);
        GrabSpeed *= Mathf.Lerp(0.92235f, 2.667f, DistanceDelta);
    }

    private void ClifHanger()
    {
        if (IsOnLedge)
        {
            if (Jumped && LedgePlatformPosition.tag != "LatterStep")
            {
                Jumped = false;
                Climbing = true;
                TriggerAnim.ClimbLedge(true);
            }
            if (Climbing)
            {
                if (!OnLedge)  //Climbing Animation
                {
                    ClimbAnimationPosition += ModelToControllerTransformOffset(Vector3.zero, 0);
                    transform.position = ClimbAnimationPosition + _LedgePlatformPosition;
                    if (_IKModelTransformOffset) _IKModelTransformOffset = false;
                }
                else // Standing Animation
                {
                    ClimbAnimationPosition += ModelToControllerTransformOffset(new Vector3(0, 1.668f, 0), 2.84f);
                    transform.position = ClimbAnimationPosition + _LedgePlatformPosition;

                    if (_IKModelTransformOffset)
                    {
                        LedgePlatformPosition = null;
                        _IKModelTransformOffset = false;
                        _AllowMoveHorizontal = true;
                        OnLedge = false;
                        Climbing = false;
                        IsOnLedge = false;
                    }
                }
            }
            else if(!ClimbingLadders && !Climbing)
            {
                if (_ArmsReach != ArmsReachPosition)
                {
                    LedgeGrabAnimationSpeedInc += Time.fixedDeltaTime * GrabSpeed;
                    _ArmsReach = Vector3.MoveTowards(ArmsReachPosition, Vector3.zero, LedgeGrabAnimationSpeedInc);
                }
                transform.position = _LedgePlatformPosition - _ArmsReach;
            }
        }
    }

    public void ToggleGravity(bool _SetGravity)
    {
        GravityT = _SetGravity;
        if (!GravityT)
        {
            MoveVertical = Vector3.zero;
        }
    }
    public void ClimbSequence()
    {
        LedgeGrabAnimationSpeedInc = 0;
        SuperLanding = false;
        LedgeStandUpPosition = ClimbAnimationPosition;
        OnLedge = true;
        TriggerAnim.ClimbLedge(false);
        TriggerAnim.LedgeGrab(false);
    }

    private float SpeedWMultiplier(float _InputOfT)
    {
        return Mathf.Lerp(Speed, MaxSpeed, _InputOfT * InverseDirection);
    }

    public void AllowMoveHorizontal()
    {
        _AllowMoveHorizontal = true;
    }

    public void ResetAllControls()
    {
        Climbing = false;
        IsOnLedge = false;
        OnLedge = true;
        TriggerAnim.ClimbLedge(false);
        TriggerAnim.LedgeGrab(false);
    }
}
