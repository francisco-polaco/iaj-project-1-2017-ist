﻿using System;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity;
using Assets.Scripts.IAJ.Unity.Movement;

public class NormalCharacterController : MonoBehaviour {

    protected float XWorldSize = SceneManager.xWorldSize;
    protected float ZWorldSize = SceneManager.zWorldSize;
    private const float MaxAcceleration = 80f;
    private const float AvoidObstacleMaxAcceleration = MaxAcceleration*2;

    private const float AvoidMargin = 5;
    private const float MaxLookAhead = 10f;

    private const float AvoidObstacleWeight = 100f;
    private const float CohesionWeight = 10f;
    private const float FlockVelocityMatchingWeight = 30f;
    private const float SeparationWeight = 30f;
    private const float MouseSeekPressedWeight = 20f;

    private const float MouseSeekDefaultWeight = 0f;
    private const float StraightAheadDefaultWeight = 5f;
    private const float StraightAheadPressedWeight = 0f;

    protected const int MouseSeekListIndex = 0;
    protected const int StraightAheadIndex = 1;
    protected const int SeparationIndex = 2;
    protected const int CohesionIndex = 3;
    protected const int FlockVelocityMatchingIndex = 4;
    protected const int AvoidObstacleStartIndex = 5;


    private const int LeftClickKey = 0; // 0 = left click

    public DynamicCharacter Character;

    public BlendedMovement BlendedMovement;

    private bool _toUpdateMousePosition = false;

    private const float SeparationRadius = 5f;
    private const float SeparationFactor = 250f;

    private const float CohesionRadius = 30f;

    private const float CohesionFanAngleDegrees = 100f;
    private readonly double _cohesionFanAngleRads = MathHelper.NormalDegreeToRadian(CohesionFanAngleDegrees);

    private const float VelocityMatchingFanAngleDegrees = 60f;
    private readonly double _velocityMatchingFanAngleRads = MathHelper.NormalDegreeToRadian(VelocityMatchingFanAngleDegrees);
    private const float VelocityMatchingRadius = 20f;

    private readonly Color _separationColor = new Color(255 / 255f, 0f / 255f, 0 / 255f); //red
    private readonly Color _separationLinksBetweenBoidsColor = new Color(165/255f,42/255f,42/255f);
    private readonly Color _separationAccelarationColor = new Color(0.5f,0,0);

    private readonly Color _cohesionColor = new Color(210 / 255f, 105 / 255f, 30 / 255f); //brown
    private readonly Color _cohesionMassCenterColor = new Color(139f / 255f, 69 / 255f, 19/255f); // orange
    private readonly Color _cohesionLinksBetweenBoidsColor = new Color(218 / 255f, 165 / 255f, 32/255f); // goldenrod


    private readonly Color _velocityMatchColor = new Color(0 / 255f, 0f / 255f, 255f / 255f); //blue
    private readonly Color _velocityMatchCurrentVelocityColor = new Color(25 / 255f, 25f / 255f, 112f / 255f); //dark blue 
    private readonly Color _velocityMatchFlocksAverageVelocityColor = new Color(173f / 255f, 216f / 255f, 230f / 255f); //light blue


    //early initialization
    void Awake()
    {
        this.Character = new DynamicCharacter(this.gameObject);

        this.BlendedMovement = new BlendedMovement
        {
            Character = this.Character.KinematicData
        };
        this.Character.Movement = this.BlendedMovement;
    }

    void ResetBlended() {
        this.BlendedMovement = new BlendedMovement {
            Character = this.Character.KinematicData
        };
        this.Character.Movement = this.BlendedMovement;
    }

    // Use this for initialization
    void Start ()
    {
    }

