using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatterToTop : StateMachineBehaviour
{
    [SerializeField]
    private float TriggerExitTime;
    private bool _ExitOneShot = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= TriggerExitTime && !_ExitOneShot)
        {
            _ExitOneShot = true;
            Player_Controller PlayerController = animator.transform.parent.gameObject.GetComponent<Player_Controller>();
            if (PlayerController) PlayerController.IKModelTransformOffsetBool();
            else Debug.Log("PlayerController is null in AnimatorBehaviour");
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ExitOneShot = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
