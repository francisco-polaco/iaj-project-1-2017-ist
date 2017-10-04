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

    private const float CohesionWeight = 10f;
    private const float FlockVelocityMatchingWeight = 20f;
    private const float SeparationWeight = 5f;


    public KeyCode stopKey = KeyCode.S;
    public KeyCode priorityKey = KeyCode.P;
    public KeyCode blendedKey = KeyCode.B;

    public GameObject movementText;
    public DynamicCharacter character;

    public BlendedMovement blendedMovement;

    private Text movementTextText;

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
            ////TODO: add your AvoidObstacle movement here
            //var avoidObstacleMovement = new DynamicAvoidObstacle(obstacle) {
            //    MaxAcceleration = MAX_ACCELERATION,
            //    AvoidMargin = AVOID_MARGIN,
            //    MaxLookAhead = MAX_LOOK_AHEAD,
            //    Character = this.character.KinematicData,
            //    DebugColor = Color.magenta
            //};
            //this.blendedMovement.Movements.Add(new MovementWithWeight(avoidObstacleMovement, 20f));
            //this.priorityMovement.Movements.Add(avoidObstacleMovement);
        }

        foreach (var otherCharacter in characters)
        {
            if (otherCharacter != this.character)
            {
                //TODO: add your AvoidCharacter movement here
                //var avoidCharacter = new DynamicAvoidCharacter(otherCharacter.KinematicData)
                //{
                //    Character = this.character.KinematicData,
                //    MaxAcceleration = MAX_ACCELERATION,
                //    AvoidMargin = AVOID_MARGIN,
                //    MaxLookAhead = MAX_LOOK_AHEAD,
                //    DebugColor = Color.cyan
                //};
                //
                //this.priorityMovement.Movements.Add(avoidCharacter);

                
            }
        }

        var wander = new DynamicWander {
            MaxAcceleration = MAX_ACCELERATION,
            WanderOffset = 3,
            WanderRadius = MathConstants.MATH_PI,
            WanderRate = MathConstants.MATH_PI,
            Character = this.character.KinematicData,
            DebugColor = Color.yellow
        };

        this.blendedMovement.Movements.Add(new MovementWithWeight(wander, 1));

        // New stuff
        Flock flock = new Flock
        {
            Members = characters,
        };

        Debug.Log("Cons: " +booleanDebugDrawGizmos);
        var separation = new DynamicSeparation
        {
            Character = this.character.KinematicData,
            DebugColor = Color.HSVToRGB(0.5f, 0.5f, 0.5f),
            Flock = flock,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = 10f,
            SeparationFactor = 20,
            DebugGizmos = booleanDebugDrawGizmos,

        };
        this.blendedMovement.Movements.Add(new MovementWithWeight(separation, SeparationWeight));

        var cohesion = new DynamicCohesion
        {
            Character = this.character.KinematicData,
            DebugColor = Color.HSVToRGB(0.6f, 0.6f, 0.6f),
            Flock = flock,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = 10f,
            FanAngle = 5f,
            Target = new KinematicData(),
            RealTarget = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos

        };
        this.blendedMovement.Movements.Add(new MovementWithWeight(cohesion, CohesionWeight));

        var flockVelocityMatching = new FlockVelocityMatching
        {
            Character = this.character.KinematicData,
            DebugColor = Color.HSVToRGB(0.6f, 0.6f, 0.6f),
            Flock = flock,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = 10f,
            FanAngle = 5f,
            Target = new KinematicData(),
            DebugGizmos = booleanDebugDrawGizmos

        };
        this.blendedMovement.Movements.Add(new MovementWithWeight(flockVelocityMatching, 
            FlockVelocityMatchingWeight));

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


