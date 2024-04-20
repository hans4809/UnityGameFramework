using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CharacterControllerEx characterController;
    // Start is called before the first frame update
    void Start()
    {
        characterController = FindObjectOfType<CharacterControllerEx>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(characterController.transform.position.x, characterController.transform.position.y, -10f);
    }
}
