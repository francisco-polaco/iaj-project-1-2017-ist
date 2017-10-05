using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour {
    public const float XWorldSize = 550;
    public const float ZWorldSize = 325f;
    public const float AvoidMargin = 4.0f;

    public int NumberOfNormalCharacters = 5; 
    public int NumberOfDebugCharacters = 2;  
    public KeyCode ReloadKey = KeyCode.R;

    public GameObject DebugCharacterGameObject;
    public GameObject NormalCharacterGameObject;

    private List<DebugCharacterController> _debugCharacterControllers;
    private List<NormalCharacterController> _normalCharacterControllers;


    // Use this for initialization
    void Start() {

        var textObj = GameObject.Find ("InstructionsText");
        if (textObj != null) 
        {
            textObj.GetComponent<Text>().text = "Colors\n\n" + "Verde - cohesion\n" + "Azul - VelocityMatch\n" + "Vermelho - Separation ";
        		
        }


        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        this._debugCharacterControllers =
            this.CloneDebugCharacters(this.DebugCharacterGameObject, NumberOfDebugCharacters - 1, obstacles);
        this._normalCharacterControllers =
            this.CloneNormalCharacters(this.NormalCharacterGameObject, NumberOfNormalCharacters - 1, obstacles);

        if(NumberOfDebugCharacters > 0) {
            this._debugCharacterControllers.Insert(0,this.DebugCharacterGameObject.GetComponent<DebugCharacterController>());
        } else {
            Destroy(this.DebugCharacterGameObject.GetComponent<DebugCharacterController>());
        }

        if(NumberOfNormalCharacters > 0) {
            this._normalCharacterControllers.Insert(0, this.NormalCharacterGameObject.GetComponent<NormalCharacterController>());
        } else {
            Destroy(this.NormalCharacterGameObject.GetComponent<NormalCharacterController>());
        }

        // LINQ expression with a lambda function, returns an array with the DynamicCharacter
        // for each secondary character controler
        var characters = this._normalCharacterControllers.Select(cc => cc.Character).ToList();
        //add the character corresponding to the main character
        characters.AddRange(this._debugCharacterControllers.Select(cc => cc.Character).ToList());

        //initialize all debug characters
        foreach (var debugCharacterController in _debugCharacterControllers) {
            debugCharacterController.InitializeMovement(obstacles, characters, true);
        }

        //initialize all secondary characters
        foreach (var normalCharacterController in this._normalCharacterControllers) {
            normalCharacterController.InitializeMovement(obstacles, characters, false);
        }
    }

    private List<NormalCharacterController> CloneNormalCharacters(GameObject objectToClone,
        int numberOfCharacters, GameObject[] obstacles) {
        var characters = new List<NormalCharacterController>();
        for (int i = 0; i < numberOfCharacters; i++) {
            var clone = GameObject.Instantiate(objectToClone);
            var characterController = clone.GetComponent<NormalCharacterController>();
            characterController.Character.KinematicData.Position = this.GenerateRandomClearPosition(obstacles);
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
            characterController.Character.KinematicData.Position = this.GenerateRandomClearPosition(obstacles);
            characters.Add(characterController);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(GameObject[] obstacles) {
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok) {
            ok = true;

            position = new Vector3(Random.Range(-XWorldSize, XWorldSize), 0, Random.Range(-ZWorldSize, ZWorldSize));

            foreach (var obstacle in obstacles) {
                var distance = (position - obstacle.transform.position).magnitude;

                //assuming obstacle is a sphere just to simplify the point selection
                if (distance < obstacle.transform.localScale.x + AvoidMargin) {
                    ok = false;
                    break;
                }
            }
        }

        return position;
    }

    void Update() {
        if (Input.GetKeyDown(this.ReloadKey)) {
            reload();
        }
    }

    private void reload() {
        //Destroy all but 1
        for(int i = 1; i < NumberOfDebugCharacters; i++) {
            var charaterController = _debugCharacterControllers[i];
            Destroy(charaterController.gameObject);
        }
        _debugCharacterControllers = new List<DebugCharacterController>();


        //Destroy all but 1
        for (int i = 1; i < NumberOfNormalCharacters; i++) {
            var charaterController = _normalCharacterControllers[i];
            Destroy(charaterController.gameObject);
        }
        _normalCharacterControllers = new List<NormalCharacterController>();

        Start();
    }
}
