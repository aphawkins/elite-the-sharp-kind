// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using Useful.Graphics.Fakes;

namespace Useful.Widgets.Tests;

public class WidgetTests
{
    private static readonly FastColor s_text = new(0xFFFFFFFF);
    private static readonly FastColor s_disabled = new(0xFF808080);
    private static readonly FastColor s_highlight = new(0xFFFF0000);

    private static readonly WidgetStyle s_style = new(
        "Small",
        WidgetColors.TextOnly(s_text),
        new(s_highlight, s_text),
        WidgetColors.TextOnly(s_disabled));

    // What a row's value and arrows are given: the name's label has already
    // filled the row, so these never paint a block of their own.
    private static readonly WidgetStyle s_valueStyle = new(
        "Small",
        WidgetColors.TextOnly(s_text),
        WidgetColors.TextOnly(s_text),
        WidgetColors.TextOnly(s_disabled));

    [Fact]
    public void ANormalLabelDrawsNoBackground()
    {
        RecordingGraphics graphics = new();
        Label label = Bounded(graphics);

        label.Draw();

        Assert.Empty(graphics.FilledRectangles);
        Assert.Equal(s_text, Assert.Single(graphics.LeftTexts).Colour);
    }

    [Fact]
    public void ASelectedLabelFillsItsOwnBounds()
    {
        RecordingGraphics graphics = new();
        Label label = Bounded(graphics);
        label.State = WidgetState.Selected;

        label.Draw();

        (Vector2 position, float width, float height, FastColor colour) = Assert.Single(graphics.FilledRectangles);
        Assert.Equal(new Vector2(10, 20), position);
        Assert.Equal(100, width);
        Assert.Equal(8, height);
        Assert.Equal(s_highlight, colour);
    }

    [Fact]
    public void ADisabledLabelKeepsTheCursorBlockWhenItIsAlsoSelected()
    {
        RecordingGraphics graphics = new();
        Label label = Bounded(graphics);
        label.State = WidgetState.SelectedDisabled;

        label.Draw();

        Assert.Equal(s_highlight, Assert.Single(graphics.FilledRectangles).Colour);
        Assert.Equal(s_disabled, Assert.Single(graphics.LeftTexts).Colour);
    }

    [Theory]
    [InlineData(TextAlignment.Left, 10)]
    [InlineData(TextAlignment.Centre, 44)]
    [InlineData(TextAlignment.Right, 78)]
    public void TextIsAlignedWithinTheLabelsOwnBounds(TextAlignment alignment, float expectedX)
    {
        RecordingGraphics graphics = new();
        Label label = Bounded(graphics);
        label.Text = "Abcd";
        label.Alignment = alignment;

        label.Draw();

        // "Abcd" measures 32 of the label's 100, at x=10.
        Assert.Equal(new Vector2(expectedX, 20), Assert.Single(graphics.LeftTexts).Position);
    }

    [Fact]
    public void CentringRoundsToTheCellWhenTheLabelSnaps()
    {
        RecordingGraphics graphics = new();
        Label label = Bounded(graphics);
        label.Text = "Ab";
        label.Alignment = TextAlignment.Centre;
        label.SnapToCell = 8;

        label.Draw();

        // Centring puts the text at 10 + (100 - 16) / 2 = 52, which is not on
        // a cell boundary; it truncates to 48. Note that the label's own left
        // edge is not on the grid either - snapping is of the final position,
        // not of the gap in front of the text.
        Assert.Equal(new Vector2(48, 20), Assert.Single(graphics.LeftTexts).Position);
    }

    [Fact]
    public void ContainerStacksItsChildrenAtItsSpacing()
    {
        RecordingGraphics graphics = new();
        Container<Label> container = new() { Position = new(10, 20), Width = 100, Spacing = 16 };
        container.Add(Bounded(graphics));
        container.Add(Bounded(graphics));

        container.Layout();

        Assert.Equal(new Vector2(10, 20), container.Children[0].Position);
        Assert.Equal(new Vector2(10, 36), container.Children[1].Position);
    }

    [Fact]
    public void ContainerAlignsEachChildAcrossItsWidth()
    {
        RecordingGraphics graphics = new();
        Container<Label> container = new()
        {
            Position = new(10, 20),
            Width = 100,
            ChildAlignment = TextAlignment.Centre,
        };

        Label child = Bounded(graphics);
        child.Width = 40;
        container.Add(child);

        container.Layout();

        Assert.Equal(new Vector2(40, 20), child.Position);
    }

    [Fact]
    public void ContainerDrawsEveryChild()
    {
        RecordingGraphics graphics = new();
        Container<Label> container = new() { Width = 100, Spacing = 8 };
        container.Add(Bounded(graphics));
        container.Add(Bounded(graphics));

        container.Draw();

        Assert.Equal(2, graphics.LeftTexts.Count);
    }

