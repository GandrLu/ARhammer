using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace de.mp.future.warhammer.game
{
    public class PlayerController : MonoBehaviour
    {
        public List<GameObject> m_Units;

        public GameObject m_CurrentUnit;

        public Unit m_CurrentUnitController;

        public Unit m_SelectedPlayerUnit = null;

        public int m_CurrentUnitID;

        public bool m_IsMoving = false;

        public bool m_IsAttacking = false;

        public bool m_IsStarting = false;

        string hitName;

        public Text text;


        // - - - Update function - - - - - - - - - - - - - - - - - - - -


        void Update()
        {
            if (GameManager.s_Instance.IsCurrentPlayer(this.gameObject))
            {
                switch (GameManager.s_Instance.GetGameStatus())
                {
                    case GameStatus.STARTUP:
                        if(!m_IsStarting)
                        {
                            Starting();
                        }
                        break;

                    case GameStatus.MOVING:
                        if (!m_IsMoving)
                        {
                            Moving();
                        }
                        break;

                    case GameStatus.ATTACK:
                        if (!m_IsAttacking)
                        {
                            Attacking();
                        }

                        CheckSelection();
                        break;

                    case GameStatus.NEXTUNIT:
                        NextUnit();
                        break;

                    case GameStatus.NEXTMOVE:
                        NextUnit();
                        break;

                    case GameStatus.NEXTATTACK:
                        NextUnit();
                        break;

                    case GameStatus.NEXTSTART:
                        NextStart();
                        break;

                    case GameStatus.FINISHED:
                        GameManager.s_Instance.SendMessage("CBEndRound");
                        break;
                }
            }
        }

        private void Start()
        {
            foreach (Transform Child in this.transform)
            {
                if (Child.gameObject.activeSelf == true)
                {
                    m_Units.Add(Child.gameObject);
                }
            }

            m_CurrentUnit = m_Units[0];
            m_CurrentUnitController = m_CurrentUnit.GetComponent<Unit>();
            m_CurrentUnitID = 0;
        }

        // - - - Public functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Starts the game and calls the StartUp() function on all units.
        /// </summary>
        public void StartUp()
        {
            foreach (GameObject Unit in m_Units)
            {
                Unit.GetComponent<Unit>().StartUp();
            }
        }

        /// <summary>
        /// Returns the current unit.
        /// </summary>
        /// <returns>GameObject</returns>
        public GameObject GetCurrentUnit()
        {
            return this.m_CurrentUnit;
        }

        /// <summary>
        /// Sets the next active Unit.
        /// When the last unit is reached - calls the next round.
        /// </summary>
        public void NextUnit()
        {
            if (this.m_CurrentUnitID + 1 >= this.m_Units.Count)
            {
                this.m_CurrentUnitID = 0;
                this.m_CurrentUnit = this.m_Units[0];

                if (GameManager.s_Instance.GetLastGameStatus() == GameStatus.MOVING)
                {
                    GameManager.s_Instance.SendMessage("CBAttack");
                }
                else if (GameManager.s_Instance.GetLastGameStatus() == GameStatus.ATTACK)
                {
                    GameManager.s_Instance.SendMessage("CBFinished");
                }
                else if (GameManager.s_Instance.GetLastGameStatus() == GameStatus.NEXTUNIT ||
                         GameManager.s_Instance.GetLastGameStatus() == GameStatus.NEXTMOVE ||
                         GameManager.s_Instance.GetLastGameStatus() == GameStatus.NEXTATTACK
                        )
                {
                    GameManager.s_Instance.SendMessage("CBFinished");
                }
            }
            else
            {
                this.m_CurrentUnitID++;
                this.m_CurrentUnit = this.m_Units[this.m_CurrentUnitID];

                if(GameManager.s_Instance.GetGameStatus() == GameStatus.NEXTMOVE)
                {
                    GameManager.s_Instance.SetGameStatus(GameStatus.MOVING);
                }
                else if(GameManager.s_Instance.GetGameStatus() == GameStatus.NEXTATTACK)
                {
                    GameManager.s_Instance.SetGameStatus(GameStatus.ATTACK);
                }
            }

            this.m_CurrentUnitController = GetUnitController();
        }

        /// <summary>
        /// Sets the next unit for start postion lock.
        ///  When the last unit is reached - calls the next round.
        /// </summary>
        public void NextStart()
        {
            if (this.m_CurrentUnitID + 1 >= this.m_Units.Count)
            {
                this.m_CurrentUnitID = 0;
                this.m_CurrentUnit = this.m_Units[0];

                GameManager.s_Instance.SendMessage("CBStartFinished");

                this.m_IsStarting = false;
            }
            else
            {
                this.m_CurrentUnitID++;
                this.m_CurrentUnit = this.m_Units[this.m_CurrentUnitID];
                GameManager.s_Instance.SetGameStatus(GameStatus.STARTUP);
                GameManager.s_Instance.DisplayMessage("Please choose start position for " + m_CurrentUnit.name);
            }
            
            this.m_CurrentUnitController = GetUnitController();
        }

        /// <summary>
        /// Handles the active unit position lock.
        /// </summary>
        public void LockPosition()
        {
            if (this.m_CurrentUnitController.Move())
            {
                GameManager.s_Instance.SendMessage("CBPositionLock");
                GameManager.s_Instance.DeleteRadius();
                this.m_IsMoving = false;
            }
        }

        /// <summary>
        /// Handles the start postion lock.
        /// </summary>
        public void LockStart()
        {
            this.m_CurrentUnitController.StartUp();
            GameManager.s_Instance.SendMessage("CBStartLock");
        }

        /// <summary>
        /// Returns the UnitController of the current active unit.
        /// </summary>
        /// <returns></returns>
        private Unit GetUnitController()
        {
            return this.m_CurrentUnit.GetComponent<Unit>();
        }

        /// <summary>
        /// Taggs all units of this player either as PlayerUnit or Enemy. 
        /// </summary>
        /// <param name="IsCurrentPlayer">bool</param>
        public void MarkUnits(bool IsCurrentPlayer)
        {
            foreach (GameObject Unit in m_Units)
            {
                if (IsCurrentPlayer)
                {
                    Unit.transform.GetChild(0).tag = "PlayerUnit";
                }
                else
                {
                    Unit.transform.GetChild(0).tag = "Enemy";
                }
            }
        }

        /// <summary>
        /// Skips the attack round for the current unit.
        /// </summary>
        public void SkipAttack()
        {
            m_SelectedPlayerUnit = null;
            GameManager.s_Instance.SendMessage("CBAttackLock");
            m_IsAttacking = false;
        }

        /// <summary>
        /// Returns true if all units of this player are defeated.
        /// </summary>
        /// <returns>bool</returns>
        public bool CheckGameOver()
        {
            foreach (GameObject Unit in m_Units)
            {
                Unit UController = Unit.GetComponent<Unit>();

                if (UController.m_UnitAttributes.troopSize > 0)
                {
                    return false;
                }
            }

            return true;
        }

        // - - - Private functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Handles the radius display on first frame in the move phase.
        /// </summary>
        private void Moving()
        {
            GameManager.s_Instance.ToggleMoveButton();
            GameManager.s_Instance.DrawRadius(this.m_CurrentUnitController.GetPosition(), new Quaternion(0f, 0f, 0f, 0f), this.m_CurrentUnitController.GetMovementRange(), GameManager.s_Instance.m_WorldAnchor);
            this.m_IsMoving = true;
        }

        /// <summary>
        /// Handles the radius display on first frame in the attack phase.
        /// </summary>
        private void Attacking()
        {
            foreach (GameObject unit in m_Units)
            {

                // Get enemys in melee distance
                if (unit.GetComponent<Unit>().CheckForMeleeEnemies())
                {
                    // Melee mode
                    Debug.Log("Melee distance");
                    unit.GetComponent<Unit>().e_AttackMode = Unit.AttackMode.melee;
                    this.m_IsAttacking = true;
                }
                else if (m_CurrentUnitController.m_UnitAttributes.hasRangedAttack)
                {
                    // Ranged mode
                    Debug.Log("Ranged distance");
                    unit.GetComponent<Unit>().e_AttackMode = Unit.AttackMode.ranged;
                    this.m_IsAttacking = true;
                }
                else
                {
                    // Skip
                    Debug.Log("No distance");
                    unit.GetComponent<Unit>().e_AttackMode = Unit.AttackMode.none;
                    GameManager.s_Instance.SendMessage("CBAttackLock");
                    this.m_IsAttacking = false;
                }
            }
        }

        private void Starting()
        {
            this.m_IsStarting = true;
            GameManager.s_Instance.DisplayMessage("Please choose start position for " + m_CurrentUnit.name);
        }

        /// <summary>
        /// Gets touch and mouse input and sets the target for a possible attack.
        /// </summary>
        private void CheckSelection()
        {
            // Change jk: First check that the player is currently attacking
            if (GameManager.s_Instance.GetGameStatus() == GameStatus.ATTACK)
            {
                // Change jk: Outsourced Click Handling to function HandleClick()
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                {
                    HandleClick(Input.GetTouch(0).position);
                }
                else if (Input.GetButtonDown("Fire1"))
                {
                    HandleClick(Input.mousePosition);
                }
            }
        }

        /// <summary>
        /// Checks if touch / click hits any possible targets and handles the attack.
        /// </summary>
        /// <param name="_Position"></param>
        private void HandleClick(Vector3 _Position)
        {
            Ray ray = Camera.main.ScreenPointToRay(_Position);
            RaycastHit Hit;

            if (Physics.Raycast(ray, out Hit))
            {
                hitName = Hit.transform.tag;
                //Debug.Log(hitName);

                switch (hitName)
                {
                    case "PlayerUnit":

                        Debug.LogWarning("CASE PlayerUnit");
                        // Change jk: Unit script is on the parent now
                        Unit unit = Hit.transform.parent.GetComponent<Unit>();

                        // When already an unit is active and it is not the current selected, deselect it
                        if (m_SelectedPlayerUnit && m_SelectedPlayerUnit != unit)
                        {
                            m_SelectedPlayerUnit.SwitchActivity();
                        }

                        // Turns selected unit active or non active
                        if (unit.SwitchActivity())
                        {
                            m_SelectedPlayerUnit = unit;
                            switch (unit.e_AttackMode)
                            {
                                case Unit.AttackMode.none:
                                    break;
                                case Unit.AttackMode.melee:
                                    m_SelectedPlayerUnit.ScanForEnemies(Unit.AttackMode.melee);
                                    break;
                                case Unit.AttackMode.ranged:
                                    m_SelectedPlayerUnit.ScanForEnemies(Unit.AttackMode.ranged);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            m_SelectedPlayerUnit = null;
                        }
                        break;

                    // Select marked enemy to attack
                    // TODO Will be replaced by "ranged" and "melee attack" case
                    case "Enemy":

                        Debug.LogWarning("CASE Enemy");
                        
                        Unit uunit = Hit.transform.parent.GetComponent<Unit>();
                        
                        if (m_SelectedPlayerUnit && uunit.m_Aura.activeSelf)
                        {
                            Debug.LogWarning(m_SelectedPlayerUnit.e_AttackMode.ToString());
                            switch (m_SelectedPlayerUnit.e_AttackMode)
                            {
                                case Unit.AttackMode.none:
                                    break;
                                case Unit.AttackMode.melee:
                                    m_SelectedPlayerUnit.MeleeAttack(uunit);
                                    break;
                                case Unit.AttackMode.ranged:
                                    m_SelectedPlayerUnit.RangedAttack(uunit);
                                    break;
                                default:
                                    break;
                            }
                            if (!m_SelectedPlayerUnit.SwitchActivity())
                            {
                                m_SelectedPlayerUnit = null;
                            }
                            GameManager.s_Instance.SendMessage("CBAttackLock");
                            m_IsAttacking = false;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
