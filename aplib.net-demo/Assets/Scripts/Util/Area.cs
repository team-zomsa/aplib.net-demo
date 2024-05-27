using UnityEngine;

/// <summary>
/// The spawn area for the enemy.
/// The size is defined by the local scale of the transform.
/// </summary>
public class Area : MonoBehaviour
{
    /// <summary>
    /// The bounds of the area.
    /// </summary>
    public Bounds Bounds { get; set; }

    /// <summary>
    /// Initialize the area bounds.
    /// </summary>
    private void Awake() => Bounds = new Bounds(transform.position, transform.localScale);

    /// <summary>
    /// Show the area in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Globals.s_LogicColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
