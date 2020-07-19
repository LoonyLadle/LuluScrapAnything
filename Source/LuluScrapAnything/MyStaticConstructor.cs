using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace LoonyLadle.ScrapAnything
{
	[StaticConstructorOnStartup]
	public static class MyStaticConstructor
	{
		static MyStaticConstructor()
		{
			IEnumerable<ThingDef> workTables = DefDatabase<ThingDef>.AllDefs.Where(t => t.IsWorkTable);
			//Log.Message($"LuluScrapAnything: recipeUsers contains {workTables.Count()} entries: {workTables.ToStringSafeEnumerable()}.");

			foreach (ThingDef workTable in workTables)
			{
				IEnumerable<RecipeDef> tableRecipes = workTable.AllRecipes.Where(r => r.ProducedThingDef?.HasSmeltProducts() ?? false);
				// If the table has no relevant recipes, no point doing anything else for it.
				if (!tableRecipes.Any()) continue;

				ThingFilter newFilter = new ThingFilter();
				//newFilter.thingDefs = tableProducts;
				typeof(ThingFilter).GetField("thingDefs", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(newFilter, tableRecipes.Select(r => r.ProducedThingDef).ToList());

				IngredientCount newCount = new IngredientCount();
				newCount.SetBaseCount(1);
				newCount.filter = newFilter;

				RecipeDef generatedRecipe = new RecipeDef
				{
					defName = "LuluScrapAnything_DisassembleAt" + workTable.defName,
					label = "disassemble",
					description = "Disassemble things made at this workbench to reclaim some of the resources used.",
					jobString = "Disassembling at " + workTable.label + ".",
					workAmount = 1600,
					workSpeedStat = MyDefOf.SmeltingSpeed,
					effectWorking = tableRecipes.First().effectWorking,
					soundWorking = tableRecipes.First().soundWorking,
					specialProducts = new List<SpecialProductType> { SpecialProductType.Smelted },
					recipeUsers = new List<ThingDef> { workTable },
					ingredients = new List<IngredientCount> { newCount },
					fixedIngredientFilter = newFilter,
				};
				generatedRecipe.ResolveReferences();
				DefDatabase<RecipeDef>.Add(generatedRecipe);
				// Clear the recipe cache because we've added a new one.
				typeof(ThingDef).GetField("allRecipesCached", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(workTable, null);
				//Log.Message($"LuluScrapAnything: recipes for table {workTable.defName} contains {tableRecipes.Count()} entries: {tableRecipes.ToStringSafeEnumerable()}.");
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
