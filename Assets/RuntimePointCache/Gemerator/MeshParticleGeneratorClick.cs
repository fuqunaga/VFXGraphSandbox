using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshParticleGeneratorClick : MonoBehaviour
{
    public MeshParticle meshParticlePrefab;
    public float interval;
    float lastClickTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastClickTime > interval)
        {
            lastClickTime = Time.time;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var other = hit.collider;
                var renderer = other.GetComponent<Renderer>();
                var mp = Instantiate(meshParticlePrefab);

                mp.model = other.gameObject;
                mp.startEffect = true;

                //StartCoroutine(DestroyDelay(mp.gameObject, mp.effectDiableDelay + 1f));
            }

        }
    }
}
