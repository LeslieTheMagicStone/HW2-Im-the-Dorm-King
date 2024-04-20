using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAnimBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out GoblinLogic goblinLogic))
            goblinLogic.SetMovable(false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out GoblinLogic goblinLogic))
            goblinLogic.SetMovable(true);
    }
}
