using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.mp.future.warhammer.game
{
    public class UnitController : MonoBehaviour
    {
        [Range(0, 40)]
        [SerializeField]
        public float m_MoveRadius;

        [Range(0, 40)]
        [SerializeField]
        public float m_AttackRange;

        [SerializeField]
        public float m_Health;

        [SerializeField]
        public float m_Damage;

        [SerializeField]
        private Vector3 m_Position;
        
        [SerializeField]
        private GameObject m_Target = null;

        // - - - Update function - - - - - - - - - - - - - - - - - - - -

        void Update()
        {
            if (GameManager.s_Instance.GetGameStatus() == GameStatus.STARTUP)
            {
                return;
            }

            if (this.gameObject == GameManager.s_Instance.GetCurrentPlayer().GetComponent<PlayerController>().m_CurrentUnit)
            {
                if (GameManager.s_Instance.GetGameStatus() == GameStatus.ATTACK)
                {
                    GameManager.s_Instance.DrawRadius(this.transform.position, this.transform.rotation, this.m_AttackRange);
                }
            }
        }


        // - - - Public functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Saves the unit start position.
        /// </summary>
        public void StartUp()
        {
            this.m_Position = this.transform.position;
        }

        /// <summary>
        /// Returns the current unit position.
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetPosition()
        {
            return this.m_Position;
        }

        /// <summary>
        /// Checks if new position is inside the move radius of the current unit.
        /// Returns true if unit is inside radius.
        /// </summary>
        /// <returns>bool</returns>
        public bool LockPosition()
        {
            float Distance = Vector3.Distance(this.m_Position, this.transform.position);

            if (Distance < this.m_MoveRadius)
            {
                this.m_Position = this.transform.position;
                Debug.Log("Unit " + this.name + " moved to Position " + this.m_Position.ToString());
                return true;
            }
            else
            {
                Debug.LogWarning("Position is out of Moving Range!");
            }

            return false;
        }

        /// <summary>
        /// Checks if target is inside the attack range of the current unit.
        /// Returns true if target is inside range.
        /// </summary>
        /// <returns>bool</returns>
        public bool LockAttack()
        {
            if (this.m_Target != null)
            {
                UnitController TargetController = this.m_Target.GetComponent<UnitController>();

                float Distance = Vector3.Distance(this.m_Position, TargetController.GetPosition());

                if (Distance < this.m_AttackRange)
                {
                    TargetController.InflictDamage(this.m_Damage);
                    Debug.Log("Unit " + this.name + " attacked " + this.m_Target.name + " and inflicted " + this.m_Damage + " damage");
                    return true;
                }
                else
                {
                    Debug.LogWarning("Target is out of Attack Range!");
                }
            }
            else
            {
                Debug.LogWarning("No Target selected!");
            }

            this.m_Target = null;
            return false;
        }

        /// <summary>
        /// Inflicts a certain amout of Demage to this unit.
        /// Returns true if unit died from the attack.
        /// </summary>
        /// <param name="_Damage"></param>
        /// <returns>bool</returns>
        public bool InflictDamage(float _Damage)
        {
            this.m_Health -= _Damage;

            if (this.m_Health < 0.0f)
            {
                this.Die();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys the GameObject of this unit.
        /// </summary>
        public void Die()
        {
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Sets the current Target for this unit.
        /// Additionally checks if the target equals this units GameObject
        /// and logs a warning in this case.
        /// </summary>
        /// <param name="_Target">GameObject</param>
        public void SetTarget(GameObject _Target)
        {
            if (_Target != this.gameObject)
            {
                this.m_Target = _Target;
                GameManager.s_Instance.SetText(_Target.name, "target");
            }
            else
            {
                Debug.LogWarning("You cannot attack yourself!");
            }

        }
    }
}