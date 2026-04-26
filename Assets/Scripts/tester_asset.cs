using System;
using System.Collections.Generic;
using UnityEngine;

public class tester_asset : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class ProjectIntegrityTester : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private string importMode = "full";

    [Header("Options")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private bool logDetailedReport = true;

    private IntegrityTestConfig loadedConfig;
    private IntegrityTestReport lastReport;

    private void Start()
    {
        if (runOnStart)
        {
            RunIntegrityTest(jsonFile, importMode);
        }
    }

    public IntegrityTestReport RunIntegrityTest(TextAsset file, string mode)
    {
        if (file == null)
        {
            Debug.LogError("[ProjectIntegrityTester] No JSON file assigned.");
            return null;
        }

        try
        {
            loadedConfig = JsonUtility.FromJson<IntegrityTestConfig>(file.text);

            if (loadedConfig == null)
            {
                Debug.LogError("[ProjectIntegrityTester] Failed to parse JSON.");
                return null;
            }

            lastReport = new IntegrityTestReport
            {
                projectName = loadedConfig.projectName,
                importMode = mode,
                testTime = DateTime.UtcNow.ToString("o"),
                results = new List<ObjectTestResult>(),
                summaryMessages = new List<string>()
            };

            ValidateObjects(mode);
            ValidateTaskLinks(mode);
            ValidateUIBindings(mode);

            BuildSummary();

            if (logDetailedReport)
            {
                PrintReport(lastReport);
            }

            return lastReport;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ProjectIntegrityTester] Exception while testing project: {ex.Message}");
            return null;
        }
    }

    private void ValidateObjects(string mode)
    {
        if (loadedConfig.objects == null)
        {
            lastReport.summaryMessages.Add("No objects defined in JSON.");
            return;
        }

        foreach (TrackedObjectDefinition expected in loadedConfig.objects)
        {
            ObjectTestResult result = new ObjectTestResult
            {
                objectId = expected.objectId,
                objectName = expected.objectName,
                checks = new List<string>(),
                passed = true
            };

            GameObject sceneObject = GameObject.Find(expected.objectName);

            if (sceneObject == null)
            {
                result.passed = false;
                result.checks.Add($"FAIL: Object '{expected.objectName}' not found in scene.");
                lastReport.results.Add(result);
                continue;
            }

            result.checks.Add($"PASS: Object '{expected.objectName}' found.");

            Vector3 actualPosition = sceneObject.transform.position;
            Vector3 expectedPosition = new Vector3(
                expected.position.x,
                expected.position.y,
                expected.position.z
            );

            if (Vector3.Distance(actualPosition, expectedPosition) > expected.positionTolerance)
            {
                result.passed = false;
                result.checks.Add(
                    $"FAIL: Position mismatch. Expected {expectedPosition}, got {actualPosition}."
                );
            }
            else
            {
                result.checks.Add("PASS: Position matches expected tolerance.");
            }

            bool actualActive = sceneObject.activeInHierarchy;
            if (actualActive != expected.isActive)
            {
                result.passed = false;
                result.checks.Add(
                    $"FAIL: Active state mismatch. Expected {expected.isActive}, got {actualActive}."
                );
            }
            else
            {
                result.checks.Add("PASS: Active state matches.");
            }

            SpawnMetadata spawnMetadata = sceneObject.GetComponent<SpawnMetadata>();
            if (spawnMetadata != null)
            {
                if (!string.IsNullOrEmpty(expected.spawnedByScript) &&
                    spawnMetadata.spawnedByScript != expected.spawnedByScript)
                {
                    result.passed = false;
                    result.checks.Add(
                        $"FAIL: Spawn script mismatch. Expected '{expected.spawnedByScript}', got '{spawnMetadata.spawnedByScript}'."
                    );
                }
                else
                {
                    result.checks.Add("PASS: Spawn script matches.");
                }

                if (expected.spawnOrder >= 0 && spawnMetadata.spawnOrder != expected.spawnOrder)
                {
                    result.passed = false;
                    result.checks.Add(
                        $"FAIL: Spawn order mismatch. Expected {expected.spawnOrder}, got {spawnMetadata.spawnOrder}."
                    );
                }
                else if (expected.spawnOrder >= 0)
                {
                    result.checks.Add("PASS: Spawn order matches.");
                }

                if (mode == "full" && expected.spawnTimestamp > 0f)
                {
                    float delta = Mathf.Abs(spawnMetadata.spawnTimestamp - expected.spawnTimestamp);
                    if (delta > expected.spawnTimeTolerance)
                    {
                        result.passed = false;
                        result.checks.Add(
                            $"FAIL: Spawn timestamp mismatch. Expected {expected.spawnTimestamp}, got {spawnMetadata.spawnTimestamp}."
                        );
                    }
                    else
                    {
                        result.checks.Add("PASS: Spawn timestamp matches tolerance.");
                    }
                }
            }
            else
            {
                result.checks.Add("WARN: No SpawnMetadata component found.");
            }

            TaskStateProvider taskStateProvider = sceneObject.GetComponent<TaskStateProvider>();
            if (taskStateProvider != null && expected.requiredTaskStates != null)
            {
                foreach (TaskStateRequirement requirement in expected.requiredTaskStates)
                {
                    string actualValue = taskStateProvider.GetTaskState(requirement.taskKey);

                    if (actualValue != requirement.expectedValue)
                    {
                        result.passed = false;
                        result.checks.Add(
                            $"FAIL: Task state mismatch for '{requirement.taskKey}'. Expected '{requirement.expectedValue}', got '{actualValue}'."
                        );
                    }
                    else
                    {
                        result.checks.Add(
                            $"PASS: Task state '{requirement.taskKey}' matches expected value."
                        );
                    }
                }
            }

            lastReport.results.Add(result);
        }
    }

    private void ValidateTaskLinks(string mode)
    {
        if (loadedConfig.taskLinks == null || loadedConfig.taskLinks.Count == 0)
        {
            lastReport.summaryMessages.Add("No task links defined in JSON.");
            return;
        }

        foreach (TaskLinkDefinition link in loadedConfig.taskLinks)
        {
            GameObject source = GameObject.Find(link.sourceObjectName);
            GameObject target = GameObject.Find(link.targetObjectName);

            if (source == null || target == null)
            {
                lastReport.summaryMessages.Add(
                    $"TaskLink FAIL: Could not validate link from '{link.sourceObjectName}' to '{link.targetObjectName}'."
                );
                continue;
            }

            TaskStateProvider sourceProvider = source.GetComponent<TaskStateProvider>();
            TaskStateProvider targetProvider = target.GetComponent<TaskStateProvider>();

            if (sourceProvider == null || targetProvider == null)
            {
                lastReport.summaryMessages.Add(
                    $"TaskLink WARN: Missing TaskStateProvider on source or target for link '{link.linkName}'."
                );
                continue;
            }

            string sourceState = sourceProvider.GetTaskState(link.sourceTaskKey);
            string targetState = targetProvider.GetTaskState(link.targetTaskKey);

            bool valid = sourceState == link.requiredSourceValue &&
                         targetState == link.requiredTargetValue;

            lastReport.summaryMessages.Add(
                valid
                    ? $"TaskLink PASS: '{link.linkName}' is valid."
                    : $"TaskLink FAIL: '{link.linkName}' is invalid. Source='{sourceState}', Target='{targetState}'."
            );
        }
    }

    private void ValidateUIBindings(string mode)
    {
        if (loadedConfig.uiChecks == null || loadedConfig.uiChecks.Count == 0)
        {
            lastReport.summaryMessages.Add("No UI bindings defined in JSON.");
            return;
        }

        foreach (UICheckDefinition uiCheck in loadedConfig.uiChecks)
        {
            GameObject uiObject = GameObject.Find(uiCheck.objectName);
            if (uiObject == null)
            {
                lastReport.summaryMessages.Add($"UI FAIL: '{uiCheck.objectName}' not found.");
                continue;
            }

            if (!uiObject.TryGetComponent<TextStateProvider>(out var textProvider))
            {
                lastReport.summaryMessages.Add($"UI WARN: '{uiCheck.objectName}' has no TextStateProvider.");
                continue;
            }

            string currentText = textProvider.GetTextValue();

            if (!string.Equals(currentText, uiCheck.expectedText, StringComparison.Ordinal))
            {
                lastReport.summaryMessages.Add(
                    $"UI FAIL: '{uiCheck.objectName}' text mismatch. Expected '{uiCheck.expectedText}', got '{currentText}'."
                );
            }
            else
            {
                lastReport.summaryMessages.Add($"UI PASS: '{uiCheck.objectName}' text matches.");
            }
        }
    }

    private void BuildSummary()
    {
        int passCount = 0;
        int failCount = 0;

        foreach (ObjectTestResult result in lastReport.results)
        {
            if (result.passed) passCount++;
            else failCount++;
        }

        lastReport.totalObjects = lastReport.results.Count;
        lastReport.passedObjects = passCount;
        lastReport.failedObjects = failCount;
        lastReport.overallPassed = failCount == 0;
    }

    private void PrintReport(IntegrityTestReport report)
    {
        Debug.Log("========== PROJECT INTEGRITY TEST REPORT ==========");
        Debug.Log($"Project: {report.projectName}");
        Debug.Log($"Mode: {report.importMode}");
        Debug.Log($"Time: {report.testTime}");
        Debug.Log($"Overall Passed: {report.overallPassed}");
        Debug.Log($"Objects Tested: {report.totalObjects}");
        Debug.Log($"Passed: {report.passedObjects}");
        Debug.Log($"Failed: {report.failedObjects}");

        foreach (ObjectTestResult result in report.results)
        {
            Debug.Log($"--- Object: {result.objectName} ({result.objectId}) | Passed: {result.passed}");
            foreach (string check in result.checks)
            {
                Debug.Log(check);
            }
        }

        foreach (string summary in report.summaryMessages)
        {
            Debug.Log(summary);
        }

        Debug.Log("==================================================");
    }

    private class TaskStateProvider
    {
        internal string GetTaskState(string taskKey)
        {
            throw new NotImplementedException();
        }
    }
}

