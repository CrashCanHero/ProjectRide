using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirbyAnimationHandler : MonoBehaviour {
    public PlayerManager playerManager;
    public Animator anim;
    public Material FaceMaterial;
    public List<Texture2D> FaceTextures = new List<Texture2D>();

    public void Update() {
        Vector3 vel = new Vector3(playerManager.rb.velocity.x, 0, playerManager.rb.velocity.z);
        anim.SetBool("Grounded", playerManager.isGrounded());
        anim.SetFloat("Speed", vel.magnitude / 10);
        anim.SetFloat("YVelocity", playerManager.rb.velocity.y / 10);
    }

    public void UpdateFace(int index) {
        FaceMaterial.SetTexture("_MainTex", FaceTextures[index]);
    }

    public void Jump() {
        anim.SetTrigger("Jump");
    }

    public void Land() {
        anim.SetTrigger("Land");
    }
}