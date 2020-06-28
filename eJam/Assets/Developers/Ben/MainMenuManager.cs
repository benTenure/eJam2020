using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    PlayerInputActions inputActions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Jump.performed += ctx => PressedPlay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressedPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void PressedExit()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
