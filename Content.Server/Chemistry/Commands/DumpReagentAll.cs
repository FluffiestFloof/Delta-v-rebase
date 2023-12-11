using System.IO;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using System.Linq;
using Content.Shared.Chemistry.Reaction;

namespace Content.Server.Chemistry.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed class DumpReagentAll : IConsoleCommand
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntitySystemManager _entSys = default!;

    public string Command => "dumpallreagents";
    public string Description => "Dumps the guidebook text for all reagent to the console";
    public string Help => "dumpallreagents";

    public string Path = "E:/SS14 Codebases/gameinfodump.txt";

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
                sw.WriteLine(Loc.GetString("wikidump-reagent-name", ("name", rea.LocalizedName)));
                sw.WriteLine(rea.SubstanceColor.ToHex());
                sw.WriteLine("{0}: {1}", "GROUP", rea.Group ?? $"Other");
                sw.WriteLine("{0}: {1}", "METABOLISMRATE", rea.MetabolismRate ?? $"Other");
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
                            sw.WriteLine(effect.GuidebookEffectDescription(_prototype, _entSys) ?? $"[skipped effect]");
                        }
                    }
                }
                sw.WriteLine("START RECIPE");

                if (!_prototype.TryIndex<ReactionPrototype>(rea.ID, out var reactionPrototype))
                {
                    reactionPrototype = _prototype.EnumeratePrototypes<ReactionPrototype>()
                        .FirstOrDefault(p => p.Products.ContainsKey(rea.ID));
                }
                if (reactionPrototype != null)
                {
                    var reactantsCount = reactionPrototype.Reactants.Count;
                    foreach (var (product, reactant) in reactionPrototype.Reactants)
                    {
                        if (reactant.Catalyst)
                        {
                            sw.WriteLine(Loc.GetString("wikidump-reagent-recipes-reagent-catalyst", ("reagent", _prototype.Index<ReagentPrototype>(product).LocalizedName), ("ratio", reactant.Amount)));
                        }
                        else
                        {
                            sw.WriteLine(Loc.GetString("wikidump-reagent-recipes-reagent", ("reagent", _prototype.Index<ReagentPrototype>(product).LocalizedName), ("ratio", reactant.Amount)));
                        }
                    }

                    if (reactionPrototype.MinimumTemperature > 0.0f)
                    {
                        sw.WriteLine(Loc.GetString("wikidump-reagent-recipes-mix-and-heat", ("temperature", reactionPrototype.MinimumTemperature)));
                    }

                    // var productCount = reactionPrototype.Products.Count;
                    // foreach (var (product, ratio) in reactionPrototype.Products)
                    // {
                    //     sw.WriteLine(Loc.GetString("wikidump-reagent-recipes-product", ("reagent", _prototype.Index<ReagentPrototype>(product).LocalizedName), ("ratio", ratio)));
                    // }
                }
                else
                {
                    sw.WriteLine("None");
                }

                sw.WriteLine("END REAGENT");
            }
        }
        File.AppendAllText(Path, "END FILE");
    }
}
