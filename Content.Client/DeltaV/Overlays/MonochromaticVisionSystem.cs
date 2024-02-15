using Content.Shared.Abilities;
using Robust.Client.Graphics;

namespace Content.Client.DeltaV.Overlays;

public sealed partial class MonochromaticVisionSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;

    private MonochromaticVisionOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MonochromaticVisionComponent, ComponentInit>(OnMonochromaticVisionInit);
        SubscribeLocalEvent<MonochromaticVisionComponent, ComponentShutdown>(OnMonochromaticVisionShutdown);

        _overlay = new();
    }

    private void OnMonochromaticVisionInit(EntityUid uid, MonochromaticVisionComponent component, ComponentInit args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnMonochromaticVisionShutdown(EntityUid uid, MonochromaticVisionComponent component, ComponentShutdown args)
    {
        _overlayMan.RemoveOverlay(_overlay);
    }
}
