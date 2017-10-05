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

    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    private const float MAX_ACCELERATION = 80.0f;
    private const float MAX_SPEED = 20.0f;
    private const float DRAG = 0.1f;

    private const float AVOID_MARGIN = 5;
    private const float MAX_LOOK_AHEAD = 10f;

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

    public KeyCode stopKey = KeyCode.S;
    public KeyCode priorityKey = KeyCode.P;
    public KeyCode blendedKey = KeyCode.B;

    public GameObject movementText;
    public DynamicCharacter character;

    public BlendedMovement blendedMovement;

    private Text movementTextText;
    private bool _toUpdateMousePosition = false;

    private float SeparationRadius = 15f;
    private float SeparationFactor = 20;

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
        this.character = new DynamicCharacter(this.gameObject);
        this.movementTextText = this.movementText.GetComponent<Text>();

        this.blendedMovement = new BlendedMovement
        {
            Character = this.character.KinematicData
        };
        this.character.Movement = this.blendedMovement;
    }

    void resetBlended() {
        this.blendedMovement = new BlendedMovement {
            Character = this.character.KinematicData
        };
        this.character.Movement = this.blendedMovement;
    }

    // Use this for initialization
    void Start ()
    {
    }

    public void InitializeMovement(GameObject[] obstacles, List<DynamicCharacter> characters, bool booleanDebugDrawGizmos)
    {
        resetBlended();

        foreach (var obstacle in obstacles)
        {
            var avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidMargin = AVOID_MARGIN,
                MaxLookAhead = MAX_LOOK_AHEAD,
                Character = this.character.KinematicData,
                DebugColor = Color.magenta
            };
            this.blendedMovement.Movements.Add(new MovementWithWeight(avoidObstacleMovement, AvoidObstacleWeight));
        }

        Flock flock = new Flock
        {
            Members = characters,
        };

        var separation = new DynamicSeparation
        {
            Character = this.character.KinematicData,
            DebugColor = separationColor,
            AccelarionColor = separationAccelarationColor,
            LinksBetweenBoidsColor = separationLinksBetweenBoidsColor,
            Flock = flock,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = SeparationRadius,
            SeparationFactor = SeparationFactor,
            DebugGizmos = booleanDebugDrawGizmos,

        };
        this.blendedMovement.Movements.Add(new MovementWithWeight(separation, SeparationWeight));

        var cohesion = new DynamicCohesion {
            Character = this.character.KinematicData,
            DebugColor = cohesionColor,
            MassCenterColor = this.cohesionMassCenterColor,
            LinksBetweenBoidsColor = this.cohesionLinksBetweenBoidsColor,
            Flock = flock,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = CohesionRadius,
            FanAngle = CohesionFanAngleRads,
            FanAngleDegrees = CohesionFanAngleDegrees,
            Target = new KinematicData(),
            RealTarget = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos

        };
        this.blendedMovement.Movements.Add(new MovementWithWeight(cohesion, CohesionWeight));

        var flockVelocityMatching = new FlockVelocityMatching {
            Character = this.character.KinematicData,
            DebugColor = velocityMatchColor,
            CurrentVelocityColor = velocityMatchCurrentVelocityColor,
            FlocksAverageVelocityColor = velocityMatchFlocksAverageVelocityColor,

            Flock = flock,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = VelocityMatchingRadius,
            FanAngle = VelocityMatchingFanAngleRads,
            FanAngleDegrees = VelocityMatchingFanAngleDegrees,
            Target = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos

        };
        this.blendedMovement.Movements.Add(new MovementWithWeight(flockVelocityMatching, 
            FlockVelocityMatchingWeight));


        var dynamicSeek = new DynamicSeek
        {
            Character = character.KinematicData,
            Target = new KinematicData(),
            MaxAcceleration = MAX_ACCELERATION
        };

        blendedMovement.Movements.Insert(MouseSeekListIndex, new MovementWithWeight(dynamicSeek, MouseSeekDefaultWeight));


        var straightAhead = new DynamicStraightAhead
        {
            Character = this.character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            DebugColor = Color.yellow
        };

        this.blendedMovement.Movements.Insert(StraightAheadIndex, new MovementWithWeight(straightAhead, StraightAheadDefaultWeight));
        this.character.Movement = this.blendedMovement;
    }



    void Update()
    {
        if (Input.GetKeyDown(this.stopKey))
        {
            this.character.Movement = null;
        }
        else if (Input.GetKeyDown(this.blendedKey))
        {
            this.character.Movement = this.blendedMovement;
        }
        else if (Input.GetMouseButtonDown(LeftClickKey))
        {
            this.blendedMovement.Movements[MouseSeekListIndex].Weight = MouseSeekPressedWeight;
            this.blendedMovement.Movements[StraightAheadIndex].Weight = StraightAheadPressedWeight;
            _toUpdateMousePosition = true;
        }
        else if (Input.GetMouseButtonUp(LeftClickKey))
        {
            this.blendedMovement.Movements[MouseSeekListIndex].Weight = MouseSeekDefaultWeight;
            this.blendedMovement.Movements[StraightAheadIndex].Weight = StraightAheadDefaultWeight;
            _toUpdateMousePosition = false;
        }

        if (_toUpdateMousePosition)
        {
            Camera c = Camera.main;
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseOnWorld = c.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                c.transform.position.y));
            mouseOnWorld.y = 0;
            this.blendedMovement.Movements[MouseSeekListIndex].Movement.Target = new KinematicData(new StaticData
            {
                Position = mouseOnWorld
            });
        }

        this.UpdateMovingGameObject();
        this.UpdateMovementText();
    }


    

    private void UpdateMovingGameObject()
    {
        if (character != null && this.character.Movement != null)
        {
            this.character.Update();
            this.character.KinematicData.ApplyWorldLimit(X_WORLD_SIZE, Z_WORLD_SIZE);
        }
    }

    private void UpdateMovementText()
    {
        //if (character != null && this.character.Movement == null)
        //{
        //    this.movementTextText.text = this.name + " Movement: Stationary";
        //}
        //else
        //{
        //    this.movementTextText.text = this.name + " Movement: " + this.character.Movement.Name;
        //}
    } 

}


