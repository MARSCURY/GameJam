using UnityEngine;

namespace Scenes.Interaction
{
    /// <summary>
    /// 交互检测主脚本（挂在玩家摄像机上）
    /// - E 键触发
    /// - 射线从屏幕中心发射
    /// - 支持“二次确认”：第二次按 E 不依赖再次瞄准（会转发给上一次成功交互的对象）
    /// </summary>
    public class InteractDetector : MonoBehaviour
    {
        [Header("检测配置")]
        [Tooltip("射线最大检测距离")]
        public float rayMaxDistance = 100f;

        [Tooltip("是否打印交互日志（调试用，发布可关闭）")]
        public bool showInteractLog = true;

        [Header("二次确认 / Held 设置")]
        [Tooltip("如果射线没命中可交互物体，是否把 E 转发给上一次交互对象（用于二次确认）")]
        public bool forwardToLastInteractableWhenNoHit = true;

        [Tooltip("按下这个键可强制清空当前 Held（可选）")]
        public KeyCode clearHeldKey = KeyCode.Q;

        private Camera cam;

        // 关键：记录“上一次成功触发交互的对象”
        private IInteractable lastInteractable;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("[InteractDetector] This script must be attached to a GameObject with a Camera component.");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(clearHeldKey))
            {
                ClearHeld();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Detect3DInteractObject(999); // 999 = 键盘E触发
            }
        }

        /// <summary>
        /// 外部可调用：清空“当前Held/上次交互对象”
        /// （建议在物品确认入栏位/放回原位/丢弃之后调用）
        /// </summary>
        public void ClearHeld()
        {
            lastInteractable = null;
            if (showInteractLog) Debug.Log("[InteractDetector] Held cleared.");
        }

        private void Detect3DInteractObject(int trigger)
        {
            if (cam == null) return;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            // 用 RaycastAll：从近到远找“第一个可交互对象”
            RaycastHit[] hits = Physics.RaycastAll(ray, rayMaxDistance);
            if (hits != null && hits.Length > 0)
            {
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                for (int i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    if (hit.collider == null) continue;

                    IInteractable interactable = FindInteractable(hit.collider);
                    if (interactable != null)
                    {
                        lastInteractable = interactable;
                        interactable.OnInteract(trigger);

                        if (showInteractLog)
                            Debug.Log($"[3D交互] 命中「{hit.collider.gameObject.name}」触发交互", hit.collider.gameObject);

                        return;
                    }
                }
            }

            // 没找到任何可交互 -> Forward 给 Held/LastInteractable（用于二次确认）
            if (forwardToLastInteractableWhenNoHit && lastInteractable != null)
            {
                if (showInteractLog)
                    Debug.Log("[3D交互] No interactable hit -> Forward to Held/LastInteractable");

                lastInteractable.OnInteract(trigger);
            }
            else
            {
                if (showInteractLog) Debug.Log("[3D交互] No hit & no held");
            }
        }



        /// <summary>
        /// 在碰撞体自己/父级上找任何实现了 IInteractable 的脚本
        /// （避免 GetComponent<IInteractable>() 在某些情况下取不到接口的问题）
        /// </summary>
        private static IInteractable FindInteractable(Collider col)
        {
            // 自己身上
            var self = col.GetComponents<MonoBehaviour>();
            foreach (var mb in self)
                if (mb is IInteractable ia) return ia;

            // 父级上
            var parents = col.GetComponentsInParent<MonoBehaviour>();
            foreach (var mb in parents)
                if (mb is IInteractable ia) return ia;

            return null;
        }

        private void OnDrawGizmos()
        {
            var c = GetComponent<Camera>();
            if (c == null) return;

            Gizmos.color = Color.green;
            Ray ray = c.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Gizmos.DrawRay(ray.origin, ray.direction * 3f);
        }
    }
}
