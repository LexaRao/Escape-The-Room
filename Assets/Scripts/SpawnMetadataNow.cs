using UnityEngine;

public class SpawnMetadataNow : MonoBehaviour
{
    public string spawnedByScript;
    public int spawnOrder;
    public float spawnTimestamp;

    public void Initialize(string scriptName, int order)
    {
        spawnedByScript = scriptName;
        spawnOrder = order;
        spawnTimestamp = Time.time;
    }
}