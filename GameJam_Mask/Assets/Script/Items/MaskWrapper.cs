using System.Collections.Generic;
using Masks;

namespace Items {
    public class MaskWrapper : Item {
        public static Dictionary<MaskType, Item> ByMask { get; } = new();
        public MaskType Mask { get; }

        public MaskWrapper(MaskType mask) {
            this.Mask = mask;
            ByMask[mask] = this;
        }
    }
}