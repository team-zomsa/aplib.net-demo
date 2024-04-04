using UnityEngine;

public class ShowArea : MonoBehaviour
{
    /// <summary>
    /// Show the area in the editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Globals.s_LogicColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
