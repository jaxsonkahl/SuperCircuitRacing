
using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{
    [HideInInspector] public bool canMove = false;
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] private TrailRenderer[] skidMarks = new TrailRenderer[2];
    [SerializeField] private ParticleSystem[] skidSmokes = new ParticleSystem[2];

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;
    private int[] wheelsIsGrounded = new int[4];
    private bool isGrounded = false;
    private string currentSurfaceTag = "Default";

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 2.5f;

    [Header("Visuals")]


    private bool isBoosted = false;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    public float carVelocityRatio = 0;

    private void Start()
    {
        if (carRB == null)
            carRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        Vfx();

        // Step 1: Apply angular damping
        float angularDamping = Mathf.Lerp(0.95f, 0.7f, Mathf.Abs(carVelocityRatio));
        if (isGrounded)
        {
            carRB.angularVelocity = Vector3.Lerp(carRB.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);
        } 

        if (!isGrounded)
        {
            Vector3 extraGravity = Physics.gravity * 2f;
            carRB.AddForce(extraGravity, ForceMode.Acceleration);
        }
        // Print current speed to the Unity console
        PrintCurrentSpeed();
    }

    // Method to print the current speed
    private void PrintCurrentSpeed()
    {
        float currentSpeed = carRB.linearVelocity.magnitude;
        Debug.Log("Current Speed: " + currentSpeed + " m/s");
    }

    #region Car Status Check

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;
        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsIsGrounded[i];
        }

        isGrounded = tempGroundedWheels > 1;
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.linearVelocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    #endregion

    #region Input Handling

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion

    #region Movement

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();

            // Step 3: Kill sideways velocity at low speeds
            if (carRB.linearVelocity.magnitude < 5f)
            {
                Vector3 localVel = transform.InverseTransformDirection(carRB.linearVelocity);
                localVel.x = Mathf.Lerp(localVel.x, 0, Time.fixedDeltaTime * 5f);
                carRB.linearVelocity = transform.TransformDirection(localVel);
            }

            if (currentSurfaceTag == "Grass")
            {
                Vector3 horizontalVelocity = new Vector3(carRB.linearVelocity.x, 0f, carRB.linearVelocity.z);

                // Reduce speed by 0–10% based on how fast the car is moving
                float slowFactor = Mathf.Lerp(1f, 0.97f, Mathf.Clamp01(carRB.linearVelocity.magnitude / maxSpeed));
                Vector3 reducedVelocity = horizontalVelocity * slowFactor;

                carRB.linearVelocity = new Vector3(reducedVelocity.x, carRB.linearVelocity.y, reducedVelocity.z);
            }
        }
    }

    private void Acceleration()
    {
        if (moveInput <= 0) return;

        float adjustedAccel = acceleration;

        if (currentSurfaceTag == "Grass")
        {
            float minGrassAccel = 5f;
            adjustedAccel = Mathf.Max(acceleration * 0.3f, minGrassAccel);
        }

        float speedFactor = Mathf.Clamp01(1f - (carRB.linearVelocity.magnitude / maxSpeed));
        float finalAccel = adjustedAccel * speedFactor;

        carRB.AddForceAtPosition(finalAccel * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        if (moveInput >= 0) return; // Only decelerate when pressing backward

        float adjustedDecel = deceleration;

        if (currentSurfaceTag == "Grass")
        {
            adjustedDecel *= 0.4f;
        }

        carRB.AddForceAtPosition(adjustedDecel * Mathf.Abs(moveInput) * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        if (!isGrounded) return;

        float steerCurveValue = turningCurve.Evaluate(Mathf.Abs(carVelocityRatio));
        float lowSpeedFactor = Mathf.Clamp01(1f - Mathf.Abs(carVelocityRatio));
        float dynamicSteerStrength = steerStrength + (lowSpeedFactor * 20f); // tighter turn at low speed

        // Apply rotation torque for turning
        carRB.AddTorque(transform.up * steerInput * dynamicSteerStrength * steerCurveValue, ForceMode.Acceleration);

        // --- Grounded Steering Assist ---
        if (carRB.linearVelocity.magnitude > 5f)
        {
            Vector3 flatVelocity = carRB.linearVelocity;
            flatVelocity.y = 0;
            flatVelocity.Normalize();

            Vector3 flatForward = transform.forward;
            flatForward.y = 0;
            flatForward.Normalize();

            float angleDifference = Vector3.SignedAngle(flatForward, flatVelocity, Vector3.up);
            float assistTorque = Mathf.Clamp(angleDifference, -45f, 45f) * 0.15f;

            carRB.AddTorque(transform.up * -assistTorque, ForceMode.Acceleration);
        }
    }


    // Step 2: Improved sideways drag
    private void SidewaysDrag()
    {
        float sideVelocity = currentCarLocalVelocity.x;
        float desiredVelocityChange = -sideVelocity * dragCoefficient;
        Vector3 dragForce = transform.right * desiredVelocityChange;

        float speedFactor = Mathf.Clamp01(currentCarLocalVelocity.magnitude / maxSpeed);
        dragForce *= Mathf.Lerp(0.5f, 2f, speedFactor);

        carRB.AddForce(dragForce * 10f, ForceMode.Acceleration);
    }

    #endregion

    private void Suspension()
    {
        currentSurfaceTag = "Default";
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                Debug.Log("Ray hit: " + hit.collider.name + " with tag: " + hit.collider.tag);
                if (hit.collider.CompareTag("Grass"))
                {
                    currentSurfaceTag = "Grass";
                }
                else {
                    currentSurfaceTag = "Default";
                }
                wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;
                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelsIsGrounded[i] = 0;
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }

    #region Boosts

    public void ActivateSpeedBoost(float extraSpeed, float duration)
    {
        if (!isBoosted)
        {
            StartCoroutine(SpeedBoostCoroutine(extraSpeed, duration));
        }
    }

    private IEnumerator SpeedBoostCoroutine(float extraSpeed, float duration)
    {
        isBoosted = true;
        acceleration += extraSpeed;
        Debug.Log("Boost activated! New acceleration: " + acceleration);

        yield return new WaitForSeconds(duration);

        acceleration = 25f;
        isBoosted = false;
        Debug.Log("Boost ended. Acceleration reset to: " + acceleration);
    }

    public void ActivateSpeedDecrease(float speedReduction, float duration)
    {
        if (!isBoosted)
        {
            StartCoroutine(DecreaseSpeedCoroutine(speedReduction, duration));
        }
    }

    private IEnumerator DecreaseSpeedCoroutine(float speedReduction, float duration)
    {
        isBoosted = true;
        acceleration -= speedReduction;
        Debug.Log("Speed decrease activated! New acceleration: " + acceleration);

        yield return new WaitForSeconds(duration);

        acceleration = 25f;
        isBoosted = false;
        Debug.Log("Speed decrease ended. Acceleration reset to: " + acceleration);
    }

    public void ActivateStarPower(float extraAcceleration, float duration)
    {
        if (!isBoosted)
        {
            StartCoroutine(StarPowerCoroutine(extraAcceleration, duration));
        }
    }

    private IEnumerator StarPowerCoroutine(float extraAcceleration, float duration)
    {
        isBoosted = true;
        float originalAcceleration = acceleration;
        acceleration += extraAcceleration;
        Debug.Log("Star Power Activated! Speed increased.");

        Renderer carRenderer = GetComponent<Renderer>();
        if (carRenderer != null)
        {
            carRenderer.material.color = Color.yellow;
        }

        yield return new WaitForSeconds(duration);

        acceleration = originalAcceleration;
        isBoosted = false;

        if (carRenderer != null)
        {
            carRenderer.material.color = Color.white;
        }

        Debug.Log("Star Power ended.");
    }

    #endregion

    #region Visuals
    private void Vfx() {
        float speed = carRB.linearVelocity.magnitude;
        float turnAmount = Mathf.Abs(steerInput);
        float sideVel = Mathf.Abs(currentCarLocalVelocity.x); // side slip

        // Customizable thresholds
        float minSpeed = 10f;
        float minTurn = .7f;     // how sharp the turn must be
        float minSideSkid = 0.5f; // how much side slide to count as drifting

        // Only show effects if all conditions are met
        bool shouldSkid = isGrounded && speed > minSpeed && turnAmount > minTurn && sideVel > minSideSkid;

        Debug.Log($"Skid Check → Speed: {speed}, Turn: {turnAmount}, SideVel: {sideVel}, Skid? {shouldSkid}");

        ToggleSkidMarks(shouldSkid);
        ToggleSkidSmokes(shouldSkid);
    }


    private void ToggleSkidMarks(bool toggle){
        foreach (var skidMark in skidMarks){
            skidMark.emitting = toggle;
            skidMark.enabled = toggle;
        }
    }
    private void ToggleSkidSmokes(bool toggle){
    foreach (var smoke in skidSmokes){
        Debug.Log($"Toggling smoke: {smoke.name}, toggle = {toggle}");
        if (toggle){
                smoke.Play();
            } else {
                smoke.Stop();
            }
        }
    }
    #endregion
}

