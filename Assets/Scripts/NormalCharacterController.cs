using System;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity;
using Assets.Scripts.IAJ.Unity.Movement;

public class NormalCharacterController : MonoBehaviour {

    public const float XWorldSize = 55;
    public const float ZWorldSize = 32.5f;
    private const float MaxAcceleration = 80.0f;

    private const float AvoidMargin = 5;
    private const float MaxLookAhead = 10f;

    private const float AvoidObstacleWeight = 50f;
    private const float CohesionWeight = 10f;
    private const float FlockVelocityMatchingWeight = 20f;
    private const float SeparationWeight = 5f;
    private const float MouseSeekDefaultWeight = 0f;
    private const float MouseSeekPressedWeight = 5f;
    private const float StraightAheadDefaultWeight = MouseSeekPressedWeight;
    private const float StraightAheadPressedWeight = MouseSeekDefaultWeight;

    private const int MouseSeekListIndex = 0;
    private const int StraightAheadIndex = 1;
    private const int LeftClickKey = 0; // 0 = left click

    public DynamicCharacter Character;

    public BlendedMovement BlendedMovement;

    private bool _toUpdateMousePosition = false;

    private float SeparationRadius = 15f;
    private float SeparationFactor = 20f;

    private float CohesionRadius = 11f;

    private static float CohesionFanAngleDegrees = 100f;
    private double CohesionFanAngleRads = MathHelper.NormalDegreeToRadian(CohesionFanAngleDegrees);

    private static float VelocityMatchingFanAngleDegrees = 60f;
    private double VelocityMatchingFanAngleRads = MathHelper.NormalDegreeToRadian(VelocityMatchingFanAngleDegrees);
    private float VelocityMatchingRadius = 12f;

    private Color separationColor = new Color(255 / 255f, 0f / 255f, 0 / 255f); //red
    private Color separationLinksBetweenBoidsColor = new Color(165/255f,42/255f,42/255f);
    private Color separationAccelarationColor = new Color(0.5f,0,0);

    private Color cohesionColor = new Color(210 / 255f, 105 / 255f, 30 / 255f); //brown
    private Color cohesionMassCenterColor = new Color(139f / 255f, 69 / 255f, 19/255f); // orange
    private Color cohesionLinksBetweenBoidsColor = new Color(218 / 255f, 165 / 255f, 32/255f); // goldenrod


    private Color velocityMatchColor = new Color(0 / 255f, 0f / 255f, 255f / 255f); //blue
    private Color velocityMatchCurrentVelocityColor = new Color(25 / 255f, 25f / 255f, 112f / 255f); //dark blue 
    private Color velocityMatchFlocksAverageVelocityColor = new Color(173f / 255f, 216f / 255f, 230f / 255f); //light blue


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

        foreach (var obstacle in obstacles)
        {
            var avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
            {
                MaxAcceleration = MaxAcceleration,
                AvoidMargin = AvoidMargin,
                MaxLookAhead = MaxLookAhead,
                Character = this.Character.KinematicData,
                DebugColor = Color.magenta
            };
            this.BlendedMovement.Movements.Add(new MovementWithWeight(avoidObstacleMovement, AvoidObstacleWeight));
        }

        Flock flock = new Flock
        {
            Members = characters,
        };

        var separation = new DynamicSeparation
        {
            DebugColor = separationColor,
            AccelarionColor = separationAccelarationColor,
            LinksBetweenBoidsColor = separationLinksBetweenBoidsColor,
            Character = this.Character.KinematicData,
            Flock = flock,
            MaxAcceleration = MaxAcceleration,
            Radius = SeparationRadius,
            SeparationFactor = SeparationFactor,
            DebugGizmos = booleanDebugDrawGizmos,

        };
        this.BlendedMovement.Movements.Add(new MovementWithWeight(separation, SeparationWeight));

        var cohesion = new DynamicCohesion {
            DebugColor = cohesionColor,
            MassCenterColor = this.cohesionMassCenterColor,
            LinksBetweenBoidsColor = this.cohesionLinksBetweenBoidsColor,
            Character = this.Character.KinematicData,
            Flock = flock,
            MaxAcceleration = MaxAcceleration,
            Radius = CohesionRadius,
            FanAngle = CohesionFanAngleRads,
            FanAngleDegrees = CohesionFanAngleDegrees,
            Target = new KinematicData(),
            RealTarget = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos

        };
        this.BlendedMovement.Movements.Add(new MovementWithWeight(cohesion, CohesionWeight));

        var flockVelocityMatching = new FlockVelocityMatching {
            DebugColor = velocityMatchColor,
            CurrentVelocityColor = velocityMatchCurrentVelocityColor,
            FlocksAverageVelocityColor = velocityMatchFlocksAverageVelocityColor,
            Character = this.Character.KinematicData,
            Flock = flock,
            MaxAcceleration = MaxAcceleration,
            Radius = VelocityMatchingRadius,
            FanAngle = VelocityMatchingFanAngleRads,
            FanAngleDegrees = VelocityMatchingFanAngleDegrees,
            Target = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos

        };
        this.BlendedMovement.Movements.Add(new MovementWithWeight(flockVelocityMatching, 
            FlockVelocityMatchingWeight));


        var dynamicSeek = new DynamicSeek
        {
            Character = Character.KinematicData,
            Target = new KinematicData(),
            MaxAcceleration = MaxAcceleration
        };

        BlendedMovement.Movements.Insert(MouseSeekListIndex, new MovementWithWeight(dynamicSeek, MouseSeekDefaultWeight));


        var straightAhead = new DynamicStraightAhead
        {
            Character = this.Character.KinematicData,
            MaxAcceleration = MaxAcceleration,
            DebugColor = Color.yellow
        };

        this.BlendedMovement.Movements.Insert(StraightAheadIndex, new MovementWithWeight(straightAhead, StraightAheadDefaultWeight));
        this.Character.Movement = this.BlendedMovement;
    }



    void Update()
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
        }

        if (_toUpdateMousePosition)
        {
            Camera c = Camera.main;
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseOnWorld = c.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                c.transform.position.y));
            mouseOnWorld.y = 0;
            this.BlendedMovement.Movements[MouseSeekListIndex].Movement.Target = new KinematicData(new StaticData
            {
                Position = mouseOnWorld
            });
        }

        this.UpdateMovingGameObject();
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


