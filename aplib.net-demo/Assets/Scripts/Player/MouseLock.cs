using UnityEngine;

public class MouseLock : MonoBehaviour
{
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
                /// Shows the mouse and unlocks it.
                /// </summary>
                public void OnShowMousePressed()
                {
                                Cursor.lockState = CursorLockMode.None;
                                Cursor.visible = true;
                                _showMouse = true;
                }

                /// <summary>
                /// On left mouse click, go back into the game and lock the cursor.
                /// </summary>
                public void OnLeftMousePressed()
                {
                                if (_showMouse)
                                {
                                                Cursor.lockState = CursorLockMode.Locked;
                                                Cursor.visible = false;
                                                _showMouse = false;
                                }
                }
}
