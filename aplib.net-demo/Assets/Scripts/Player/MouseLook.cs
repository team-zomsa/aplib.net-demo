using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(0.05f, 0.30f)][SerializeField] float sensitivity = 0.15f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
    [SerializeField] Transform playerBody;
    Vector2 rotation = Vector2.zero;
    Vector2 mouseInput;
    bool showMouse = false;

    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// </summary>
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Rotates the camera and the player's body based on the mouse input.
    /// Does nothing if the cursor is visible.
    /// </summary>
    void Update()
    {
        if (showMouse) return;

        rotation.x += mouseInput.x; 
        rotation.y += mouseInput.y; 
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        // Rotate camera
        transform.localRotation = xQuat * yQuat;

        // Rotate player body
        playerBody.rotation = Quaternion.Euler(0, rotation.x, 0);
    }

    /// <summary>
    /// Shows the mouse and unlocks it.
    /// </summary>
    public void OnShowMousePressed() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        showMouse = true;
    }

    /// <summary>
    /// On left mouse click, go back into the game and lock the cursor.
    /// </summary>
    public void OnLeftMousePressed() {
        if (showMouse){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            showMouse = false;
        }
    }
    public void ReveiveMouseInput(Vector2 _input) => mouseInput = _input * sensitivity;
}
