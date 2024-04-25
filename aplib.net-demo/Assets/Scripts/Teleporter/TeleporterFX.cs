using UnityEngine;

namespace Teleporter
{
    public class TeleporterFX : MonoBehaviour
    {
        /// <summary>
        /// The momentum gained over time once triggered.
        /// </summary>
        [Header("Parameters")]
        [Range(0, .04f)] public float acceleration;

        /// <summary>
        /// The width gained over time once triggered.
        /// </summary>
        [Range(0, .4f)] public float widthGain;

        /// <summary>
        /// Whether the FX should rotate clockwise to the right or left. True if right.
        /// </summary>
        public bool rightHandedRotation;

        /// <summary>
        /// The 'FX' which is manipulated by this script.
        /// </summary>
        protected Transform _plane;

        /// <summary>
        /// The starting scale as configured in the editor.
        /// </summary>
        protected Vector3 _originalPlaneScale;

        /// <summary>
        /// The current acceleration, diverging from the original <see cref="acceleration"/>.
        /// </summary>
        protected float _currentAcceleration;

        /// <summary>
        /// The current speed, diverging from 0.
        /// </summary>
        protected float _currentSpeed;

        /// <summary>
        /// The current width, diverging from the <see cref="_originalPlaneScale"/>'s width.
        /// </summary>
        protected float _currentWidth;

        /// <summary>
        /// Determine all original values of the FX, as configured in the editor.
        /// These values will change over time, but the original must be remembered.
        /// </summary>
        private void Start()
        {
            acceleration *= rightHandedRotation ? 1f : -1f;

            _plane = GetComponentInChildren<Transform>();
            _originalPlaneScale = _plane.localScale;
            _originalPlaneScale.z = 0; // Start with no width
        }

        /// <summary>
        /// (Re)set to initial state
        /// </summary>
        private void OnEnable()
        {
            _currentAcceleration = acceleration;
            _currentSpeed = 0;
            _currentWidth = 0;
        }

        /// <summary>
        /// Animate the FX.
        /// </summary>
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
