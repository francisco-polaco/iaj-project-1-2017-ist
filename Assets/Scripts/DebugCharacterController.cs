using System;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;

public class DebugCharacterController : NormalCharacterController
{
    private readonly Vector3 _defaultito = new Vector3();
    private readonly Vector3 _cameraNormal = new Vector3(0, 1, 0);
    private const KeyCode SeparationKeyActivate = KeyCode.X;
    private const KeyCode SeparationKeyDeactivate = KeyCode.S;
    private const KeyCode CohesionKeyActivate = KeyCode.C;
    private const KeyCode CohesionKeyDeactivate = KeyCode.D;
    private const KeyCode VelocityMatchingKeyActivate = KeyCode.V;
    private const KeyCode VelocityMatchingKeyDeactivate = KeyCode.F;

    void Awake()
    {
        this.Character = new DynamicCharacter(this.gameObject);

        this.BlendedMovement = new BlendedMovement
        {
            Character = this.Character.KinematicData
        };
    }

    void OnDrawGizmos()
    {
        if (this.Character != null && this.Character.Movement != null)
        {
            BlendedMovement blendedMov = this.Character.Movement as BlendedMovement;
            if (blendedMov != null)
            {
                foreach (var movementWithWeight in blendedMov.Movements)
                {
                    var separation = movementWithWeight.Movement as DynamicSeparation;
                    if (separation != null && separation.DebugGizmos)
                    {
                        UnityEditor.Handles.color = separation.DebugColor;
                        UnityEditor.Handles.DrawWireDisc(separation.Character.Position, _cameraNormal,
                            separation.Radius);
                    }

                    var cohesion = movementWithWeight.Movement as DynamicCohesion;
                    if (cohesion != null && cohesion.DebugGizmos)
                    {
                        UnityEditor.Handles.color = cohesion.DebugColor;
                        UnityEditor.Handles.DrawWireDisc(cohesion.Character.Position, _cameraNormal, cohesion.Radius);

                        Vector3 rightVector = Quaternion.AngleAxis(cohesion.FanAngleDegrees, Vector3.up) *
                                              cohesion.Character.GetOrientationAsVector();
                        Vector3 leftVector = Quaternion.AngleAxis(-cohesion.FanAngleDegrees, Vector3.up) *
                                             cohesion.Character.GetOrientationAsVector();

                        var point1 = cohesion.Character.Position + rightVector.normalized * cohesion.Radius;
                        var point2 = cohesion.Character.Position + leftVector.normalized * cohesion.Radius;
                        UnityEditor.Handles.DrawLine(cohesion.Character.Position, point1);
                        UnityEditor.Handles.DrawLine(cohesion.Character.Position, point2);

                        if (cohesion.MassCenter != _defaultito)
                        {
                            UnityEditor.Handles.color = cohesion.MassCenterColor;
                            UnityEditor.Handles.DrawWireDisc(cohesion.MassCenter, _cameraNormal, 0.4f);
                            Gizmos.color = cohesion.MassCenterColor;
                            Gizmos.DrawSphere(cohesion.MassCenter, 0.4f);
                        }
                    }
                    var flockVelocityMatch = movementWithWeight.Movement as DynamicFlockVelocityMatching;
                    if (flockVelocityMatch != null && flockVelocityMatch.DebugGizmos)
                    {
                        UnityEditor.Handles.color = flockVelocityMatch.DebugColor;
                        UnityEditor.Handles.DrawWireDisc(flockVelocityMatch.Character.Position, _cameraNormal,
                            flockVelocityMatch.Radius);

                        Vector3 rightVector = Quaternion.AngleAxis(flockVelocityMatch.FanAngleDegrees, Vector3.up) *
                                              flockVelocityMatch.Character.GetOrientationAsVector();
                        Vector3 leftVector = Quaternion.AngleAxis(-flockVelocityMatch.FanAngleDegrees, Vector3.up) *
                                             flockVelocityMatch.Character.GetOrientationAsVector();

                        var point1 = flockVelocityMatch.Character.Position +
                                     rightVector.normalized * flockVelocityMatch.Radius;
                        var point2 = flockVelocityMatch.Character.Position +
                                     leftVector.normalized * flockVelocityMatch.Radius;
                        UnityEditor.Handles.DrawLine(flockVelocityMatch.Character.Position, point1);
                        UnityEditor.Handles.DrawLine(flockVelocityMatch.Character.Position, point2);

                        if (flockVelocityMatch.CurrentVelocity != _defaultito)
                        {
                            Gizmos.color = flockVelocityMatch.CurrentVelocityColor;
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position,
                                flockVelocityMatch.Character.Position + flockVelocityMatch.CurrentVelocity);
                        }
                        if (flockVelocityMatch.FlocksAverageVelocity != _defaultito)
                        {
                            Gizmos.color = flockVelocityMatch.FlocksAverageVelocityColor;
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position,
                                flockVelocityMatch.Character.Position + flockVelocityMatch.FlocksAverageVelocity);
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

    protected override void Update2()
    {
        if (Input.GetKeyDown(SeparationKeyActivate))
        {
            var mov = this.BlendedMovement.Movements[SeparationIndex].Movement as DynamicSeparation;
            mov.DebugGizmos = true;
        }
        if (Input.GetKeyDown(SeparationKeyDeactivate))
        {
            var mov = this.BlendedMovement.Movements[SeparationIndex].Movement as DynamicSeparation;
            mov.DebugGizmos = false;
        }
        if (Input.GetKeyDown(CohesionKeyActivate))
        {
            var mov = this.BlendedMovement.Movements[CohesionIndex].Movement as DynamicCohesion;
            mov.DebugGizmos = true;
        }
        if (Input.GetKeyDown(CohesionKeyDeactivate))
        {
            var mov = this.BlendedMovement.Movements[CohesionIndex].Movement as DynamicCohesion;
            mov.DebugGizmos = false;
        }
        if (Input.GetKeyDown(VelocityMatchingKeyActivate))
        {
            var mov = this.BlendedMovement.Movements[FlockVelocityMatchingIndex].Movement as DynamicFlockVelocityMatching;
            mov.DebugGizmos = true;
        }
        if (Input.GetKeyDown(VelocityMatchingKeyDeactivate))
        {
            var mov = this.BlendedMovement.Movements[FlockVelocityMatchingIndex].Movement as DynamicFlockVelocityMatching;
            mov.DebugGizmos = false;
        }

        base.Update2();
    }
}