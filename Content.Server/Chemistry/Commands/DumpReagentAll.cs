using System.IO;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using System.Linq;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Localizations;

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
                // Start the reagent info
                sw.WriteLine("START REAGENT");
                // Write the reagent's name, color, group and metabolism rate
                sw.WriteLine(Loc.GetString("wikidump-reagent-name", ("name", rea.LocalizedName)));
                sw.WriteLine(rea.SubstanceColor.ToHex());
                sw.WriteLine("{0}: {1}", "GROUP", rea.Group ?? $"Other");
                if (rea.Metabolisms is null)
                {
                    sw.WriteLine("None");
                }
                else
                {
                    foreach (var (_, entry) in rea.Metabolisms)
                    {
                        sw.WriteLine(Loc.GetString("wikidump-reagent-effects-metabolism-rate", ("rate", entry.MetabolismRate)));
                        foreach (var effect in entry.Effects)
                        {
                            sw.WriteLine(effect.GuidebookEffectDescription(_prototype, _entSys) ?? $"[skipped effect]");
                        }
                    }
                }
                // Start the recipe
                sw.WriteLine("START RECIPE");

                var reactions = _prototype.EnumeratePrototypes<ReactionPrototype>()
                .Where(p => p.Products.ContainsKey(rea.ID))
                .OrderBy(p => p.Priority)
                .ThenBy(p => p.Products.Count)
                .ToList();

                if (reactions.Any())
                {
                    foreach (var reactionPrototype in reactions)
                    {
                        // Get the mixing categories
                        var mixingCategories = new List<MixingCategoryPrototype>();
                        if (reactionPrototype.MixingCategories != null)
                        {
                            foreach (var category in reactionPrototype.MixingCategories)
                            {
                                mixingCategories.Add(_prototype.Index(category));
                            }
                        }

                        var mixingVerb = mixingCategories.Count == 0
                            ? Loc.GetString("wikidump-reagent-recipes-mix")
                            : ContentLocalizationManager.FormatList(mixingCategories.Select(p => Loc.GetString(p.VerbText)).ToList());

                        var text = Loc.GetString("wikidump-reagent-recipes-mix-info",
                            ("verb", mixingVerb),
                            ("minTemp", reactionPrototype.MinimumTemperature),
                            ("maxTemp", reactionPrototype.MaximumTemperature),
                            ("hasMax", !float.IsPositiveInfinity(reactionPrototype.MaximumTemperature)));

                        sw.WriteLine(text);

                        // Get all the reagents of the recipe
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

                        // var productCount = reactionPrototype.Products.Count;
                        // foreach (var (product, ratio) in reactionPrototype.Products)
                        // {
                        //     sw.WriteLine(Loc.GetString("wikidump-reagent-recipes-product", ("reagent", _prototype.Index<ReagentPrototype>(product).LocalizedName), ("ratio", ratio)));
                        // }
                    }
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
