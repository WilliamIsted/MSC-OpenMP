using System.IO;

using MSCLoader;
using HutongGames.PlayMaker.Actions;

namespace OpenMP
{
    public class OpenMP : Mod
    {

        public override string ID => "OpenMP";
        public override string Name => "OpenMP";
        public override string Version => "0.0.1";
        public override string Author => "WilliamIsted";
        //public override byte[] Icon => null;
        public override string Description => "Multiplayer Mod for My Summer Car";

        public static string assetsFolder { get; private set; }

        public override void ModSetup()
        {
            assetsFolder = ModLoader.GetModAssetsFolder(this);

            SetupFunction(Setup.OnNewGame, DoOnNewGame);
            SetupFunction(Setup.OnMenuLoad, DoOnMenuLoad);
            SetupFunction(Setup.PreLoad, DoPreLoad);
            SetupFunction(Setup.OnLoad, DoOnLoad);
            SetupFunction(Setup.PostLoad, DoPostLoad);
            SetupFunction(Setup.OnSave, DoOnSave);
            SetupFunction(Setup.OnGUI, DoOnGUI);
            SetupFunction(Setup.Update, DoUpdate);
            SetupFunction(Setup.FixedUpdate, DoFixedUpdate);
            SetupFunction(Setup.OnModEnabled, DoOnModEnabled);
            SetupFunction(Setup.OnModDisabled, DoOnModDisabled);
            SetupFunction(Setup.ModSettingsLoaded, DoModSettingsLoaded);
            SetupFunction(Setup.ModSettings, DoModSettings);
        }

        /*
         * 
         * 
         * 
         */

        private void DoOnNewGame()
        {
            ModConsole.Print("DoOnNewGame");
        }

        private void DoOnMenuLoad()
        {
            ModConsole.Print("DoOnMenuLoad");
        }

        private void DoPreLoad()
        {
            ModConsole.Print("DoPreLoad");
        }

        private void DoOnLoad()
        {
            ModConsole.Print("DoOnLoad");

            // Apply all patches using Harmony
            var harmony = Harmony.HarmonyInstance.Create("com.mscti.patches");
            harmony.PatchAll();
        }

        private void DoPostLoad()
        {
            ModConsole.Print("DoPostLoad");
        }

        private void DoOnSave()
        {
            ModConsole.Print("DoOnSave");
        }

        private void DoOnGUI()
        {
            ModConsole.Print("DoOnGUI");
        }

        private void DoUpdate()
        {

        }

        private void DoFixedUpdate()
        {

        }

        private void DoOnModEnabled()
        {
            ModConsole.Print("DoOnModEnabled");
        }

        private void DoOnModDisabled()
        {
            ModConsole.Print("DoOnModDisabled");
        }

        private void DoModSettingsLoaded()
        {
            ModConsole.Print("DoModSettingsLoaded");
        }

        private void DoModSettings()
        {
            ModConsole.Print("DoModSettings");
        }

        /*
         * 
         * 
         * 
         */

        

    }
}
