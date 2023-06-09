using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace PathCreation.Examples
{
    public class MovementIndicator : MonoBehaviour
    {
        public PathCreator pathCreator;
        public PathFollower pathFollower;
        public EndOfPathInstruction endOfPathInstruction;
        public float distanceAheadOfFollower;

        void Start()
        {
        }

        void Update()
        {
            if (pathCreator != null)
            {
                float distanceTravelled = pathFollower.getDistanceTravelled() + distanceAheadOfFollower;
                transform.localRotation = Quaternion.Euler(0, 90, 0) * pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

    }
}

