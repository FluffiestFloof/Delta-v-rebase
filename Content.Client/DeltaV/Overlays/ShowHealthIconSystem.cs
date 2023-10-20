using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Mobs;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowHealthIconsSystem : EquipmentHudSystem<ShowHealthIconsComponent>
{
    [Dependency] private readonly IPrototypeManager _prototypeMan = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly MobThresholdSystem _mobThresholdSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, StatusIconComponent _, ref GetStatusIconsEvent args)
    {
        if (!IsActive || args.InContainer)
            return;

        var healthIcons = DecideHealthIcon(uid);

        args.StatusIcons.AddRange(healthIcons);
    }

    private IReadOnlyList<StatusIconPrototype> DecideHealthIcon(EntityUid uid)
    {
        var result = new List<StatusIconPrototype>();

        if(!TryComp<MobStateComponent>(uid, out var state))
        {
            if (_mobStateSystem.IsAlive(uid, state))
                if (_prototypeMan.TryIndex<StatusIconPrototype>("HealthIconPerfect", out var perfect))
                {
                    result.Add(perfect);
                }

            if (_mobStateSystem.IsCritical(uid, state))
                if (_prototypeMan.TryIndex<StatusIconPrototype>("HealthIconCrit", out var crit))
                {
                    result.Add(crit);
                }

            if (_mobStateSystem.IsDead(uid, state))
                if (_prototypeMan.TryIndex<StatusIconPrototype>("HealthIconDead", out var dead))
                {
                    result.Add(dead);
                }
        }
        return result;
    }
}
