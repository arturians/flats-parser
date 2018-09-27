using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlatsParser
{
    public class Flat
    {
        public int Section;
        public int Floor;
        public int Number;
        public int RoomsCount;
        public decimal TotalArea;
        public decimal LivingArea;
        [Obsolete]
        public double? KitchenArea;
        public decimal Price;
        public decimal AreaRatio => SaveDevide(LivingArea, TotalArea);
        public decimal PricePerMetre => SaveDevide(Price, TotalArea);
        public decimal FlatHight;
        public int Id;
        public string Url;
        [JsonConverter(typeof(StringEnumConverter))]
        public State CurrentState;
        [JsonConverter(typeof(StringEnumConverter))]
        public Decoration Decoration;
        public bool IsClassicKitchen;
        public string BuildName;
        public int Bathroom;
        public int Balcony;
        public string Deadline;

        public bool IsSame(Flat other)
        {
            return Section == other.Section
                   && Floor == other.Floor
                   && Number == other.Number
                   && RoomsCount == other.RoomsCount
                   && TotalArea == other.TotalArea
                   && LivingArea == other.LivingArea
                   && Id == other.Id;
        }

        private static decimal SaveDevide(decimal devidend, decimal devisor)
        {
            return devisor == 0 
                ? default
                : decimal.Divide(devidend, devisor);
        }

        public override string ToString()
        {
            return $"Number {Number}; rooms {RoomsCount}; floor {Floor}; section {Section}; Id {Id}; BuildName {BuildName}";
        }
    }

    [DefaultValue(Free)]
    public enum State
    {
        Free = 1, //note: for right serialization
        Reserved = 2,
        Sold = 3
    }

    [DefaultValue(Unknown)]
    public enum Decoration
    {
        Unknown = 1,
        None = 2,
        Full = 3
    }
}