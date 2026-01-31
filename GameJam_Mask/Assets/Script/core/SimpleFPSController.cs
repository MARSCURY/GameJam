using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class SimpleFPSController : MonoBehaviour
{
    /* ================= Movement ================= */

    [Header("Move")]
    public float walkSpeed = 3f;
    public float runSpeed = 6.5f;
    public float gravity = -9.8f;

    /* ================= Look ================= */

    [Header("Look")]
    public float mouseSensitivity = 2.5f;
    public Transform cameraPivot;

    /* ================= Footsteps (Loop) ================= */

    [Header("Footsteps (Loop)")]
    public AudioSource footstepAudio;

    [Tooltip("行走时的基础 pitch")]
    public float walkPitch = 1.0f;

    [Tooltip("奔跑时的最大 pitch")]
    public float runPitch = 1.3f;

    /* ================= Private ================= */

    private CharacterController controller;
    private float yVelocity;
    private float xRotation;

    /* ================= Unity ================= */

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (footstepAudio == null)
            footstepAudio = GetComponent<AudioSource>();

        // 脚步声循环配置
        footstepAudio.loop = true;
        footstepAudio.playOnAwake = false;
    }

    private void OnEnable()
    {
        // ✅ 进入“游戏态”才锁鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // ✅ 离开“游戏态”（Menu / UI）自动解锁
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 防止脚步声残留
        if (footstepAudio != null && footstepAudio.isPlaying)
            footstepAudio.Stop();
    }

    private void Update()
    {
        Look();
        Move();
    }

    /* ================= Look ================= */

    private void Look()
    {
        if (cameraPivot == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    /* ================= Move + Footsteps ================= */

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.right * x + transform.forward * z;

        // Ground check
        if (controller.isGrounded)
        {
            if (yVelocity < 0f)
                yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        controller.Move(move * speed * Time.deltaTime);

        HandleFootsteps(x, z, speed);
    }

    /* ================= Footstep Logic (Loop) ================= */

    private void HandleFootsteps(float x, float z, float speed)
    {
        bool isMoving =
            controller.isGrounded &&
            (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f) &&
            footstepAudio != null &&
            footstepAudio.clip != null;

        if (!isMoving)
        {
            if (footstepAudio.isPlaying)
                footstepAudio.Stop();
            return;
        }

        float speedRatio =
            Mathf.Clamp01((speed - walkSpeed) / Mathf.Max(0.01f, runSpeed - walkSpeed));

        footstepAudio.pitch =
            Mathf.Lerp(walkPitch, runPitch, speedRatio);

        if (!footstepAudio.isPlaying)
            footstepAudio.Play();
    }
}
