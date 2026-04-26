using UnityEngine;

public class TestRunner : MonoBehaviour
{
    [SerializeField] private ProjectIntegrityTester tester;
    [SerializeField] private TextAsset configFile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IntegrityTestReport report = tester.RunIntegrityTest(configFile, "full");

        if (report != null && report.overallPassed)
        {
            Debug.Log("Project integrity test passed.");
        }
        else
        {
            Debug.LogWarning("Project integrity test failed.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