internal class TextStateProvider
{
    // Stub for the meta data related to the project.
    internal string GetTextValue()
    {
        throw new NotImplementedException();
    }
}

internal class SpawnMetadata
{
    // Stub for the meta data related to the project.
    internal float spawnTimestamp;
    internal int spawnOrder;
    internal string spawnedByScript;
}

[Serializable]
public class IntegrityTestConfig
{
    public string projectName;
    public List<TrackedObjectDefinition> objects;
    public List<TaskLinkDefinition> taskLinks;
    public List<UICheckDefinition> uiChecks;
}

[Serializable]
public class TrackedObjectDefinition
{
    public string objectId;
    public string objectName;
    public SerializableVector3 position;
    public float positionTolerance = 0.1f;
    public bool isActive = true;

    public string spawnedByScript;
    public int spawnOrder = -1;
    public float spawnTimestamp = -1f;
    public float spawnTimeTolerance = 0.5f;

    public List<TaskStateRequirement> requiredTaskStates;
}

[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class TaskStateRequirement
{
    public string taskKey;
    public string expectedValue;
}

[Serializable]
public class TaskLinkDefinition
{
    public string linkName;
    public string sourceObjectName;
    public string targetObjectName;
    public string sourceTaskKey;
    public string targetTaskKey;
    public string requiredSourceValue;
    public string requiredTargetValue;
}

[Serializable]
public class UICheckDefinition
{
    public string objectName;
    public string expectedText;
}

[Serializable]
public class IntegrityTestReport
{
    public string projectName;
    public string importMode;
    public string testTime;

    public int totalObjects;
    public int passedObjects;
    public int failedObjects;
    public bool overallPassed;

    public List<ObjectTestResult> results;
    public List<string> summaryMessages;
}

[Serializable]
public class ObjectTestResult
{
    public string objectId;
    public string objectName;
    public bool passed;
    public List<string> checks;
}