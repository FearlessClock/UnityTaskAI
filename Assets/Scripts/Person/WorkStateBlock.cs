using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Person
{
    public class WorkStateBlock : StateBlock
    {
        private PersonAIDebugHolder debugHolder = null;
        private AnimationCommandController animatorController = null;
        private PersonTaskHandler taskHandler = null;

        public WorkStateBlock(PersonAIDebugHolder debug, AnimationCommandController animator, PersonTaskHandler taskHandler)
        {
            debugHolder = debug;
            animatorController = animator;
            this.taskHandler = taskHandler;
        }

        public void Entry()
        {
            debugHolder.Log("Working Entry Called", eDebugImportance.Entry);
            if (taskHandler.ActiveTask.DoesWork)
            {
                animatorController.ChangeState(taskHandler.ActiveTask.GetWorkAnimationType);
                taskHandler.ActiveTask.SetWorkTimer(taskHandler.ActiveTask.GetWorkTime);
            }
        }

        public void Exit()
        {
            debugHolder.Log("Working Exit Called", eDebugImportance.Exit);
            if (taskHandler.IsActiveTaskValid)
            {
                taskHandler.SetNewFollowUpTask();
            }
        }

        /// <summary>
        /// While the person is in the working state, Check if the work is done
        /// pop back to idle. Otherwise, keep working at the problem
        /// </summary>
        public bool Update()
        {
            if (!taskHandler.IsActiveTaskValid)
            {
                return true;
            }
            if (!taskHandler.ActiveTask.DoesWork)
            {
                taskHandler.ActiveTask.GetWorkDoneFunction?.Invoke();
                return true;
            }

            taskHandler.ActiveTask.UpdateWorkTimer(Time.deltaTime);
            if (taskHandler.ActiveTask.IsWorkDone)
            {
                debugHolder.Log("Work is done", eDebugImportance.State);
                taskHandler.ActiveTask.GetWorkDoneFunction?.Invoke();
                animatorController.ChangeState(taskHandler.ActiveTask.GetWorkAnimationType);
                return true;
            }
            return false;
        }
    }
}