    public void InitializeMovement(GameObject[] obstacles, List<DynamicCharacter> characters, bool booleanDebugDrawGizmos)
    {
        ResetBlended();

        var dynamicSeek = new DynamicSeek
        {
            Character = Character.KinematicData,
            Target = new KinematicData(),
            MaxAcceleration = MaxAcceleration,
            OutputDebug = booleanDebugDrawGizmos,
            OutputDebugColor = Color.black,
        };
        BlendedMovement.Movements.Insert(MouseSeekListIndex, new MovementWithWeight(dynamicSeek, MouseSeekDefaultWeight));

        var straightAhead = new DynamicStraightAhead
        {
            Character = this.Character.KinematicData,
            MaxAcceleration = MaxAcceleration,
            DebugColor = Color.yellow,
            OutputDebug = booleanDebugDrawGizmos,
            OutputDebugColor = Color.black,
        };
        this.BlendedMovement.Movements.Insert(StraightAheadIndex, new MovementWithWeight(straightAhead, StraightAheadDefaultWeight));



        Flock flock = new Flock
        {
            Members = new List<DynamicCharacter>(characters),
        };
        /* Optimization
         * Removing this character from the flock, allow us to avoid the != operation of Static Data
         * which greatly optimizes the execution of flock movements.
         */
        if(!flock.Members.Remove(Character)) throw new Exception("Impossible to Optimize. Should not have happened, try again later!");
        var separation = new DynamicSeparation
        {
            DebugColor = _separationColor,
            AccelarionColor = _separationAccelarationColor,
            LinksBetweenBoidsColor = _separationLinksBetweenBoidsColor,
            Character = this.Character.KinematicData,
            Flock = flock,
            MaxAcceleration = AvoidObstacleMaxAcceleration*3,
            Radius = SeparationRadius,
            SeparationFactor = SeparationFactor,
            DebugGizmos = booleanDebugDrawGizmos,
            OutputDebug = booleanDebugDrawGizmos,
            OutputDebugColor = _separationColor,

        };
        this.BlendedMovement.Movements.Insert(SeparationIndex, new MovementWithWeight(separation, SeparationWeight));

        var cohesion = new DynamicCohesion {
            DebugColor = _cohesionColor,
            MassCenterColor = this._cohesionMassCenterColor,
            LinksBetweenBoidsColor = this._cohesionLinksBetweenBoidsColor,
            Character = this.Character.KinematicData,
            Flock = flock,
            MaxAcceleration = MaxAcceleration,
            Radius = CohesionRadius,
            FanAngle = _cohesionFanAngleRads,
            FanAngleDegrees = CohesionFanAngleDegrees,
            Target = new KinematicData(),
            RealTarget = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos,
            OutputDebug = booleanDebugDrawGizmos,
            OutputDebugColor = _cohesionColor,
            SlowRadius = 10f,
            StopRadius = 1f,
            MaxSpeed = MaxAcceleration



        };
        this.BlendedMovement.Movements.Insert(CohesionIndex, new MovementWithWeight(cohesion, CohesionWeight));

        var flockVelocityMatching = new DynamicFlockVelocityMatching {
            DebugColor = _velocityMatchColor,
            CurrentVelocityColor = _velocityMatchCurrentVelocityColor,
            FlocksAverageVelocityColor = _velocityMatchFlocksAverageVelocityColor,
            Character = this.Character.KinematicData,
            Flock = flock,
            MaxAcceleration = MaxAcceleration,
            Radius = VelocityMatchingRadius,
            FanAngle = _velocityMatchingFanAngleRads,
            FanAngleDegrees = VelocityMatchingFanAngleDegrees,
            Target = new KinematicData(),
            OutputDebug = booleanDebugDrawGizmos,
            DebugGizmos = booleanDebugDrawGizmos,
            OutputDebugColor = _velocityMatchColor,


        };
        this.BlendedMovement.Movements.Insert(FlockVelocityMatchingIndex, new MovementWithWeight(flockVelocityMatching, 
            FlockVelocityMatchingWeight));


        foreach (var obstacle in obstacles)
        {
            var avoidObstacleMovement = new DynamicAvoidObstacle(obstacle) {
                MaxAcceleration = AvoidObstacleMaxAcceleration,
                AvoidMargin = AvoidMargin,
                MaxLookAhead = MaxLookAhead,
                Character = this.Character.KinematicData,
                DebugColor = Color.magenta,
                DebugGizmos = booleanDebugDrawGizmos,
                OutputDebug = booleanDebugDrawGizmos,
                OutputDebugColor = Color.magenta,
                SideDecreaseFactor = 2f,
            };
            this.BlendedMovement.Movements.Add(new MovementWithWeight(avoidObstacleMovement, AvoidObstacleWeight));
        }


        this.Character.Movement = this.BlendedMovement;
    }



    void Update()
    {
        Update2();
    }

    private Vector3 mouseOnWorld = new Vector3();
    private Vector3 defaultito = new Vector3();


    protected virtual void Update2()
    {
        if (Input.GetMouseButtonDown(LeftClickKey))
        {
            this.BlendedMovement.Movements[MouseSeekListIndex].Weight = MouseSeekPressedWeight;
            this.BlendedMovement.Movements[StraightAheadIndex].Weight = StraightAheadPressedWeight;
            _toUpdateMousePosition = true;
        }
        else if (Input.GetMouseButtonUp(LeftClickKey))
        {
            this.BlendedMovement.Movements[MouseSeekListIndex].Weight = MouseSeekDefaultWeight;
            this.BlendedMovement.Movements[StraightAheadIndex].Weight = StraightAheadDefaultWeight;
            _toUpdateMousePosition = false;
            mouseOnWorld = new Vector3();
        }

        if (_toUpdateMousePosition)
        {
            Camera c = Camera.main;
            Vector3 mousePosition = Input.mousePosition;
            mouseOnWorld = c.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                c.transform.position.y));
            mouseOnWorld.y = 0;

            this.BlendedMovement.Movements[MouseSeekListIndex].Movement.Target = new KinematicData(new StaticData
            {
                Position = mouseOnWorld
            });

        }
       

        this.UpdateMovingGameObject();
    }

    void OnDrawGizmos()
    {
        if (mouseOnWorld != defaultito)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(mouseOnWorld, 0.5f);
        }
    }


    private void UpdateMovingGameObject()
    {
        if (Character != null && this.Character.Movement != null)
        {
            this.Character.Update();
            this.Character.KinematicData.ApplyWorldLimit(XWorldSize, ZWorldSize);
        }
    }

}


