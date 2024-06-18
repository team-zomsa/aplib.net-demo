using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Component that shows the path to the target on the NavMesh.
/// </summary>
public class ShowGoldenPath : MonoBehaviour
{
    public Transform target;
    private float elapsed;
    private NavMeshPath path;

    private void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    private void Update()
    {
        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed = 0;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }

        Color[] colors = { Color.blue, Color.cyan, Color.red, Color.magenta };
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(start: path.corners[i], end: path.corners[i + 1], color: colors[i % 4]);
        }
    }
}
