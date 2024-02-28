using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(0.05f, 0.30f)][SerializeField] private float _sensitivity = 0.15f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
    [SerializeField] private Transform _playerBody;
    private Vector2 _rotation = Vector2.zero;
    private Vector2 _mouseInput;
    private bool _showMouse = false;

    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// </summary>
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Rotates the camera and the player's body based on the mouse input.
    /// Does nothing if the cursor is visible.
    /// </summary>
    private void Update()
    {
        if (_showMouse) return;

        _rotation.x += _mouseInput.x; 
        _rotation.y += _mouseInput.y; 
	    _rotation.y = Mathf.Clamp(_rotation.y, -_yRotationLimit, _yRotationLimit);
	    Quaternion xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
	    Quaternion yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

        // Rotate camera
        transform.localRotation = xQuat * yQuat;

        // Rotate player body
        _playerBody.rotation = Quaternion.Euler(0, _rotation.x, 0);
    }

    /// <summary>
    /// Shows the mouse and unlocks it.
    /// </summary>
    public void OnShowMousePressed() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _showMouse = true;
    }

    /// <summary>
    /// On left mouse click, go back into the game and lock the cursor.
    /// </summary>
    public void OnLeftMousePressed() {
        if (_showMouse){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _showMouse = false;
        }
    }
    public void ReveiveMouseInput(Vector2 input) => _mouseInput = input * _sensitivity;
}
