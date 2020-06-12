using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.mp.future.warhammer.game
{
    /// <summary>
    /// A script that handles the animation and offers a simple interface to 
    /// start them. Because one of the used models does not (directly) support 
    /// animator usage, animation component has to be used, what complicates
    /// the general usage a little bit. But this script handles it, as long as
    /// the game hierachy is fitting. Models with animators like the used 
    /// animators will have no problem but models with animation component 
    /// need to be adjusted.
    /// </summary>
    public class UnitAnimation : MonoBehaviour
    {
        private bool m_UsesAnimator;
        private Unit unit;
        private Animator[] m_TroopModelsAnimators;
        private Animation[] m_TroopModelsAnimations;
        private Animator[] m_ShotAnimators;

        /// <summary>
        /// Play the death animation of the attached units. Uses animator parameter or direct animation, depending on what component is used.
        /// </summary>
        public void PlayDeathAnimation()
        {
            switch (m_UsesAnimator)
            {
                case true:
                    if (unit.m_UnitAttributes.troopSize >= 0)
                    {
                        m_TroopModelsAnimators[unit.m_UnitAttributes.troopSize].SetBool("dead", true);
                    }
                    break;
                case false:
                    if (unit.m_UnitAttributes.troopSize >= 0)
                    {
                        Debug.Log("Death " + unit.m_UnitAttributes.troopSize);
                        m_TroopModelsAnimations[unit.m_UnitAttributes.troopSize].Stop();
                        m_TroopModelsAnimations[unit.m_UnitAttributes.troopSize].Play("death");
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Plays the melee animation of attached units. Uses animator parameter or direct animation, depending on what component is used.
        /// </summary>
        public void PlayMeleeAttackAnimation()
        {
            switch (m_UsesAnimator)
            {
                case true:
                    foreach (Animator animator in m_TroopModelsAnimators)
                    {
                        animator.SetTrigger("meleeAttack");
                    }
                    break;
                case false:
                    for (int i = 0; i < unit.m_UnitAttributes.troopSize; ++i)
                    {
                        m_TroopModelsAnimations[i].Play("meleeAttack");
                        m_TroopModelsAnimations[i].PlayQueued("idle", QueueMode.CompleteOthers);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Plays the melee animation of attached units. Uses animator parameter or direct animation, depending on what component is used.
        /// </summary>
        /// <param name="_HowManyTimes">The amount of animations, used by direct animation method.</param>
        public void PlayRangedAttackAnimation(int _HowManyTimes = 1)
        {
            switch (m_UsesAnimator)
            {
                case true:
                    foreach (Animator animator in m_TroopModelsAnimators)
                    {
                        animator.SetTrigger("rangedAttack");
                    }
                    break;
                case false:
                    for(int i = 0; i < unit.m_UnitAttributes.troopSize; ++i)
                    {
                        m_TroopModelsAnimations[i].Play("rangedAttack");
                        if (_HowManyTimes > 1)
                        {
                            for (int j = 1; j < _HowManyTimes; j++)
                            {
                                m_TroopModelsAnimations[i].PlayQueued("rangedAttack", QueueMode.CompleteOthers);
                            }
                            m_ShotAnimators[i].Play("shot2");
                        }
                        else
                        {
                            m_ShotAnimators[i].Play("shot");
                        }
                        m_TroopModelsAnimations[i].PlayQueued("idle", QueueMode.CompleteOthers);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Collects animators or animation of the attached unit. Determines the component to use by that.
        /// </summary>
        void Start()
        {
            unit = GetComponent<Unit>();
            m_TroopModelsAnimators = gameObject.GetComponentsInChildren<Animator>();
            m_TroopModelsAnimations = gameObject.GetComponentsInChildren<Animation>();
            m_ShotAnimators = new Animator[unit.m_UnitAttributes.troopSize];

            if (m_TroopModelsAnimators.Length > 0)
            {
                m_UsesAnimator = true;
            }
            else
            {
                m_UsesAnimator = false;
                for (int i = 0; i < m_TroopModelsAnimations.Length; i++)
                {
                    m_TroopModelsAnimations[i].transform.GetChild(0).gameObject.SetActive(true);
                    m_ShotAnimators[i] = m_TroopModelsAnimations[i].transform.GetChild(0).GetComponent<Animator>();
                }
            }
        }
    }
}