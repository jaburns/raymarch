using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RayMarchCamera))]
public class RayMarchCameraEditor : Editor
{
    RayMarchCamera targ { get { return target as RayMarchCamera; } }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Build Shader")) {
            buildShader(buildDistanceFunction());
        }
    }

    void buildShader(string distanceFunction)
    {
        var outputFile = RayMarchCamera.SHADER_PATH_PREFIX + targ.GetInstanceID();

        var template = File.ReadAllText(AssetDatabase.GetAssetPath(targ.ShaderTemplate));
        var lines = template.Split('\n');
        for (int i = 0; i < lines.Length; ++i) {
            if (lines[i].Contains("__DISTANCE_FUNCTION")) {
                lines[i] = distanceFunction;
            }
            else if (lines[i].Contains("__SHADER_TITLE")) {
                lines[i] = "Shader \"GeneratedRMCShader/"+outputFile+"\" {";
            }
            else if (lines[i].Contains("__UNIFORMS")) {
                lines[i] = getUniforms();
            }
        }
        var shaderCode = string.Join("\n", lines);

        if (!AssetDatabase.IsValidFolder("Assets/Generated/Resources")) {
            AssetDatabase.CreateFolder("Assets", "Generated");
            AssetDatabase.CreateFolder("Assets/Generated", "Resources");
        }

        File.WriteAllText(Application.dataPath + "/Generated/Resources/"+outputFile+".shader", shaderCode);

        AssetDatabase.Refresh();
    }

    string getUniforms()
    {
        var ret = "";
        foreach (var obj in FindObjectsOfType<RayMarchObject>()) {
            ret += "uniform float4x4 _obj_"+obj.name+";";
        }
        return ret;
    }

    string buildDistanceFunction()
    {
        var nameIndex = 0;
        var names = new List<string>();
        var code = "";

        foreach (var obj in FindObjectsOfType<RayMarchObject>()) {
            names.Add("df"+nameIndex);
            code += "float dfx"+nameIndex+"(float3 p) {\n"+obj.GetDistanceFunction()+"}\n";
            code += "float df"+nameIndex+"(float3 p) {\nreturn dfx"+nameIndex+"(mul(_obj_"+obj.name+", float4(p,1)).xyz);}\n";
            nameIndex++;
        }

        code += "float distfunc(float3 p) {\n";

        var blendFunc = "";
        if (names.Count == 1) {
            blendFunc = name+"(p)";
        } else {
            blendFunc = "blend("+names[0]+"(p),"+names[1]+"(p))";
            for (int i = 2; i < names.Count; ++i) {
                blendFunc = "blend("+blendFunc+","+names[i]+"(p))";
            }
        }

        code += "return "+blendFunc+";}";

        return code;
    }
}
