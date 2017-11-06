using System.Collections.Generic;
using System.Globalization;

namespace FlatsParser
{
    abstract class ResultExporter
    {
	    public abstract void Export(IEnumerable<Flat> flats);

	    protected static string FormatDouble(double d)
	    {
		    var formatDouble = d.ToString("F", CultureInfo.GetCultureInfo("ru-RU"));
		    return formatDouble;
	    }
	}
}