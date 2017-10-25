using System;
using System.ComponentModel;
using System.Globalization;

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

        public override string ToString()
        {
            return $"Секция: {Section}{Environment.NewLine}" +
                   $"Этаж: {Floor}{Environment.NewLine}" +
                   $"Номер: {Number}{Environment.NewLine}" +
                   $"Количество комнат: {RoomsCount}{Environment.NewLine}" +
                   $"Площадь: {TotalArea}{Environment.NewLine}" +
                   $"Жилая площадь: {LivingArea}{Environment.NewLine}" +
                   $"Площадь кухни-студии: {KitchenArea?.ToString(CultureInfo.InvariantCulture) ?? string.Empty}{Environment.NewLine}" +
                   $"Отношение площади: {AreaRatio}{Environment.NewLine}" +
                   $"Цена за кв.м.: {Price}{Environment.NewLine}" +
                   $"Цена за кв.м.: {Id}";
        }
    }

    [DefaultValue(Free)]
    public enum State
    {
        Free,
        Reserved,
        Saled
    }
}