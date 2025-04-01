using UnityEngine;

public class CarAudio : MonoBehaviour
{
   public float minSpeed;
   public float maxSpeed;
   private float currentSpeed;
   private Rigidbody carRB;
   private AudioSource carAudio;
   public float minPitch;
   public float maxPitch;
   private float pitchFromCar;

   void Start()
   {
       carRB = GetComponent<Rigidbody>();
       carAudio = GetComponent<AudioSource>();
   }
   void Update(){
     EngineSound();
   }

   void EngineSound(){
        currentSpeed = carRB.linearVelocity.magnitude;
        pitchFromCar = carRB.linearVelocity.magnitude / 50f;
        if(currentSpeed < minSpeed){
            carAudio.pitch = minPitch;

        }
        if(currentSpeed > minSpeed && currentSpeed < maxSpeed){
            carAudio.pitch = minPitch + pitchFromCar;
        }
        if(currentSpeed > maxSpeed){
            carAudio.pitch = maxPitch;
        }
         carAudio.volume = Mathf.Lerp(1f, 1.4f, currentSpeed / maxSpeed);
   }
}
