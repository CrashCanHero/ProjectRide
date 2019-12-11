using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirbyFaceSetter : StateMachineBehaviour {
    public int index;
    KirbyAnimationHandler animationHandler;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animationHandler = animator.GetComponent<KirbyAnimationHandler>();
        animationHandler.UpdateFace(index);
    }
}