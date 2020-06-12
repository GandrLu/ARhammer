using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace de.mp.future.warhammer.game
{
    public class GameManager : MonoBehaviour
    {
        [HideInInspector]
        public static GameManager s_Instance;
        
        [SerializeField]
        public GameObject m_WorldAnchor;

        [Space(15)]

        [SerializeField]
        private int m_Round;

        [SerializeField]
        private GameObject m_CurrentPlayer;

        [SerializeField]
        private GameStatus m_GameStatus;

        [Space(15)]
        
        [SerializeField]
        private Text m_PhaseText;

        [SerializeField]
        private Text m_PlayerText;

        [SerializeField]
        private Text m_UnitText;

        [Space(15)]

        [SerializeField]
        private GameObject m_AttackRadiusPrefab;

        [Space(15)]

        [SerializeField]
        private GameObject[] m_StatusElements;

        [Space(15)]

        [SerializeField]
        private GameObject m_StartButton;

        [SerializeField]
        private GameObject m_MoveButton;

        [SerializeField]
        private GameObject m_StartLockButton;

        [SerializeField]
        private GameObject m_AttackRangeButton;

        [SerializeField]
        private GameObject m_SkipButton;

        private util.EllipseRenderer ERenderer;

        private util.Messages Messages;

        private int m_CurrentPlayerID;

        private GameStatus m_LastGameStatus;

        private GameObject[] m_Players;

        private GameObject m_AttackRadius;

        private float m_TimeSinceLastUpdate;

        // - - - Start function - - - - - - - - - - - - - - - - - - - -


        void Start()
        {
            s_Instance = this;

            this.ERenderer = this.GetComponent<util.EllipseRenderer>();
            this.Messages = this.GetComponent<util.Messages>();

            m_Players = GameObject.FindGameObjectsWithTag("Player");
            this.m_CurrentPlayerID = 0;
            this.m_CurrentPlayer = m_Players[this.m_CurrentPlayerID];
            this.m_Round = 1;
            
            GameManager.s_Instance.DisplayMessage(m_CurrentPlayer.name + "'s turn!");

             // Show Status Bar
            this.ToggleStats();

            this.SetGameStatus(GameStatus.STARTUP);
        }

        private void Update() {
            
            if(m_TimeSinceLastUpdate > 0.1f)
            {
              UpdateStatusBar();  
              m_TimeSinceLastUpdate = 0.0f;
            }
            
            m_TimeSinceLastUpdate += Time.deltaTime;
            
        }

        // - - - Set/Get functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Returns the current GameStatus.
        /// </summary>
        /// <returns>GameStatus</returns>
        public GameStatus GetGameStatus()
        {
            return this.m_GameStatus;
        }

        /// <summary>
        /// Returns the last GameStatus.
        /// </summary>
        /// <returns>GameStatus</returns>
        public GameStatus GetLastGameStatus()
        {
            return this.m_LastGameStatus;
        }

        /// <summary>
        /// Sets the current GameStatus.
        /// </summary>
        /// <param name="_GameStatus">Gamestatus</param>
        public void SetGameStatus(GameStatus _GameStatus)
        {
            Debug.LogWarning("LAST: " + this.m_GameStatus.ToString());
            this.m_LastGameStatus = this.m_GameStatus;
            this.m_GameStatus = _GameStatus;
            Debug.LogWarning("NEW: " + this.m_GameStatus.ToString());
        }

        /// <summary>
        /// Returns the current player.
        /// </summary>
        /// <returns>GameObject</returns>
        public GameObject GetCurrentPlayer()
        {
            return this.m_CurrentPlayer;
        }

        // - - - Public functions - - - - - - - - - - - - - - - - - - - -

        /// <summary>
        /// Starts the game and calls the StartUp() function on all players.
        /// </summary>
        public void StartUp()
        {
            // Hide Start Button
            this.ToggleStartButton();

            // Call StartUp 
            foreach (GameObject Player in m_Players)
            {
                //Player.GetComponent<PlayerController>().StartUp();
            }

            // Mark units
            this.MarkUnits();

            this.SetGameStatus(GameStatus.MOVING);
        }

        /// <summary>
        /// Shows the status bar.
        /// </summary>
        public void ToggleStats()
        {
            foreach (GameObject StatusElement in m_StatusElements)
            {  
                StatusElement.SetActive(!StatusElement.activeSelf);
            }
        }

        /// <summary>
        /// Toggles the start button display.
        /// </summary>
        public void ToggleStartButton()
        {
            m_StartButton.SetActive(!m_StartButton.activeSelf);
        }

        /// <summary>
        /// Toggles the move button display.
        /// </summary>
        public void ToggleMoveButton()
        {
            m_MoveButton.SetActive(!m_MoveButton.activeSelf);
        }

        /// <summary>
        /// Toggles the start position lock button display.
        /// </summary>
        public void ToggleStartLockButton()
        {
            m_StartLockButton.SetActive(!m_StartLockButton.activeSelf);
        }

        /// <summary>
        /// Toggles the attack range button display.
        /// </summary>
        public void ToggleAttackRangeButton()
        {
            m_AttackRangeButton.SetActive(!m_AttackRangeButton.activeSelf);
        }

        /// <summary>
        /// Toggles the skip button display.
        /// </summary>
        public void ToggleSkipButton()
        {
            m_SkipButton.SetActive(!m_SkipButton.activeSelf);
        }

        /// <summary>
        /// Toggles the skip button display.
        /// </summary>
        public void ToggleSkipButton(bool _State)
        {
            m_SkipButton.SetActive(_State);
        }

        /// <summary>
        /// Ends the current round.
        /// </summary>
        public void EndRound()
        {
            if(CheckGameOver())
            {
                GameOver();
            }

            NextPlayer();
            this.m_Round++;
            this.SetGameStatus(GameStatus.MOVING);
        }

        /// <summary>
        /// Checks if the passed player is the currently active player.
        /// </summary>
        /// <param name="_Player">GameObject</param>
        /// <returns>bool</returns>
        public bool IsCurrentPlayer(GameObject _Player)
        {
            if (_Player == this.m_CurrentPlayer)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the passed player is the currently active player
        /// </summary>
        /// <param name="_Player">PlayerController</param>
        /// <returns>bool</returns>
        public bool IsCurrentPlayer(PlayerController _Player)
        {
            if (_Player == this.m_CurrentPlayer.GetComponent<PlayerController>())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the position lock of the current Unit.
        /// </summary>
        public void LockPosition()
        {
            if (this.m_GameStatus == GameStatus.MOVING)
            {
                this.m_CurrentPlayer.GetComponent<PlayerController>().LockPosition();
            }
        }

        /// <summary>
        /// Handles the start position lock of the current unit.
        /// </summary>
        public void LockStart()
        {
            if (this.m_GameStatus == GameStatus.STARTUP)
            {
                this.m_CurrentPlayer.GetComponent<PlayerController>().LockStart();
            }
        }

        public void SkipAttack()
        {
            m_CurrentPlayer.GetComponent<PlayerController>().SkipAttack();
        }

        /// <summary>
        ///  Spawns an Cylinder at a specific position and radius.
        /// </summary>
        /// <param name="_Position">Vector3</param>
        /// <param name="_Radius">float</param>
        public GameObject DrawRadius(Vector3 _Position, Quaternion _Rotation, float _Radius)
        {

            return this.ERenderer.DrawRadius(_Position, _Rotation, _Radius);
        }

        /// <summary>
        ///  Spawns an Cylinder at a specific position and radius.
        /// </summary>
        /// <param name="_Position">Vector3</param>
        /// <param name="_Radius">float</param>
        /// <param name="_Anchor">GameObject</param>
        public GameObject DrawRadius(Vector3 _Position, Quaternion _Rotation, float _Radius, GameObject _Anchor)
        {
            return this.ERenderer.DrawRadius(_Position, _Rotation, _Radius, _Anchor);
        }

        /// <summary>
        /// Deletes the current radius.
        /// </summary>
        public void DeleteRadius()
        {
            this.ERenderer.DeleteRadius();
        }

        /// <summary>
        /// Draws the attack radius of the current unit.
        /// </summary>
        public void ShowAttackRadius()
        {
            GameObject Unit = m_CurrentPlayer.GetComponent<PlayerController>().GetCurrentUnit();
            Unit UnitController = Unit.GetComponent<Unit>();
            this.m_AttackRadius = Instantiate(m_AttackRadiusPrefab, Unit.transform.position, Unit.transform.rotation, m_WorldAnchor.transform);
            this.m_AttackRadius.transform.localScale = new Vector3(UnitController.GetAttackRange(), 0.01f, UnitController.GetAttackRange());
            this.m_AttackRadius.SetActive(true);
        }

        /// <summary>
        /// Hides the attack radius of the current unit.
        /// </summary>
        public void HideAttackRadius()
        {
            Destroy(this.m_AttackRadius);
        }

        /// <summary>
        /// Sets the specified UI Text.
        /// </summary>
        /// <param name="_Target"></param>
        /// <param name="_Type"></param>
        public void SetText(string _Target, string _Type)
        {
            switch (_Type)
            {
                case "player":
                    this.m_PlayerText.text = _Target;
                    break;

                case "unit":
                    this.m_UnitText.text = _Target;
                    break;

                case "phase":
                    this.m_PhaseText.text = _Target;
                    break;
            }
        }

        public void DisplayMessage(string _Message)
        {
            this.Messages.AddMessageToQueue(_Message);
        }


        // - - - Private functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Sets the next active player
        /// </summary>
        /// <returns>int</returns>
        private int NextPlayer()
        {
            int PlayerCount = this.m_Players.Length;

            if (this.m_CurrentPlayerID + 1 >= PlayerCount)
            {
                this.m_CurrentPlayerID = 0;
                this.m_CurrentPlayer = m_Players[this.m_CurrentPlayerID];
            }
            else
            {
                this.m_CurrentPlayerID++;
                this.m_CurrentPlayer = m_Players[this.m_CurrentPlayerID];
            }

            this.MarkUnits();

            GameManager.s_Instance.DisplayMessage(m_CurrentPlayer.name + "'s turn!");

            return this.m_CurrentPlayerID;
        }

        /// <summary>
        /// Taggs all units of the current player either as PlayerUnit or Enemy. 
        /// </summary>
        private void MarkUnits()
        {
            foreach (GameObject Player in m_Players)
            {
                if(Player == m_CurrentPlayer)
                {
                    Player.GetComponent<PlayerController>().MarkUnits(true);                
                }
                else
                {
                    Player.GetComponent<PlayerController>().MarkUnits(false);    
                }
                
            }
        }

        /// <summary>
        /// Checks if the game has ended.
        /// </summary>
        /// <returns></returns>
        private bool CheckGameOver()
        {
            foreach (GameObject  Player in m_Players)
            {
                PlayerController PController = Player.GetComponent<PlayerController>();
                if(PController.CheckGameOver())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the gameover.
        /// </summary>
        private void GameOver()
        {
            SceneManager.LoadScene(0);
        }

        // - - - Callback functions - - - - - - - - - - - - - - - - - - - -

        private void CBStartLock()
        {
            SetGameStatus(GameStatus.NEXTSTART);
        }    

        /// <summary>
        /// Position lock Callback
        /// </summary>
        private void CBPositionLock()
        {
            ToggleMoveButton();
            ToggleSkipButton(true);
            SetGameStatus(GameStatus.ATTACK);
        }

        /// <summary>
        /// Attack Callback
        /// </summary>
        private void CBAttack()
        {
            ToggleSkipButton();
            SetGameStatus(GameStatus.ATTACK);
        }

        /// <summary>
        /// Attack lock Callback
        /// </summary>
        private void CBAttackLock()
        {
            ToggleSkipButton(false);
            SetGameStatus(GameStatus.NEXTMOVE);
        }

        /// <summary>
        /// Round finished Callback
        /// </summary>
        private void CBFinished()
        {
            SetGameStatus(GameStatus.FINISHED);
            this.EndRound();
        }

        /// <summary>
        /// Start position lock Callback
        /// </summary>
        private void CBStartFinished()
        {
            if (this.NextPlayer() == 0)
            {
                this.SetGameStatus(GameStatus.READY);
                ToggleStartButton();
                ToggleStartLockButton();
            }
            else
            {
                this.SetGameStatus(GameStatus.STARTUP);
            }
        }

        /// <summary>
        /// Round ended Callback
        /// </summary>
        private void CBEndRound()
        {
            EndRound();
        }


        private void UpdateStatusBar()
        {
            PlayerController CurrentPlayerController = m_CurrentPlayer.GetComponent<PlayerController>();
            GameObject CurrentUnit = CurrentPlayerController.GetCurrentUnit();

            SetText(m_CurrentPlayer.name, "player");
            SetText(CurrentUnit.name, "unit");
            SetText(m_GameStatus.ToString(), "phase");
        }

    }
}