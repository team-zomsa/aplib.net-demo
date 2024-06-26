// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages the bar and master volume.
/// </summary>
public class VolumeSlider : MonoBehaviour
{
    /// <summary>
    /// Used to set the volume bar to the correct repesentation.
    /// </summary>
    private Slider _volSlider;

    /// <summary>
    /// Used to call and store volume value.
    /// </summary>
    private float _volumeValue;

    /// <summary>
    /// Takes the value from the volume slider and sends it to the sound manager.
    /// </summary>
    /// <param name="newValue"></param>
    public void VolumeValue(float newValue)
    {
        _volumeValue = newValue;
        // ToDo:: Send newValue that is from 1 to 100 to the Sound manager
    }

    /// <summary>
    /// When the application is closed, it saved the master volume variable to the key "MasterVolume".
    /// </summary>
    private void OnDisable()
    {
        PlayerPrefs.SetFloat("MasterVolume", _volumeValue);
    }

    /// <summary>
    /// When the slider is enabled it retrieves the volume value from last session 
    /// and sets the volume bar at that % and the volume to that noise level.
    /// </summary>
    private void OnEnable()
    {
        _volSlider = GetComponent<Slider>();

        _volumeValue = PlayerPrefs.GetFloat("MasterVolume");
        _volSlider.value = _volumeValue;
    }
}
