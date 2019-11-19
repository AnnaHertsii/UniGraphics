namespace UniGraphics.ColorModels
{
    public class HSLColor
    {
        public short H { get; set; }
        public byte S { get; set; }
        public byte L { get; set; }

        public HSLColor() { }

        public HSLColor(short H, byte S, byte L) 
        {
            this.H = H;
            this.S = S;
            this.L = L;
        }

        public HSLColor DeepCopy()
        {
            return new HSLColor() { H = this.H, S = this.S, L = this.L };
        }
    }
}
