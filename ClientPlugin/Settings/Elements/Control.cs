using Sandbox.Graphics.GUI;
using VRage.Utils;
using VRageMath;

namespace ClientPlugin.Settings.Elements;

internal class Control
{
    // FIXME: This is global and not determined automatically
    public static readonly float LabelMinWidth = 0.3f;

    // Where a row starts and where it has to end, both measured on the dialog itself: the
    // panel's left edge plus its padding, and a little short of the 0.95 where the scrolled
    // area is clipped by the scrollbar. The margin is there because a glyph's ink can reach
    // slightly past the width it is measured at, which clips the last letter of a line that
    // ends exactly on the boundary. Anything past the clip is drawn but never seen.
    public static readonly float RowLeft = 0.04f;
    public static readonly float RowRight = 0.94f;

    // Keeps a long label from touching the description next to it.
    public static readonly float LabelGap = 0.01f;

    // What is left of a checkbox row for the description once the checkbox and a label of the
    // minimum width have taken theirs. A row whose label is wider than the minimum has less
    // than this, which is why CheckboxAttribute works it out per row instead of using this.
    public static readonly float DescriptionMinWidth = RowRight - RowLeft - 0.0356f - LabelMinWidth - LabelGap;

    // Tooltips are drawn next to the cursor, not inside the dialog, so they are wrapped to
    // a width of their own.
    public static readonly float ToolTipWidth = 0.6f;

    public readonly MyGuiControlBase GuiControl;
    public readonly float? FixedWidth;
    public readonly float MinWidth;
    public readonly float? FillFactor;
    public readonly MyGuiDrawAlignEnum OriginAlign;
    public readonly Vector2 Offset;
    public readonly float RightMargin;

    public Control(MyGuiControlBase guiControl, float? fixedWidth = null, float minWidth = 0f, float? fillFactor = null, MyGuiDrawAlignEnum originAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, Vector2? offset = null, float rightMargin = 0f)
    {
        GuiControl = guiControl;
        FixedWidth = fixedWidth;
        MinWidth = minWidth;
        FillFactor = fillFactor;
        OriginAlign = originAlign;
        Offset = offset ?? Vector2.Zero;
        RightMargin = rightMargin;
    }
}