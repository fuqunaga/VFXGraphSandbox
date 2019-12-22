using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShot : MonoBehaviour
{
    public GameObject ballPrefab;
    public float shotSpeed = 2f;
    public float interval = 1f;
    float lastShotTime;

    void Update()
    {
        if ( Input.GetMouseButton(0) && Time.time - lastShotTime > interval)
        {
            Shot();
            lastShotTime = Time.time;
        }
    }

    void Shot()
    {
        var cam = Camera.main;
        var cameraTrans = cam.transform;

        var go = Instantiate(ballPrefab);
        go.transform.position = cameraTrans.position;
            

        var rd = go.GetComponent<Rigidbody>();
        rd.velocity = cameraTrans.forward * shotSpeed;
    }
}
