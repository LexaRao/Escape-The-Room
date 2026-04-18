using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextStateProviderManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] private Text legacyText;
    [SerializeField] private TMP_Text tmpText;

    public string GetTextValue()
    {
        if (tmpText != null)
            return tmpText.text;

        if (legacyText != null)
            return legacyText.text;

        return string.Empty;
    }
}
