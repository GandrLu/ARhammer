using UnityEngine;

namespace de.mp.future.warhammer.util
{
    /// Simple script to destroy a spawned floating text after certain time and updates the 
    /// rotation to keep it screen oriented on each frame.
    public class FloatingText : MonoBehaviour
    {
        public float DestroyTime = 3f;
        public Material m_BowLogo;
        public Material m_SwordLogo;

        public void SetSwordLogo()
        {
            if (m_SwordLogo)
            {
                this.gameObject.GetComponent<MeshRenderer>().material = m_SwordLogo;
            }
        }

        public void SetBowLogo()
        {
            if (m_BowLogo)
            {
                this.transform.GetComponent<MeshRenderer>().material = m_BowLogo;
            }
        }

        void Start()
        {
            //Destroy(gameObject, DestroyTime);
        }

        void Update()
        {
            //transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles);
        }
    }
}
