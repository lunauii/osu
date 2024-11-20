// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit.Components;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class ComboToolboxGroup : EditorToolboxGroup
    {
        private readonly EditorToolButton comboColourButton;

        public ComboToolboxGroup()
            : base("Combo")
        {
            Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    comboColourButton = new EditorToolButton("Colour",
                        () => new SpriteIcon { Icon = FontAwesome.Solid.Palette },
                        () => new ComboColourPopover()),
                }
            };
        }
    }
}
