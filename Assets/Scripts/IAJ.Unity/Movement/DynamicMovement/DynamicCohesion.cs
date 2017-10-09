using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicCohesion : DynamicArrive
    {
        public override string Name
        {
            get { return "DynamicCohesion"; }
        }


        public override MovementOutput GetMovement()
        {

            var massCenter = new Vector3();
            int closeBoids = 0;
            List<DynamicCharacter> massCenterFlocks = new List<DynamicCharacter>();
            foreach (var boid in Flock.Members)
            {
                var boidKinematicData = boid.KinematicData;


                var direction = boidKinematicData.Position - Character.Position;
                if (direction.magnitude <= Radius)
                {
                    var angle = MathHelper.ConvertVectorToOrientation(direction);
                    var angleDifference = MathHelper.ShortestAngleDifference(Character.Orientation, angle);
                    if (Math.Abs(angleDifference) <= FanAngle)
                    {
                        massCenter += boidKinematicData.Position;

                        closeBoids++;
                        if (DebugGizmos)
                        {
                            massCenterFlocks.Add(boid);
                        }
                    }
                }

            }
            if (closeBoids == 0) {
                MassCenter = new Vector3();
                return new MovementOutput();
            }
            massCenter /= closeBoids;
            if (DebugGizmos)
            {
                //foreach (var boid in massCenterFlocks)
                //{

                //    Debug.DrawLine(boid.KinematicData.Position, massCenter, this.LinksBetweenBoidsColor);

                //}
                //Gizmos.color = Color.HSVToRGB(30, 97, 99);
                // Gizmos.DrawWireSphere(massCenter, 2);
                //Debug.DrawLine(Character.Position, massCenter, Color.blue);

            }
            RealTarget.Position = massCenter;
            MassCenter = massCenter;

            return base.GetMovement();
        }

        public float Radius { get; set; }

        public double FanAngle { get; set; }

        public Flock Flock { get; set; }

        public Boolean DebugGizmos { get; set; }
        public Vector3 MassCenter { get; set; }
        public Color MassCenterColor { get; internal set; }
        public Color LinksBetweenBoidsColor { get; internal set; }
        public float FanAngleDegrees { get; internal set; }
    }


}
