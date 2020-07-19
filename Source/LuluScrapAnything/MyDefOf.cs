using RimWorld;
using Verse;

namespace LoonyLadle.ScrapAnything
{
	[DefOf]
	public static class MyDefOf
	{
		static MyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));
		public static StatDef SmeltingSpeed;
	}
}
