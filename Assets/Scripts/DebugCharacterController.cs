using System;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;

public class DebugCharacterController : NormalCharacterController {
    private readonly Vector3 _defaultito = new Vector3();
    private readonly Vector3 _cameraNormal = new Vector3(0, 1, 0);
    private const KeyCode SeparationKeyActivate = KeyCode.X;
    private const KeyCode SeparationKeyDeactivate = KeyCode.S;
    private const KeyCode CohesionKeyActivate = KeyCode.C;
    private const KeyCode CohesionKeyDeactivate = KeyCode.D;
    private const KeyCode VelocityMatchingKeyActivate = KeyCode.V;
    private const KeyCode VelocityMatchingKeyDeactivate = KeyCode.F;
    private const KeyCode AvoidObstacleKeyActivate = KeyCode.Z;
    private const KeyCode AvoidObstacleKeyDeactivate = KeyCode.A;

    private const KeyCode DirectionalMouseSeekKeyActivate = KeyCode.B;
    private const KeyCode DirectionalMouseSeekKeyDeactivate = KeyCode.G;

    private const KeyCode NextDebug = KeyCode.N;

    private int counterVariationDebug = 0;
    private Boolean OverrideLinesVelocity = true;
    private Boolean OverrideLinesRadius = true;

    void Awake() {
        this.Character = new DynamicCharacter(this.gameObject);

        this.BlendedMovement = new BlendedMovement {
            Character = this.Character.KinematicData
        };
    }

