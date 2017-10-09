using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicSeparation : DynamicMovement
    {
        public override string Name
        {
            get { return "DynamicSeparation"; }
        }

        public DynamicSeparation()
        {
            this.Output = new MovementOutput();
        }

        public override MovementOutput GetMovement()
        {

            Output = new MovementOutput();

            foreach (var boid in Flock.Members)
            {
                var boidKinematicData = boid.KinematicData;

                
                var direction = Character.Position - boidKinematicData.Position; //Boid to Char
                var distance = direction.magnitude;
                if (direction.magnitude < Radius)
                {
                    var separationStrength = Math.Min(SeparationFactor / (distance * distance),
                        MaxAcceleration);
                    direction.Normalize();
                    Output.linear += direction * separationStrength;


                    //if (DebugGizmos)
                    //{  
                    //    Debug.DrawLine(boid.KinematicData.Position, Character.Position, this.LinksBetweenBoidsColor);
                    //}
                }
                
            }


            //if (DebugGizmos)
            //{
            //    Debug.DrawLine(Character.Position, Character.Position + Output.linear, this.AccelarionColor);
            //}


            if (Output.linear.magnitude > MaxAcceleration)
            {
                Output.linear.Normalize();
                Output.linear *= MaxAcceleration;
            }


            return this.Output;
        }


        public float SeparationFactor { get; set; }

        public float Radius { get; set; }

        public Flock Flock { get; set; }

        public Boolean DebugGizmos { get; set; }

        public Color AccelarionColor { get; set; }
        public Color LinksBetweenBoidsColor { get; set; }
    }
}
