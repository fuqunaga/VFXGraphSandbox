using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class MeshParticle : MonoBehaviour
{
    public MeshParticleUnit unitPrefab;
    public GameObject model;


    public bool startEffect;

    public float pointCountPerArea = 10000f;
    public float modelDiableDelay = 0.5f;
    public float modelEnableDelay = 9f;
    public float animEnableDelay = 1f;
    public float effectDiableDelay = 10f;

    List<MeshParticleUnit> units = new List<MeshParticleUnit>();
    Animator pausedAnim;

    public bool autoDestroy;

    private void Update()
    {
        UpdateEffect();
    }

    private void UpdateEffect()
    {
        if (startEffect && units.Any() == false)
        {
            InitEffect();

            StartCoroutine(StartEffectAtiveSequence());
            StartCoroutine(StartTargetActiveSequence());
        }
    }

    IEnumerator StartTargetActiveSequence()
    {
        yield return new WaitForSeconds(modelDiableDelay);

        model.SetActive(false);

        yield return new WaitForSeconds(modelEnableDelay - modelDiableDelay);

        model.SetActive(true);

        if (pausedAnim != null)
        {
            yield return new WaitForSeconds(animEnableDelay);
            pausedAnim.enabled = true;
            pausedAnim = null;
        }


        if (autoDestroy) Destroy(gameObject);
    }

    IEnumerator StartEffectAtiveSequence()
    {
        yield return new WaitForSeconds(effectDiableDelay);

        units.ForEach(unit => Destroy(unit.gameObject));
        units.Clear();

        startEffect = false;
    }


    private void InitEffect()
    {
        var modelTrans = model.transform;
        transform.SetPositionAndRotation(modelTrans.position, modelTrans.rotation);

        var meshAndTexs = GetMeshData();

        foreach (var (mesh, tex) in meshAndTexs)
        {
            var unit = Instantiate(unitPrefab);
            unit.transform.SetParent(transform, false);

            unit.mapSet = MeshToMap.ComputeMap(mesh, pointCountPerArea);
            unit.modelMainTex = tex;

            units.Add(unit);
        }
    }


    IEnumerable<(Mesh, Texture)> GetMeshData()
    {
        var smr = model.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
        {
            // stop animation
            pausedAnim = model.GetComponentInParent<Animator>();
            pausedAnim.enabled = false;

            var material = smr.sharedMaterial;

            var mesh = new Mesh();
            smr.BakeMesh(mesh);

            return new[] { (mesh, material.mainTexture) };
        }
        else
        {
            var mf = model.GetComponentInChildren<MeshFilter>();
            var renderer = model.GetComponentInChildren<Renderer>();

            var mesh = mf.sharedMesh;
            var meshCount = mesh.subMeshCount;
            var materials = renderer.sharedMaterials; ;

            return new[] { (mesh, materials.First().mainTexture) };
        }
    }
}
