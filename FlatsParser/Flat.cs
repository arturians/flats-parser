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

        public bool IsSame(Flat other)
        {
            const double accuracy = 0.01;
            return Section == other.Section
                   && Floor == other.Floor
                   && Number == other.Number
                   && RoomsCount == other.RoomsCount
                   && AlmostEqual(TotalArea, other.TotalArea, accuracy)
                   && AlmostEqual(LivingArea, other.LivingArea, accuracy)
                   && Id == other.Id;
        }

        private static bool AlmostEqual(double a, double b, double accuracy)
        {
            return Math.Abs(a - b) < accuracy;
        }

        public override string ToString()
        {
            return $"Number {Number}; rooms {RoomsCount}; floor {Floor}; section {Section}; Id {Id}";
        }
    }

    [DefaultValue(Free)]
    public enum State
    {
        Free = 1, //note: for right serialization
        Reserved = 2,
        Sold = 3
    }
}