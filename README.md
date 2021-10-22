# Pokémon Emerald Warp Randomizer

This is a randomizer for nearly all warps in Pokémon Emerald, written in C#.

To play with your own Seeds, you'll need to build the game yourself ([Take a look at the original decomp project for more infos on how to build pokeemerald.gba](https://github.com/pret/pokeemerald)), but I will provide a few .ips-Patches for everyone to use in the releases.

## Available Options
### Game Options
These are to fix up certain aspects of the game to make it playable and avoid hardlocks.
 - Enable Teleport/Fly everywhere (as the name implies, this enables the field moves Teleport and Fly on every map)
 - Disable auto-walking in elite four rooms (this avoids you walking into the void, when entering an elite four room)
 - Receive HM Waterfall after defeating Juan 
 - Receive all HMs after receiving the Pokedex (This also enables you to use all HMs from the very beginning)

### Gym Options
These change scripts or warps in gyms for either more challenge or to fix some bugs.
 - Randomize interior of Lavaridge Gym
 - Disable Petalburg Gym Events (This disables all events, so you can just battle your dad on first entry)
 - Randomize interior of Mossdeep Gym
 - Don't randomize ladder in Sootopolis Gym

### Event/Map Options (aka: Easy Mode)
These just make the game easier by disabling certain scripts.
 - Remove stair guard in Devon Corp 1F (Just to avoid a possible softlock)
 - Disable walk to Gym in Petalburg (Disables the tutorial boy in Petalburg for some extra warps)
 - Connect Sootopolis City (Adds some tiles in Sootopolis City, so you can just walk from the left to the right part)

