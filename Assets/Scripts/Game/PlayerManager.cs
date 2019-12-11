using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Rewired;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Player player;
    public int PlayerID;
    public CameraDirection cameraDirection;
    public KirbyAnimationHandler kirbyAnimationHandler;
    public CinemachineFreeLook Camera;
    public bool CanMove = true;

    [Header("Physics Things")]
    public float Weight;
    float weightStore;
    public float MovementSpeed;
    public float JumpPower;
    public int JumpAmount;
    int JumpAmountStore;
    public float TurnSpeed;
    public Collider EnvironmentCollider;
    public Vector3 MovementVector {
        get {
            Vector3 Pos = Vector3.zero;
            Pos.x = player.GetAxisRaw("Horizontal") * MovementSpeed;
            Pos.z = player.GetAxisRaw("Vertical") * MovementSpeed;
            return Pos;
        }
    }

    Vector3 PositionStore;

    [Header("Ground Checking")]
    public float DistanceToGround;
    public LayerMask GroundMask;

    [Header("Warp Star Related Things")]
    public AnimationCurve HopCurve;
    public Transform StarPivot;
    public Transform LookAtTarget;
    public Transform CamStarLookAtTarget;
    Rigidbody StarBody;
    StarManager Star;
    public bool OnStar {
        get {
            return transform.parent != null;
        }
    }

    [Header("Debug Stuff")]
    public bool DrawDebugStuff;

    public bool isGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, DistanceToGround, GroundMask);
    }

    public IEnumerator GetOnStar(float time, GameObject star) {
        CanMove = false;
        float t = 0;
        Camera.LookAt = CamStarLookAtTarget;
        Camera.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
        while (t < time) {
            float percentage = t / time;
            Vector3 movePos = new Vector3(star.transform.position.x, transform.position.y, star.transform.position.z);
            float dist = Vector3.Distance(transform.position, movePos);
            transform.position = Vector3.MoveTowards(transform.position, movePos, dist / 20);
            float yPos = star.transform.position.y * HopCurve.Evaluate(percentage);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.parent = star.transform;
        EnvironmentCollider.enabled = false;
        rb.velocity = Vector3.zero;
        Vector3 rot = new Vector3(0, star.transform.localEulerAngles.y, 0);
        transform.localEulerAngles = rot;
        StarBody = star.GetComponent<Rigidbody>();
        Star = star.GetComponent<StarManager>();
        Star.manager = this;
    }

    public IEnumerator GetOffStar(GameObject star) {
        transform.parent = null;
        rb.velocity = (-transform.forward * 5) + (transform.up * 20);
        Camera.LookAt = LookAtTarget;
        Camera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        yield return new WaitForSeconds(1);
        EnvironmentCollider.enabled = true;
        CanMove = true;
        yield return null;
    }

    public void Awake() {
        rb = GetComponent<Rigidbody>();
        player = ReInput.players.GetPlayer(PlayerID);
        JumpAmountStore = JumpAmount;
        weightStore = Weight;
    }

    public void OnEnable() {
        GameEvents.OnStarSpawned += StarSpawned;
    }

    public void OnDisable() {
        GameEvents.OnStarSpawned -= StarSpawned;
    }

    public void OnDrawGizmos() {
        if (DrawDebugStuff) {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - DistanceToGround, transform.position.z), Color.green);
        }
    }

    public void LateUpdate() {
        if (OnStar) {
            OnStarPhysics();
        } else {
            OffStarPhysics();
        }
    }

    public void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Environment") {
            kirbyAnimationHandler.Land();
            JumpAmount = JumpAmountStore;
            return;
        }
    }

    public void OnTriggerEnter(Collider col) {
        if (col.tag == "WarpStar") {
            if (OnStar) {
                return;
            }
            StartCoroutine(GetOnStar(0.3f, col.transform.parent.gameObject));
            return;
        }
    }

    public void OffStarPhysics() {
        if (CanMove) {
            //Movement
            if (MovementVector != Vector3.zero) {
                Vector3 vel = (cameraDirection.GetCameraForward() * MovementVector.z + cameraDirection.GetCameraRight() * MovementVector.x) * Time.deltaTime * MovementSpeed;
                vel.y = rb.velocity.y;
                vel += transform.forward;
                rb.velocity = vel;
            } else {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

            RotateToVelocity(TurnSpeed, true);

            //Jumping
            if (player.GetButtonDown("Jump") && JumpAmount > 0) {
                if (isGrounded()) {
                    kirbyAnimationHandler.Jump();
                }
                rb.velocity = new Vector3(rb.velocity.x, JumpPower, rb.velocity.z);
                JumpAmount--;
            }
        }

        //Physics weight thing
        if (player.GetButton("Jump") && rb.velocity.y > 0.1f) {
            Weight = weightStore / 2;
        } else {
            Weight = weightStore;
        }

        //Gravity
        if (!isGrounded()) {
            rb.AddForce(new Vector3(0, -(Weight * Global.Instance.Gravity) * Time.deltaTime, 0));
        } else {
            JumpAmount = JumpAmountStore;
        }
    }

    public void OnStarPhysics() {
        transform.position = transform.parent.position + (transform.position - StarPivot.position);
    }

    public void RotateToVelocity(float turnSpeed, bool ignoreY = true) {
        Vector3 dir;
        dir = new Vector3(rb.velocity.x, ignoreY ? 0f : rb.velocity.y, rb.velocity.z);

        if (dir.magnitude > 0.1) {
            Quaternion dirQ = Quaternion.LookRotation(dir);
            Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, dir.magnitude * turnSpeed * Time.deltaTime);
            rb.MoveRotation(slerp);
        }
    }

    void StarSpawned(WarpStar star, Collider col) {
        Physics.IgnoreCollision(EnvironmentCollider, col);
    }
}