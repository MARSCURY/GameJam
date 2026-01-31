using UnityEngine;
using Ideas;

public class TestIdeaSeeder : MonoBehaviour
{
    private void Awake()
    {
        // 如果已经有数据，就不重复塞（避免覆盖你后面正经表）
        if (!IdeaManager.Ideas.ContainsKey("ticket"))
            IdeaManager.Register("ticket", "车票", "车票背后写着一串陌生号码。", "");

        if (!IdeaManager.Ideas.ContainsKey("passbook"))
            IdeaManager.Register("passbook", "旧存折", "余额被反复划掉又写上。", "");
    }
}