    [Fact]
    public void AComboBoxDrawsItsValueAtTheGivenOffsetAndNoArrowsWhenUnselected()
    {
        RecordingGraphics graphics = new();
        ComboBox box = Combo(graphics);

        box.Draw();

        Assert.Collection(
            graphics.LeftTexts,
            name => Assert.Equal(new Vector2(10, 20), name.Position),
            value =>
            {
                Assert.Equal("Solid", value.Text);
                Assert.Equal(new Vector2(70, 20), value.Position);
            });
    }

    [Fact]
    public void AComboBoxBracketsItsValueWhenSelected()
    {
        RecordingGraphics graphics = new();
        ComboBox box = Combo(graphics);
        box.State = WidgetState.Selected;

        box.Draw();

        // The arrows hug the value rather than sitting at fixed columns: the
        // value starts at 70 and "Solid" measures 40, so the opening arrow
        // ends a gap short of 70 - putting its own left edge at 52 - and the
        // closing one starts a gap past 110.
        Assert.Equal(["Graphic Style:", "Solid", "<", ">"], graphics.LeftTexts.Select(t => t.Text));
        Assert.Equal(new Vector2(52, 20), graphics.LeftTexts[2].Position);
        Assert.Equal(new Vector2(120, 20), graphics.LeftTexts[3].Position);
    }

    [Fact]
    public void AComboBoxWithOneValueOffersNothingToCycleTo()
    {
        RecordingGraphics graphics = new();
        ComboBox box = Combo(graphics, new FakeSetting("Graphic Style:", "Solid"));
        box.State = WidgetState.Selected;

        box.Draw();

        Assert.Equal(["Graphic Style:", "Solid"], graphics.LeftTexts.Select(t => t.Text));
    }

    [Fact]
    public void AComboBoxFillsItsWholeWidthWhenSelected()
    {
        RecordingGraphics graphics = new();
        ComboBox box = Combo(graphics);
        box.State = WidgetState.Selected;

        box.Draw();

        // The name's label carries the block for the whole row; everything
        // else draws over it, so nothing repaints a second one.
        (Vector2 position, float width, _, _) = Assert.Single(graphics.FilledRectangles);
        Assert.Equal(new Vector2(10, 20), position);
        Assert.Equal(100, width);
    }

    [Fact]
    public void AComboBoxCyclesThroughItsValuesBothWays()
    {
        RecordingGraphics graphics = new();
        ComboBox box = Combo(graphics);

        Assert.Equal("Solid", box.Value);

        box.Next();
        Assert.Equal("Wireframe", box.Value);

        box.Next();
        Assert.Equal("Solid", box.Value);

        box.Previous();
        Assert.Equal("Wireframe", box.Value);
    }

    [Fact]
    public void AComboBoxWithNoValuesShowsNothing()
    {
        RecordingGraphics graphics = new();
        ComboBox box = Combo(graphics, new FakeSetting("Nothing"));

        box.Next();
        box.Previous();

        Assert.Equal(string.Empty, box.Value);
    }

    [Fact]
    public void AComboBoxWritesStraightBackToItsBinding()
    {
        RecordingGraphics graphics = new();
        FakeSetting setting = new("Graphic Style:", "Solid", "Wireframe");
        ComboBox box = Combo(graphics, setting);

        box.Next();

        // The widget keeps no copy: the binding is the only place the choice
        // is recorded, so this is what the application sees.
        Assert.Equal(1, setting.SelectedIndex);
        Assert.Equal("Wireframe", box.Value);
    }

    [Fact]
    public void AComboBoxShowsWhateverItsBindingSaysNow()
    {
        RecordingGraphics graphics = new();
        FakeSetting setting = new("Graphic Style:", "Solid", "Wireframe");
        ComboBox box = Combo(graphics, setting);

        // Changed behind the widget's back, as the config file being reloaded
        // would: the next draw has to follow it.
        setting.SelectedIndex = 1;

        Assert.Equal("Wireframe", box.Value);
    }

    [Fact]
    public void WidgetsRejectAMissingSurfaceOrStyle()
    {
        RecordingGraphics graphics = new();

        Assert.Throws<ArgumentNullException>(() => new Label(null!, s_style));
        Assert.Throws<ArgumentNullException>(() => new Label(graphics, null!));
    }

    private static ComboBox Combo(RecordingGraphics graphics, ISetting? setting = null)
        => new(graphics, s_style, s_valueStyle, setting ?? new FakeSetting("Graphic Style:", "Solid", "Wireframe"))
        {
            Position = new(10, 20),
            Width = 100,
            Height = 8,
            ValueOffsetX = 60,
            ArrowGap = 10,
        };

    private static Label Bounded(RecordingGraphics graphics) => new(graphics, s_style)
    {
        Position = new(10, 20),
        Width = 100,
        Height = 8,
        Text = "Abcd",
    };
}
