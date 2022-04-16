using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BallHandler : MonoBehaviour
{
    //makes editable fields in Unity
    [SerializeField] private float detachDelay;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField]  private Rigidbody2D pivot;
    [SerializeField]  private float respawnDelay;

    //variables
    private Camera mainCamera;
    private bool isDragging;
    private Rigidbody2D currentBallRigitBody;
    private SpringJoint2D currentBallSpringJoint;

    // Start is called before the first frame update
    void Start()
    {
        //links the reference to our game camera
        mainCamera = Camera.main;
        //spawn new ball
        SpawnBall();
    }

    // Update is called once per frame
    void Update()
    {
        //if we have no ball, don't render the code
        if(currentBallRigitBody == null)
        {
            return;
        }

        //start reading user input via touch
        if(!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if(isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            return;  
        }

        //whilst input is received, the ball will act kinematically
        isDragging = true;
        currentBallRigitBody.isKinematic = true;
        Vector2 screenTouch = Touchscreen.current.primaryTouch.position.ReadValue();
     
        //converts the touch input screen to world space, so we can move ball to the where we click
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenTouch);
        currentBallRigitBody.position = worldPosition;
    }

    //ball respawn
    private void SpawnBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigitBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        //will attach ball to pivot
        currentBallSpringJoint.connectedBody = pivot;
    }

    //when we launch the ball via release
    private void LaunchBall()
    {
        currentBallRigitBody.isKinematic = false;
        currentBallRigitBody = null;
        //will only call after delay to prevent ball from dropping down
        Invoke(nameof(DetachBall), detachDelay);
    }

    //will disable the pivot keeping the ball attached
    private void DetachBall()
    {    
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnBall), respawnDelay);
    }
}
