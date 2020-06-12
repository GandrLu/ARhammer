using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace de.mp.future.warhammer.util
{
    public enum MessageType
    {
        NORMAL,
        WARNING,
        ALERT
    }

    public struct Message
    {
        public string Text;
        public MessageType Type;
    }

    public class Messages : MonoBehaviour
    {
        [SerializeField]
        private Canvas m_MainCanvas;

        [SerializeField]
        private Text m_TextPrefab;

        private List<Message> m_Queue = new List<Message>();

        private bool m_Idle = true;    

        private float m_TimeSinceLastMessage;

        // Update is called once per frame
        void Update()
        {
           
            if (m_TimeSinceLastMessage > 4.2f)
            {
                m_Idle = true;
                m_TimeSinceLastMessage = 0.0f;
            }
                
            if (m_Queue.Count > 0)
            {
                if (m_Idle == true)
                {
                    m_Idle = false;
                    SendMessage(m_Queue[0]);
                    m_Queue.RemoveAt(0);
                }   
            }
            
            m_TimeSinceLastMessage += Time.deltaTime;
        }


        // Private functions //////


        /// <summary>
        /// Sends the current message to the canvas.
        /// </summary>
        /// <param name="_Message"></param>
        private void SendMessage(Message _Message)
        {
            Debug.Log("Display Message");
            m_TextPrefab.text = _Message.Text;
            Text Text = Instantiate(m_TextPrefab, new Vector3(0, 0, 0) ,Quaternion.identity, m_MainCanvas.transform);
            Text.transform.localPosition = new Vector3(0, -100, 0);
            Destroy(Text.gameObject, 4);
        }


        // Public functions //////


        /// <summary>
        /// Adds a message to the message queue.
        /// </summary>
        /// <param name="_Message"></param>
        public void AddMessageToQueue(string _Message)
        {
            Message MessageToAdd;

            MessageToAdd.Text = _Message;
            MessageToAdd.Type = MessageType.NORMAL;

            this.m_Queue.Add(MessageToAdd);
        }

    }
}


