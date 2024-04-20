using Aplib.Core;
using Aplib.Core.Belief;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aplib.Integrations.Unity.Actions
{
    public class TransformPathfinderAction<TBeliefSet> : Core.Intent.Actions.Action<TBeliefSet>
        where TBeliefSet : IBeliefSet
    {
        private static readonly Func<TBeliefSet, bool> _alwaysTrue = _ => true;

        public TransformPathfinderAction(Func<TBeliefSet, Transform> objectQuery,
            Vector3 location,
            float heightOffset = 0f,
            Func<TBeliefSet, bool> guard = null,
            Metadata metadata = null)
            : this(objectQuery,
                location: ConstantLocation(location),
                heightOffset,
                guard,
                metadata
            )
        {
        }

        public TransformPathfinderAction(Func<TBeliefSet, Transform> objectQuery,
            Func<TBeliefSet, Vector3> location,
            float heightOffset = 0f,
            Func<TBeliefSet, bool> guard = null,
            Metadata metadata = null)
            : base(
                effect: PathfindingAction(objectQuery, location, heightOffset: heightOffset),
                guard: guard ?? _alwaysTrue,
                metadata
            )
        {
        }

        private static Func<TBeliefSet, Vector3> ConstantLocation(Vector3 location) => _ => location;

        private static Action<TBeliefSet> PathfindingAction(Func<TBeliefSet, Transform> objectQuery,
            Func<TBeliefSet, Vector3> locationQuery,
            float speed = 7f,
            float heightOffset = 0f) => beliefSet =>
        {
            Transform transform = objectQuery(beliefSet);
            Vector3 target = locationQuery(beliefSet);

            NavMeshPath path = new();
            NavMesh.CalculatePath(transform.position,
                target,
                NavMesh.AllAreas,
                path
            );

            if (path.corners.Length <= 1)
            {
                return;
            }


            // Move Towards
            Vector3 targetPosition = path.corners[1] + (Vector3.up * heightOffset);
            Vector3 newPosition =
                Vector3.MoveTowards(transform.position, targetPosition, maxDistanceDelta: Time.deltaTime * speed);
            Debug.DrawLine(targetPosition, transform.position, Color.blue);

            // Calculate the new direction
            Vector3 direction = newPosition - transform.position;
            direction.Normalize();

            transform.position = newPosition;

            // If the direction is zero, don't rotate the object
            if (direction == Vector3.zero)
            {
                return;
            }


            transform.rotation = Quaternion.LookRotation(direction);
        };
    }
}
