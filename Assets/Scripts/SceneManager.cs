using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour {
    public const float X_WORLD_SIZE = 550;
    public const float Z_WORLD_SIZE = 325f;
    public const float AVOID_MARGIN = 4.0f;
    public const float MAX_SPEED = 20.0f;
    public const float MAX_ACCELERATION = 40.0f;
    public const float DRAG = 0.1f;

    public int NumberOfNormalCharacters = 5; // 
    public int NumberOfDebugCharacters = 15;  // 
    public KeyCode reloadKey = KeyCode.R;

    public GameObject debugCharacterGameObject;
    public GameObject normalCharacterGameObject;

    private BlendedMovement Blended { get; set; }
    private PriorityMovement Priority { get; set; }

    private List<DebugCharacterController> debugCharacterControllers;
    private List<NormalCharacterController> normalCharacterControllers;

    // Use this for initialization
    void Start() {

        //var textObj = GameObject.Find ("InstructionsText");
        //if (textObj != null) 
        //{
        //	textObj.GetComponent<Text>().text = 
        //		"Instructions\n\n" +
        //		this.debugCharacterControllers.blendedKey + " - Blended\n" +
        //		this.debugCharacterControllers.priorityKey + " - Priority\n"+
        //              this.debugCharacterControllers.stopKey + " - Stop"; 
        //}


        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        this.debugCharacterControllers =
            this.CloneDebugCharacters(this.debugCharacterGameObject, NumberOfDebugCharacters - 1, obstacles);
        this.normalCharacterControllers =
            this.CloneNormalCharacters(this.normalCharacterGameObject, NumberOfNormalCharacters - 1, obstacles);



        if(NumberOfDebugCharacters > 0) {
            this.debugCharacterControllers.Insert(0,this.debugCharacterGameObject.GetComponent<DebugCharacterController>());
        } else {
            Destroy(this.debugCharacterGameObject.GetComponent<DebugCharacterController>());
        }

        if(NumberOfNormalCharacters > 0) {
            this.normalCharacterControllers.Insert(0, this.normalCharacterGameObject.GetComponent<NormalCharacterController>());
        } else {
            Destroy(this.normalCharacterGameObject.GetComponent<NormalCharacterController>());
        }

        // LINQ expression with a lambda function, returns an array with the DynamicCharacter
        // for each secondary character controler
        var characters = this.normalCharacterControllers.Select(cc => cc.character).ToList();
        //add the character corresponding to the main character
        characters.AddRange(this.debugCharacterControllers.Select(cc => cc.character).ToList());

        //initialize all debug characters
        foreach (var debugCharacterController in debugCharacterControllers) {
            debugCharacterController.InitializeMovement(obstacles, characters, true);
        }

        //initialize all secondary characters
        foreach (var normalCharacterController in this.normalCharacterControllers) {
            normalCharacterController.InitializeMovement(obstacles, characters, false);
        }
    }

    private List<NormalCharacterController> CloneNormalCharacters(GameObject objectToClone,
        int numberOfCharacters, GameObject[] obstacles) {
        var characters = new List<NormalCharacterController>();
        for (int i = 0; i < numberOfCharacters; i++) {
            var clone = GameObject.Instantiate(objectToClone);
            var characterController = clone.GetComponent<NormalCharacterController>();
            characterController.character.KinematicData.Position = this.GenerateRandomClearPosition(obstacles);
            characters.Add(characterController);
        }

        return characters;
    }

    private List<DebugCharacterController> CloneDebugCharacters(GameObject objectToClone,
        int numberOfCharacters, GameObject[] obstacles) {
        var characters = new List<DebugCharacterController>();
        for (int i = 0; i < numberOfCharacters; i++) {
            var clone = GameObject.Instantiate(objectToClone);
            var characterController = clone.GetComponent<DebugCharacterController>();
            characterController.character.KinematicData.Position = this.GenerateRandomClearPosition(obstacles);
            characters.Add(characterController);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(GameObject[] obstacles) {
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok) {
            ok = true;

            position = new Vector3(Random.Range(-X_WORLD_SIZE, X_WORLD_SIZE), 0, Random.Range(-Z_WORLD_SIZE, Z_WORLD_SIZE));

            foreach (var obstacle in obstacles) {
                var distance = (position - obstacle.transform.position).magnitude;

                //assuming obstacle is a sphere just to simplify the point selection
                if (distance < obstacle.transform.localScale.x + AVOID_MARGIN) {
                    ok = false;
                    break;
                }
            }
        }

        return position;
    }

    //private void 
    void Update() {
        if (Input.GetKeyDown(this.reloadKey)) {
            reload();
        }
    }

    private void reload() {
        //Destroy all but 1
        for(int i = 1; i < NumberOfDebugCharacters; i++) {
            var charaterController = debugCharacterControllers[i];
            Destroy(charaterController.gameObject);
        }
        debugCharacterControllers = new List<DebugCharacterController>();


        //Destroy all but 1
        for (int i = 1; i < NumberOfNormalCharacters; i++) {
            var charaterController = normalCharacterControllers[i];
            Destroy(charaterController.gameObject);
        }
        normalCharacterControllers = new List<NormalCharacterController>();

        Start();
    }
}
