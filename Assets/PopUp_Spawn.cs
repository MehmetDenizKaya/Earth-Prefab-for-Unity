using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PopUp_Spawn : MonoBehaviour
{
    //earth
    public Transform earthTransform;
    public float earthRadius = 31.5f; // +1 for to show the point of popup prefab
    private GameObject earth;
    private Earth_Animation animationCheck;


    //popup prefab
    public GameObject popupPrefab;


    //settings
    public float spawnInterval = 3f; //prefab spawn interval
    bool checkSpawnedPopUp = true;
    public List<GameObject> spawnedPrefabs = new List<GameObject>(); // keep track of spawned popup


    //UI Control Script
    private UI_Control uiscript;

    //there is a problem when starting the game.
    //spawners shrink to earth' scales times from their original position.
    //ex: spawner with (4.4,2.1,-3.3) shrinks to (0.0044,0.0021 etc.) (if the world scale is 1000) 
    //pop ups remains same in scale


    void Start()
    {
        earth = GameObject.FindGameObjectWithTag("Earth");
        animationCheck = earth.GetComponent<Earth_Animation>();
        StartCoroutine(SetPopUp(checkSpawnedPopUp, earthRadius));

        uiscript = GameObject.Find("Main_UI").GetComponent<UI_Control>();

    }

    private IEnumerator SetPopUp(bool checkSpawnedPopUp, float radius) //make changes before spawning popups
    {
        gameObject.transform.parent = earth.transform;

        if (checkSpawnedPopUp){
            Vector3 initialPlace = gameObject.transform.position;

            //set spawnprefab to the surface
            Vector3 directionToEarth = (earthTransform.position - gameObject.transform.position).normalized;

            if (spawnedPrefabs.Count <= 1) {//if user pop up prefabs ready to be created
                SpawnPopUp(gameObject.transform.position);

                /*Debug.Log("Spawned at: " + popupPrefab.transform.position +
                "\n\tVector: " + directionToEarth + 
                "\n\tRotation: " + popupPrefab.transform.rotation);
                */
            }
        }
        else {
            Debug.LogWarning("Instantiated pop up prefab detected.");
            yield return null;
        }

    }
    private object SpawnPopUp(Vector3 spawnerPposition) //spawn pop up prefab
    {
        GameObject spawnedPopupPrefab = Instantiate(popupPrefab, spawnerPposition, Quaternion.identity, gameObject.transform);
        spawnedPopupPrefab.tag = "Pop-Up";

        Quaternion lookRotation = Quaternion.LookRotation(gameObject.transform.position * (-1));
        spawnedPopupPrefab.transform.rotation = Quaternion.Euler(lookRotation.eulerAngles + new Vector3(-180f, 0f, 0f));
        spawnedPopupPrefab.transform.localScale = new Vector3(30,30,30);

        //set spawnprefab parent (for changing popup position when turning earth)
        spawnedPrefabs.Add(spawnedPopupPrefab); // no need when country will be added

        return popupPrefab = spawnedPopupPrefab;
    }

    

    void FixedUpdate() // check for user input
    {

        //normally will check if the user did the case, for now only will destroy object when mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log(hitObject.tag);

                if (hitObject.CompareTag("Pop-Up"))
                {
                    animationCheck.checkSpeed(); //for not returning the earth animation correctly (it wasn't spinning after pop up prefab menu opens)
                    spawnedPrefabs.Remove(hitObject);
                    StartCoroutine(MoveCamera(hitObject)); //coroutine includes wait sec, its good 
                }
            }
        }
    }

    private IEnumerator MoveCamera(GameObject hitObject) //camera movement on popup prefab 
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.transform.parent = hitObject.transform; //takes the pop up transformation for sticking it while earth is moving

        Debug.Log("Hit Object Position:" + hitObject.transform.position + "\nTask Started = " + uiscript.isTaskStarted);

        Vector3 initialCameraPosition = camera.transform.position;
        Quaternion initialCameraRotation = camera.transform.rotation;

        Vector3 targetPopUpPosition = hitObject.transform.position + hitObject.transform.up * 1.4f + hitObject.transform.right * -1f;
        //+ (hitObject.transform.position - gameObject.transform.position).normalized;

        float animationDuration = 3f;
        float elapsedTime = 0f;



        while (elapsedTime < animationDuration) //looking to popup animation for camera 
        {

            /* rotate itself respect to popup prefab (rotation)
             
            float t = elapsedTime / (animationDuration / 2);
            t = t * t * (3f - 2f * t);
            camera.transform.rotation = Quaternion.Slerp(initialCameraRotation,
                                        Quaternion.LookRotation(hitObject.transform.position - camera.transform.position),
                                        t);

            */

            //get close to popup (animation)
            camera.transform.position = Vector3.Lerp(initialCameraPosition,
                                                     targetPopUpPosition,
                                                     elapsedTime / animationDuration);

            elapsedTime += Time.fixedDeltaTime;

            // popup menu look (camera rotation towards left up after being popup's child)
            camera.transform.LookAt(hitObject.transform.position
                                    //+ new Vector3(1.3f, 1.3f, 0f)
                                    );
            yield return null;


        } //camera focusing on pop up prefab

       
        Debug.Log("Camera Position Near Pop-Up:" + camera.transform.position + "\nTask Started = " + uiscript.isTaskStarted);

        /////////////////////////////////////////////// UI Instantiation ///////////////////////////////////////////////
        uiscript.SpawnUI();

        yield return new WaitUntil(() => uiscript.CloseUI()); //wait until isTaskStarted == true

        elapsedTime = 0f;
        while (elapsedTime < animationDuration) //go back to original place
        {
            camera.transform.parent = null;//detach camera from popup
            //turn back to original place (animation)
            camera.transform.position = Vector3.Lerp(targetPopUpPosition, initialCameraPosition, elapsedTime / animationDuration); //transform.position

            Quaternion newRotation = camera.transform.rotation; // keep current rotation (alignment for camera when it is going back to its original place in next while)

            float t = elapsedTime / animationDuration;
            t = t * t * (3f - 2f * t);
            camera.transform.rotation = Quaternion.Slerp(newRotation, initialCameraRotation, t);//transform.rotation


            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        } // camera going back to its original place


        camera.transform.position = initialCameraPosition; // fix for misplacement
        camera.transform.rotation = initialCameraRotation; // fix for misalignment

        Debug.Log("Original Camera Position:" + camera.transform.position + "\nTask Started = " + uiscript.isTaskStarted);
    }


}
