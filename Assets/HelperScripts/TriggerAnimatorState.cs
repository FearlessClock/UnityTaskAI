using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(Animator))]
public class TriggerAnimatorState : MonoBehaviour
{
    private Animator animator = null;
    [SerializeField] private string triggerName = "";
    private void OnEnable() {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(){
        animator.SetTrigger(triggerName);
    }
}
