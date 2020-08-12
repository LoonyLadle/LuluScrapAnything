using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace LoonyLadle.ScrapAnything
{
	[StaticConstructorOnStartup]
	public static class MyStaticConstructor
	{
		static MyStaticConstructor()
		{
			IEnumerable<ThingDef> workTables = DefDatabase<ThingDef>.AllDefs.Where(t => t.IsWorkTable);
			FieldInfo thingDefs = typeof(ThingFilter).GetField("thingDefs", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo allRecipesCached = typeof(ThingDef).GetField("allRecipesCached", BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (ThingDef workTable in workTables)
			{
				IEnumerable<RecipeDef> tableRecipes = workTable.AllRecipes.Where(r => r.ProducedThingDef?.HasSmeltProducts() ?? false);
				// If the table has no relevant recipes, no point doing anything else for it.
				if (!tableRecipes.Any()) continue;

				ThingFilter newFilter = new ThingFilter();
				thingDefs.SetValue(newFilter, tableRecipes.Select(r => r.ProducedThingDef).ToList());

				IngredientCount newCount = new IngredientCount();
				newCount.SetBaseCount(1);
				newCount.filter = newFilter;

				RecipeDef generatedRecipe = new RecipeDef
				{
					defName = "LuluScrapAnything_DisassembleAt" + workTable.defName,
					label = "LuluScrapAnything_BillLabel".Translate(),
					description = "LuluScrapAnything_BillDesc".Translate(),
					jobString = "LuluScrapAnything_BillJob".Translate(workTable.label),
					workAmount = 1600,
					workSpeedStat = MyDefOf.SmeltingSpeed,
					effectWorking = tableRecipes.GroupBy(r => r.effectWorking).OrderByDescending(g => g.Count()).Select(o => o.Key).First(),
					soundWorking = tableRecipes.GroupBy(r => r.soundWorking).OrderByDescending(g => g.Count()).Select(o => o.Key).First(),
					specialProducts = new List<SpecialProductType> { SpecialProductType.Smelted },
					recipeUsers = new List<ThingDef> { workTable },
					ingredients = new List<IngredientCount> { newCount },
					fixedIngredientFilter = newFilter,
					forceHiddenSpecialFilters = new List<SpecialThingFilterDef>
					{
						MyDefOf.AllowBurnableApparel,
						MyDefOf.AllowBurnableWeapons,
						MyDefOf.AllowNonBurnableApparel,
						MyDefOf.AllowNonBurnableWeapons,
						MyDefOf.AllowNonSmeltableApparel,
						MyDefOf.AllowNonSmeltableWeapons,
						MyDefOf.AllowSmeltable,
						MyDefOf.AllowSmeltableApparel,
					}
				};
				generatedRecipe.ResolveReferences();
				DefDatabase<RecipeDef>.Add(generatedRecipe);
				// Clear the recipe cache because we've added a new recipe.
				allRecipesCached.SetValue(workTable, null);
			}
		}

		private static bool HasSmeltProducts(this ThingDef def)
		{
			if (def.HasComp(typeof(CompRottable)) || (def.costList?.Any(c => c.thingDef.HasComp(typeof(CompRottable))) ?? false))
			{
				return false;
			}
			else if (def.MadeFromStuff)
			{
				return true;
			}
			else if (def.smeltProducts?.Any() ?? false)
			{
				return true;
			}
			else if (def.costList?.Select(c => c.thingDef).Where(t => !t.intricate).Any() ?? false)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
