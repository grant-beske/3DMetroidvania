using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour {

    public float radius;
    public float velocity;
    public float timeToLive;
    public Transform tip;
    public Transform root;
    public LayerMask hittableLayers = -1;

    private Vector3 velocityVector;
    private Vector3 lastRootPosition;

    public GameObject hitSfx;

    public GameObject hitVfx;
    public float hitVfxSpawnOffset = 0.1f;

    void Start() {
        Destroy(gameObject, timeToLive);
        velocityVector = transform.forward * velocity;
        lastRootPosition = root.position;
    }

    void Update() {
        // Move the projectile
        transform.position += (velocityVector * Time.deltaTime);
        // Orient towards velocity
        transform.forward = velocityVector.normalized;
        // Check for hits
        CheckHit();
        // Store new position as last root position
        lastRootPosition = root.position;
    }

    private void CheckHit() {
        RaycastHit closestHit = new RaycastHit();
        closestHit.distance = Mathf.Infinity;
        bool foundHit = false;

        Vector3 displacementSinceLastFrame = tip.position - lastRootPosition;
        RaycastHit[] hits =
            Physics.SphereCastAll(
                lastRootPosition,
                radius,
                displacementSinceLastFrame.normalized,
                displacementSinceLastFrame.magnitude,
                hittableLayers,
                QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits) {
            if (IsHitValid(hit) && hit.distance < closestHit.distance) {
                foundHit = true;
                closestHit = hit;
            }
        }

        if (foundHit) {
            // Handle case of casting while already inside a collider
            if (closestHit.distance <= 0f) {
                closestHit.point = root.position;
                closestHit.normal = -transform.forward;
            }
            OnHit(closestHit.point, closestHit.normal, closestHit.collider);
        }
    }

    private bool IsHitValid(RaycastHit hit) {
        // TODO - add behavior to check whether a hit is valid.
        return true;
    }

    private void OnHit(Vector3 point, Vector3 normal, Collider collider) {
        PlayHitVfx(point, normal);
        PlayHitSfx(point, normal);
        Destroy(gameObject);
    }

    private void PlayHitVfx(Vector3 point, Vector3 normal) {
        if (hitVfx != null) {
            GameObject impactVfxInstance =
                Instantiate(
                    hitVfx,
                    point + (normal * hitVfxSpawnOffset),
                    Quaternion.LookRotation(normal));
            // TODO - impact VFX lifetime.
        }
    }

    private void PlayHitSfx(Vector3 point, Vector3 normal) {
        if (hitSfx != null) {
            GameObject impactSfxInstance =
                Instantiate(hitSfx, point, Quaternion.LookRotation(normal));
        }
    }
}
