using System;
using UnityEngine;

public enum MaskTargetType
{
    Wife,
    Daughter,
    Self,
    TicketClerk,
    Environment
}

[Serializable]
public class MaskDef
{
    public string id;
    public string displayName;
    public MaskTargetType targetType;
    [TextArea] public string onUseSubtitle;

    public MaskDef(string id, string name, MaskTargetType targetType, string subtitle)
    {
        this.id = id;
        this.displayName = name;
        this.targetType = targetType;
        this.onUseSubtitle = subtitle;
    }
}
