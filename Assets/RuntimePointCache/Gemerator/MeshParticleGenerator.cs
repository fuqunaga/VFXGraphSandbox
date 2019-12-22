using System.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider))]
public class MeshParticleGenerator : MonoBehaviour
{
    public MeshParticle meshParticlePrefab;

    private void OnTriggerEnter(Collider other)
    {
        var renderer = other.GetComponent<Renderer>();
        var mp = Instantiate(meshParticlePrefab);

        mp.model = other.gameObject;
        mp.startEffect = true;

        StartCoroutine(DestroyDelay(mp.gameObject, mp.effectDiableDelay + 1f));

    }

    IEnumerator DestroyDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(target);
    }
}