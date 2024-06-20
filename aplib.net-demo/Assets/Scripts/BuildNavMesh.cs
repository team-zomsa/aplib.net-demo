using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// This component builds the nav mesh on awake.
/// </summary>
[RequireComponent(typeof(NavMeshSurface))]
public class BuildNavMesh : MonoBehaviour
{
    void Awake()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
