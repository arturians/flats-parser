using JetBrains.Annotations;

namespace FlatsParser
{
	public class FlatsDistinct
	{
		public FlatsDistinct([CanBeNull] Flat latestState = null, [CanBeNull] Flat previousState = null)
		{
			LatestState = latestState;
			PreviousState = previousState;
		}

		[CanBeNull]
		public Flat LatestState { get; }
		[CanBeNull]
		public Flat PreviousState { get; }
	}
}