using UnityEngine;

// This is YOUR script. It wraps the team's systems.
// If teammates change their code, you only fix THIS file.
// Your puzzle scripts never touch teammate code directly.
public class LabRoomManager : MonoBehaviour
{
    public static LabRoomManager Instance;

    [Header("Team Systems — assign in Inspector")]
    public TaskStateProvider taskState;  // drag teammate's component here
    public GameObject finalDoor;         // your exit door object

    // Puzzle completion flags
    private bool p1Solved = false;
    private bool p2Solved = false;
    private bool p3Solved = false;
    private bool p4Solved = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ─── Called by YOUR puzzle scripts ───────────────────────────

    public void SolvePuzzle(int puzzleNumber)
    {
        switch (puzzleNumber)
        {
            case 1: p1Solved = true; RecordState("lab_puzzle1", "solved"); break;
            case 2: p2Solved = true; RecordState("lab_puzzle2", "solved"); break;
            case 3: p3Solved = true; RecordState("lab_puzzle3", "solved"); break;
            case 4: p4Solved = true; RecordState("lab_puzzle4", "solved"); break;
        }

        CheckAllSolved();
    }

    public bool IsSolved(int puzzleNumber)
    {
        switch (puzzleNumber)
        {
            case 1: return p1Solved;
            case 2: return p2Solved;
            case 3: return p3Solved;
            case 4: return p4Solved;
            default: return false;
        }
    }

    // ─── Talks to teammate's system ───────────────────────────────

    // If TaskStateProvider changes, you only fix this one method
    private void RecordState(string key, string value)
    {
        if (taskState != null)
            taskState.SetTaskState(key, value);
        else
            Debug.LogWarning("TaskStateProvider not assigned in LabRoomManager!");
    }

    // ─── Room completion ──────────────────────────────────────────

    private void CheckAllSolved()
    {
        if (p1Solved && p2Solved && p3Solved && p4Solved)
            OpenFinalDoor();
    }

    private void OpenFinalDoor()
    {
        Debug.Log("Lab escaped! Opening final door.");
        if (finalDoor != null)
            finalDoor.SetActive(false);
    }
}
