using MSCLoader;

namespace MscOpenMp.Mod
{
    public static class ModInfo { public const string ClientVersion = "0.1.0"; }

    public class MscOpenMpMod : MSCLoader.Mod
    {
        public override string ID => "MscOpenMp";
        public override string Name => "MSC OpenMP";
        public override string Author => "WilliamIsted";
        public override string Version => ModInfo.ClientVersion;

        Ui.ConnectChatUi _ui;

        // NOTE: MSCLoader.Mod.OnLoad/Update/OnGUI are `internal virtual` in this
        // MSCLoader version (verified via ilspycmd against libs/MSCLoader.dll,
        // no InternalsVisibleTo to mod assemblies) so they cannot be overridden
        // from here. Lifecycle callbacks are registered instead via
        // SetupFunction() inside ModSetup(), which is the supported public entry
        // point for this API surface.
        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, OnLoad);
            SetupFunction(Setup.Update, OnUpdate);
            SetupFunction(Setup.OnGUI, OnDrawGui);
        }

        void OnLoad() { _ui = new Ui.ConnectChatUi(); }
        void OnUpdate() { if (_ui != null) _ui.Pump(); }
        void OnDrawGui() { if (_ui != null) _ui.Draw(); }
    }
}
