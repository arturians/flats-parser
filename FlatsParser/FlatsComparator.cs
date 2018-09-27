using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FlatsParser
{
    public class FlatsComparator
    {
        private readonly Flat[] oldFlats;
        private readonly Flat[] latestFlats;

        public FlatsComparator(Flat[] oldFlats, Flat[] latestFlats)
        {
            this.oldFlats = oldFlats;
            this.latestFlats = latestFlats;
        }

		public List<FlatsDistinct> GetDistincts()
		{
			var distincts = new List<FlatsDistinct>();
			var oldFlatsDict = oldFlats.ToDictionary(flat => flat.Id, flat => flat);
			var latestFlatsDict = latestFlats.ToDictionary(flat => flat.Id, flat => flat);
			foreach (var flatWithId in latestFlatsDict)
			{
				if (!oldFlatsDict.TryGetValue(flatWithId.Key, out var oldFlatState))
				{
					distincts.Add(new FlatsDistinct(flatWithId.Value));
					continue;
				}
				if (!oldFlatState.IsSame(flatWithId.Value))
					throw new DataException($"Old flat {oldFlatState} is not same as {flatWithId.Value}");
				if (oldFlatState.Price != flatWithId.Value.Price || oldFlatState.CurrentState != flatWithId.Value.CurrentState)
					distincts.Add(new FlatsDistinct(flatWithId.Value, oldFlatState));
			}

			var lostIds = oldFlatsDict.Keys.Except(latestFlatsDict.Keys).ToArray();
			distincts.AddRange(lostIds.Select(lostId => new FlatsDistinct(previousState: oldFlatsDict[lostId])));

			return distincts;
		}
    }
}