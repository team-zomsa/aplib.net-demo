using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple script to make an object follow another object with a specific tag.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PathFind : MonoBehaviour
{
    [SerializeField] private string _tagToFind = "Player";
    private Transform _goal;
    private NavMeshAgent _agent;

    /// <summary>
    /// Get the agent and find the goal object by tag.
    /// </summary>
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        GameObject goalObject = GameObject.FindGameObjectWithTag(_tagToFind);
        if (goalObject == null)
            Debug.LogError("No object with tag " + _tagToFind + " found!");
        else
            _goal = goalObject.transform;
    }

    /// <summary>
    /// Update the NavMeshAgent's destination to the goal's position.
    /// </summary>
    public void UpdateAgent()
    {
        if (_goal != null)
        {
            _agent.SetDestination(_goal.position);
            _agent.transform.LookAt(_goal);
        }
    }
}
