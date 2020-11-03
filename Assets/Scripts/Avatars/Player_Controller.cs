using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    //ToDo's
    // Must fix StandUp complete animation gives player back controlls
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
    private bool Climbing = false;
    private bool OnLedge = false;
    private Player_LedgeGrab LedgeDetection_Offset;
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
    private Vector3 _LedgePlatformPosition;

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
        if (LedgePlatformPosition && LedgePlatformPosition.tag == "MovingPlatform")
        {
            Vector3 _LedgeOffset = _GrabableLedge.ReverseLedgeGrab(InverseDirection);
            Vector3 _LedgeDetection_Offset = LedgeDetection_Offset.ReverseLedgeGrab(InverseDirection);
            _LedgePlatformPosition = _LedgeOffset - _LedgeDetection_Offset;
        }
        else
        {
            _LedgePlatformPosition = Vector3.zero;
         }
        if(Input.GetAxisRaw("Vertical") < 0 && IsOnLedge)
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
            if (Grounded) MoveVertical.y = 0;
            PlayerController.Move(_MovingPlatform + (MoveVertical + (MoveHorizontal * SpeedWMultiplier(MoveHorizontal.z))) * Time.fixedDeltaTime);
        }
        Vector3 HorizontalAnimation = new Vector3(0, 0 , Input.GetAxis("Horizontal"));
        if(!IsOnLedge)TriggerAnim.CharacterAnimations((MoveVertical + HorizontalAnimation * InverseDirection));
        ClifHanger();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(!PlayerController.isGrounded && hit.normal == (Vector3.back * InverseDirection) && !IsOnLedge)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.black);
            _GrabableLedge = hit.gameObject.GetComponent<LedgeGrab>();
            if(_GrabableLedge != null)
            {
                Debug.Break();
                  LedgePlatformPosition = hit.gameObject.transform;
                IsOnLedge = true;
                _AllowMoveHorizontal = false;
                Vector3 _LedgeOffset = _GrabableLedge.ReverseLedgeGrab(InverseDirection);
                Vector3 _LedgeDetection_Offset = LedgeDetection_Offset.ReverseLedgeGrab(InverseDirection);
                ArmsReachPosition = _LedgeOffset + (transform.position - _LedgeDetection_Offset);
                GrabSpeed = Vector3.Distance(LedgeDetection_Offset.transform.position, ArmsReachPosition);
                float DistanceDelta = Mathf.Clamp((GrabSpeed - 8.81437f) / 4.11763f, 0, 1);
                GrabSpeed *= Mathf.Lerp(1, 1.917f, DistanceDelta);
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

    private void ClifHanger()
    {
        if (IsOnLedge)
        {
            if (Jumped)
            {
                Jumped = false;
                Climbing = true;
                TriggerAnim.ClimbLedge(true);
            }
            if (Climbing)
            {
                if (!OnLedge)
                {
                    Vector3 _ModelPosition = TriggerAnim.ModelPosition;
                    _ModelPosition.z *= InverseDirection;
                    transform.position = MovingPlatformOffset(transform.position) + _ModelPosition;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(MovingPlatformOffset(transform.position), MovingPlatformOffset(LedgeStandUpPosition), Time.fixedDeltaTime * 4.535f);
                    if (transform.position == MovingPlatformOffset(LedgeStandUpPosition))
                    {
                        LedgePlatformPosition = null;
                        _AllowMoveHorizontal = true;
                        OnLedge = false;
                        Climbing = false;
                        IsOnLedge = false;
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(MovingPlatformOffset(transform.position), MovingPlatformOffset(ArmsReachPosition), Time.fixedDeltaTime * GrabSpeed);
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

    private Vector3 MovingPlatformOffset(Vector3 movingOffsetPosition)
    {
         Vector3 _movingOffsetPosition = movingOffsetPosition;
        _movingOffsetPosition.z += _LedgePlatformPosition.z;
        return _movingOffsetPosition;
    }
    public void ClimbSequence()
    {

        SuperLanding = false;
        LedgeStandUpPosition = transform.position;
        LedgeStandUpPosition.y += (3-0.369f);
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
