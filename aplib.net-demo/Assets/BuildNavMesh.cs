using Unity.AI.Navigation;
using UnityEngine;

public class BuildNavMesh : MonoBehaviour
{
    void Awake()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
