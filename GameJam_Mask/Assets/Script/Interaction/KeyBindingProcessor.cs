using UnityEngine;

namespace Scenes.Interaction {
    //主控制代码，绑定到主摄像机
    [RequireComponent(typeof(CharacterController))] // 自动添加CharacterController组件
    public class KeyBindingProcessor : MonoBehaviour {
        [Header("鼠标视角设置")] public float mouseSensitivity = 100f;
        private float xRotation = 0f;

        [Header("移动设置")] [Tooltip("移动速度（单位/秒）")]
        public float moveSpeed = 8f;

        [Tooltip("是否忽略Y轴移动（防止摄像机上下飘移）")] public bool ignoreYAxis = true;

        [Header("跳跃设置")] [Tooltip("跳跃高度")] public float jumpHeight = 2f;
        [Tooltip("重力加速度（建议-9.81或更小，模拟真实重力）")] public float gravity = -9.81f;

        [Header("地面检测")] [Tooltip("地面检测的胶囊体半径偏移")]
        public float groundCheckRadius = 0.2f;

        [Tooltip("地面检测的位置偏移（相对于摄像机Transform）")]
        public Vector3 groundCheckOffset = new(0, -0.1f, 0);

        [Tooltip("哪些层被判定为地面")] public LayerMask groundLayer;

        // 核心组件和物理参数
        private CharacterController _cc; // 角色控制器（替代Rigidbody，更适合第一人称控制）
        private Vector3 _moveDirection; // 移动方向
        private Vector3 _velocity; // 速度（用于重力和跳跃）
        private bool _isGrounded; // 是否在地面（防止二段跳）

        private void Awake() {
            // 获取/自动添加CharacterController组件
            this._cc = this.GetComponent<CharacterController>();
            // 初始化CharacterController参数（适配摄像机）
            if (this._cc.radius < 0.1f) this._cc.radius = 0.1f;
            if (this._cc.height < 0.2f) this._cc.height = 0.2f;
        }

        private void Update() {
            // 鼠标视角旋转逻辑（添加到原有Update开头）
            float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;

            this.xRotation -= mouseY;
            this.xRotation = Mathf.Clamp(this.xRotation, -90f, 90f); // 限制上下视角（避免抬头低头过度）

            this.transform.localRotation = Quaternion.Euler(this.xRotation, 0f, 0f);
            this.transform.Rotate(Vector3.up * mouseX); // 左右旋转

            // 1. 地面检测：球形检测，判断是否接触地面层
            this._isGrounded = Physics.CheckSphere(this.transform.TransformPoint(this.groundCheckOffset),
                this.groundCheckRadius,
                this.groundLayer);

            // 2. 落地时重置垂直速度（防止持续下坠）
            if (this._isGrounded && this._velocity.y < 0) {
                this._velocity.y = -2f; // 轻微下压力，保证贴地
            }

            // 3. 获取WSAD输入（水平/垂直轴，Unity默认映射WS=Vertical、AD=Horizontal）
            float horizontal = Input.GetAxisRaw("Horizontal"); // 左右：A=-1，D=1
            float vertical = Input.GetAxisRaw("Vertical"); // 前后：S=-1，W=1

            // 4. 计算世界空间下的移动方向（基于摄像机朝向，忽略Y轴避免斜向移动）
            this._moveDirection = (this.transform.forward * vertical + this.transform.right * horizontal).normalized;
            if (this.ignoreYAxis) this._moveDirection.y = 0; // 锁定Y轴，仅水平移动

            // 5. 应用水平移动
            this._cc.Move(this._moveDirection * (this.moveSpeed * Time.deltaTime));

            // 6. 跳跃逻辑：地面时按空格键触发
            if (Input.GetKeyDown(KeyCode.Space) && this._isGrounded) {
                // 跳跃速度公式：v = √(2 * 重力大小 * 跳跃高度)（物理自由落体公式推导）
                this._velocity.y = Mathf.Sqrt(this.jumpHeight * -2f * this.gravity);
            }

            // 7. 应用重力
            this._velocity.y += this.gravity * Time.deltaTime;
            // 8. 应用垂直速度（跳跃/下坠）
            this._cc.Move(this._velocity * Time.deltaTime);

            //其他按键处理
            PlayDataManager.Inventory.UpdateHotKeys();
            ItemViewingManager.UpdateInteraction();
        }

        // Gizmos绘制：场景视图中显示地面检测球的位置和大小，方便调试
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.TransformPoint(this.groundCheckOffset), this.groundCheckRadius);
        }
    }
}