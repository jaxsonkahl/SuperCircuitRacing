using System.Collections.Generic;
using UnityEngine;

public class GhostReplay : MonoBehaviour
{
    public List<Vector3> positions;
    public List<Quaternion> rotations;

    private int frameIndex = 0;
    private bool isReplaying = false;
    private float replayInterval = 0.02f; // 50 FPS playback

    public void StartReplay(List<Vector3> recordedPositions, List<Quaternion> recordedRotations)
    {
        positions = recordedPositions;
        rotations = recordedRotations;
        frameIndex = 0;
        isReplaying = true;
    }

    void Update()
    {
        if (isReplaying && frameIndex < positions.Count)
        {
            // Move ghost to the recorded position and rotation
            transform.position = positions[frameIndex];
            transform.rotation = rotations[frameIndex];

            frameIndex++;
        }
    }

    public void StopReplay()
    {
        isReplaying = false;
    }
}