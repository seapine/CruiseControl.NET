using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public interface IQuietPeriod
	{
		Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to);
	}

	public class QuietPeriod : IQuietPeriod
	{
		public const int TurnOffQuietPeriod = 0;
		public double ModificationDelaySeconds = TurnOffQuietPeriod;
		private readonly DateTimeProvider dtProvider;

		public QuietPeriod(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

		public Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications = GetMods(sourceControl, from, to);
			DateTime nextBuildTime = to.StartTime;
			while (ModificationsAreDetectedInQuietPeriod(modifications, nextBuildTime))
			{
				double secondsUntilNextBuild = ModificationDelaySeconds - SecondsSinceLastBuild(modifications, nextBuildTime);
				nextBuildTime = nextBuildTime.AddSeconds(secondsUntilNextBuild);

				Log.Info("Modifications have been detected in the quiet delay; waiting until " + nextBuildTime);
				dtProvider.Sleep((int) (secondsUntilNextBuild*1000));
				to.StartTime = nextBuildTime;

				// TODO: need to increment the start time for to to include the modification delay!!
				modifications = GetMods(sourceControl, from, to);
			}
			return modifications;
		}

		private Modification[] GetMods(ISourceControl sc, IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications = sc.GetModifications(from, to);
			if (modifications == null) modifications = new Modification[0];
			return modifications;
		}

		private bool ModificationsAreDetectedInQuietPeriod(Modification[] modifications, DateTime to)
		{
			return SecondsSinceLastBuild(modifications, to) < ModificationDelaySeconds;
//			return ModificationDelaySeconds != TurnOffQuietPeriod && SecondsSinceLastBuild(modifications, to) < ModificationDelaySeconds;
		}

		private double SecondsSinceLastBuild(Modification[] modifications, DateTime to)
		{
			return (to - GetMostRecentModificationDate(modifications)).TotalSeconds;
		}

		private DateTime GetMostRecentModificationDate(Modification[] modifications)
		{
			DateTime maxDate = DateTime.MinValue;
			foreach (Modification mod in modifications)
			{
				maxDate = DateUtil.MaxDate(mod.ModifiedTime, maxDate);
			}
			return maxDate;
		}
	}
}