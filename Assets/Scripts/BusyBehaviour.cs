using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out PlayerLogic playerLogic))
            playerLogic.SetBusy(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out PlayerLogic playerLogic))
            playerLogic.SetBusy(false);
    }
}
