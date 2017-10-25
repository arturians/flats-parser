using System.ComponentModel;

namespace FlatsParser
{
    public class Flat
    {
        public int Section;
        public int Floor;
        public int Number;
        public int RoomsCount;
        public double TotalArea;
        public double LivingArea;
        public double? KitchenArea;
        public int Price;
        public double AreaRatio => LivingArea / TotalArea;
        public double PricePerMetre => Price / TotalArea;
        public int Id;
        public string Url;
        public State CurrentState;
    }

    [DefaultValue(Free)]
    public enum State
    {
        Free,
        Reserved,
        Saled
    }
}