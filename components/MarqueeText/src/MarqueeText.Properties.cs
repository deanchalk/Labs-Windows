// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Text;

namespace CommunityToolkit.Labs.WinUI.MarqueeTextRns;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
public partial class MarqueeText
{
    private static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarqueeText), new PropertyMetadata(null, PropertyChanged));

    private static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(MarqueeText), new PropertyMetadata(32d, PropertyChanged));

    private static readonly DependencyProperty RepeatBehaviorProperty =
        DependencyProperty.Register(nameof(RepeatBehavior), typeof(RepeatBehavior), typeof(MarqueeText), new PropertyMetadata(new RepeatBehavior(1), PropertyChanged));

    private static readonly DependencyProperty BehaviorProperty =
        DependencyProperty.Register(nameof(Behavior), typeof(MarqueeBehavior), typeof(MarqueeText), new PropertyMetadata(0, BehaviorPropertyChanged));

    private static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(MarqueeDirection), typeof(MarqueeText), new PropertyMetadata(MarqueeDirection.Left, DirectionPropertyChanged));

    private static readonly DependencyProperty TextDecorationsProperty =
        DependencyProperty.Register(nameof(TextDecorations), typeof(TextDecorations), typeof(MarqueeText), new PropertyMetadata(TextDecorations.None));

    /// <summary>
    /// Gets or sets the text being displayed in Marquee.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the speed the text moves in the Marquee.
    /// </summary>
    public double Speed
    {
        get => (double)GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the marquee scroll repeats.
    /// </summary>
    public RepeatBehavior RepeatBehavior
    {
        get => (RepeatBehavior)GetValue(RepeatBehaviorProperty);
        set => SetValue(RepeatBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets the marquee behavior.
    /// </summary>
    public MarqueeBehavior Behavior
    {
        get => (MarqueeBehavior)GetValue(BehaviorProperty);
        set => SetValue(BehaviorProperty, value);
    }

    private bool IsTicker => Behavior == MarqueeBehavior.Ticker;

    private bool IsLooping => Behavior == MarqueeBehavior.Looping;

#if !HAS_UNO
    private bool IsBouncing => Behavior == MarqueeBehavior.Bouncing;
#else
    private bool IsBouncing => false;
#endif

    /// <summary>
    /// Gets or sets a value indicating whether or not the marquee text wraps.
    /// </summary>
    /// <remarks>
    /// Wrapping text won't scroll if the text can already fit in the screen.
    /// </remarks>
    public MarqueeDirection Direction
    {
        get => (MarqueeDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    private bool IsDirectionHorizontal => Direction is MarqueeDirection.Left or MarqueeDirection.Right;

    private bool IsDirectionInverse => Direction is MarqueeDirection.Up or MarqueeDirection.Right;

    /// <summary>
    /// Gets or sets a value that indicates what decorations are applied to the text.
    /// </summary>
    public TextDecorations TextDecorations
    {
        get => (TextDecorations)GetValue(TextDecorationsProperty);
        set => SetValue(TextDecorationsProperty, value);
    }

    private static void BehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        bool active = control._isActive;
        var newBehavior = (MarqueeBehavior)e.NewValue;

        VisualStateManager.GoToState(control, GetVisualStateName(newBehavior), true);

        control.StopMarquee(false);
        if (active)
        {
            control.StartMarquee();
        }
    }

    private static void DirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        bool active = control._isActive;
        var oldDirection = (MarqueeDirection)e.OldValue;
        var newDirection = (MarqueeDirection)e.NewValue;
        bool oldAxisX = oldDirection is MarqueeDirection.Left or MarqueeDirection.Right;
        bool newAxisX = newDirection is MarqueeDirection.Left or MarqueeDirection.Right;

        VisualStateManager.GoToState(control, GetVisualStateName(newDirection), true);

        if (oldAxisX != newAxisX)
        {
            control.StopMarquee(false);
        }

        if (active)
        {
            control.StartMarquee();
        }
    }

    private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        control.UpdateAnimation();
    }
}
