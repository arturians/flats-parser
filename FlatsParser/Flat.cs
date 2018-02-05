using System;
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
        public double AreaRatio => Math.Round(LivingArea / TotalArea, 2);
        public double PricePerMetre => Math.Round(Price / TotalArea, 2);
        public int Id;
        public string Url;
        public State CurrentState;
    }

    [DefaultValue(Free)]
    public enum State
    {
        Free,
        Reserved,
        Sold
    }
}