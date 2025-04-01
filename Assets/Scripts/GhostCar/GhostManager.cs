using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public GameObject ghostPrefab; // Ghost car prefab
    private GameObject currentGhost;
    private GhostReplay ghostReplay;
    private GhostRecorder ghostRecorder;

    void Start()
    {
        // Find and assign the GhostRecorder on the player
        ghostRecorder = GameObject.FindGameObjectWithTag("Player").GetComponent<GhostRecorder>();
    }

    // Call this when the player completes the first lap
    public void SpawnGhost()
    {
        if (ghostRecorder.recordedPositions.Count > 0)
        {
            // Instantiate ghost at start line
            currentGhost = Instantiate(ghostPrefab, ghostRecorder.recordedPositions[0], ghostRecorder.recordedRotations[0]);
            ghostReplay = currentGhost.GetComponent<GhostReplay>();

            // Start ghost replay using the recorded positions/rotations
            ghostReplay.StartReplay(ghostRecorder.recordedPositions, ghostRecorder.recordedRotations);
        }
    }

    // Destroy ghost before restarting a lap
    public void DestroyGhost()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
        }
    }
}