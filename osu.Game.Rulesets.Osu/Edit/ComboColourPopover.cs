// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class ComboColourPopover : OsuPopover
    {
        private BindableList<Colour4> colours { get; } = new BindableList<Colour4>();
        private readonly ColourPalette comboColours = new ColourPalette();

        private readonly BindableWithCurrent<Colour4> current = new BindableWithCurrent<Colour4>();

        [Resolved]
        protected EditorBeatmap Beatmap { get; private set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new FillFlowContainer
            {
                Width = 50,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new ColourButton(),
                    new ColourButton(),
                    new ColourButton(),
                    new ColourButton()
                }
            };
        }

        public partial class ColourButton : OsuClickableContainer, IHasContextMenu
        {
            public Bindable<Colour4> Current { get; } = new Bindable<Colour4>(new Colour4(255, 255, 255, 255));
            public Action? DeleteRequested { get; set; }

            private Box background = null!;

            [BackgroundDependencyLoader]
            private void load()
            {
                Size = new Vector2(50);

                Masking = true;
                CornerRadius = 25;
                //Action = this.ShowPopover;

                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Current.BindValueChanged(_ => updateState(), true);
            }

            public MenuItem[] ContextMenuItems => new MenuItem[]
            {
                new OsuMenuItem(CommonStrings.ButtonsDelete, MenuItemType.Destructive, () => DeleteRequested?.Invoke())
            };

            private void updateState()
            {
                background.Colour = Current.Value;
            }
        }
    }
}

