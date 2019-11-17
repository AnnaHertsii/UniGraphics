namespace UniGraphics.ColorModels
{
    public class HSLColor
    {
        public short H { get; set; }
        public byte S { get; set; }
        public byte L { get; set; }

        public HSLColor DeepCopy()
        {
            return new HSLColor() { H = this.H, S = this.S, L = this.L };
        }
    }
}
