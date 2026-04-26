using System.Collections.Generic;
using UnityEngine;

public class TaskStateProvider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     [System.Serializable]
    public class TaskEntry
    {
        public string key;
        public string value;
    }

    [SerializeField] private List<TaskEntry> taskEntries = new List<TaskEntry>();

    public string GetTaskState(string taskKey)
    {
        foreach (TaskEntry entry in taskEntries)
        {
            if (entry.key == taskKey)
            {
                return entry.value;
            }
        }

        return string.Empty;
    }

    public void SetTaskState(string taskKey, string newValue)
    {
        foreach (TaskEntry entry in taskEntries)
        {
            if (entry.key == taskKey)
            {
                entry.value = newValue;
                return;
            }
        }

        taskEntries.Add(new TaskEntry { key = taskKey, value = newValue });
    }
}
