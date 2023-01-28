using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    CharacterController controller;
    new Transform transform;
    Animator animator;
    new Camera camera;

    Plane plane;
    Ray ray;
    Vector3 hit_point;

    public float move_speed = 10;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        plane = new Plane(transform.up, transform.position);
    }

    void Update()
    {
        Move();
        Turn();
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    void Move()
    {
        Vector3 camera_forward = camera.transform.forward;
        Vector3 camera_right = camera.transform.right;
        camera_forward.y = 0;
        camera_right.y = 0;

        Vector3 move_dir = (camera_forward * v) + (camera_right * h);
        move_dir.Set(move_dir.x, 0, move_dir.z);

        controller.SimpleMove(move_dir * move_speed);

        float forward = Vector3.Dot(move_dir, transform.forward);
        float strafe = Vector3.Dot(move_dir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);

        float enter = 0;
        plane.Raycast(ray, out enter);
        hit_point = ray.GetPoint(enter);

        Vector3 look_dir = hit_point - transform.position;
        look_dir.y = 0;

        transform.localRotation = Quaternion.LookRotation(look_dir);
    }
}
