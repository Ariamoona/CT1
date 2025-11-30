using UnityEditor;
using UnityEngine;

public class WebGLTemplateValidator : Editor
{
    [MenuItem("Build/Validate WebGL Template")]
    public static void ValidateTemplate()
    {
        string currentTemplate = PlayerSettings.WebGL.template;
        Debug.Log($"Current WebGL template: {currentTemplate}");

        if (string.IsNullOrEmpty(currentTemplate) || currentTemplate == "PROJECT:MyCustomTemplate")
        {
            Debug.Log("Custom WebGL template is configured correctly!");
            EditorUtility.DisplayDialog("Template Valid", "Custom WebGL template is configured correctly!", "OK");
        }
        else
        {
            Debug.LogWarning("Custom WebGL template is not configured!");
            EditorUtility.DisplayDialog("Template Warning", "Custom WebGL template is not configured!", "OK");
        }
    }
}