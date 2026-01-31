using Masks;

namespace Characters {
    //所有木偶的基类
    public class Puppet : IMask {
        public MaskType CurrentMask { get; set; }

        public MaskType GetMask() {
            return this.CurrentMask;
        }
    }
}