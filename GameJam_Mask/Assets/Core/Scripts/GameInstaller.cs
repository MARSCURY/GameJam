using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    private void Awake()
    {
        // Core singletons（简单粗暴版）
        var state = gameObject.AddComponent<GameState>();
        var subs = gameObject.AddComponent<SubtitleSystem>();
        var inv = gameObject.AddComponent<ThoughtInventory>();
        var book = gameObject.AddComponent<RecipeBook>();
        var maskDb = gameObject.AddComponent<MaskDatabase>();
        var maskSys = gameObject.AddComponent<MaskSystem>();
        var craft = gameObject.AddComponent<CraftSystem>();
        var ui = gameObject.AddComponent<RuntimeUI>();
        var tester = gameObject.AddComponent<TestItemSpawner>();

        // 填充面具定义（先用占位）
        maskDb.Add(new MaskDef(
            id: "mask_anger",
            name: "愤怒面具",
            targetType: MaskTargetType.Wife,
            subtitle: "妻子姿势突变：她把包狠狠摔在地上，指着你（占位演出）。"
        ));

        maskDb.Add(new MaskDef(
            id: "mask_happy",
            name: "快乐面具",
            targetType: MaskTargetType.Wife,
            subtitle: "妻子姿势突变：她抱住女儿，憧憬未来（占位演出）。"
        ));

        // 填充配方（对应你文档的玄关逻辑）
        book.Add(new Recipe("ticket_hidden", "old_bankbook", "mask_anger"));
        book.Add(new Recipe("south_job", "daughter_score", "mask_happy"));

        // 系统 Init
        maskSys.Init(maskDb, subs, state);
        craft.Init(inv, book, maskSys, subs);
        ui.Init(inv, craft, maskSys, state, subs);
        tester.Init(inv, subs);

        subs.Show("启动成功：按 1/2/3/4 获得念头；右侧选择 A+B 合成；对妻子/女儿使用。", 6f);
    }
}
