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
    private Transform _goal;
    private NavMeshAgent _agent;

    /// <summary>
    /// Get the agent and find the goal object by tag.
    /// </summary>
    private void Start()
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
    public void UpdateAgent()
    {
        if (_goal is null) return;

        _agent.SetDestination(_goal.position);
        _agent.transform.LookAt(_goal);
    }
}

