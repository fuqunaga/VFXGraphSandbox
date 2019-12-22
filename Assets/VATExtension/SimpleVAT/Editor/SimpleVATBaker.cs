using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class SimpleVATBaker : EditorWindow
{
    [MenuItem("Window/SimpleVATBaker")]
    public static void ShowWindow()
    {
        GetWindow<SimpleVATBaker>();
    }


    public GameObject targetObj;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float fps = 60f;


    private void OnGUI()
    {
        targetObj = (GameObject)EditorGUILayout.ObjectField(targetObj, typeof(GameObject), true);
        skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);

        if (GUILayout.Button("Bake!"))
        {
            Bake();
        }
    }

    void Bake()
    {
        var animation = targetObj.GetComponent<Animation>();
        var clip = animation.clip;
        var length = clip.length;
        var timeStepCount = Mathf.FloorToInt(length * fps);

        var vertexCount = skinnedMeshRenderer.sharedMesh.vertexCount;
        /*
        var totalCount = vertexCount * timeStepCount;

        
        var width = Mathf.Min(totalCount, 2048);
        var height = Mathf.CeilToInt(totalCount / width);
        */


        var tex = new Texture2D(vertexCount, timeStepCount, TextureFormat.RGBAFloat, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        var colors = new List<Color>(vertexCount * timeStepCount);


        var mesh = new Mesh();
        var vertices = new List<Vector3>(vertexCount);
        for (var i = 0; i < timeStepCount; ++i)
        {
            clip.SampleAnimation(targetObj, i / fps);

            skinnedMeshRenderer.BakeMesh(mesh);

            mesh.GetVertices(vertices);

            colors.AddRange(vertices.Select(v => new Color(v.x, v.y, v.z)));
        }

        tex.SetPixels(colors.ToArray());

        AssetDatabase.CreateAsset(tex, "Assets/vat.asset");

#if false
        var data = tex.EncodeToPNG();

        string filePath = EditorUtility.SaveFilePanel("Save Texture", Application.dataPath, "vat.png", "png");
        if (!string.IsNullOrEmpty(filePath))
        {
            File.WriteAllBytes(filePath, data);
        }
#endif
    }
}
