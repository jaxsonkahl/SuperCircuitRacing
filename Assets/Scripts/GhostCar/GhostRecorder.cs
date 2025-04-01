using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public List<Vector3> recordedPositions = new List<Vector3>();
    public List<Quaternion> recordedRotations = new List<Quaternion>();

    private bool isRecording = false;
    private float recordInterval = 0.02f; // 50 FPS recording
    private float nextRecordTime = 0f;

    void Start()
    {
        StartRecording(); // Start recording when the race starts
    }

    void Update()
    {
        if (isRecording && Time.time >= nextRecordTime)
        {
            // Record the player's position and rotation at regular intervals
            recordedPositions.Add(transform.position);
            recordedRotations.Add(transform.rotation);
            nextRecordTime = Time.time + recordInterval;
        }
    }

    // Start recording the player's lap
    public void StartRecording()
    {
        recordedPositions.Clear();
        recordedRotations.Clear();
        isRecording = true;
        nextRecordTime = Time.time;
    }

    // Stop recording after the lap is complete
    public void StopRecording()
    {
        isRecording = false;
    }
}