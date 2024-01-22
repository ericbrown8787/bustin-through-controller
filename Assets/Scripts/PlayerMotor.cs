using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 5.0f;
    private Animator animator;
    private AudioSource audioSource;

    [Header("Jump")]
    private bool isGrounded;
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;

    [Header("Attack")]
    public GameObject axe;
    public float attackRange = 3f;
    public float attackDelay = .6f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swingSound;
    public AnimationClip swingAnimation;
    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    [Header("Camera/Look")]
    public Camera cam;
    private float xRotation = 0.0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;


    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection)*speed*Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0) playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3 * gravity);
        }
    }

    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;
        AttackAnimation();
        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);
        audioSource.pitch = Random.Range(.9f, 1.1f);
        audioSource.PlayOneShot(swingSound);
    }

    public void AttackAnimation()
    {
        Animator anim = axe.GetComponent<Animator>();
        anim.SetTrigger("Attack");
    }
    private void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    private void AttackRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackRange, attackLayer))
        {
            Debug.Log("Attacking!");
            HitTarget(hit);
            if(hit.transform.TryGetComponent<HittableObject>(out HittableObject target)){
                target.TakeDamage(attackDamage);
            }
        }
    }

    private void HitTarget(RaycastHit hit)
    {
        Vector3 pos = hit.point;
        HittableObject target = hit.transform.GetComponent<HittableObject>();
        audioSource.pitch = 1;
        audioSource.PlayOneShot(target.GetCurrentImpactSound());
        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        // calculate camera rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);

        // Rotate player to look horizontally
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}
