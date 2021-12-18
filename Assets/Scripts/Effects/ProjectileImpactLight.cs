using UnityEngine;
using System.Collections;
 
public class ProjectileImpactLight : MonoBehaviour {

    public float timeToLive = 0.2f;
    private float timeAlive = 0;

    private Light impactLight;
    private float initIntensity;

    void Start() {
        impactLight = gameObject.GetComponent<Light>();
        initIntensity = impactLight.intensity;
        Destroy(gameObject, timeToLive);
    }

    void Update() {
        timeAlive += Time.deltaTime;
        impactLight.intensity = initIntensity * (1 - (timeAlive / timeToLive));
    }
}