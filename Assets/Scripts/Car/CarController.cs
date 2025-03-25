using UnityEditor.ShaderGraph;
using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    private int[] wheelsIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f;

    
    private bool isBoosted = false;
    

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    public float carVelocityRatio = 0;

    private void Start()
    {
        if (carRB == null)
            carRB = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate(){
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }

    #region Car Status Check

    private void GroundCheck(){
        int tempGroundedWheels = 0;
        for (int i = 0; i < wheelsIsGrounded.Length; i++){
            tempGroundedWheels += wheelsIsGrounded[i];
        }

        if (tempGroundedWheels > 1){
            isGrounded = true;
        } else {
            isGrounded = false;
        }
    }

    private void CalculateCarVelocity(){
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.linearVelocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    #endregion

    #region Input Handling

    private void GetPlayerInput(){
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion

    #region Movement

    private void Movement(){
        if (isGrounded){
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration(){
        carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration(){
        carRB.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn() {
        carRB.AddTorque(steerStrength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag(){
        float currentSidewaySpeed = currentCarLocalVelocity.x;
        float dragMagnitude = -currentSidewaySpeed * dragCoefficient;
        Vector3 dragForce = transform.right* dragMagnitude;
        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }

    #endregion

    private void Suspension(){
        for (int i = 0; i < rayPoints.Length; i++){
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable)){

                wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;
                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            } else {
                wheelsIsGrounded[i] = 0;
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }



     public void ActivateSpeedBoost(float extraSpeed, float duration)
    {
        if (!isBoosted) // Prevent stacking boosts
        {
            StartCoroutine(SpeedBoostCoroutine(extraSpeed, duration));
        }
    }

      private IEnumerator SpeedBoostCoroutine(float extraSpeed, float duration)
    {
        isBoosted = true;
        acceleration += extraSpeed;
         Debug.Log("Boost activated! New acceleration: " + acceleration);
          Debug.Log("Boost ended. Acceleration reset to: " + acceleration);

        yield return new WaitForSeconds(duration);

        acceleration = 25f;
        isBoosted = false;
    }

    public void ActivateSpeedDecrease(float speedReduction, float duration)
{
    if (!isBoosted) // Prevent stacking speed changes
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

    acceleration = 25f; // Reset to default acceleration
    isBoosted = false;
    Debug.Log("Speed decrease ended. Acceleration reset to: " + acceleration);
}


public void ActivateStarPower(float extraAcceleration, float duration){
    if(!isBoosted){
        StartCoroutine(StarPowerCoroutine(extraAcceleration, duration));
    }
}

private IEnumerator StarPowerCoroutine(float extraAcceleration, float duration){
    isBoosted = true;
    float originalAcceleration = acceleration;
    acceleration += extraAcceleration; 
    Debug.Log("Star Power Activated! Speed increased.");
    Renderer carRenderer = GetComponent<Renderer>();
    if (carRenderer != null)
    {
        carRenderer.material.color = Color.yellow; // Change to star power color
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



}
