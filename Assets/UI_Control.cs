using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Control : MonoBehaviour
{ 

    private bool backButtonClicked;
    private bool startButtonClicked;

    public GameObject uiPrefab;

    public bool isTaskStarted = false; //true = when the player is in task menu
                                       //false = when player goes back to earth

    void Start()
    {
        backButtonClicked = false; //control for new menu
        startButtonClicked = false;

        uiPrefab.SetActive(false);

        
        if (uiPrefab == null)
        {
            Debug.LogError("UI Prefab is not assigned!");
            uiPrefab = Resources.Load("Task Menu") as GameObject;
        }

    }

    public void BackButton()
    {
        Debug.Log("Back button clicked");
        backButtonClicked = true;
        CloseUI();
    }

    public void StartButton()
    {
        Debug.Log("Start button clicked");
        startButtonClicked = true;
        CloseUI();
    }

    public void SpawnUI()// when player clicks on pop up prefab, this func will work
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Log("UI Initialized");

        if (uiPrefab == null) { Debug.LogError("UI not set");
                                uiPrefab = Resources.Load("Task Menu") as GameObject;
                                } //check if ui is set

        uiPrefab.SetActive(true);
        uiPrefab.transform.SetParent(camera.transform);
        uiPrefab.transform.localPosition = new Vector3(0f,2f,4f); //change ui's position according to parent's position (parent = camera)

        uiPrefab.transform.LookAt(camera.transform.position); 

        isTaskStarted = true;
    }

    public bool CloseUI() //for ui to close in popupspawn (this return value connected to "yield return new WaitUntil(() => spawn_ui.closeUI())" in popupspawn script)
    {
        if (backButtonClicked || startButtonClicked)
        {
            uiPrefab.transform.parent = null;
            isTaskStarted = false;
            Debug.Log("UI Closed");
            uiPrefab.SetActive(false);
            return true;
        }

        return false;
    }


}