    void OnDrawGizmos() {
        if (this.Character != null && this.Character.Movement != null) {
            BlendedMovement blendedMov = this.Character.Movement as BlendedMovement;
            if (blendedMov != null) {
                foreach (var movementWithWeight in blendedMov.Movements) {
                    if (movementWithWeight.Movement.OutputDebug && OverrideLinesVelocity) {
                        var position = movementWithWeight.Movement.Character.Position;
                        var currentContribution = movementWithWeight.CurrentContribution;
                        this.DrawThickLine(position, currentContribution, movementWithWeight.Movement.OutputDebugColor);
                    }

                    var separation = movementWithWeight.Movement as DynamicSeparation;
                    if (separation != null) {
                        if (separation.DebugGizmos && OverrideLinesRadius) {
                            UnityEditor.Handles.color = separation.DebugColor;
                            UnityEditor.Handles.DrawWireDisc(separation.Character.Position, _cameraNormal,
                                separation.Radius);
                        }
                    }


                    var cohesion = movementWithWeight.Movement as DynamicCohesion;
                    if (cohesion != null && cohesion.DebugGizmos && OverrideLinesRadius) {
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

                        if (cohesion.MassCenter != _defaultito) {
                            UnityEditor.Handles.color = cohesion.MassCenterColor;
                            UnityEditor.Handles.DrawWireDisc(cohesion.MassCenter, _cameraNormal, 0.4f);
                            UnityEditor.Handles.DrawWireDisc(cohesion.MassCenter, _cameraNormal, cohesion.SlowRadius);
                            UnityEditor.Handles.DrawWireDisc(cohesion.MassCenter, _cameraNormal, cohesion.StopRadius);

                            Gizmos.color = cohesion.MassCenterColor;
                            Gizmos.DrawSphere(cohesion.MassCenter, 0.4f);
                        }
                    }
                    var flockVelocityMatch = movementWithWeight.Movement as DynamicFlockVelocityMatching;
                    if (flockVelocityMatch != null && flockVelocityMatch.DebugGizmos && OverrideLinesRadius) {
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

                        if (flockVelocityMatch.CurrentVelocity != _defaultito) {
                            Gizmos.color = flockVelocityMatch.CurrentVelocityColor;
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position,
                                flockVelocityMatch.Character.Position + flockVelocityMatch.CurrentVelocity);
                        }
                        if (flockVelocityMatch.FlocksAverageVelocity != _defaultito) {
                            Gizmos.color = flockVelocityMatch.FlocksAverageVelocityColor;
                            Gizmos.DrawLine(flockVelocityMatch.Character.Position,
                                flockVelocityMatch.Character.Position + flockVelocityMatch.FlocksAverageVelocity);
                        }
                    }


                    var avoidObstacle = movementWithWeight.Movement as DynamicAvoidObstacle;
                    if (avoidObstacle != null && avoidObstacle.DebugGizmos && OverrideLinesRadius) {
                        //UnityEditor.Handles.color = avoidObstacle.DebugColor;
                        //UnityEditor.Handles.DrawWireDisc(avoidObstacle.Character.Position, _cameraNormal, avoidObstacle.AvoidMargin);
                        if (!object.ReferenceEquals(avoidObstacle.Target, null)) {
                            Gizmos.color = avoidObstacle.DebugColor;
                            Gizmos.DrawWireSphere(avoidObstacle.Target.Position, 0.5f);

                        }
                    }
                }
            }
        }
    }

    private void DrawThickLine(Vector3 start, Vector3 direction, Color colorToPrint, float thickness = 0.5f, float pace = 0.1f) {
        //Vector3 direction = end - start;
        if (direction == new Vector3(0, 0, 0)) return;
        Vector3 perpendicular = new Vector3(direction.z, 0, -direction.x).normalized;

        var paceVector = perpendicular * pace;
        var numberLines = thickness / pace;

        var goLeftNumber = numberLines / 2;
        Vector3 startLeft = start - paceVector * goLeftNumber;
        Gizmos.color = colorToPrint;
        for (int index = 0; index < numberLines; index++) {
            Gizmos.DrawLine(startLeft, startLeft + direction);
            startLeft = startLeft + paceVector;
        }
    }

    protected override void Update2() {
        if (Input.GetKeyDown(NextDebug)) {
            counterVariationDebug = (counterVariationDebug + 1) % 4;
            if (counterVariationDebug == 0) {
                OverrideLinesRadius = true;
                OverrideLinesVelocity = true;
            } else if (counterVariationDebug == 1) {
                OverrideLinesRadius = false;
                OverrideLinesVelocity = true;
            } else if (counterVariationDebug == 2) {
                OverrideLinesRadius = true;
                OverrideLinesVelocity = false;
            } else if (counterVariationDebug == 3) {
                OverrideLinesRadius = false;
                OverrideLinesVelocity = false;
            }

            //foreach (MovementWithWeight mov in this.BlendedMovement.Movements) {
            //    mov.Movement.OutputDebug = mov.Movement.OutputDebug && OverrideLinesVelocity;
            //}
        }

        if (Input.GetKeyDown(SeparationKeyActivate)) {
            var mov = this.BlendedMovement.Movements[SeparationIndex].Movement as DynamicSeparation;
            mov.DebugGizmos = true && OverrideLinesRadius;
            mov.OutputDebug = true && OverrideLinesVelocity;
        }
        if (Input.GetKeyDown(SeparationKeyDeactivate)) {
            var mov = this.BlendedMovement.Movements[SeparationIndex].Movement as DynamicSeparation;
            mov.DebugGizmos = false;
            mov.OutputDebug = false;
        }
        if (Input.GetKeyDown(CohesionKeyActivate)) {
            var mov = this.BlendedMovement.Movements[CohesionIndex].Movement as DynamicCohesion;
            mov.DebugGizmos = true && OverrideLinesRadius;
            mov.OutputDebug = true && OverrideLinesVelocity;

        }
        if (Input.GetKeyDown(CohesionKeyDeactivate)) {
            var mov = this.BlendedMovement.Movements[CohesionIndex].Movement as DynamicCohesion;
            mov.DebugGizmos = false;
            mov.OutputDebug = false;
        }
        if (Input.GetKeyDown(VelocityMatchingKeyActivate)) {
            var mov = this.BlendedMovement.Movements[FlockVelocityMatchingIndex].Movement as DynamicFlockVelocityMatching;
            mov.DebugGizmos = true && OverrideLinesRadius;
            mov.OutputDebug = true && OverrideLinesVelocity;

        }
        if (Input.GetKeyDown(VelocityMatchingKeyDeactivate)) {
            var mov = this.BlendedMovement.Movements[FlockVelocityMatchingIndex].Movement as DynamicFlockVelocityMatching;
            mov.DebugGizmos = false;
            mov.OutputDebug = false;
        }
        if (Input.GetKeyDown(AvoidObstacleKeyDeactivate)) {
            for (int i = AvoidObstacleStartIndex; i < this.BlendedMovement.Movements.Count; i++) {
                var mov = this.BlendedMovement.Movements[i].Movement as DynamicAvoidObstacle;
                mov.DebugGizmos = false;
                mov.OutputDebug = false;

            }
        }
        if (Input.GetKeyDown(AvoidObstacleKeyActivate)) {
            for (int i = AvoidObstacleStartIndex; i < this.BlendedMovement.Movements.Count; i++) {
                var mov = this.BlendedMovement.Movements[i].Movement as DynamicAvoidObstacle;
                mov.DebugGizmos = true && OverrideLinesRadius;
                mov.OutputDebug = true && OverrideLinesVelocity;

            }
        }
        if (Input.GetKeyDown(DirectionalMouseSeekKeyDeactivate)) {
            var mov = this.BlendedMovement.Movements[MouseSeekListIndex].Movement as DynamicSeek;
            mov.OutputDebug = false;
            var straigt = this.BlendedMovement.Movements[StraightAheadIndex].Movement as DynamicStraightAhead;
            straigt.OutputDebug = false;


        }
        if (Input.GetKeyDown(DirectionalMouseSeekKeyActivate)) {
            var mov = this.BlendedMovement.Movements[MouseSeekListIndex].Movement as DynamicSeek;
            mov.OutputDebug = true && OverrideLinesVelocity;
            var straigt = this.BlendedMovement.Movements[StraightAheadIndex].Movement as DynamicStraightAhead;
            straigt.OutputDebug = true;
        }
        base.Update2();
    }
}