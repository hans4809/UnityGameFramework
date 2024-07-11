using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CharacterControllerEx characterController;

    private void Awake()
    {
        Camera camera = Camera.main;
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)16 / 9); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;
    }
    // Start is called before the first frame update
    void Start()
    {
        //characterController = FindObjectOfType<CharacterControllerEx>();
        
    }

    void OnPreCull() => GL.Clear(true, true, Color.black);

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(characterController.transform.position.x, characterController.transform.position.y, -10f);
    }
}
