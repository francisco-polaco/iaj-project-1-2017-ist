using System;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;

public class DebugCharacterController : NormalCharacterController
{

    //public const float X_WORLD_SIZE = 55;
    //public const float Z_WORLD_SIZE = 32.5f;
    //private const float MAX_ACCELERATION = 40.0f;
    //private const float MAX_SPEED = 20.0f;
    //private const float DRAG = 0.1f;
    //
    //private const float AVOID_MARGIN = 5;
    //private const float MAX_LOOK_AHEAD = 10f;
    //
    //public DynamicCharacter character;
    //private PriorityMovement priorityMovement;

    private Vector3 defaultito = new Vector3();
    private readonly Vector3 cameraNormal = new Vector3(0, 1, 0);

    void Awake()
    {
        
        this.character = new DynamicCharacter(this.gameObject);
    
        this.blendedMovement = new BlendedMovement
        {
            Character = this.character.KinematicData
        };
    }
    void OnDrawGizmos()
    {
        //TODO: this code is not working, try to figure it out
        if (this.character != null && this.character.Movement != null)
        {
            BlendedMovement blendedMov = this.character.Movement as BlendedMovement;
            if (blendedMov != null)
            {
                foreach (var movementWithWeight in blendedMov.Movements)
                {
                    //var wander = movementWithWeight.Movement as DynamicWander;
                    //if (wander != null)
                    //{
                    //    UnityEditor.Handles.color = wander.DebugColor;
                    //    UnityEditor.Handles.DrawWireDisc(wander.CircleCenter, cameraNormal,wander.WanderRadius);
                    //}
                    var separation = movementWithWeight.Movement as DynamicSeparation;
                    if (separation != null)
                    {
                        //Gizmos.color = separation.DebugColor;
                        UnityEditor.Handles.color = separation.DebugColor;
                        UnityEditor.Handles.DrawWireDisc(separation.Character.Position, cameraNormal,separation.Radius);


                        
                        //var direction = boidKinematicData.Position - Character.Position;
                        //if (direction.magnitude <= Radius) {
                        //var angle = MathHelper.ConvertVectorToOrientation(direction);
                        //    var angleDifference = MathHelper.ShortestAngleDifference(Character.Orientation, angle);
                        //    if (Math.Abs(angleDifference) <= FanAngle)


                        //Gizmos.DrawWireSphere(separation.Character.Position, separation.Radius);
                    }

                        var cohesion = movementWithWeight.Movement as DynamicCohesion;
                    if (cohesion != null)
                    {
                        UnityEditor.Handles.color = cohesion.DebugColor;
                        UnityEditor.Handles.DrawWireDisc(cohesion.Character.Position, cameraNormal, cohesion.Radius);

                        //Vector3 vector = (cohesion.Character.Position + cohesion.Character.velocity.normalized * cohesion.Radius);
                        Vector3 rightVector = Quaternion.AngleAxis(cohesion.FanAngle, Vector3.up) * cohesion.Character.GetOrientationAsVector();
                        Vector3 leftVector = Quaternion.AngleAxis(-cohesion.FanAngle, Vector3.up) * cohesion.Character.GetOrientationAsVector();

                        var point1 = cohesion.Character.Position + rightVector.normalized * cohesion.Radius;
                        var point2 = cohesion.Character.Position + leftVector.normalized * cohesion.Radius;
                        UnityEditor.Handles.DrawLine(cohesion.Character.Position, point1);
                        UnityEditor.Handles.DrawLine(cohesion.Character.Position, point2);

                        if (cohesion.MassCenter != defaultito)
                        {
                            UnityEditor.Handles.color = new Color(255f / 255f, 162f / 255f, 0);
                            UnityEditor.Handles.DrawWireDisc(cohesion.MassCenter,cameraNormal, 0.4f);
                        }
                    }
                    var flockVelocityMatch = movementWithWeight.Movement as FlockVelocityMatching;
                    if (flockVelocityMatch != null) {
                        UnityEditor.Handles.color = flockVelocityMatch.DebugColor;
                        UnityEditor.Handles.DrawWireDisc(flockVelocityMatch.Character.Position, cameraNormal,flockVelocityMatch.Radius);

                        Vector3 rightVector = Quaternion.AngleAxis(flockVelocityMatch.FanAngle, Vector3.up) * flockVelocityMatch.Character.GetOrientationAsVector();
                        Vector3 leftVector = Quaternion.AngleAxis(-flockVelocityMatch.FanAngle, Vector3.up) * flockVelocityMatch.Character.GetOrientationAsVector();

                        var point1 = flockVelocityMatch.Character.Position + rightVector.normalized * flockVelocityMatch.Radius;
                        var point2 = flockVelocityMatch.Character.Position + leftVector.normalized * flockVelocityMatch.Radius;
                        UnityEditor.Handles.DrawLine(flockVelocityMatch.Character.Position, point1);
                        UnityEditor.Handles.DrawLine(flockVelocityMatch.Character.Position, point2);



                        if (flockVelocityMatch.CurrentVelocity != defaultito) {
                            Gizmos.color = new Color(139f / 255f, 69f / 255f, 19f / 255f);
                            //Gizmos.DrawWireSphere(cohesion.MassCenter, 0.2f);
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position, flockVelocityMatch.Character.Position + flockVelocityMatch.CurrentVelocity);
                        }
                        if (flockVelocityMatch.FlocksAverageVelocity != defaultito) {
                            Gizmos.color = new Color(218f / 255f, 165f / 255f, 32f / 255f);
                            //Gizmos.DrawWireSphere(cohesion.MassCenter, 0.2f);
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position, flockVelocityMatch.Character.Position + flockVelocityMatch.FlocksAverageVelocity);
                        }
                    }


                    //var avoidObstacle = movementWithWeight.Movement as DynamicAvoidObstacle;
                    //if (avoidObstacle != null)
                    //{
                    //    Gizmos.color = avoidObstacle.DebugColor;
                    //    if (!object.ReferenceEquals(avoidObstacle.Target, null))
                    //    {
                    //        Gizmos.DrawWireSphere(avoidObstacle.Target.Position, 5f);
                    //        Gizmos.color = Color.cyan;
                    //        Gizmos.DrawWireSphere(avoidObstacle.Target.Position, avoidObstacle.AvoidMargin);
                    //    }
                    //}
                }
            }
        }

    }

}