// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class ComboColourPopover : OsuPopover
    {
        private BindableList<Colour4> colours { get; } = new BindableList<Colour4>();
        private ColourPalette palette = new ColourPalette();

        private readonly BindableWithCurrent<Colour4> current = new BindableWithCurrent<Colour4>();

        [BackgroundDependencyLoader]
        private void load() { }
    }
}

