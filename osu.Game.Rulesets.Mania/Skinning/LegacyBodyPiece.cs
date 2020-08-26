// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Game.Rulesets.Mania.Objects.Drawables;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Mania.Skinning
{
    public class LegacyBodyPiece : LegacyManiaColumnElement
    {
        private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();
        private readonly IBindable<bool> isHitting = new Bindable<bool>();

        private Drawable bodySprite;
        private Drawable lightContainer;
        private Drawable light;

        public LegacyBodyPiece()
        {
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin, IScrollingInfo scrollingInfo, DrawableHitObject drawableObject)
        {
            string imageName = GetColumnSkinConfig<string>(skin, LegacyManiaSkinConfigurationLookups.HoldNoteBodyImage)?.Value
                               ?? $"mania-note{FallbackColumnIndex}L";

            string lightImage = GetColumnSkinConfig<string>(skin, LegacyManiaSkinConfigurationLookups.HoldNoteLightImage)?.Value
                                ?? "lightingL";

            float lightScale = GetColumnSkinConfig<float>(skin, LegacyManiaSkinConfigurationLookups.HoldNoteLightScale)?.Value
                               ?? 1;

            // Create a temporary animation to retrieve the number of frames, in an effort to calculate the intended frame length.
            // This animation is discarded and re-queried with the appropriate frame length afterwards.
            var tmp = skin.GetAnimation(lightImage, true, false);
            double frameLength = 0;
            if (tmp is IFramedAnimation tmpAnimation && tmpAnimation.FrameCount > 0)
                frameLength = Math.Max(1000 / 60.0, 170.0 / tmpAnimation.FrameCount);

            light = skin.GetAnimation(lightImage, true, true, frameLength: frameLength).With(d =>
            {
                if (d == null)
                    return;

                d.Origin = Anchor.Centre;
                d.Blending = BlendingParameters.Additive;
                d.Scale = new Vector2(lightScale);
            });

            if (light != null)
                lightContainer = new HitTargetInsetContainer { Child = light };

            bodySprite = skin.GetAnimation(imageName, WrapMode.ClampToEdge, WrapMode.ClampToEdge, true, true).With(d =>
            {
                if (d == null)
                    return;

                if (d is TextureAnimation animation)
                    animation.IsPlaying = false;

                d.Anchor = Anchor.TopCentre;
                d.RelativeSizeAxes = Axes.Both;
                d.Size = Vector2.One;
                d.FillMode = FillMode.Stretch;
                // Todo: Wrap
            });

            if (bodySprite != null)
                InternalChild = bodySprite;

            direction.BindTo(scrollingInfo.Direction);
            direction.BindValueChanged(onDirectionChanged, true);

            var holdNote = (DrawableHoldNote)drawableObject;
            isHitting.BindTo(holdNote.IsHitting);
            isHitting.BindValueChanged(onIsHittingChanged, true);
        }

        private void onIsHittingChanged(ValueChangedEvent<bool> isHitting)
        {
            if (bodySprite is TextureAnimation bodyAnimation)
            {
                bodyAnimation.GotoFrame(0);
                bodyAnimation.IsPlaying = isHitting.NewValue;
            }

            if (lightContainer != null)
            {
                if (isHitting.NewValue)
                {
                    Column.TopLevelContainer.Add(lightContainer);

                    // The light must be seeked only after being loaded, otherwise a nullref happens (https://github.com/ppy/osu-framework/issues/3847).
                    if (light is TextureAnimation lightAnimation)
                        lightAnimation.GotoFrame(0);
                }
                else
                    Column.TopLevelContainer.Remove(lightContainer);
            }
        }

        private void onDirectionChanged(ValueChangedEvent<ScrollingDirection> direction)
        {
            if (direction.NewValue == ScrollingDirection.Up)
            {
                if (bodySprite != null)
                {
                    bodySprite.Origin = Anchor.BottomCentre;
                    bodySprite.Scale = new Vector2(1, -1);
                }

                if (light != null)
                    light.Anchor = Anchor.TopCentre;
            }
            else
            {
                if (bodySprite != null)
                {
                    bodySprite.Origin = Anchor.TopCentre;
                    bodySprite.Scale = Vector2.One;
                }

                if (light != null)
                    light.Anchor = Anchor.BottomCentre;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            lightContainer?.Expire();
        }

        private class HitTargetInsetContainer : Container
        {
            private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();

            protected override Container<Drawable> Content => content;
            private readonly Container content;

            private float hitPosition;

            public HitTargetInsetContainer()
            {
                RelativeSizeAxes = Axes.Both;

                InternalChild = content = new Container { RelativeSizeAxes = Axes.Both };
            }

            [BackgroundDependencyLoader]
            private void load(ISkinSource skin, IScrollingInfo scrollingInfo)
            {
                hitPosition = skin.GetManiaSkinConfig<float>(LegacyManiaSkinConfigurationLookups.HitPosition)?.Value ?? Stage.HIT_TARGET_POSITION;

                direction.BindTo(scrollingInfo.Direction);
                direction.BindValueChanged(onDirectionChanged, true);
            }

            private void onDirectionChanged(ValueChangedEvent<ScrollingDirection> direction)
            {
                content.Padding = direction.NewValue == ScrollingDirection.Up
                    ? new MarginPadding { Top = hitPosition }
                    : new MarginPadding { Bottom = hitPosition };
            }
        }
    }
}
