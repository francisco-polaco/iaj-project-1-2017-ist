using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicFlockVelocityMatching : DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "DynamicFlockVelocityMatching"; }
        }


        public override MovementOutput GetMovement()
        {
            var averageVelocity = new Vector3();
            var closeBoids = 0;
            if (DebugGizmos)
            {
                // Entao e debug e estamos no caso do carater
                CurrentVelocity = Character.velocity;
            }
            foreach (var boid in Flock.Members)
            {
                var boidKinematicData = boid.KinematicData;


                var direction = boidKinematicData.Position - Character.Position;

                if (direction.magnitude <= Radius)
                {
                    var directonVector = MathHelper.ConvertVectorToOrientation(direction);

                    var angleDifference =
                        MathHelper.ShortestAngleDifference(Character.Orientation, directonVector);
                    if (Math.Abs(angleDifference) <= FanAngle)
                    {
                        averageVelocity += boidKinematicData.velocity;
                        closeBoids++;
                    }
                }

            }
            if (closeBoids == 0) {
                CurrentVelocity = new Vector3();
                FlocksAverageVelocity = new Vector3();
                return new MovementOutput();
            }
            averageVelocity /= closeBoids;
            Target.velocity = averageVelocity;
            if (DebugGizmos) {
                FlocksAverageVelocity = averageVelocity;
            }
            return base.GetMovement();
        }

        public float Radius { get; set; }

        public double FanAngle { get; set; }

        public Flock Flock { get; set; }

        public Vector3 FlocksAverageVelocity { get; set; }
        public Vector3 CurrentVelocity { get; set; }
        public Boolean DebugGizmos { get; set; }
        public Color CurrentVelocityColor { get; internal set; }
        public Color FlocksAverageVelocityColor { get; internal set; }
        public float FanAngleDegrees { get; internal set; }
    }
}
