using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json;
using PokeWarpEventRandomizer.Warps;

namespace PokeWarpEventRandomizer
{
    internal static class RandomizerHelper
    {
        public static bool GetIsChecked(this CheckBox cb)
        {
            return cb != null && cb.IsChecked.HasValue && cb.IsChecked.Value;
        }
        public static bool GetIsChecked(this RadioButton rb)
        {
            return rb != null && rb.IsChecked.HasValue && rb.IsChecked.Value;
        }

        public static bool RandomizeWarps(List<Warp> warpMap, Patches patches, Random rng, string folderName)
        {
            applyPrePatch(warpMap, patches);

            List<Warp> rngWarps = new(warpMap.Where(p => !p.IsLocked));
            Dictionary<string, string> doneWarpsCheck = new();
            List<Warp> doneWarps = new();

            for (int i = 0; i < rngWarps.Count; i++)
            {
                // check if warp is already connected
                if (doneWarpsCheck.ContainsKey(rngWarps[i].GlobalWarpId)) continue;
                // loop for additional checks at the end
                bool isOK = false;
                while (!isOK)
                {
                    // randomly choose a target
                    int rngTarget = rngWarps.chooseTarget(doneWarpsCheck, rng, i);

                    // create new warp
                    Warp targetRandom = rngWarps[rngTarget];
                    Warp newBaseWarp = new()
                    {
                        GlobalWarpId = rngWarps[i].GlobalWarpId,
                        MapWarpId = rngWarps[i].MapWarpId,
                        MapId = rngWarps[i].MapId,
                        LocationX = rngWarps[i].LocationX,
                        LocationY = rngWarps[i].LocationY,
                        LocationElevation = rngWarps[i].LocationElevation,
                        DestinationMap = targetRandom.MapId,
                        DestinationWarpId = targetRandom.MapWarpId,
                        IsLocked = rngWarps[i].IsLocked,
                        IsTarget = rngWarps[i].IsTarget,
                        TargetWarpGlobalId = targetRandom.GlobalWarpId
                    };
                    foreach (string id in rngWarps[i].NeighborWarpsGlobalId)
                    {
                        newBaseWarp.NeighborWarpsGlobalId.Add(id);
                    }
                    // create neighbor warp(s)
                    List<Warp> sameTarget = new(rngWarps.Where(p
                                => (rngWarps[i].NeighborWarpsGlobalId.Count >= 1
                                    && p.GlobalWarpId == rngWarps[i].NeighborWarpsGlobalId[0])
                                || (rngWarps[i].NeighborWarpsGlobalId.Count == 2
                                    && p.GlobalWarpId == rngWarps[i].NeighborWarpsGlobalId[1])));
                    List<Warp> newNeighborWarps = new();
                    foreach (Warp item in sameTarget)
                    {
                        newNeighborWarps.Add(new Warp()
                        {
                            GlobalWarpId = item.GlobalWarpId,
                            MapWarpId = item.MapWarpId,
                            MapId = item.MapId,
                            LocationX = item.LocationX,
                            LocationY = item.LocationY,
                            LocationElevation = item.LocationElevation,
                            DestinationMap = targetRandom.MapId,
                            DestinationWarpId = targetRandom.MapWarpId,
                            IsLocked = item.IsLocked,
                            IsTarget = item.IsTarget,
                            TargetWarpGlobalId = targetRandom.GlobalWarpId
                        });
                    }
                    // search target from new base warps
                    string targetMap = "";
                    int targetWarpId = 0;
                    string targetGlobalId = "";
                    if (newBaseWarp.IsTarget)
                    {
                        targetMap = newBaseWarp.MapId;
                        targetWarpId = newBaseWarp.MapWarpId;
                        targetGlobalId = newBaseWarp.GlobalWarpId;
                    }
                    if (string.IsNullOrEmpty(targetMap))
                    {
                        foreach (Warp item in newNeighborWarps)
                        {
                            if (string.IsNullOrEmpty(targetMap))
                            {
                                if (item.IsTarget)
                                {
                                    targetMap = item.MapId;
                                    targetWarpId = item.MapWarpId;
                                    targetGlobalId = item.GlobalWarpId;
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(targetMap))
                    {
                        targetMap = newBaseWarp.MapId;
                        targetWarpId = newBaseWarp.MapWarpId;
                        targetGlobalId = newBaseWarp.GlobalWarpId;
                    }

                    // create warp from target to original "isTarget"
                    Warp newTargetWarp = new()
                    {
                        GlobalWarpId = targetRandom.GlobalWarpId,
                        MapWarpId = targetRandom.MapWarpId,
                        MapId = targetRandom.MapId,
                        LocationX = targetRandom.LocationX,
                        LocationY = targetRandom.LocationY,
                        LocationElevation = targetRandom.LocationElevation,
                        DestinationMap = targetMap,
                        DestinationWarpId = targetWarpId,
                        IsLocked = targetRandom.IsLocked,
                        IsTarget = targetRandom.IsTarget,
                        TargetWarpGlobalId = targetGlobalId
                    };
                    foreach (string id in targetRandom.NeighborWarpsGlobalId)
                    {
                        newTargetWarp.NeighborWarpsGlobalId.Add(id);
                    }
                    // create neighbor warp(s)
                    List<Warp> sameTargetTarget = new(rngWarps.Where(p
                                => (newTargetWarp.NeighborWarpsGlobalId.Count >= 1
                                    && p.GlobalWarpId == newTargetWarp.NeighborWarpsGlobalId[0])
                                || (newTargetWarp.NeighborWarpsGlobalId.Count == 2
                                    && p.GlobalWarpId == newTargetWarp.NeighborWarpsGlobalId[1])));
                    List<Warp> newTargetNeighborWarps = new();
                    foreach (Warp item in sameTargetTarget)
                    {
                        newTargetNeighborWarps.Add(new Warp()
                        {
                            GlobalWarpId = item.GlobalWarpId,
                            MapWarpId = item.MapWarpId,
                            MapId = item.MapId,
                            LocationX = item.LocationX,
                            LocationY = item.LocationY,
                            LocationElevation = item.LocationElevation,
                            DestinationMap = targetMap,
                            DestinationWarpId = targetWarpId,
                            IsLocked = item.IsLocked,
                            IsTarget = item.IsTarget,
                            TargetWarpGlobalId = targetGlobalId
                        });
                    }

                    // check again for basically everything...
                    if (!doneWarpsCheck.checkAndAdd(doneWarps, newBaseWarp, newNeighborWarps, newTargetWarp, newTargetNeighborWarps))
                        continue;

                    isOK = true;
                }
            }
            List<Warp> randomizedWarps = new(doneWarps);
            randomizedWarps.AddRange(warpMap.Where(p => p.IsLocked));
            if (patches.RandomizeLavaridge)
            {
                randomizedWarps = randomizeLavaridge(randomizedWarps, rng);
            }
            if (patches.RandomizeMossdeep)
            {
                randomizedWarps = randomizeMossdeep(randomizedWarps, rng);
            }

            Dictionary<string, MapData> randomizedMapData = new();
            string[] mapFolders = Directory.GetDirectories(@".\Resources\data\maps\");
            foreach (string folder in mapFolders)
            {
                string json = File.ReadAllText(folder + @"\map.json");
                MapData data = JsonConvert.DeserializeObject<MapData>(json);
                if (data != null)
                {
                    randomizedMapData.Add(data.id, data);
                }
            }
            Dictionary<string, MapData> newList = new();
            foreach (KeyValuePair<string, MapData> item in randomizedMapData)
            {
                List<Warp> mapWarps = randomizedWarps.Where(p => p.MapId == item.Key).ToList();
                mapWarps = mapWarps.OrderBy(o => o.MapWarpId).ToList();
                for (int i = 0; i < mapWarps.Count; i++)
                {
                    item.Value.warp_events[i].dest_map = mapWarps[i].DestinationMap;
                    item.Value.warp_events[i].dest_warp_id = mapWarps[i].DestinationWarpId;
                }
                newList.Add(item.Key, item.Value);
            }

            if (patches.DisableGymWalk)
            {
                _ = newList.First(p => p.Key == "MAP_PETALBURG_CITY")
                        .Value.coord_events.RemoveAll(match: r => r.script.StartsWith("PetalburgCity_EventScript_ShowGymToPlayer"));
            }

            string dir = @".\Out\" + folderName;
            if (!Directory.Exists(@".\Out"))
                _ = Directory.CreateDirectory(@".\Out");
            _ = Directory.CreateDirectory(dir + @"\data\maps");
            foreach (KeyValuePair<string, MapData> item in newList)
            {
                _ = Directory.CreateDirectory(dir + @"\data\maps\" + item.Value.name);
                File.WriteAllText(dir + @"\data\maps\" + item.Value.name + @"\map.json", JsonConvert.SerializeObject(item.Value));
            }
            cleanupDirectories(dir);
            revertPrePatch(warpMap);
            applyPatches(patches, dir);

            return true;
        }

        public static bool IsNextTo(this Warp warpA, Warp warpB)
        {
            return warpA.MapId == warpB.MapId
                 && ((warpA.LocationX == warpB.LocationX && (warpA.LocationY == warpB.LocationY - 1 || warpA.LocationY == warpB.LocationY + 1))
                 || (warpA.LocationY == warpB.LocationY && (warpA.LocationX == warpB.LocationX - 1 || warpA.LocationX == warpB.LocationX + 1)));
        }
        private static void applyPrePatch(List<Warp> map, Patches patches)
        {
            if (patches.KeepSootopolisLadder)
            {
                map.First(p => p.MapId == "MAP_SOOTOPOLIS_CITY_GYM_1F"
                            && p.DestinationMap == "MAP_SOOTOPOLIS_CITY_GYM_B1F"
                            && p.DestinationWarpId == 0).IsLocked = true;
                map.First(p => p.MapId == "MAP_SOOTOPOLIS_CITY_GYM_B1F"
                            && p.DestinationMap == "MAP_SOOTOPOLIS_CITY_GYM_1F"
                            && p.DestinationWarpId == 2).IsLocked = true;
            }
        }
        private static void revertPrePatch(List<Warp> map)
        {
            map.First(p => p.MapId == "MAP_SOOTOPOLIS_CITY_GYM_1F"
                        && p.DestinationMap == "MAP_SOOTOPOLIS_CITY_GYM_B1F"
                        && p.DestinationWarpId == 0).IsLocked = false;
            map.First(p => p.MapId == "MAP_SOOTOPOLIS_CITY_GYM_B1F"
                        && p.DestinationMap == "MAP_SOOTOPOLIS_CITY_GYM_1F"
                        && p.DestinationWarpId == 2).IsLocked = false;
        }
        private static void applyPatches(Patches patches, string dir)
        {
            if (patches.EnableTeleport)
            {
                _ = Directory.CreateDirectory(dir + @"\src");
                File.Copy(@".\Resources\src\overworld.c", dir + @"\src\overworld.c");
            }
            if (patches.DisableEliteWalking)
            {
                File.Copy(@".\Resources\data\maps\EverGrandeCity_SidneysRoom\scripts.inc",
                          dir + @"\data\maps\EverGrandeCity_SidneysRoom\scripts.inc");
                File.Copy(@".\Resources\data\maps\EverGrandeCity_PhoebesRoom\scripts.inc",
                          dir + @"\data\maps\EverGrandeCity_PhoebesRoom\scripts.inc");
                File.Copy(@".\Resources\data\maps\EverGrandeCity_GlaciasRoom\scripts.inc",
                          dir + @"\data\maps\EverGrandeCity_GlaciasRoom\scripts.inc");
                File.Copy(@".\Resources\data\maps\EverGrandeCity_DrakesRoom\scripts.inc",
                          dir + @"\data\maps\EverGrandeCity_DrakesRoom\scripts.inc");
            }
            if (patches.ReceiveWaterfall)
            {
                File.Copy(@".\Resources\data\maps\SootopolisCity_Gym_1F\scripts.inc",
                          dir + @"\data\maps\SootopolisCity_Gym_1F\scripts.inc");
            }
            if (patches.ReceiveAllHM)
            {
                File.Copy(@".\Resources\data\maps\LittlerootTown_ProfessorBirchsLab\scripts.inc",
                          dir + @"\data\maps\LittlerootTown_ProfessorBirchsLab\scripts.inc");
                _ = Directory.CreateDirectory(dir + @"\data\scripts\");
                File.Copy(@".\Resources\data\scripts\field_move_scripts.inc",
                          dir + @"\data\scripts\field_move_scripts.inc");
                File.Copy(@".\Resources\src\field_control_avatar.c",
                          dir + @"\src\field_control_avatar.c");
            }

            if (patches.DisablePetalburg)
            {
                File.Copy(@".\Resources\data\maps\PetalburgCity_Gym\scripts.inc",
                          dir + @"\data\maps\PetalburgCity_Gym\scripts.inc");
            }
            if (patches.RemoveDevonGuard)
            {
                File.Copy(@".\Resources\data\maps\RustboroCity_DevonCorp_1F\scripts.inc",
                          dir + @"\data\maps\RustboroCity_DevonCorp_1F\scripts.inc");
            }
            if (patches.ConnectSootopolis)
            {
                if (!Directory.Exists(dir + @"\data\layouts\SootopolisCity"))
                    _ = Directory.CreateDirectory(dir + @"\data\layouts\SootopolisCity");
                File.Copy(@".\Resources\data\layouts\SootopolisCity\border.bin",
                          dir + @"\data\layouts\SootopolisCity\border.bin");
                File.Copy(@".\Resources\data\layouts\SootopolisCity\map.bin",
                          dir + @"\data\layouts\SootopolisCity\map.bin");
            }
        }
        private static int chooseTarget(this List<Warp> warps, Dictionary<string, string> doneWarps, Random rng, int index)
        {
            bool OK = false;
            int rngTarget = -1;
            while (!OK)
            {
                rngTarget = rng.Next(index + 1, warps.Count);
                // check if target already exists
                OK = !doneWarps.ContainsKey(warps[index].GlobalWarpId);
                // don't connect neighboring warps
                OK = OK && !warps[index].IsNextTo(warps[rngTarget]);
                // check if neighboring warps are already in the list
                if (warps[index].HasNeighbor && OK)
                {
                    OK = OK && !doneWarps.ContainsKey(warps[index].NeighborWarpsGlobalId[0]);
                    if (warps[index].NeighborWarpsGlobalId.Count == 2)
                    {
                        OK = OK && !doneWarps.ContainsKey(warps[index].NeighborWarpsGlobalId[1]);
                    }
                }
                // check if neighboring warp targets are already in the list
                OK = OK && !doneWarps.ContainsKey(warps[rngTarget].GlobalWarpId);
                if (warps[rngTarget].HasNeighbor && OK)
                {
                    OK = OK && !doneWarps.ContainsKey(warps[rngTarget].NeighborWarpsGlobalId[0]);
                    if (warps[rngTarget].NeighborWarpsGlobalId.Count == 2)
                    {
                        OK = OK && !doneWarps.ContainsKey(warps[rngTarget].NeighborWarpsGlobalId[1]);
                    }
                }
            }
            return rngTarget;
        }
        private static bool checkAndAdd(this Dictionary<string, string> doneWarpsCheck, List<Warp> doneWarps,
                                        Warp newBaseWarp, List<Warp> newNeighborWarps,
                                        Warp newTargetWarp, List<Warp> newTargetNeighborWarps)
        {
            if (doneWarpsCheck.ContainsKey(newBaseWarp.GlobalWarpId) || string.IsNullOrEmpty(newBaseWarp.DestinationMap))
                return false;
            foreach (Warp item in newNeighborWarps)
            {
                if (doneWarpsCheck.ContainsKey(item.GlobalWarpId) || string.IsNullOrEmpty(item.DestinationMap))
                    return false;
            }
            if (doneWarpsCheck.ContainsKey(newTargetWarp.GlobalWarpId) || string.IsNullOrEmpty(newTargetWarp.DestinationMap))
                return false;
            foreach (Warp item in newTargetNeighborWarps)
            {
                if (doneWarpsCheck.ContainsKey(item.GlobalWarpId) || string.IsNullOrEmpty(item.DestinationMap))
                    return false;
            }
            // add stuff to lists
            doneWarpsCheck.Add(newBaseWarp.GlobalWarpId, newBaseWarp.TargetWarpGlobalId);
            doneWarps.Add(newBaseWarp);
            foreach (Warp item in newNeighborWarps)
            {
                doneWarpsCheck.Add(item.GlobalWarpId, item.TargetWarpGlobalId);
                doneWarps.Add(item);
            }
            doneWarpsCheck.Add(newTargetWarp.GlobalWarpId, newTargetWarp.TargetWarpGlobalId);
            doneWarps.Add(newTargetWarp);
            foreach (Warp item in newTargetNeighborWarps)
            {
                doneWarpsCheck.Add(item.GlobalWarpId, item.TargetWarpGlobalId);
                doneWarps.Add(item);
            }
            return true;
        }
        private static List<Warp> randomizeLavaridge(List<Warp> warps, Random rng)
        {
            List<Warp> rngWarps = new(warps.Where(p => (p.MapId == "MAP_LAVARIDGE_TOWN_GYM_1F" && p.DestinationMap == "MAP_LAVARIDGE_TOWN_GYM_B1F")
                                                    || (p.MapId == "MAP_LAVARIDGE_TOWN_GYM_B1F" && p.DestinationMap == "MAP_LAVARIDGE_TOWN_GYM_1F")));
            Dictionary<string, string> doneWarpsCheck = new();
            List<Warp> doneWarps = new();
            for (int i = 0; i < rngWarps.Count; i++)
            {
                if (doneWarpsCheck.ContainsKey(rngWarps[i].GlobalWarpId)) continue;
                bool isOK = false;
                while (!isOK)
                {
                    int rngTarget = rngWarps.chooseTarget(doneWarpsCheck, rng, i);
                    Warp targetRandom = rngWarps[rngTarget];
                    Warp newBaseWarp = new()
                    {
                        GlobalWarpId = rngWarps[i].GlobalWarpId,
                        MapWarpId = rngWarps[i].MapWarpId,
                        MapId = rngWarps[i].MapId,
                        LocationX = rngWarps[i].LocationX,
                        LocationY = rngWarps[i].LocationY,
                        LocationElevation = rngWarps[i].LocationElevation,
                        DestinationMap = targetRandom.MapId,
                        DestinationWarpId = targetRandom.MapWarpId,
                        IsLocked = rngWarps[i].IsLocked,
                        IsTarget = rngWarps[i].IsTarget,
                        TargetWarpGlobalId = targetRandom.GlobalWarpId
                    };
                    string targetMap = newBaseWarp.MapId;
                    int targetWarpId = newBaseWarp.MapWarpId;
                    string targetGlobalId = newBaseWarp.GlobalWarpId;
                    Warp newTargetWarp = new()
                    {
                        GlobalWarpId = targetRandom.GlobalWarpId,
                        MapWarpId = targetRandom.MapWarpId,
                        MapId = targetRandom.MapId,
                        LocationX = targetRandom.LocationX,
                        LocationY = targetRandom.LocationY,
                        LocationElevation = targetRandom.LocationElevation,
                        DestinationMap = targetMap,
                        DestinationWarpId = targetWarpId,
                        IsLocked = targetRandom.IsLocked,
                        IsTarget = targetRandom.IsTarget,
                        TargetWarpGlobalId = targetGlobalId
                    };
                    if (!doneWarpsCheck.checkAndAdd(doneWarps, newBaseWarp, new List<Warp>(), newTargetWarp, new List<Warp>()))
                        continue;
                    isOK = true;
                }
            }
            foreach (Warp warp in doneWarps)
            {
                int oldIndex = warps.FindIndex(p => p.GlobalWarpId == warp.GlobalWarpId);
                if (oldIndex == -1)
                {
                    warps.RemoveAt(oldIndex);
                    warps.Add(warp);
                }
            }
            return warps;
        }
        private static List<Warp> randomizeMossdeep(List<Warp> warps, Random rng)
        {
            List<Warp> rngWarps = new(warps.Where(p => p.MapId == "MAP_MOSSDEEP_CITY_GYM" && p.DestinationMap == "MAP_MOSSDEEP_CITY_GYM"));
            Dictionary<string, string> doneWarpsCheck = new();
            List<Warp> doneWarps = new();
            for (int i = 0; i < rngWarps.Count; i++)
            {
                if (doneWarpsCheck.ContainsKey(rngWarps[i].GlobalWarpId)) continue;
                bool isOK = false;
                while (!isOK)
                {
                    int rngTarget = rngWarps.chooseTarget(doneWarpsCheck, rng, i);
                    Warp targetRandom = rngWarps[rngTarget];
                    Warp newBaseWarp = new()
                    {
                        GlobalWarpId = rngWarps[i].GlobalWarpId,
                        MapWarpId = rngWarps[i].MapWarpId,
                        MapId = rngWarps[i].MapId,
                        LocationX = rngWarps[i].LocationX,
                        LocationY = rngWarps[i].LocationY,
                        LocationElevation = rngWarps[i].LocationElevation,
                        DestinationMap = targetRandom.MapId,
                        DestinationWarpId = targetRandom.MapWarpId,
                        IsLocked = rngWarps[i].IsLocked,
                        IsTarget = rngWarps[i].IsTarget,
                        TargetWarpGlobalId = targetRandom.GlobalWarpId
                    };
                    string targetMap = newBaseWarp.MapId;
                    int targetWarpId = newBaseWarp.MapWarpId;
                    string targetGlobalId = newBaseWarp.GlobalWarpId;
                    Warp newTargetWarp = new()
                    {
                        GlobalWarpId = targetRandom.GlobalWarpId,
                        MapWarpId = targetRandom.MapWarpId,
                        MapId = targetRandom.MapId,
                        LocationX = targetRandom.LocationX,
                        LocationY = targetRandom.LocationY,
                        LocationElevation = targetRandom.LocationElevation,
                        DestinationMap = targetMap,
                        DestinationWarpId = targetWarpId,
                        IsLocked = targetRandom.IsLocked,
                        IsTarget = targetRandom.IsTarget,
                        TargetWarpGlobalId = targetGlobalId
                    };
                    if (!doneWarpsCheck.checkAndAdd(doneWarps, newBaseWarp, new List<Warp>(), newTargetWarp, new List<Warp>()))
                        continue;
                    isOK = true;
                }
            }
            foreach (Warp warp in doneWarps)
            {
                int oldIndex = warps.FindIndex(p => p.GlobalWarpId == warp.GlobalWarpId);
                if (oldIndex == -1)
                {
                    warps.RemoveAt(oldIndex);
                    warps.Add(warp);
                }
            }
            return warps;
        }
        private static bool checkCompletion(this List<Warp> fullList, bool keepSootopolisLadder)
        {
            Warp spawnWarp = fullList.First(p => p.MapId == "MAP_LITTLEROOT_TOWN_BRENDANS_HOUSE_2F");
            List<Warp> RustboroGymWarps = fullList.Where(p => p.MapId == "MAP_RUSTBORO_CITY_GYM").ToList();
            List<Warp> DewfordGymWarps = fullList.Where(p => p.MapId == "MAP_DEWFORD_TOWN_GYM").ToList();
            List<Warp> MauvilleGymWarps = fullList.Where(p => p.MapId == "MAP_MAUVILLE_CITY_GYM").ToList();
            List<Warp> LavaridgeGymWarps = fullList.Where(p => p.MapId == "MAP_LAVARIDGE_TOWN_GYM_1F"
                                                            && (p.DestinationMap != "MAP_LAVARIDGE_TOWN_GYM_1F"
                                                            || p.DestinationMap != "MAP_LAVARIDGE_TOWN_GYM_B1F")).ToList();
            List<Warp> PetalburgGymWarps = fullList.Where(p => p.MapId == "MAP_PETALBURG_CITY_GYM"
                                                            && p.DestinationMap != "MAP_PETALBURG_CITY_GYM").ToList();
            List<Warp> FortreeGymWarps = fullList.Where(p => p.MapId == "MAP_FORTREE_CITY_GYM").ToList();
            List<Warp> MossdeepGymWarps = fullList.Where(p => p.MapId == "MAP_MOSSDEEP_CITY_GYM"
                                                           && p.DestinationMap != "MAP_MOSSDEEP_CITY_GYM").ToList();
            List<Warp> SootopolisGymWarps = fullList.Where(p => (!keepSootopolisLadder && p.MapId == "MAP_SOOTOPOLIS_CITY_GYM_1F")
                                                             || (keepSootopolisLadder && p.MapId == "MAP_SOOTOPOLIS_CITY_GYM_1F"
                                                                 && p.DestinationMap != "MAP_SOOTOPOLIS_CITY_GYM_B1F")).ToList();
            List<Warp> SidneyWarps = fullList.Where(p => p.MapId == "MAP_EVER_GRANDE_CITY_SIDNEYS_ROOM").ToList();
            List<Warp> PhoebeWarps = fullList.Where(p => p.MapId == "MAP_EVER_GRANDE_CITY_PHOEBES_ROOM").ToList();
            List<Warp> GlaciaWarps = fullList.Where(p => p.MapId == "MAP_EVER_GRANDE_CITY_GLACIAS_ROOM").ToList();
            List<Warp> DrakeWarps = fullList.Where(p => p.MapId == "MAP_EVER_GRANDE_CITY_DRAKES_ROOM").ToList();
            List<Warp> ChampWarps = fullList.Where(p => p.MapId == "MAP_EVER_GRANDE_CITY_CHAMPIONS_ROOM"
                                                     && p.DestinationMap != "MAP_EVER_GRANDE_CITY_HALL_OF_FAME").ToList();


            return true;
        }
        private static bool checkRoute(this List<Warp> fullList, Warp spawn, List<Warp> targetWarps, bool checkSurf, bool checkWaterfall)
        {



            return true;
        }
        private static void cleanupDirectories(string dir)
        {
            deleteFileDir(dir, "BattlePyramidSquare01");
            deleteFileDir(dir, "BattlePyramidSquare02");
            deleteFileDir(dir, "BattlePyramidSquare03");
            deleteFileDir(dir, "BattlePyramidSquare04");
            deleteFileDir(dir, "BattlePyramidSquare05");
            deleteFileDir(dir, "BattlePyramidSquare06");
            deleteFileDir(dir, "BattlePyramidSquare07");
            deleteFileDir(dir, "BattlePyramidSquare08");
            deleteFileDir(dir, "BattlePyramidSquare09");
            deleteFileDir(dir, "BattlePyramidSquare10");
            deleteFileDir(dir, "BattlePyramidSquare11");
            deleteFileDir(dir, "BattlePyramidSquare12");
            deleteFileDir(dir, "BattlePyramidSquare13");
            deleteFileDir(dir, "BattlePyramidSquare14");
            deleteFileDir(dir, "BattlePyramidSquare15");
            deleteFileDir(dir, "BattlePyramidSquare16");
            deleteFileDir(dir, "ContestHall");
            deleteFileDir(dir, "ContestHallBeauty");
            deleteFileDir(dir, "ContestHallCool");
            deleteFileDir(dir, "ContestHallCute");
            deleteFileDir(dir, "ContestHallSmart");
            deleteFileDir(dir, "ContestHallTough");
            deleteFileDir(dir, "SecretBase_BlueCave1");
            deleteFileDir(dir, "SecretBase_BlueCave2");
            deleteFileDir(dir, "SecretBase_BlueCave3");
            deleteFileDir(dir, "SecretBase_BlueCave4");
            deleteFileDir(dir, "SecretBase_BrownCave1");
            deleteFileDir(dir, "SecretBase_BrownCave2");
            deleteFileDir(dir, "SecretBase_BrownCave3");
            deleteFileDir(dir, "SecretBase_BrownCave4");
            deleteFileDir(dir, "SecretBase_RedCave1");
            deleteFileDir(dir, "SecretBase_RedCave2");
            deleteFileDir(dir, "SecretBase_RedCave3");
            deleteFileDir(dir, "SecretBase_RedCave4");
            deleteFileDir(dir, "SecretBase_Shrub1");
            deleteFileDir(dir, "SecretBase_Shrub2");
            deleteFileDir(dir, "SecretBase_Shrub3");
            deleteFileDir(dir, "SecretBase_Shrub4");
            deleteFileDir(dir, "SecretBase_Tree1");
            deleteFileDir(dir, "SecretBase_Tree2");
            deleteFileDir(dir, "SecretBase_Tree3");
            deleteFileDir(dir, "SecretBase_Tree4");
            deleteFileDir(dir, "SecretBase_YellowCave1");
            deleteFileDir(dir, "SecretBase_YellowCave2");
            deleteFileDir(dir, "SecretBase_YellowCave3");
            deleteFileDir(dir, "SecretBase_YellowCave4");
            deleteFileDir(dir, "UnionRoom");
            deleteFileDir(dir, "UnusedContestHall1");
            deleteFileDir(dir, "UnusedContestHall2");
            deleteFileDir(dir, "UnusedContestHall3");
            deleteFileDir(dir, "UnusedContestHall4");
            deleteFileDir(dir, "UnusedContestHall5");
            deleteFileDir(dir, "UnusedContestHall6");
            deleteFileDir(dir, "LittlerootTown");
            deleteFileDir(dir, "LittlerootTown_BrendansHouse_1F");
            deleteFileDir(dir, "LittlerootTown_BrendansHouse_2F");
            deleteFileDir(dir, "LittlerootTown_MaysHouse_1F");
            deleteFileDir(dir, "LittlerootTown_MaysHouse_2F");
            //deleteFileDir(dir, "LittlerootTown_ProfessorBirchsLab");
            deleteFileDir(dir, "InsideOfTruck");
        }
        private static void deleteFileDir(string dir, string mapName)
        {
            if (File.Exists(dir + @"\data\maps\" + mapName + @"\map.json"))
                File.Delete(dir + @"\data\maps\" + mapName + @"\map.json");
            if (File.Exists(dir + @"\data\maps\" + mapName + @"\scripts.inc"))
                File.Delete(dir + @"\data\maps\" + mapName + @"\scripts.inc");
            if (Directory.Exists(dir + @"\data\maps\" + mapName))
                Directory.Delete(dir + @"\data\maps\" + mapName);
        }
    }
}
