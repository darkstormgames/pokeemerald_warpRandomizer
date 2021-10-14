using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Newtonsoft.Json;
using PokeWarpEventRandomizer.MapObjects;
using PokeWarpEventRandomizer.Warps;

namespace PokeWarpEventRandomizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        List<Warp> warpMap = new List<Warp>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string mapJson = File.ReadAllText(@".\Resources\warpMap.json");
            warpMap = JsonConvert.DeserializeObject<List<Warp>>(mapJson);
        }

        private void btnTitle_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem
            {
                Header = "Rebase 'warpMap.json'",
                IsCheckable = false,
                Command = new Command(
                delegate { Menu_Rebase_Click(this, new RoutedEventArgs()); }, p => true)
            };
            menu.Items.Add(menuItem);

            menu.Items.Add(new Separator());

            menuItem = new MenuItem
            {
                Header = "Exit",
                IsCheckable = false,
                Command = new Command(delegate { btnClose_Click(this, new RoutedEventArgs()); }, p => true)
            };
            menu.Items.Add(menuItem);

            menu.IsOpen = true;
        }

        #region Close/Minimize/Maximize/Restore window
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
        #endregion

        private void Menu_Rebase_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Creating a new 'warpMap.json' will unlock ALL warps for randomization.\nAfter this, warps need to be locked in file manually afterwards to prevent errors.\n\nAre you sure, you want to continue?", "Rebase Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;

            Dictionary<string, MapData> baseMapData = new Dictionary<string, MapData>();
            string[] mapFolders = Directory.GetDirectories(@".\Resources\data\maps\");
            foreach (string folder in mapFolders)
            {
                string json = File.ReadAllText(folder + @"\map.json");
                MapData data = JsonConvert.DeserializeObject<MapData>(json);
                if (data != null)
                {
                    baseMapData.Add(data.id, data);
                }
            }
            foreach (MapData map in baseMapData.Values)
            {
                if (map.warp_events != null && map.warp_events.Count > 0)
                {
                    for (int i = 0; i < map.warp_events.Count; i++)
                    {
                        Warp newWarp = new Warp()
                        {
                            MapWarpId = i,
                            MapId = map.id,
                            LocationX = map.warp_events[i].x,
                            LocationY = map.warp_events[i].y,
                            LocationElevation = map.warp_events[i].elevation,
                            DestinationMap = map.warp_events[i].dest_map,
                            DestinationWarpId = map.warp_events[i].dest_warp_id
                        };
                        warpMap.Add(newWarp);
                    }
                }
            }

            foreach (Warp warp in warpMap)
            {
                warp.SetFullList(warpMap);

                Warp targetWarp = warpMap.FirstOrDefault(p => p.MapId == warp.DestinationMap
                                                            && p.MapWarpId == warp.DestinationWarpId);
                if (targetWarp != null)
                {
                    warp.TargetWarpGlobalId = targetWarp.GlobalWarpId;
                    warp.IsTarget = warp.TargetWarp.DestinationMap == warp.MapId
                                  && warp.TargetWarp.DestinationWarpId == warp.MapWarpId;
                }

                List<Warp> sameMap = warpMap.Where(p => p.MapId == warp.MapId
                                                      && p.GlobalWarpId != warp.GlobalWarpId).ToList();
                foreach (Warp newWarp in sameMap)
                {
                    if (isNextTo(newWarp, warp))
                    {
                        warp.NeighborWarpsGlobalId.Add(newWarp.GlobalWarpId);
                    }
                }
            }

            File.WriteAllText(@".\Resources\warpMap.json", JsonConvert.SerializeObject(warpMap));
        }
        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Randomize_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Patches patches = new Patches()
            {
                EnableTeleport = cbTeleport.GetIsChecked(),
                DisableEliteWalking = cbEliteWalk.GetIsChecked(),
                ReceiveWaterfall = rbWaterfall.GetIsChecked(),
                ReceiveAllHM = rbAll.GetIsChecked(),

                RandomizeLavaridge = cbGymLava.GetIsChecked(),
                DisablePetalburg = cbGymPetal.GetIsChecked(),
                RandomizeMossdeep = cbGymMoss.GetIsChecked(),
                KeepSootopolisLadder = cbGymSootop.GetIsChecked(),

                RemoveDevonGuard = cbRustboroGuard.GetIsChecked(),
                DisableGymWalk = cbPetalburgBoy.GetIsChecked(),
                ConnectSootopolis = cbSootopolisConnect.GetIsChecked(),
            };

            List<Warp> rngWarps = new List<Warp>(warpMap.Where(p => !p.IsLocked));
            string seedText = txtSeed.Text;
            int seed = 0;
            foreach (char c in seedText)
                seed += (byte)c;
            Random rng;
            if (seed == 0)
            {
                seed = new Random().Next(10000, 100000);
                rng = new Random(seed);
                seedText = "rng";
            }
            else
                rng = new Random(seed);

            RandomizerHelper.RandomizeWarps(warpMap, patches, rng, seed + "_" + seedText);

            Cursor = Cursors.Arrow;

            MessageBox.Show("Success!");
        }

        private bool isNextTo(Warp warpA, Warp warpB)
        {
            if (warpA.MapId == warpB.MapId 
                 && ((warpA.LocationX == warpB.LocationX && (warpA.LocationY == warpB.LocationY - 1 || warpA.LocationY == warpB.LocationY + 1))
                 || (warpA.LocationY == warpB.LocationY && (warpA.LocationX == warpB.LocationX - 1 || warpA.LocationX == warpB.LocationX + 1))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void txtSeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtSeed.IsFocused)
            {
                Cursor = Cursors.Wait;
                this.Randomize_Click(sender, new RoutedEventArgs());
                Cursor = Cursors.Arrow;
            }
        }
    }
}
