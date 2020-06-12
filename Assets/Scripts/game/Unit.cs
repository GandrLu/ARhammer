using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// For visual debugging of physical methods like spherecast
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using Vuforia;
using de.mp.future.warhammer.util;

namespace de.mp.future.warhammer.game
{
    public class Unit : MonoBehaviour
    {
        // 3D GameObject that shows selection of unit
        public GameObject m_Aura;
        // 3D Plane Mesh that shows a logo that indicates type of attack
        public GameObject m_Logo;
        public UnitAttributes m_UnitAttributes;
        public string m_OpponentTag = "Enemy";
        public string m_PlayerTag = "PlayerUnit";

        // Flags this unit as active
        private bool m_IsActive = false;
        // Class that handles animations
        private UnitAnimation m_Animation;

        // Ranges
        [SerializeField]
        private float m_MovementRange;

        [SerializeField]
        private float m_AttackRange = 5f;

        // Scanned enemy colliders will be stored here
        private List<GameObject> m_CloseOpponents = new List<GameObject>();

        private Vector3 m_Position;

        // Enum to decide actions 
        public enum AttackMode
        {
            none, melee, ranged
        }
        // Indicates which type of attack this unit can perform
        public AttackMode e_AttackMode = 0;

        /// <summary>
        /// Saves the unit start position. Receives the animations script.
        /// </summary>
        public void StartUp()
        {
            this.m_Position = this.transform.position;
            m_Animation = GetComponent<UnitAnimation>();
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
        /// Returns the movment range.
        /// </summary>
        /// <returns>float</returns>
        public float GetMovementRange()
        {
            return this.m_MovementRange;
        }

        /// <summary>
        /// Returns the attack range.
        /// </summary>
        /// <returns>float</returns>
        public float GetAttackRange()
        {
            return this.m_AttackRange;
        }

        /// <summary>
        /// Checks if new position is inside the move radius of the current unit.
        /// Returns true if unit is inside radius.
        /// </summary>
        /// <returns>bool</returns>
        public bool Move()
        {
            float Distance = Vector3.Distance(this.m_Position, this.transform.position);

            if (this.ScanForTerrain())
            {
                GameManager.s_Instance.DisplayMessage("Cannot place Unit inside of Terrain");
                Debug.LogWarning("Cannot place Unit inside of Terrain");
                return false;
            }

            if (Distance < this.m_MovementRange)
            {
                this.m_Position = this.transform.position;
                Debug.Log("Unit " + this.name + " moved to Position " + this.m_Position.ToString());
                return true;
            }
            else
            {
                GameManager.s_Instance.DisplayMessage("Position is out of Moving Range!");
                Debug.LogWarning("Position is out of Moving Range!");
            }

            return false;
        }
 
        /// <summary>
        /// Perform a melee attack
        /// </summary>
        /// <param name="_foe">Unit</param>
        public void MeleeAttack(Unit _foe)
        {
            if (m_Animation)
            {
                m_Animation.PlayMeleeAttackAnimation();
            }
            for (int i = 0; i < m_UnitAttributes.attacks * m_UnitAttributes.troopSize && _foe.m_UnitAttributes.troopSize > 0; i++)
            {
                bool notFailed = true;
                // Allocate if unit hits the foe, uses weapon skill
                int roll = Random.Range(1, 6 + 1);
                //Debug.Log("Hit Roll: " + roll);
                if (m_UnitAttributes.weaponSkill > roll)
                {
                    Debug.Log(this.name + "'s attack has failed! (Hit)");
                    GameManager.s_Instance.DisplayMessage(this.name + "'s attack has failed! (Hit)");
                    notFailed = false;
                }
                // Allocate if unit hurts the foe
                if (notFailed)
                {
                    notFailed = HurtOppenent(this, _foe);
                }
                // Allocate if foes save value saves it
                // Melee does not support cover
                roll = Random.Range(1, 6 + 1);
                //Debug.Log("Save Roll: " + roll);
                if (notFailed && _foe.m_UnitAttributes.save <= roll)
                {
                    Debug.Log(this.name + "'s attack has failed! (Save)");
                    notFailed = false;
                }
                if (notFailed)
                {
                    _foe.ReceiveDamage(1);
                    Debug.Log(this.name + "'s attack was successfull!");
                }
            }

            // When opponent is still alive at end of attack, it performs a counter attack
            if (_foe.m_UnitAttributes.troopSize > 0)
            {
                if (_foe.m_Animation)
                {
                    _foe.m_Animation.PlayMeleeAttackAnimation();
                }
                for (int i = 0; i < _foe.m_UnitAttributes.troopSize; i++)
                {
                    bool notFailed = true;
                    // Allocate if unit hits the foe, uses weapon skill
                    int roll = Random.Range(1, 6 + 1);
                    //Debug.Log("Hit Roll: " + roll);
                    if (_foe.m_UnitAttributes.weaponSkill > roll)
                    {
                        Debug.Log(_foe + "'s counter attack has failed! (Hit)");
                        notFailed = false;
                    }
                    // Allocate if unit hurts the foe
                    if (notFailed)
                    {
                        notFailed = HurtOppenent(_foe, this);
                    }
                    // Allocate if foes save value saves it
                    // Melee does not support cover
                    roll = Random.Range(1, 6 + 1);
                    //Debug.Log("Save Roll: " + roll);
                    if (notFailed && this.m_UnitAttributes.save <= roll)
                    {
                        Debug.Log(_foe.name + "'s counter attack has failed! (Save)");

                        notFailed = false;
                    }
                    if (notFailed)
                    {
                        this.ReceiveDamage(1);
                        Debug.Log(_foe.name + "'s counter attack was successfull!");

                    }
                }
            }
        }

        /// <summary>
        /// Perform a ranged attack
        /// </summary>
        /// <param name="_foe">Unit</param>
        public void RangedAttack(Unit _foe)
        {
            RaycastHit[] allHits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), m_AttackRange * 0.5f);
            RaycastHit directHit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out directHit, m_AttackRange * 0.5f, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Game, 80f, Color.cyan, Color.red))
            {
                Debug.Log(directHit.collider.tag);
            }
            
            if (m_Animation)
            {
                m_Animation.PlayRangedAttackAnimation(m_UnitAttributes.attacks);
            }
            for (int j = 0; j < m_UnitAttributes.troopSize && _foe.m_UnitAttributes.troopSize > 0; j++)
            {
                bool notFailed = false;
                foreach (RaycastHit hit in allHits)
                {
                    if (hit.transform.parent.name == _foe.transform.name)
                    {
                        notFailed = true;
                    }
                }

                // Allocate if unit hits the foe, uses ballistic skill
                if (notFailed && directHit.collider != null && directHit.collider.tag == "FullCover")
                {
                    notFailed = false;
                }
                int roll = Random.Range(1, 6 + 1);
                //Debug.Log("Hit Roll: " + roll);
                if (notFailed && 6 - m_UnitAttributes.ballisticSkill > roll)
                {
                    Debug.Log(this.name + "'s attack has failed! (Hit)");

                    notFailed = false;
                }
                // Allocate if unit hurts the foe
                if (notFailed)
                {
                    notFailed = HurtOppenent(this, _foe);
                }
                // Allocate if foes save value saves it
                // TODO: Will be extended to support cover values of terrain
                roll = Random.Range(1, 6 + 1);
                if (directHit.collider != null && directHit.collider.tag == "Cover")
                {
                    roll += 2;
                }
                //Debug.Log("Save Roll: " + roll);
                if (notFailed && _foe.m_UnitAttributes.save <= roll)
                {
                    Debug.Log(this.name + "'s attack has failed! (Save)");

                    notFailed = false;
                }
                if (notFailed)
                {
                    _foe.ReceiveDamage(1);
                    Debug.Log(this.name + "'s attack was successfull!");

                }
            }
        }

        /// <summary>
        /// Calculates if the opponent of this unit is hurt, according to strength of attacker and toughness of defender
        /// </summary>
        /// <param name="_attacker">Unit</param>
        /// <param name="_foe">Unit</param>
        /// <returns>bool</returns>
        bool HurtOppenent(Unit _attacker, Unit _foe)
        {
            bool notFailed = true;
            // Allocate what value unit needs to hurt the foe
            float strengthToughnessRatio = _attacker.m_UnitAttributes.strength / _foe.m_UnitAttributes.toughness;
            int requiredRoll = 6;
            if (strengthToughnessRatio < 0.5)
            {
                requiredRoll = 6;
            }
            else if (strengthToughnessRatio < 1)
            {
                requiredRoll = 5;
            }
            else if (strengthToughnessRatio == 1)
            {
                requiredRoll = 4;
            }
            else if (strengthToughnessRatio > 1)
            {
                requiredRoll = 3;
            }
            else if (strengthToughnessRatio >= 2)
            {
                requiredRoll = 2;
            }
            // Allocate if unit hurts the foe
            int roll = Random.Range(1, 6 + 1);
            //Debug.Log("Hurt Roll: " + roll);
            if (notFailed && requiredRoll > roll)
            {
                Debug.Log(_attacker.name + "'s attack has failed! (Hurt)");
                notFailed = false;
            }
            return notFailed;
        }

        /// <summary>
        /// Inflicts an certain amount of damage to the unit.
        /// </summary>
        /// <param name="_Amount">int</param>
        public void ReceiveDamage(int _Amount)
        {
            // deactivate / destroy unit models the show reducement
            m_UnitAttributes.troopSize -= _Amount;
            //if (m_UnitAttributes.troopSize <= 0)
            //{
            for (int i = 0; i < _Amount; i++)
            {
                KillUnit();
            }
            //}
        }

        /// <summary>
        /// Deactivates the unit and plays the death animation.
        /// </summary>
        void KillUnit()
        {
            m_Animation.PlayDeathAnimation();
            Debug.Log(this.name + " was killed!");
            GameManager.s_Instance.DisplayMessage(this.name + " was killed!");
            if (this.m_UnitAttributes.troopSize <= 0)
            {
                this.transform.GetComponentInChildren<Collider>().enabled = false;
            }
            //Destroy(this.transform.parent.gameObject, 5);
        }

        /// <summary>
        /// Checks for enemies in melee distance and returns if are any near
        /// </summary>
        /// <returns>bool</returns>
        public bool CheckForMeleeEnemies()
        {
            float meleeDistance = m_Aura.transform.localScale.x * 0.5f;
            List<Collider> opponentColliders = new List<Collider>();

            //opponentColliders.AddRange(Physics.OverlapSphere(transform.position, meleeDistance, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor, 90, Color.yellow, Color.cyan));
            opponentColliders.AddRange(Physics.OverlapSphere(transform.position, meleeDistance));

            int opponentCollidersCount = opponentColliders.Count;
            for (int i = opponentCollidersCount - 1; i >= 0; --i)
            {
                if (opponentColliders[i].tag != m_OpponentTag)
                {
                    opponentColliders.RemoveAt(i);
                }
            }
            if (opponentColliders.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Scans enemy colliders for attack (ranged distance)
        public void ScanForEnemies(AttackMode _attackMode)
        {
            float attackRange = 0f;
            switch (_attackMode)
            {
                case Unit.AttackMode.melee:
                    attackRange = m_Aura.transform.localScale.x * 0.5f;
                    break;
                case Unit.AttackMode.ranged:
                    attackRange = m_AttackRange * 0.5f;
                    break;
                default:
                    break;
            }

            Collider[] opponentColliders = Physics.OverlapSphere(transform.position, attackRange);
            //Collider[] opponentColliders = Physics.OverlapSphere(transform.position, attackRange, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor, 90, Color.green, Color.red);

            foreach (Collider collider in opponentColliders)
            {
                if (collider.transform.parent.gameObject != this.gameObject && collider.tag == m_OpponentTag)
                {
                    m_CloseOpponents.Add(collider.transform.parent.gameObject);
                    Unit opponent = collider.transform.parent.GetComponent<Unit>();
                    opponent.m_Aura.SetActive(true);
                    switch (_attackMode)
                    {
                        case Unit.AttackMode.melee:
                            if (opponent.m_Logo)
                            {
                                opponent.m_Logo.GetComponent<FloatingText>().SetSwordLogo();
                            }
                            break;
                        case Unit.AttackMode.ranged:
                            if (opponent.m_Logo)
                            {
                                opponent.m_Logo.GetComponent<FloatingText>().SetBowLogo();
                            }
                            break;
                        default:
                            break;
                    }
                    opponent.m_Logo.SetActive(true);
                    opponent.m_Aura.GetComponentInChildren<Renderer>().material.color = Color.red;
                }
            }
        }

        /// <summary>
        /// Scans the current unit position for terrain objects.
        /// Returns true if there are any colliding terrain objects.
        /// </summary>
        /// <returns>bool</returns>
        public bool ScanForTerrain()
        {
            Collider[] TerrainColliders = Physics.OverlapSphere(transform.position, transform.position.y);

            foreach (Collider TerrainCollider in TerrainColliders)
            {
                if (TerrainCollider.gameObject.tag == "Terrain")
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Turns the unit active or inactive
        /// Currently also scans for enemies in ranged distance
        /// </summary>
        /// <returns>bool</returns>
        public bool SwitchActivity()
        {
            if (m_Aura)
            {
                m_Aura.SetActive(!m_Aura.activeSelf);
                m_IsActive = !m_IsActive;
            }
            if (m_IsActive)
            {
                // Add enemy markers when active
                //ScanForEnemies(m_AttackRange);
            }
            else
            {
                // Delete enemy markers when inactive
                foreach (GameObject marker in m_CloseOpponents)
                {
                    // Avoid it has been destroyed by killing the unit
                    if (marker)
                    {
                        marker.transform.GetComponent<Unit>().m_Aura.GetComponent<Renderer>().material.color = Color.green;
                        marker.transform.GetComponent<Unit>().m_Aura.SetActive(false);
                        marker.transform.GetComponent<Unit>().m_Logo.SetActive(false);
                    }
                }
            }
            return m_IsActive;
        }

        void Start()
        {
            m_Aura.SetActive(false);
        }
    }
}
