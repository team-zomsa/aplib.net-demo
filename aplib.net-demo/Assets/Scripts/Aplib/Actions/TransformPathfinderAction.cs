using Aplib.Core;
using Aplib.Core.Belief;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aplib.Integrations.Unity.Actions
{
    public class TransformPathfinderAction<TBeliefSet> : UnityPathfinderAction<TBeliefSet>
        where TBeliefSet : IBeliefSet
    {
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
                objectQuery,
                location,
                effect: PathfindingAction(objectQuery),
                heightOffset: heightOffset,
                guard: guard ?? _alwaysTrue,
                metadata
            )
        {
        }

        private static Action<TBeliefSet, Vector3> PathfindingAction(Func<TBeliefSet, Transform> objectQuery) => (beliefSet, destination) =>
        {
            Transform transform = objectQuery(beliefSet);

            // Calculate the new direction
            Vector3 direction = destination - transform.position;
            direction.Normalize();

            transform.position = destination;

            // If the direction is zero, don't rotate the object
            if (direction == Vector3.zero)
            {
                return;
            }


            transform.rotation = Quaternion.LookRotation(direction);
        };
    }
}
