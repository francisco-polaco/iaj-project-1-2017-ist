using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        private const int Angle = 30;
        //private GameObject _obstacle;
        //public static int _counter = 0;
        private readonly Collider _collider;

        public override string Name
        {
            get { return "AvoidObstacle"; }
        }

        public float MaxLookAhead { get; set; }
        public float AvoidMargin { get; set; }
        public Boolean DebugGizmos { get; set; }

        public DynamicAvoidObstacle()
        {
            this.Output = new MovementOutput();
        }

        public DynamicAvoidObstacle(GameObject obstacle) {
            //this._obstacle = obstacle;
            _collider = obstacle.GetComponent<Collider>();
        }

        public override MovementOutput GetMovement()
        {
            

            Vector3 characterOrientation = Character.GetOrientationAsVector();
            var rayOrigin = Character.Position;

            // Main Ray
            Ray mainRayVector = new Ray(rayOrigin, characterOrientation);
            RaycastHit mainRaycastHit;
            bool mainResult = _collider.Raycast(mainRayVector, out mainRaycastHit, MaxLookAhead);

            // left ray
            Vector3 leftVector = Quaternion.AngleAxis(-Angle, Vector3.up) * characterOrientation;
            Ray leftRayVector = new Ray(rayOrigin, leftVector);
            RaycastHit leftRaycastHit;
            bool leftResult = _collider.Raycast(leftRayVector, out leftRaycastHit, MaxLookAhead / 4);
            
            // right ray
            Vector3 rightVector = Quaternion.AngleAxis(Angle, Vector3.up) * characterOrientation;
            Ray rightRayVector = new Ray(rayOrigin, rightVector);
            RaycastHit rightRaycastHit;
            bool rightResult = _collider.Raycast(rightRayVector, out rightRaycastHit, MaxLookAhead / 4);

            if (DebugGizmos)
            {
                //main
                Debug.DrawLine(rayOrigin, rayOrigin + characterOrientation * MaxLookAhead, Color.black);
                //left
                Debug.DrawLine(rayOrigin, rayOrigin + leftVector * MaxLookAhead / 2, Color.black);
                //right
                Debug.DrawLine(rayOrigin, rayOrigin + rightVector * MaxLookAhead / 2, Color.black);
            }

            if (mainResult) {
                // Debug.Log("Entrei main");
                this.Target = new KinematicData {Position = mainRaycastHit.point + mainRaycastHit.normal * AvoidMargin};
            }
            else if (leftResult)
            {
                // Debug.Log("Entrei left");
                this.Target = new KinematicData { Position = leftRaycastHit.point + leftRaycastHit.normal * AvoidMargin };
            }
            else if (rightResult)
            {
                // Debug.Log("Entrei right");
                this.Target = new KinematicData { Position = rightRaycastHit.point + rightRaycastHit.normal * AvoidMargin };
            }
            else
            {
                this.Target = null;
                return new MovementOutput();
            }
            return base.GetMovement();
        }
    }
}
