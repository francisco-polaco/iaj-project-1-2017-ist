using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour {
    public const float xWorldSize = 550f / 6;
    public const float zWorldSize = 325f / 7;
    public float AvoidMargin = 20f;

    public int NumberOfNormalCharacters = 5; 
    public int NumberOfDebugCharacters = 2;  
    public KeyCode ReloadKey = KeyCode.R;

    public GameObject DebugCharacterGameObject;
    public GameObject NormalCharacterGameObject;

    private List<DebugCharacterController> _debugCharacterControllers;
    private List<NormalCharacterController> _normalCharacterControllers;

    private List<GameObject> _characters;

    //public static float XWorldSize {
    //    get {
    //        return xWorldSize;
    //    }
    //}

    //public static float ZWorldSize {
    //    get {
    //        return zWorldSize;
    //    }
    //}


    // Use this for initialization
    void Start()
    {

        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        _characters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Obstacle"));

        this._debugCharacterControllers =
            this.CloneDebugCharacters(this.DebugCharacterGameObject, NumberOfDebugCharacters - 1);
        this._normalCharacterControllers =
            this.CloneNormalCharacters(this.NormalCharacterGameObject, NumberOfNormalCharacters - 1);

        if(NumberOfDebugCharacters > 0)
        {

            var characterController = this.DebugCharacterGameObject.GetComponent<DebugCharacterController>();
            this._debugCharacterControllers.Insert(0,characterController);
            characterController.Character.KinematicData.Position = this.GenerateRandomClearPosition(_characters);
            characterController.Character.KinematicData.SetOrientationFromVelocity((new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1))));

        } else {
            Destroy(this.DebugCharacterGameObject.GetComponent<DebugCharacterController>());
        }

        if(NumberOfNormalCharacters > 0) {
            var characterController = this.NormalCharacterGameObject.GetComponent<NormalCharacterController>();
            this._normalCharacterControllers.Insert(0, this.NormalCharacterGameObject.GetComponent<NormalCharacterController>());
            characterController.Character.KinematicData.Position = this.GenerateRandomClearPosition(_characters);
            characterController.Character.KinematicData.SetOrientationFromVelocity((new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100))));

        }
        else {
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
        int numberOfCharacters) {
        var characters = new List<NormalCharacterController>();
        for (int i = 0; i < numberOfCharacters; i++) {
            var clone = GameObject.Instantiate(objectToClone);
            var characterController = clone.GetComponent<NormalCharacterController>();
            characterController.Character.KinematicData.Position = this.GenerateRandomClearPosition(_characters);
            characterController.Character.KinematicData.SetOrientationFromVelocity((new Vector3(Random.Range(-1, 1),0, Random.Range(-1, 1))));
            _characters.Add(characterController.gameObject);
            characters.Add(characterController);
        }
        return characters;
    }

    private List<DebugCharacterController> CloneDebugCharacters(GameObject objectToClone,
        int numberOfCharacters) {
        var characters = new List<DebugCharacterController>();
        for (int i = 0; i < numberOfCharacters; i++) {
            var clone = GameObject.Instantiate(objectToClone);
            var characterController = clone.GetComponent<DebugCharacterController>();
            characterController.Character.KinematicData.Position = this.GenerateRandomClearPosition(_characters);
            characterController.Character.KinematicData.SetOrientationFromVelocity((new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1))));
            _characters.Add(characterController.gameObject);
            characters.Add(characterController);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(List<GameObject> obstacles)
    {
        int safeGuard = 0;
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok)
        {
            safeGuard++;
            if (safeGuard % 500 == 0)
            {
                AvoidMargin *= 0.90f;
                if (AvoidMargin < 0.5f)
                {
                    AvoidMargin = 0f;
                }
            }
            ok = true;

            position = new Vector3(Random.Range(-xWorldSize, xWorldSize), 0, Random.Range(-zWorldSize, zWorldSize));

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
            Reload();
        }
    }

    private void Reload() {
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

    void OnDrawGizmos() {
      Gizmos.color = Color.black;

      var UR = new Vector3(xWorldSize-1f , 0, zWorldSize-1f );
      var UL = new Vector3(-xWorldSize +1f, 0, zWorldSize-1f );
      var LR = new Vector3(xWorldSize-1f , 0, -zWorldSize+1f );
      var LL = new Vector3(-xWorldSize+1f , 0, -zWorldSize+1f );

      Gizmos.DrawLine(UR,UL);
      Gizmos.DrawLine(UR,LR);
      Gizmos.DrawLine(LL,UL);
      Gizmos.DrawLine(LL,LR);
    }

}
