using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 acceleration;
    private Vector3 velocity;
    [SerializeField] private float drag = 0.9f;
    [SerializeField] private float maxSpeed = 3;
    [SerializeField] private float dragSpeed = 3;
    [SerializeField] private float cameraSpeed = 1;
    private new Camera camera = null;
    [SerializeField] private float percentageToEdge = 0.1f;
    private bool canMove = false;
    private bool isMovingUp = false;
    private bool isMovingRight = false;
    private bool isMovingDown = false;
    private bool isMovingLeft = false;
    private bool isTurning = false;
    private bool isMovingWithMouseDown = false;
    private Vector3 pressPosition;
    [SerializeField] private float maxRotationSpeed = 1;

    [Space]
    [SerializeField] private bool useDragMovement = true;
    [SerializeField] private bool useKeyMovement = true;
    [SerializeField] private bool useEdgeMovement = true;
    private Vector3 worldPos;

    [SerializeField] private float timeTillDeactivatingMouse = 0.5f;
    private float timer = 0;
    private bool isTiming = false;

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (isTiming)
        {
            timer -= Time.deltaTime ;
            if(timer <= 0)
            {
                InputManager.Instance.IsMouseFree = false;
                isTiming = false;
            }
        }
        acceleration = Vector3.zero;

        Vector2 mousePos = Input.mousePosition;
        Vector2 screenPos = camera.ScreenToViewportPoint(mousePos);

        if (InputManager.Instance.IsPressing(1) && !isMovingWithMouseDown)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isTurning = true;
                pressPosition = screenPos;
            }

            if (isTurning)
            {
                float delta = screenPos.x - pressPosition.x;
                this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, maxRotationSpeed * delta* Time.deltaTime, 0);
            }
        }
        else if (InputManager.HasReleased(1))
        {
            isTurning = false;
        }
        else if (useDragMovement && InputManager.Instance.IsPressing(0) && !isTurning )
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMovingWithMouseDown = true;
                timer = timeTillDeactivatingMouse;
                isTiming = true;
                worldPos = camera.ScreenToViewportPoint(mousePos);
                pressPosition = worldPos;
            }

            if (isMovingWithMouseDown)
            {
                worldPos = camera.ScreenToViewportPoint(mousePos);
                float deltaX = worldPos.x - pressPosition.x;
                float deltaZ = worldPos.y - pressPosition.y;

                this.transform.position += this.transform.rotation  * new Vector3(-dragSpeed * deltaX, 0, -dragSpeed * deltaZ) * Time.deltaTime;
                pressPosition = worldPos;
            }
        }
        else if (useDragMovement && InputManager.HasReleased(0))
        {
            isTiming = false;
            InputManager.Instance.IsMouseFree = true;
            isMovingWithMouseDown = false;
        }
        else
        {
            if (InputManager.Instance.GetButtonDown(InputDirection.Down) && useKeyMovement || 
                (useEdgeMovement&&(canMove || isMovingDown || isMovingRight || isMovingLeft) && (screenPos.y >= 0 - percentageToEdge && screenPos.y < percentageToEdge)))
            {
                isMovingDown = true;
                acceleration -= this.gameObject.transform.forward;
            }
            else
            {
                isMovingDown = false;
            }
            if (InputManager.Instance.GetButtonDown(InputDirection.Up) && useKeyMovement || 
                (useEdgeMovement && (canMove || isMovingUp || isMovingRight || isMovingLeft) && screenPos.y <= 1 + percentageToEdge && screenPos.y > 1 - percentageToEdge))
            {
                isMovingUp = true;
                acceleration += this.gameObject.transform.forward;
            }
            else
            {
                isMovingUp = false;
            }

            if (InputManager.Instance.GetButtonDown(InputDirection.Left) && useKeyMovement || (useEdgeMovement && (canMove || isMovingLeft || isMovingDown || isMovingUp) && screenPos.x >= 0 - percentageToEdge && screenPos.x < percentageToEdge))
            {
                isMovingLeft = true;
                acceleration -= this.gameObject.transform.right;
            }
            else
            {
                isMovingLeft = false;
            }
            if (InputManager.Instance.GetButtonDown(InputDirection.Right) && useKeyMovement || (useEdgeMovement && (canMove || isMovingRight || isMovingDown || isMovingUp) && screenPos.x <= 1 + percentageToEdge && screenPos.x > 1 - percentageToEdge))
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
