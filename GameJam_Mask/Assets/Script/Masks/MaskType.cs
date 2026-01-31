using Items;

namespace Masks {
    //面具类型基类
    public record MaskType {
        public MaskType() {
            MaskWrapper _ = new(this);
        }
    }
}