using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FlatsParser
{
    public class FlatsComparator
    {
        private readonly Flat[] _oldFlats;
        private readonly Flat[] _latestFlats;

        public FlatsComparator(Flat[] oldFlats, Flat[] latestFlats)
        {
            _oldFlats = oldFlats;
            _latestFlats = latestFlats;
        }

        public List<Tuple<Flat, Flat>> GetDistincts()
        {
            var distincts = new List<Tuple<Flat, Flat>>();
            if (_oldFlats.Length != _latestFlats.Length)
                throw new DataException($"Current array contains {_latestFlats.Count()} flats, but previous {_oldFlats.Count()}");
            for (var i = 0; i < _oldFlats.Length; ++i)
            {
                var oldFlatInfo = _oldFlats[i];
                var latestFlatInfo = _latestFlats[i];
                if (!oldFlatInfo.IsSame(latestFlatInfo))
                    throw new DataException($"Old flat {oldFlatInfo} is not same as {latestFlatInfo}");
                if (oldFlatInfo.Price != latestFlatInfo.Price || oldFlatInfo.CurrentState != latestFlatInfo.CurrentState)
                    distincts.Add(new Tuple<Flat, Flat>(oldFlatInfo, latestFlatInfo));
            }

            return distincts;
        }
    }
}