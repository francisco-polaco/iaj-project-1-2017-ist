using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement {
    public class DynamicArrive : DynamicVelocityMatch {

        public float MaxSpeed { get; set; }
        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }
        public Vector3 CircleCenter { get; private set; }
        public KinematicData RealTarget { get; set; }

        public override string Name {
            get { return "DynamicArrive"; }
        }


        public override MovementOutput GetMovement() {
            this.CircleCenter = RealTarget.Position;
            var direction = RealTarget.Position - Character.Position;
            var distance = direction.magnitude;
            float targetSpeed = 0;
            if (distance < StopRadius)
            {
                targetSpeed = 0;
            }
            else if (distance > SlowRadius)
            {
                targetSpeed = MaxSpeed;
            }
            else
            {
                targetSpeed = MaxSpeed * (distance / SlowRadius);
            }

            Target.velocity = direction.normalized * targetSpeed;
            return base.GetMovement();
        }
    }
}