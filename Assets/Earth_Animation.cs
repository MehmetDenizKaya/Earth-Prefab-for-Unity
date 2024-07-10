using System;
using System.Collections;
using UnityEngine;

public class Earth_Animation : MonoBehaviour
{
    //world move
    public GameObject parent;
    public Camera cam;

    public float MouseRotationPower = 8f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float slowedSpeed = 0.1f; //world speed after clicking on it or making transition after the WaitandReset()

    //mouse check
    private bool isMoved = false;

    //animation
    private Animator animator;

    public float resetDuration = 5f; // how much time passes for earth to go into original rotation
    private float currentSpeed; //parent(Earth) -> animator -> speed  
    public float smoothSpeed = 10f; // smoothness of animation pass

    void Start()
    {
        animator = parent.GetComponent<Animator>(); //for setting speed of animation
        currentSpeed = animator.speed; // inital value in case of animation speed changes
        // originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnMouseDown() // mouse button pressed down
    {
        Debug.Log("OnMouseDown, currentSpeed: " + currentSpeed);
        animator.speed = slowedSpeed; // Slow down animation when the mouse is pressed down
        StopAllCoroutines(); //just in case
    }

    void OnMouseDrag() // when the earth is moving by mouse
    {
        float rotX = Input.GetAxis("Mouse X") * MouseRotationPower;
        float rotY = Input.GetAxis("Mouse Y") * MouseRotationPower;

        Vector3 right = Vector3.Cross(cam.transform.up, transform.position - cam.transform.position);
        Vector3 up = Vector3.Cross(transform.position - cam.transform.position, right);

        transform.rotation = Quaternion.AngleAxis(-rotX, up) * transform.rotation;
        transform.rotation = Quaternion.AngleAxis(rotY, right) * transform.rotation;

        isMoved = true; // will start waitandreset coroutine
    }

    private bool isSpinning = false;
    void FixedUpdate() // control for mouse scroll function
    {

        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation, smoothSpeed * Time.fixedDeltaTime);

        //change rotation of earth with mouse scroll
        float scroll = Input.mouseScrollDelta.y;

        float rotationAmount = -scroll * (MouseRotationPower * 60) * Time.fixedDeltaTime;
        transform.Rotate(Vector3.forward, rotationAmount, Space.World);

    }

    void OnMouseUp()
    {
        // When the mouse button is released
        if (isMoved)
        {
            isMoved = false; // Reset the move flag
            StartCoroutine(WaitAndReset());
        }
    }


    private bool isResetting = false; // for checking if earth only did animation once

    public IEnumerator WaitAndReset() // control the animation of earth going back
    {

        Debug.Log("WaitReset, animation speed: " + currentSpeed);

        yield return new WaitForSeconds(resetDuration);

        float elapsedTime = 0f;
        // Vector3 startingPosition = transform.position;
        Quaternion startingRotation = transform.rotation;

        while (elapsedTime < resetDuration)
        {
            float t = elapsedTime / resetDuration;
            t = t * t * (3f - 2f * t); // Smoothstep formula for more smooth exponential interpolation
            transform.rotation = Quaternion.Slerp(startingRotation, originalRotation, t);
            elapsedTime += Time.deltaTime;

            yield return null; // this controls while loop movement, doesn't end all coroutine
        }

        transform.rotation = originalRotation;

        animator.speed = currentSpeed; // resume the animation to its original speed

        checkSpeed();
        Debug.Log("Speed returned back to normal:" + currentSpeed);

        isResetting = false;
    }


    public void checkSpeed()
    {
        check_Speed(this.isResetting);
    }
    public void check_Speed(bool isResetting)
    {
        if (isResetting)
        {
            Debug.Log("Animation Problem Detected, earth will return to normal");

            animator.speed = currentSpeed;
        }
    }
}
