// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Specialized;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Screens.Edit;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class ComboColourPopover : OsuPopover
    {
        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; }

        public ComboColourPopover(EditorBeatmap editorBeatmap)
        {
            this.editorBeatmap = editorBeatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new FillFlowContainer
            {
                Width = 50,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10),
                Child = new ComboColourPalette(editorBeatmap)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };
        }

        internal partial class ComboColourPalette : CompositeDrawable
        {
            public BindableList<Colour4> Colours { get; } = new BindableList<Colour4>();

            private FillFlowContainer palette = null!;

            private EditorBeatmap editorBeatmap { get; }

            public ComboColourPalette(EditorBeatmap editorBeatmap)
            {
                this.editorBeatmap = editorBeatmap;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                AutoSizeDuration = fade_duration;
                AutoSizeEasing = Easing.OutQuint;

                InternalChild = palette = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(10),
                    Direction = FillDirection.Full
                };
            }

            private bool syncingColours;

            protected override void LoadComplete()
            {
                base.LoadComplete();

                if (editorBeatmap.BeatmapSkin != null)
                    Colours.AddRange(editorBeatmap.BeatmapSkin.ComboColours);

                if (Colours.Count == 0)
                {
                    // compare ctor of `EditorBeatmapSkin`
                    for (int i = 0; i < SkinConfiguration.DefaultComboColours.Count; ++i)
                        Colours.Add(SkinConfiguration.DefaultComboColours[(i + 1) % SkinConfiguration.DefaultComboColours.Count]);
                }

                Colours.BindCollectionChanged((_, args) =>
                {
                    if (editorBeatmap.BeatmapSkin != null)
                    {
                        if (syncingColours)
                            return;

                        syncingColours = true;

                        editorBeatmap.BeatmapSkin.ComboColours.Clear();
                        editorBeatmap.BeatmapSkin.ComboColours.AddRange(Colours);

                        syncingColours = false;
                    }

                    editorBeatmap.BeatmapSkin?.ComboColours.BindCollectionChanged((_, _) =>
                    {
                        if (syncingColours)
                            return;

                        syncingColours = true;

                        Colours.Clear();
                        Colours.AddRange(editorBeatmap.BeatmapSkin?.ComboColours);

                        syncingColours = false;
                    });

                    if (args.Action != NotifyCollectionChangedAction.Replace)
                    {
                        updatePalette();
                    }
                }, true);

                FinishTransforms(true);
            }

            private const int fade_duration = 200;

            private void updatePalette()
            {
                palette.Clear();

                for (int i = 0; i < Colours.Count; ++i)
                {
                    // copy to avoid accesses to modified closure.
                    int colourIndex = i;

                    ColourButton button = new ColourButton { Current = { Value = Colours[colourIndex] } };

                    button.Current.BindValueChanged(colour => Colours[colourIndex] = colour.NewValue);

                    palette.Add(button);
                }
            }
        }

        internal partial class ColourButton : OsuClickableContainer
        {
            public Bindable<Colour4> Current { get; } = new Bindable<Colour4>();

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

            private void updateState()
            {
                background.Colour = Current.Value;
            }
        }
    }
}

