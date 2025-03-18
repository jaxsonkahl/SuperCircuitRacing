using UnityEngine;

public class RotatePower : MonoBehaviour
{
    public float speed = 50f;
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
