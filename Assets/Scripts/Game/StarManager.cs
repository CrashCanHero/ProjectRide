using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StarManager : MonoBehaviour {
    public WarpStar Base;
    public bool Charging {
        get {
            return manager.player.GetButton("Charge");
        }
    }

    [HideInInspector] public PlayerManager manager;

    [Header("Physics")]
    public LayerMask GroundMask;
    public float MinJumpOffSpeed;
    [HideInInspector] public float GetOffTime;
    Rigidbody rb;

    [Header("Debug")]
    public bool DrawDebugStuff = true;

    public bool HasRider {
        get {
            return transform.childCount > 1;
        }
    }

    public bool isGrounded {
        get {
            return Physics.Raycast(transform.position, Vector3.down, Base.DistanceToGround, GroundMask);
        }
    }

    public void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void Start() {
        Collider col = GetComponent<Collider>();
        GameEvents.StarSpawned(Base, col);
    }

    public void Update() {
        if (HasRider) {
            RiderPhysics();
        } else {
            NoRiderPhysics();
        }
    }

    public void OnDrawGizmos() {
        if (DrawDebugStuff) {
            Debug.DrawRay(transform.position, Vector3.down * Base.DistanceToGround, Color.green);
        }
    }

    public void RiderPhysics() {
        if (!Charging) {
            Vector3 vel = rb.velocity;
            vel += transform.forward * Base.NewAcceleration;
            vel = Vector3.ClampMagnitude(vel, Base.NewMaxSpeed);
            rb.velocity = vel;

            Debug.DrawRay(transform.position, vel, Color.red);
            Debug.DrawRay(transform.position, transform.forward * Base.NewAcceleration, Color.blue);
        } else {

        }

        Vector3 rot = transform.localEulerAngles;
        if (manager.player.GetAxis("horizontal") >= 0.1f || manager.player.GetAxis("horizontal") <= -0.1f) {
            rot.y += manager.player.GetAxis("horizontal") * Base.NewTurnSpeed;
        }
        transform.localEulerAngles = rot;

        if (!isGrounded) {
            rb.AddForce(Vector3.down * Base.NewWeight);
        } else {
            if (manager.player.GetButton("Charge") && manager.player.GetAxis("vertical") < -0.1f && rb.velocity.magnitude < MinJumpOffSpeed) {
                GetOffTime += Time.deltaTime;
                if (GetOffTime >= Global.Instance.StarGetOffWaitTime) {
                    StartCoroutine(manager.GetOffStar(gameObject));
                }
            } else {
                GetOffTime = 0;
            }
        }
    }

    public void NoRiderPhysics() {
        if (!isGrounded) {
            rb.AddForce(Vector3.down * Global.Instance.GlobalStarWeight);
        }
    }
}