using RimWorld;
using Verse;

namespace LoonyLadle.ScrapAnything
{
	[DefOf]
	public static class MyDefOf
	{
		static MyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));
		public static StatDef SmeltingSpeed;
		public static SpecialThingFilterDef AllowSmeltable;
		public static SpecialThingFilterDef AllowBurnableWeapons;
		public static SpecialThingFilterDef AllowNonBurnableWeapons;
		public static SpecialThingFilterDef AllowNonSmeltableWeapons;
		public static SpecialThingFilterDef AllowSmeltableApparel;
		public static SpecialThingFilterDef AllowBurnableApparel;
		public static SpecialThingFilterDef AllowNonBurnableApparel;
		public static SpecialThingFilterDef AllowNonSmeltableApparel;
	}
}
