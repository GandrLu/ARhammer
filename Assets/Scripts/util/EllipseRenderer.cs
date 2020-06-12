using UnityEngine;
using System.Collections;

namespace de.mp.future.warhammer.util
{
    public class EllipseRenderer : MonoBehaviour
    {
        public GameObject m_Radius;

        private GameObject m_CurrentRadius;


        // - - - Public functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Instatiates a cylinder at the specified _Position with a given _Rotation and an _Radius.
        /// </summary>
        /// <param name="_Position"></param>
        /// <param name="_Rotation"></param>
        /// <param name="_Radius"></param>
        public GameObject DrawRadius(Vector3 _Position, Quaternion _Rotation, float _Radius)
        {
            Destroy(this.m_CurrentRadius);
            this.m_CurrentRadius = Instantiate(m_Radius, _Position, _Rotation);
            this.m_CurrentRadius.transform.localScale = new Vector3(_Radius, 0.01f, _Radius);

            return this.m_CurrentRadius;
        }

        /// <summary>
        /// Instatiates a cylinder at the specified _Position with a given _Rotation and an _Radius.
        /// The instantiated object is a child of _Anchor GameObject.
        /// </summary>
        /// <param name="_Position"></param>
        /// <param name="_Rotation"></param>
        /// <param name="_Radius"></param>
        /// <param name="_Anchor"></param>
        public GameObject DrawRadius(Vector3 _Position, Quaternion _Rotation, float _Radius, GameObject _Anchor)
        {
            Destroy(this.m_CurrentRadius);
            this.m_CurrentRadius = Instantiate(m_Radius, _Position, _Rotation, _Anchor.transform);
            this.m_CurrentRadius.transform.localScale = new Vector3(_Radius, 0.01f, _Radius);
            this.m_CurrentRadius.SetActive(true);

            return this.m_CurrentRadius;
        }

        /// <summary>
        /// Deletes the drawed Radius.
        /// </summary>
        public void DeleteRadius()
        {
            Destroy(this.m_CurrentRadius);
        }
    }
}