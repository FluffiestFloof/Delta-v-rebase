using System.IO;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed class DumpReagentAll : IConsoleCommand
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntitySystemManager _entSys = default!;

    public string Command => "dumpreagents";
    public string Description => "Dumps the guidebook text for all reagent to the console";
    public string Help => "dumpreagents";

    public string Path = "E:/SS14 Codebases/allreagenteffects.txt";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 0)
        {
            shell.WriteError("Must have no argument");
            return;
        }

        // Create file if it doesn't exist
        if (!File.Exists(Path))
        {
            // Create a file to write to.
            File.CreateText(Path);
        }
        // Automatically wipes the file
        File.WriteAllText(Path, "FILE START\n");

        var prototypes = _prototype.EnumeratePrototypes<ReagentPrototype>();
        using (StreamWriter sw = File.AppendText(Path))
        {
            foreach (var rea in prototypes)
            {
                sw.WriteLine("START REAGENT");
                sw.WriteLine(rea.LocalizedName);
                if (rea.Metabolisms is null)
                {
                    sw.WriteLine("None");
                }
                else
                {
                    foreach (var (_, entry) in rea.Metabolisms)
                    {
                        foreach (var effect in entry.Effects)
                        {
                            sw.WriteLine(effect.GuidebookEffectDescription(_prototype, _entSys) ?? $"[skipped effect of type {effect.GetType()}]");
                        }
                    }
                }
                sw.WriteLine("END REAGENT");
            }
        }
        using (StreamWriter sw = File.AppendText(Path))
        {
            sw.WriteLine("FILE END");
        }
    }
}
