using Aplib.Core;
using Aplib.Core.Belief;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aplib.Integrations.Unity.Actions
{
    public class UnityPathfinderAction<TBeliefSet> : Core.Intent.Actions.Action<TBeliefSet>
        where TBeliefSet : IBeliefSet
    {
        protected static readonly Func<TBeliefSet, bool> _alwaysTrue = _ => true;

        public UnityPathfinderAction(
            Func<TBeliefSet, Rigidbody> objectQuery,
            Action<TBeliefSet, Vector3> effect,
            Vector3 location,
            float heightOffset = 0f,
            Func<TBeliefSet, bool> guard = null,
            Metadata metadata = null)
            : this(objectQuery,
                location: ConstantLocation(location),
                effect,
                heightOffset,
                guard,
                metadata
            )
        {
        }

        public UnityPathfinderAction(
            Func<TBeliefSet, Rigidbody> objectQuery,
            Func<TBeliefSet, Vector3> location,
            Action<TBeliefSet, Vector3> effect,
            float heightOffset = 0f,
            Func<TBeliefSet, bool> guard = null,
            Metadata metadata = null)
            : base(
                effect: PathfindingAction(objectQuery, location, effect, heightOffset: heightOffset),
                guard: guard ?? _alwaysTrue,
                metadata
            )
        {
        }

        protected static Func<TBeliefSet, Vector3> ConstantLocation(Vector3 location) => _ => location;

        private static Action<TBeliefSet> PathfindingAction(Func<TBeliefSet, Rigidbody> objectQuery,
            Func<TBeliefSet, Vector3> locationQuery,
            Action<TBeliefSet, Vector3> effect,
            float speed = 7f,
            float heightOffset = 0f) => beliefSet =>
        {
            Rigidbody rigidbody = objectQuery(beliefSet);
            Vector3 target = locationQuery(beliefSet);

            NavMeshPath path = new();
            NavMesh.CalculatePath(rigidbody.position,
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
                Vector3.MoveTowards(rigidbody.position, targetPosition, maxDistanceDelta: Time.deltaTime * speed);
            Debug.DrawLine(targetPosition, rigidbody.position, Color.blue);

            // Call the effect with the new position and direction
            effect(beliefSet, newPosition);
        };
    }
}
