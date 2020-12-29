using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 acceleration;
    private Vector3 velocity;
    [SerializeField] private float drag = 0.9f;
    [SerializeField] private float maxSpeed = 3;
    [SerializeField] private float cameraSpeed = 1;
    private new Camera camera = null;
    [SerializeField] private float percentageToEdge = 0.1f;
    private bool canMove = false;
    private bool isMovingUp = false;
    private bool isMovingRight = false;
    private bool isMovingDown = false;
    private bool isMovingLeft = false;
    private bool isTurning = false;
    private Vector2 pressPosition;
    [SerializeField] private float maxRotationSpeed = 1;


    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        acceleration = Vector3.zero;

        Vector2 mousePos = Input.mousePosition;
        Vector2 screenPos = camera.ScreenToViewportPoint(mousePos);

        if (InputManager.IsPressing(1))
        {
            if (Input.GetMouseButtonDown(1))
            {
                isTurning = true;
                pressPosition = screenPos;
            }
            else if(Input.GetMouseButtonUp(1))
            {
                isTurning = false;
            }

            if (isTurning)
            {
                float delta = screenPos.x - pressPosition.x;
                this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, maxRotationSpeed * delta* Time.deltaTime, 0);
            }
        }
        else
        {
            if (InputManager.GetButtonDown(InputDirection.Down) || 
                ((canMove || isMovingDown || isMovingRight || isMovingLeft) && (screenPos.y >= 0 - percentageToEdge && screenPos.y < percentageToEdge)))
            {
                isMovingDown = true;
                acceleration -= this.gameObject.transform.forward;
            }
            else
            {
                isMovingDown = false;
            }
            if (InputManager.GetButtonDown(InputDirection.Up) || 
                ((canMove || isMovingUp || isMovingRight || isMovingLeft) && screenPos.y <= 1 + percentageToEdge && screenPos.y > 1 - percentageToEdge))
            {
                isMovingUp = true;
                acceleration += this.gameObject.transform.forward;
            }
            else
            {
                isMovingUp = false;
            }

            if (InputManager.GetButtonDown(InputDirection.Left) || ((canMove || isMovingLeft || isMovingDown || isMovingUp) && screenPos.x >= 0 - percentageToEdge && screenPos.x < percentageToEdge))
            {
                isMovingLeft = true;
                acceleration -= this.gameObject.transform.right;
            }
            else
            {
                isMovingLeft = false;
            }
            if (InputManager.GetButtonDown(InputDirection.Right) || ((canMove || isMovingRight || isMovingDown || isMovingUp) && screenPos.x <= 1 + percentageToEdge && screenPos.x > 1 - percentageToEdge))
            {
                isMovingRight = true;
                acceleration += this.gameObject.transform.right;
            }
            else
            {
                isMovingRight = false;
            }

            velocity = (velocity + acceleration) * drag;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            this.transform.position += velocity * Time.deltaTime * cameraSpeed;
        }

        // Check if we can move the camera with the mouse
        if (screenPos.y > percentageToEdge && screenPos.x > percentageToEdge && screenPos.y < 1 - percentageToEdge && screenPos.x < 1 - percentageToEdge)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
    }
}
