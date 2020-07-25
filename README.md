# Lulu's Scrap Anything
Break down (almost) any player-craftable object for resources at the same workbench used to create it.

**For best results, this mod should be loaded after any other mod that adds recipes**.

## Gameplay Notes
This mod adds a new recipe called "Disassemble" to every work table. This recipe works similarity to the "Smelt Weapon/Apparel" recipe on the electric smelter in that it returns 25% of the object's resources and destroys intricate items such as components.

Objects that are rottable or which have rottable ingredients are excepted from being disassembled. Additionally, drugs and neutroamine have been flagged as intricate.

## Technical Details
This mod uses dynamic code to create a new recipe for every work table in the game, and populates its ingredient list using the output of `workTable.AllRecipes`. As a result, this mod should be compatible with items and work tables added by any other mod.
