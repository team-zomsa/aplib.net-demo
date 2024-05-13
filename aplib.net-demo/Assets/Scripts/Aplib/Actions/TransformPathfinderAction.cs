using Aplib.Core;
using Aplib.Core.Belief;
using System;
using UnityEngine;

namespace Aplib.Integrations.Unity.Actions
{
    public class TransformPathfinderAction<TBeliefSet> : UnityPathfinderAction<TBeliefSet>
        where TBeliefSet : IBeliefSet
    {
        public TransformPathfinderAction(Func<TBeliefSet, Rigidbody> objectQuery,
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

        public TransformPathfinderAction(Func<TBeliefSet, Rigidbody> objectQuery,
            Func<TBeliefSet, Vector3> location,
            float heightOffset = 0f,
            Func<TBeliefSet, bool> guard = null,
            Metadata metadata = null)
            : base(
                objectQuery,
                location,
                effect: PathfindingAction(objectQuery),
                heightOffset,
                guard: guard ?? _alwaysTrue,
                metadata
            )
        {
        }

        private static Action<TBeliefSet, Vector3> PathfindingAction(Func<TBeliefSet, Rigidbody> objectQuery)
            => (beliefSet, destination) =>
            {
                Rigidbody rigidbody = objectQuery(beliefSet);

                // Calculate the new direction
                Vector3 direction = destination - rigidbody.position;
                direction = new Vector3(direction.x, 0, direction.z);
                direction.Normalize();

                rigidbody.position = destination;

                // If the direction is zero, don't rotate the object
                if (direction == Vector3.zero)
                {
                    return;
                }

                rigidbody.rotation = Quaternion.LookRotation(direction);
            };
    }
}
