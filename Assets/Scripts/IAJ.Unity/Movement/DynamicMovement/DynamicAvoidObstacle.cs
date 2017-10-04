using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        private const int Angle = 30;
        private GameObject _obstacle;
        public static int _counter = 0;

        public override string Name
        {
            get { return "AvoidObstacle"; }
        }

        public float MaxLookAhead { get; set; }
        public float AvoidMargin { get; set; }

        public DynamicAvoidObstacle()
        {
            this.Output = new MovementOutput();
        }

        public DynamicAvoidObstacle(GameObject obstacle) {
            this._obstacle = obstacle;
        }

        public override MovementOutput GetMovement()
        {
            

            Vector3 characterOrientation = Character.GetOrientationAsVector();
            var rayOrigin = Character.Position;

            if (Math.Abs((_obstacle.transform.localPosition - Character.Position).magnitude) < 0.3f)
            {
                _counter++;
                Debug.Log("Obstacle: " +_counter);
            }

            // Main Ray
            Ray mainRayVector = new Ray(rayOrigin, characterOrientation);
            //Debug.DrawLine(rayOrigin, rayOrigin + characterOrientation * MaxLookAhead, Color.black);
            RaycastHit mainRaycastHit;
            bool mainResult = _obstacle.GetComponent<Collider>().Raycast(mainRayVector, out mainRaycastHit, MaxLookAhead);

            // left ray
            Vector3 leftVector = Quaternion.AngleAxis(-Angle, Vector3.up) * characterOrientation;
            Ray leftRayVector = new Ray(rayOrigin, leftVector);
            //Debug.DrawLine(rayOrigin, rayOrigin + leftVector*MaxLookAhead/2, Color.black);
            RaycastHit leftRaycastHit;
            bool leftResult = _obstacle.GetComponent<Collider>().Raycast(leftRayVector, out leftRaycastHit, MaxLookAhead / 4);
            
            // right ray
            Vector3 rightVector = Quaternion.AngleAxis(Angle, Vector3.up) * characterOrientation;
            Ray rightRayVector = new Ray(rayOrigin, rightVector);
            //Debug.DrawLine(rayOrigin, rayOrigin + rightVector * MaxLookAhead / 2, Color.black);
            RaycastHit rightRaycastHit;
            bool rightResult = _obstacle.GetComponent<Collider>().Raycast(rightRayVector, out rightRaycastHit, MaxLookAhead / 4);

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
            else {
                return new MovementOutput();
            }
            return base.GetMovement();
        }
    }
}
