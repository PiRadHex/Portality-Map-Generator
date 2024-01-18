using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : PortalTraveller
{
    public bool isPlayer = true;
    public CinemachineVirtualCamera virtualCamera;

    [Header(".:: MOVEMENT ::.")]
    public float walkSpeed = 3;
    public float runSpeed = 5;
    public float jumpHeight = 2f;
    public float gravityMultiplier = 1.7f;
    public float inAirSpeedMultiplier = 0.5f;
    public float smoothMoveTime = 0.1f;

    [Header(".:: STAMINA ::.")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRecoveryRate = 50f;
    public Slider staminaBarSlider;
    public float runnigStaminaConsumptionRate = 20f;
    public float jumpingStaminaConsumption = 20f;

    [Header(".:: SETTING ::.")]
    public bool lockCursor;
    public float mouseSensitivity = 2;

    private Vector3 smoothV;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0.0f;
    private Vector3 velocity = Vector3.zero;

    private bool isSprinting = false;
    private bool isGrounded = false;
    private float lastGroundedTime = 0.0f;
    private Vector3 lastVelocity = Vector3.zero;

    private CharacterController characterController;

    private Vector3 initPos;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        initPos = transform.position;
    }

    public void ResetPos()
    {
        gameObject.SetActive(false);
        transform.position = initPos;
        Physics.SyncTransforms();
        gameObject.SetActive(true);
    }

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (!isPlayer) return;

        HandleMovement();
        HandleStamina();
    }

    private void HandleMovement()
    {
        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -89, 89);

        transform.Rotate(0f, mouseX, 0f);
        virtualCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);


        // Movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir) * (isGrounded ? 1f : inAirSpeedMultiplier);

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting && currentStamina > 0f ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;

        if (characterController.isGrounded)
        {
            velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);
            lastVelocity = velocity * inAirSpeedMultiplier;
        }
        else
        {
            velocity = Vector3.SmoothDamp(velocity, lastVelocity + targetVelocity, ref smoothV, smoothMoveTime);
        }


        // Gravity
        verticalVelocity += gravityMultiplier * Physics.gravity.y * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);


        // Ground check
        var flags = characterController.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            isGrounded = true;
            verticalVelocity = 0;

        }
        else if (flags == CollisionFlags.Above)
        {
            verticalVelocity = -inAirSpeedMultiplier * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            isGrounded = false;
        }
        else
        {
            isGrounded = false;
        }


        // Jump
        if (Input.GetKey(KeyCode.Space) && Time.time - lastGroundedTime > 0.15f)
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                lastGroundedTime = Time.time;
                currentStamina -= jumpingStaminaConsumption;
            }
        }

    }

    private void HandleStamina()
    {
        if (currentStamina <= maxStamina)
        {
            if (isSprinting && characterController.isGrounded)
            {
                currentStamina -= runnigStaminaConsumptionRate * Time.deltaTime;
            }
            else
            {
                /*
                    currentStamina += -(exp(-x) - 1):

                                               /   -staminaRecoveryRate * Time.deltaTime * maxStamina  \
                    currentStamina += 1 - exp |   ----------------------------------------------------  |
                                               \          2 * currentStamina +  0.2 * maxStamina       /
                */
                currentStamina += -(Mathf.Exp(-staminaRecoveryRate * Time.deltaTime * maxStamina / ((2f * currentStamina) + (0.2f * maxStamina))) - 1f);
            }

            if (currentStamina < 0)
            {
                currentStamina = 0f;
            }
            else if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

        }

        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {
        if (staminaBarSlider.value == 1) staminaBarSlider.enabled = false;
        staminaBarSlider.value = currentStamina / maxStamina;
        staminaBarSlider.enabled = true;
    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.eulerAngles = Vector3.up * rot.eulerAngles.y;
        lastVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(lastVelocity));
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector(velocity));
        Physics.SyncTransforms();
    }

}