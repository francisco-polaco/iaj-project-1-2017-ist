using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public sealed class DynamicAvoidCharacter : DynamicMovement
    {
        public DynamicAvoidCharacter(KinematicData otherCharacterKinematicData)
        {
            this.Target = otherCharacterKinematicData;
        }

        public override string Name
        {
            get { return "AvoidCharacter"; }
        }

        public float AvoidMargin { get; set; }
        public float MaxLookAhead { get; set; }

        public static int Counter = 0;

        public override MovementOutput GetMovement()
        {

            //CENASASDASDFJAFGJASD

            if (Math.Abs((Target.Position - Character.Position).magnitude) < 0.3f)
            {
                Counter++;
                Debug.Log("Character: " + Counter);
            }
            //CENASASDASDFJAFGJASD


            Output = new MovementOutput();
            Vector3 deltaPos = Target.Position - Character.Position;
            var deltaVel = Target.velocity - Character.velocity;
            var deltaSpeed = deltaVel.magnitude;

            if (Math.Abs(deltaSpeed) < 0.1f) return Output;

            var timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / (deltaSpeed * deltaSpeed);

            if (timeToClosest > MaxLookAhead) return Output;

            var futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            var futureDistance = futureDeltaPos.magnitude;

            if(futureDistance > 2 * AvoidMargin) return new MovementOutput();

            if (futureDistance <= 0 || deltaPos.magnitude < 2 * AvoidMargin)
                Output.linear = Character.Position - Target.Position;
            else
                Output.linear = futureDeltaPos * -1;

            Output.linear = Output.linear.normalized * MaxAcceleration;

            return Output;
        }
    }
}
