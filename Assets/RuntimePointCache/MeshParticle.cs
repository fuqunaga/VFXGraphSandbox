using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class MeshParticle : MonoBehaviour
{
    public static class PropName
    {
        public const string PositionMap = "PositionMap";
        public const string UVMap = "UVMap";
        public const string NormalMap = "NormalMap";
        public const string ModelMainTex = "ModelMainTex";
        public const string VtxCount = "VtxCount";

    }

    public GameObject model;
    public VisualEffect effect;

    public bool startEffect;

    public float pointCountPerArea = 10000f;
    public float modelDiableDelay = 0.5f;
    public float modelEnableDelay = 9f;
    public float effectDiableDelay = 10f;


    MapSet mapSet;
    Texture modelMainTex;

    private void Update()
    {
        UpdateEffect();
    }

    private void UpdateEffect()
    {
        var effectGo = effect.gameObject;
        if (startEffect && effectGo.activeSelf == false)
        {
            InitEffect();

            StartCoroutine(StartEffectAtiveSequence());
            StartCoroutine(StartTargetActiveSequence());
        }

        if (startEffect && mapSet != null)
        {
            effect.SetTexture(PropName.PositionMap, mapSet.position);
            effect.SetTexture(PropName.UVMap, mapSet.uv);
            effect.SetTexture(PropName.NormalMap, mapSet.normal);
            effect.SetTexture(PropName.ModelMainTex, modelMainTex);
            effect.SetInt(PropName.VtxCount, mapSet.vtxCount);
        }
    }


    IEnumerator StartTargetActiveSequence()
    {
        yield return new WaitForSeconds(modelDiableDelay);

        model.SetActive(false);

        yield return new WaitForSeconds(modelEnableDelay - modelDiableDelay);

        model.SetActive(true);
    }

    IEnumerator StartEffectAtiveSequence()
    {
        var effectGo = effect.gameObject;
        effectGo.SetActive(startEffect);

        yield return new WaitForSeconds(effectDiableDelay);

        effectGo.SetActive(false);

        startEffect = false;
    }

    private void InitEffect()
    {
        var (mesh, tex, normal) = GetMeshData();
        modelMainTex = tex;
        mapSet = MeshToMap.ComputeMap(mesh, pointCountPerArea);

        var modelTrans = model.transform;
        transform.SetPositionAndRotation(modelTrans.position, modelTrans.rotation);
    }

    (Mesh, Texture, Texture) GetMeshData()
    {
        Mesh mesh = null;
        Material material = null;

        var smr = model.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
        {
            material = smr.sharedMaterial;

            mesh = new Mesh();
            smr.BakeMesh(mesh);
        }
        else
        {
            var mf = model.GetComponentInChildren<MeshFilter>();
            var renderer = model.GetComponentInChildren<Renderer>();
            mesh = mf.sharedMesh;
            material = renderer?.sharedMaterial;
        }


        return (mesh, material.mainTexture, material.GetTexture("_NormalMap"));

    }
}
