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
                Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);               
                transform.localRotation = Quaternion.Euler(0, 180, 0) * new Quaternion(pathRotation.x, 0, pathRotation.z, pathRotation.w);
            }
        }

    }
}

