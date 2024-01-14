using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private int groundLectureNumber = 4;
    [SerializeField]
    private int lateralLectureNumber = 4;
    [SerializeField]
    private float lectureDistance = 0.1f;
    [SerializeField]
    private float lectureHolgure = 0.01f;
    [SerializeField]
    private LayerMask layersToIgnore = new LayerMask();
    [SerializeField]
    private float jumpHeigth = 0.0f;
    [SerializeField]
    private float timeToJumpApex = 0.0f;
    [SerializeField]
    private float CoyoteTimePeriod = 0.1f;
    [SerializeField]
    private float jumpInputBuffer = 0.1f;
    [SerializeField]
    private float appexPoint = 0.1f;
    [SerializeField]
    private int numberOfAirJumps = 1;
    [SerializeField]
    private float maxFallSpeed = 20.0f;
    private float gravityForce = 1.6f;
    private float jumpForce = 1.6f;
    private CapsuleCollider2D playerCollider;
    private Vector2 desiredMovement = new Vector2();
    private Bounds colliderBounds;
    private float finalGravityForce = 0.0f;
    private float lastTimeGrounded = 0.0f;
    private float lastTimeJumpPressed = float.MaxValue;
    private bool jumpBufferActive = false;
    private bool jumpButtonPressed = false;
    private int currentAirJumps;
    private bool CanJump
    {
        get
        {
            return (Time.time - lastTimeGrounded < CoyoteTimePeriod);
        }
    }


    private void Start()
    {
        SetCollisionDetectionPoints();
        gravityForce = -(2 * jumpHeigth) / Mathf.Pow(timeToJumpApex, 2);
        jumpForce = Mathf.Abs(gravityForce) * timeToJumpApex;
        finalGravityForce = gravityForce;
        currentAirJumps = numberOfAirJumps;
    }
    private void SetCollisionDetectionPoints()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        if (!playerCollider)
        {
            return;
        }
        colliderBounds = playerCollider.bounds;
    }
    private void OnEnable()
    {
        playerInput.OnMoveEvent += ReadPlayerMovement;
        playerInput.OnJumpEvent += Jump;
        playerInput.OnJumpStopEvent += JumpStop;
    }
    private void OnDisable()
    {
        playerInput.OnMoveEvent -= ReadPlayerMovement;
        playerInput.OnJumpEvent -= Jump;
        playerInput.OnJumpStopEvent -= JumpStop;
    
    }
    private void ReadPlayerMovement(Vector2 value)
    {
        desiredMovement.x = value.x * speed;
    }
    private void Update()
    {
        if (GameManager.Instance.IsStoped)
        {
            return;
        }
       
        CheckJumpBuffer();
        Move();
    }
    private float CalculateVerticalVelocity()
    {
        finalGravityForce = gravityForce;
        if (Mathf.Abs(desiredMovement.y) < appexPoint && jumpButtonPressed)
        {
            finalGravityForce = gravityForce / 10;
        }
        if (desiredMovement.y < 0.0f)
        {
            finalGravityForce = gravityForce * 2;
        }
        return desiredMovement.y + finalGravityForce * Time.deltaTime;

    }
    private void Jump()
    {
        jumpButtonPressed = true;
        JumpAction();
    }
    private void JumpAction()
    {
        
        if (CanJump || currentAirJumps > 0)
        {
            if (!CanJump)
            {
                currentAirJumps--;
            }
            desiredMovement.y = jumpForce;
            jumpBufferActive = false;
            return;
        }
        lastTimeJumpPressed = Time.time;
    }
    private void JumpStop()
    {
        jumpButtonPressed = false;
        if (desiredMovement.y > 0.0f)
        {
            desiredMovement.y = desiredMovement.y / 2;
        }
    }

    private void ShouldJumpAfterFall()
    {
        if (Mathf.Abs(Time.time - lastTimeJumpPressed) < jumpInputBuffer && jumpButtonPressed)
        {
            jumpBufferActive = true;
        }
    }
    private void CheckJumpBuffer()
    {
        if (jumpBufferActive)
        {
            JumpAction();
        }
    }
    private float MaxHorizontalSpeed(float currentMovement)
    {
        colliderBounds = playerCollider.bounds;
        Vector2 direction = (Vector2.right * currentMovement).normalized;
        Vector2 origin = Vector2.up * colliderBounds.min + Vector2.up * 0.025f;
        float width = colliderBounds.max.y - colliderBounds.min.y;
        origin += direction == Vector2.right ? Vector2.right * colliderBounds.max : Vector2.right * colliderBounds.min;

        return MaxMovementInDirection(origin, Vector2.up, direction, width, lateralLectureNumber, currentMovement);
    }
    private float MaxMovementInDirection(Vector2 origin, Vector2 lectureDirection, Vector2 direction, float width, int numLectures, float currentSpeed)
    {
        RaycastHit2D ray;
        float auxCurrentSpeed  = Mathf.Abs(currentSpeed);
        float spaceBetweenLectures = width / (float)numLectures;
        for (int i = 0; i <= numLectures; i++)
        {
            ray = Physics2D.Raycast(origin + lectureDirection * spaceBetweenLectures * i, direction, auxCurrentSpeed, ~layersToIgnore);
            Debug.DrawLine(origin + lectureDirection * spaceBetweenLectures * i, origin + lectureDirection * spaceBetweenLectures * i +direction *( auxCurrentSpeed));
            if (ray.collider != null)
            {
                auxCurrentSpeed = ray.distance ;
            }
        }
        return auxCurrentSpeed * Mathf.Sign(currentSpeed);
    }
    private float MaxVerticalSpeed(float currentSpeed)
    {

        RaycastHit2D ray;
        float auxCurrentSpeed = Mathf.Abs(currentSpeed);
        colliderBounds = playerCollider.bounds;
        float width = (colliderBounds.max.x - colliderBounds.min.x) - lectureHolgure;
        float spaceBetweenLectures = width / (float)groundLectureNumber;
        if (currentSpeed > 0.0f)
        {
            return MaxMovementInDirection(new Vector2 (colliderBounds.min.ToVector2().x, colliderBounds.max.ToVector2().y) + Vector2.right * lectureHolgure / 2, Vector2.right, Vector2.up, width, lateralLectureNumber, currentSpeed);
        }
        return MaxMovementInDirection(colliderBounds.min.ToVector2() + Vector2.right * lectureHolgure / 2, Vector2.right, Vector2.down, width, lateralLectureNumber, currentSpeed);
    }
    private void Move()
    {
        desiredMovement.y = Mathf.Max(CalculateVerticalVelocity(), -maxFallSpeed) ;
        ConstraintMovement(desiredMovement * Time.deltaTime).ToVector3();
    }
    private Vector2 ConstraintMovement(Vector2 currentMovement)
    {

        currentMovement.x = MaxHorizontalSpeed(currentMovement.x);
        transform.position += Vector3.right * currentMovement.x;
        currentMovement.y = MaxVerticalSpeed(currentMovement.y);
        transform.position += Vector3.up * currentMovement.y;
        CheckIfGrounded(currentMovement);
        return currentMovement;
    }
    private void CheckIfGrounded(Vector2 currentMovement)
    {
        if (currentMovement.y == 0.0f)
        {
            if (desiredMovement.y < 0.0f)
            {
                currentAirJumps = numberOfAirJumps;
                ShouldJumpAfterFall();
            }
            lastTimeGrounded = Time.time;
            desiredMovement.y = 0.0f;
        }
    }
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * 2, desiredMovement.y.ToString()) ;
        if (!playerCollider)
        {
            return;
        }
        Bounds colliderBounds = playerCollider.bounds;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(colliderBounds.max, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(colliderBounds.min, 0.1f);
        float width = colliderBounds.max.x - colliderBounds.min.x;
        Gizmos.color = Color.green;
    }
}
