using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float life = 5f;

    void Start()
    {
        StartCoroutine(DelayDestroy());
    }


    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(life);
        Destroy(gameObject);
    }
}
