using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class FlockVelocityMatching : DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "FlockVelocityMatching"; }
        }


        public override MovementOutput GetMovement()
        {
            var averageVelocity = new Vector3();
            var closeBoids = 0;
            foreach (var boid in Flock.Members)
            {
                var boidKinematicData = boid.KinematicData;
                if (Character != boidKinematicData)
                {
                    var direction = boidKinematicData.Position - Character.Position;

                    if (direction.magnitude <= Radius)
                    {
                        var angle = MathHelper.ConvertVectorToOrientation(direction);

                        var angleDifference = 
                            MathHelper.ShortestAngleDifference(Character.Orientation, angle);
                        if (Math.Abs(angleDifference) <= FanAngle)
                        {
                            averageVelocity += boidKinematicData.velocity;
                            closeBoids++;
                        }
                    }
                } else if ( DebugGizmos) {
                    // Entao e debug e estamos no caso do carater
                    CurrentVelocity = boidKinematicData.velocity;
                }
            }
            if (closeBoids == 0) return new MovementOutput();
            averageVelocity /= closeBoids;
            Target.velocity = averageVelocity;
            if (DebugGizmos) {
                FlocksAverageVelocity = averageVelocity;
            }
            return base.GetMovement();
        }

        public float Radius { get; set; }

        public float FanAngle { get; set; }

        public Flock Flock { get; set; }

        public Vector3 FlocksAverageVelocity { get; set; }
        public Vector3 CurrentVelocity { get; set; }
        public Boolean DebugGizmos { get; set; }

    }
}
