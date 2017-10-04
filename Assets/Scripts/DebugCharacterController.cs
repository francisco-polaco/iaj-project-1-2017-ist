using System;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System.Collections.Generic;
using UnityEngine;

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
                    var wander = movementWithWeight.Movement as DynamicWander;
                    if (wander != null)
                    {
                        Gizmos.color = wander.DebugColor;
                        Gizmos.DrawWireSphere(wander.CircleCenter, wander.WanderRadius);
                    }
                    var separation = movementWithWeight.Movement as DynamicSeparation;
                    if (separation != null)
                    {
                        Gizmos.color = separation.DebugColor;
                        Gizmos.DrawWireSphere(separation.Character.Position, separation.Radius);
                    }

                    var cohesion = movementWithWeight.Movement as DynamicCohesion;
                    if (cohesion != null)
                    {
                        Gizmos.color = cohesion.DebugColor;
                        Gizmos.DrawWireSphere(cohesion.Character.Position, cohesion.Radius);
                        if (cohesion.MassCenter != null)
                        {
                            Gizmos.color = new Color(255f / 255f, 162f / 255f, 0);

                            Gizmos.DrawWireSphere(cohesion.MassCenter, 0.2f);
                        }

                    }
                    var flockVelocityMatch = movementWithWeight.Movement as FlockVelocityMatching;
                    if (flockVelocityMatch != null) {
                        Gizmos.color = flockVelocityMatch.DebugColor;
                        Gizmos.DrawWireSphere(flockVelocityMatch.Character.Position, flockVelocityMatch.Radius);
                        if (flockVelocityMatch.CurrentVelocity != null) {
                            Gizmos.color = new Color(139f / 255f, 69f / 255f, 19f / 255f);
                            //Gizmos.DrawWireSphere(cohesion.MassCenter, 0.2f);
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position, flockVelocityMatch.Character.Position + flockVelocityMatch.CurrentVelocity);
                        }
                        if (flockVelocityMatch.FlocksAverageVelocity != null) {
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