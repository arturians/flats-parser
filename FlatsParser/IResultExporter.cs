using System.Collections.Generic;

namespace FlatsParser
{
    internal interface IResultExporter
    {
        void Export(IEnumerable<Flat> flats);
    }
}