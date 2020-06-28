using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public IntVariable distance;
    public IntVariable people;

    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI peopleText;

    public TextMeshProUGUI distanceStats;
    public TextMeshProUGUI peopleStats;
    
    public GameObject inGamePanel;
    public GameObject deathPanel;

    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Jump.performed += ctx => PressJumpRestart();
    }

    private void Start()
    {
        inGamePanel.SetActive(true);
        deathPanel.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        distanceText.text = distance.value.ToString();

        peopleText.text = people.value.ToString();
    }

    void PressJumpRestart()
    {
        if (deathPanel.activeInHierarchy)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        // We're just assuming we're reloading the current game scene since yeah... idk
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchToDeathScreen()
    {
        distanceStats.text = $"{distance.value} blocks";
        peopleStats.text = $"{people.value} people";
        
        inGamePanel.SetActive(false);
        deathPanel.SetActive(true);
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
