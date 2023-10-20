using Robust.Shared.GameStates;

namespace Content.Shared.Overlays;

/// <summary>
/// This component allows you to see the health of mobs.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ShowHealthIconsComponent : Component { }
