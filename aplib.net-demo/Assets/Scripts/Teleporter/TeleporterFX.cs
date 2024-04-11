using UnityEngine;

namespace Teleporter
{
    public class TeleporterFX : MonoBehaviour
    {
        [Range(0, .04f)] public float acceleration;
        [Range(0, .4f)] public float widthGain;
        public bool rightHandedRotation;

        protected Transform _plane;
        protected Vector3 _originalPlaneScale;
        protected float _currentAcceleration;

        protected float _currentSpeed;
        protected float _currentWidth;

        private void Start()
        {
            acceleration *= rightHandedRotation ? 1f : -1f;

            _plane = GetComponentInChildren<Transform>();
            _originalPlaneScale = _plane.localScale;
            _originalPlaneScale.z = 0; // Start with no width
        }

        /// <summary>
        /// Reset to initial state
        /// </summary>
        private void OnEnable()
        {
            _currentAcceleration = acceleration;
            _currentSpeed = 0;
            _currentWidth = 0;
        }

        private void Update()
        {
            // Rotate over time
            _currentSpeed += _currentAcceleration * Time.deltaTime;
            transform.Rotate(0, _currentSpeed * 360, 0);

            // Scale over time
            _currentWidth += widthGain * Time.deltaTime;
            _plane.transform.localScale = new Vector3(_originalPlaneScale.x, _originalPlaneScale.y, _currentWidth);
        }
    }
}
