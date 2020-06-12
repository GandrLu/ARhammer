using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.mp.future.warhammer.util
{
    public class PopUpNumbers : MonoBehaviour
    {
        private TextMesh Text;
        private Transform Transform;

        [SerializeField]
        private GameObject TextObject;
        
        [Range(0, 10)]
        [SerializeField]
        private float YOffset;
        
        [SerializeField]
        private Camera Camera;

        private void Start()
        {
            this.Transform = this.gameObject.transform;
            this.Text = this.TextObject.GetComponent<TextMesh>();
            this.Transform.position.Set(this.Transform.position.x, this.Transform.position.y, this.Transform.position.z + this.YOffset);

            if (this.Camera == null)
            {
                this.Camera = GameObject.FindObjectOfType<Camera>();
            }
        }


        // - - - Public functions - - - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Displays a PopUp text on script position.
        /// The text is facing the camera specified in Start() method.
        /// </summary>
        /// <param name="_Value">string</param>
        /// <param name="_Color">Color</param>
        public void PopUp(string _Value, Color _Color)
        {
            this.Text.text = _Value;
            this.Text.color = _Color;
            GameObject Instance = Instantiate(this.TextObject, this.Transform.position, Camera.gameObject.transform.rotation);
            Destroy(Instance.gameObject, 1);
        }
    }
}