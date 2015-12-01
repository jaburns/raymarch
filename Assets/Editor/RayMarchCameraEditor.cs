using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RayMarchCamera))]
public class RayMarchCameraEditor : Editor
{
    RayMarchCamera targ { get { return target as RayMarchCamera; } }

    void OnSceneGUI()
    {

    }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Build Shader")) {
            buildShader(buildDistanceFunction());
        }
    }

    void buildShader(string distanceFunction)
    {
        var template = File.ReadAllText(Application.dataPath + "/SceneShaderTemplate.shader");
        var lines = template.Split('\n');
        for (int i = 0; i < lines.Length; ++i) {
            if (lines[i].Contains("DISTANCE_FUNCTION")) {
                lines[i] = distanceFunction;
            }
        }
        var shaderCode = string.Join("\n", lines);

        File.WriteAllText(Application.dataPath + "/SceneShader.shader", shaderCode);
        AssetDatabase.Refresh();
    }

    string buildDistanceFunction()
    {
        var func = "";
        foreach (var obj in FindObjectsOfType<RayMarchObject>()) {
            func = obj.GetDistanceFunction(); // lol
        }
        return func;
    }
}
