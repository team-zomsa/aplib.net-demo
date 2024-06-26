// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Component that shows the path to the target on the NavMesh.
/// </summary>
public class ShowGoldenPath : MonoBehaviour
{
    public Transform Target;
    private float _elapsed;
    private NavMeshPath _path;

    private void Start()
    {
        _path = new NavMeshPath();
        _elapsed = 0.0f;
    }

    private void Update()
    {
        // Update the way to the goal every second.
        _elapsed += Time.deltaTime;
        if (_elapsed > 1.0f)
        {
            _elapsed = 0;
            NavMesh.CalculatePath(transform.position, Target.position, NavMesh.AllAreas, _path);
        }

        Color[] colors = { Color.blue, Color.cyan, Color.red, Color.magenta };
        for (int i = 0; i < _path.corners.Length - 1; i++)
        {
            Debug.DrawLine(start: _path.corners[i], end: _path.corners[i + 1], color: colors[i % 4]);
        }
    }
}
