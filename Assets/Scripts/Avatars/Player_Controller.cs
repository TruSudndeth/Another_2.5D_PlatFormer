using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    //ToDo's
    // Player can't climb while a shild of elevator
    // Fix PreClimb don't allow jump while animation wall hang idle isn't done.
    // On landing choose to roll forwards or dissable input till landing animation is over.
    // on moving Backwards jump landing automaticly rolls backwards. with an option to back power flip.
    [HideInInspector]
    public Transform MovingPlatform;
    private Vector3 _MovingPlatform;
    [SerializeField]
    private float OffSet_Y;
    private float GrabSpeed;
    private Vector3 LedgeStandUpPosition;
    private bool Climbing = false;
    private bool OnLedge = false;
    private Player_LedgeGrab LedgeDetection_Offset;
    private bool IsOnLedge = false;
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
    private bool Jumped = false;
    private bool Jumped_FixedUpdate = false;
    private bool SuperLanding = false;
    private IEnumerator _Climbed;
    private bool GravityT = true;
    private bool Grounded = false;
    private bool LandPosition = false;
    private float PlayerPlatformOffset = 0;
    private bool ZeroGravityOnLanding_thisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        LedgeDetection_Offset = GetComponentInChildren<Player_LedgeGrab>();
        PlayerRotation = transform.localEulerAngles;
        SuperLanding = true;
        PlayerController = GetComponent<CharacterController>();
        TriggerAnim = GetComponentInChildren<Player_ControlAnimation>();
        if (PlayerController == null) Debug.Log("PlayerController is null at void Start");
        if (TriggerAnim == null) Debug.Log("Player_ControlAnimation is null at void start");
    }

    // Update is called once per frame
    void Update()
    {
        if(MovingPlatform == null && !IsOnLedge)
        {
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
                    PlayerPlatformOffset = (transform.position.z - MovingPlatform.position.z) * 2;
                }
            if(Input.GetAxis("Horizontal") != 0)
            {
                
                MoveHorizontal.z += Input.GetAxis("Horizontal");
            }
            _MovingPlatform = MovingPlatform.position - new Vector3(transform.position.x, transform.position.y + OffSet_Y, transform.position.z + PlayerPlatformOffset);
        }
        if(Input.GetKeyDown(KeyCode.Space) && !Jumped && !Jumped_FixedUpdate) Jumped = true;
        if(Input.GetAxis("Horizontal") != 0 && (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
        {
                if (MoveHorizontal.z > 0)
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
                MoveVertical = Vector3.zero;
                ZeroGravityOnLanding_thisFrame = true;
                LandingDamage = Mathf.Abs(MoveVertical.y / 300);
                SuperLanding = false;
                TriggerAnim.CharactorAnimaition_Landing(LandingDamage);
                LandingDamage = 0;
            }
            if(Jumped_FixedUpdate)Jumped_FixedUpdate = false;
            if(Jumped && !Jumped_FixedUpdate)
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
            if (PlayerController.isGrounded && !Jumped_FixedUpdate && !IsOnLedge && !Grounded && !ZeroGravityOnLanding_thisFrame)
            {
                ZeroGravityOnLanding_thisFrame = false;
                MoveVertical.y = -1;
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
        if (!IsOnLedge)
        {
            transform.rotation = Quaternion.Euler(PlayerRotation);
            if (Grounded) MoveVertical.y = 0;
            PlayerController.Move(_MovingPlatform + (MoveVertical + (MoveHorizontal * Mathf.Lerp(Speed, MaxSpeed, MoveHorizontal.z * InverseDirection))) * Time.fixedDeltaTime);
        }
        TriggerAnim.CharacterAnimations((MoveVertical + MoveHorizontal.normalized * InverseDirection));
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
                IsOnLedge = true;
                Vector3 _LedgeOffset = _GrabableLedge.ReverseLedgeGrab(InverseDirection);
                Vector3 _LedgeDetection_Offset = LedgeDetection_Offset.ReverseLedgeGrab(InverseDirection);
                ArmsReachPosition = (_LedgeOffset - _LedgeDetection_Offset);
                
                GrabSpeed = Vector3.Distance(LedgeDetection_Offset.transform.position, ArmsReachPosition);
                float DistanceDelta = Mathf.Clamp((GrabSpeed - 8.81437f) / 4.11763f, 0, 1);
                GrabSpeed *= Mathf.Lerp(1, 1.917f, DistanceDelta);
                TriggerAnim.LedgeGrab(true);
                Jumped_FixedUpdate = false;
                MoveHorizontal = Vector3.zero;
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
                    transform.position += _ModelPosition;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, LedgeStandUpPosition, Time.fixedDeltaTime * 4.535f);
                    if (transform.position == LedgeStandUpPosition)
                    {
                        OnLedge = false;
                        Climbing = false;
                        IsOnLedge = false;
                    }
                }
            }
            else transform.position = Vector3.MoveTowards(transform.position, ArmsReachPosition, Time.fixedDeltaTime * GrabSpeed);
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
    public void ResetAllControls()
    {
        
        SuperLanding = false;
        LedgeStandUpPosition = transform.position;
        LedgeStandUpPosition.y += (3-0.369f);
        OnLedge = true;
        TriggerAnim.ClimbLedge(false);
        TriggerAnim.LedgeGrab(false);
    }
}
