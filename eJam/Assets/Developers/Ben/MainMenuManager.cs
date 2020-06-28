using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Jump.performed += ctx => PressedPlay();
    }

    public void PressedPlay()
    {
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(1);
    }

    public void PressedExit()
    {
        GetComponent<AudioSource>().Play();
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
