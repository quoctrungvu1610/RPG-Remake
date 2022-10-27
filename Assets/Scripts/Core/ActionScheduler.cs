using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        //Ham StartAction nhan vao 1 IAction de luon bat buoc goi ham Cancel() cua action do
        //neu currentAction == action thi giu nguyen khong lam gi ca. VD dang combat thi se de nguyen trang thai combat con neu IAction la trang thai dang di chuyen thi currentAction se khac action 
        //nen ta se gan current action = voi action dong thoi goi ham Cancel de huy hoat dong dang lam
        public void StartAction(IAction action)
        {
            if (currentAction == action) return;
            if(currentAction != null)
            {
                currentAction.Cancel();
            }
            currentAction = action;
        }

        //ham CancelCurrentAction se huy tat cac action trong StartAction
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
