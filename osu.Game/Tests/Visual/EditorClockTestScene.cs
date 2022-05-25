﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Overlays;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit;

namespace osu.Game.Tests.Visual
{
    /// <summary>
    /// Provides a clock, beat-divisor, and scrolling capability for test cases of editor components that
    /// are preferrably tested within the presence of a clock and seek controls.
    /// </summary>
    public abstract class EditorClockTestScene : OsuManualInputManagerTestScene
    {
        [Cached]
        private readonly OverlayColourProvider overlayColour = new OverlayColourProvider(OverlayColourScheme.Aquamarine);

        [Cached(typeof(EditorBeatmap))]
        [Cached(typeof(IBeatSnapProvider))]
        private readonly EditorBeatmap editorBeatmap;

        protected readonly BindableBeatDivisor BeatDivisor = new BindableBeatDivisor();
        protected new readonly EditorClock Clock;

        protected virtual bool ScrollUsingMouseWheel => true;

        protected EditorClockTestScene()
        {
            editorBeatmap = new EditorBeatmap(new Beatmap());

            Clock = new EditorClock(editorBeatmap, BeatDivisor) { IsCoupled = false };
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.Cache(BeatDivisor);
            dependencies.CacheAs(Clock);

            return dependencies;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Beatmap.BindValueChanged(beatmapChanged, true);
        }

        private void beatmapChanged(ValueChangedEvent<WorkingBeatmap> e)
        {
            Clock.Beatmap = e.NewValue.Beatmap;
            Clock.ChangeSource(e.NewValue.Track);
            Clock.ProcessFrame();
        }

        protected override void Update()
        {
            base.Update();

            Clock.ProcessFrame();
        }

        protected override bool OnScroll(ScrollEvent e)
        {
            if (!ScrollUsingMouseWheel)
                return false;

            if (e.ScrollDelta.Y > 0)
                Clock.SeekBackward(true);
            else
                Clock.SeekForward(true);

            return true;
        }
    }
}
