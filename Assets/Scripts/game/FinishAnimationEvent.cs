using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.mp.future.warhammer.game
{
    public class FinishAnimationEvent : MonoBehaviour
    {
        void FinishAnimation()
        {
            if (this.transform.parent.GetChild(0).tag == "PlayerUnit")
            {
                GameManager.s_Instance.SendMessage("CBAttackLock");

            }
        }
    }
}