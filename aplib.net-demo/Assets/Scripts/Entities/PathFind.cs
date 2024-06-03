using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple script to make an object follow another object with a specific tag.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PathFind : MonoBehaviour
{
    /// <summary>
    /// The tag of the object to follow.
    /// </summary>
    public string TagToFind = "Player";

    private NavMeshAgent _agent;

    private bool _enabled;

    private Transform _goal;

    /// <summary>
    /// Get the agent and find the goal object by tag.
    /// </summary>
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        GameObject goalObject = GameObject.FindGameObjectWithTag(TagToFind);
        if (goalObject is null)
            Debug.LogError("No object with tag " + TagToFind + " found!");

        _goal = goalObject.transform;
    }

    /// <summary>
    /// Update the NavMeshAgent's destination to the goal's position.
    /// </summary>
    public void UpdateAgent(float remainingDistanceThreshold)
    {
        if (_goal == null) return;

        if (_agent.isOnNavMesh) _agent.SetDestination(_goal.position);

        if (_agent.remainingDistance < remainingDistanceThreshold && _agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _enabled = true;
            ToggleAgent(true);
        }
        else if (_enabled)
        {
            ToggleAgent(_agent.pathStatus == NavMeshPathStatus.PathComplete);
        }

        _agent.transform.LookAt(_goal);
    }

    /// <summary>
    /// Set the distance at which the agent will stop from the goal.
    /// </summary>
    /// <param name="distance"></param>
    public void SetStoppingDistance(float distance) => _agent.stoppingDistance = distance;

    /// <summary>
    /// Start/stop the agent.
    /// </summary>
    /// <param name="enabled">True to enable the agent, false to disable it.</param>
    public void ToggleAgent(bool enabled) => _agent.enabled = enabled;

    /// <summary>
    /// Check if the goal is within a certain range.
    /// </summary>
    /// <param name="range">The range to check.</param>
    /// <returns>True if the goal is within the range, false otherwise.</returns>
    public bool GoalWithinRange(float range) => Vector3.Distance(transform.position, _goal.position) <= range;
}
