using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// RenderQuality will save the render state and forward it to the class that changed the quality.
/// </summary>
public class RenderQuality : MonoBehaviour
{
    /// <summary>
    /// Most current render state.
    /// </summary>
    private int _renderState;

    /// <summary>
    /// Used to set the int state to the render quality component.
    /// </summary>
    private TMP_Dropdown _renderComponent;

    /// <summary>
    /// Will be dynamically called and given the new state value.
    /// </summary>
    /// <param name="newValue">Render state value.</param>
    public void RenderState(int newValue)
    {
        _renderState = newValue;
        // ToDo:: do something with the retrieved value.
        // 0 == low, 1 == medium & 2 == high.
    }

    /// <summary>
    /// When the application is closed, the state is saved to the key "RenderQuality".
    /// </summary>
    private void OnDisable()
    {
        PlayerPrefs.SetInt("RenderQuality", _renderState);
    }

    /// <summary>
    /// When the dropbox is enabled it retrieves the render state from last session 
    /// and sets the textbox at the correct state.
    /// </summary>
    private void OnEnable()
    {
        _renderComponent = GetComponent<TMP_Dropdown>();

        _renderState = PlayerPrefs.GetInt("RenderQuality");
        _renderComponent.value = _renderState;
    }
}
