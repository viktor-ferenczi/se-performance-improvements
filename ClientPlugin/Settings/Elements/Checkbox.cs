using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;

namespace ClientPlugin.Settings.Elements;

class CheckboxAttribute : Attribute, IElement
{
    public readonly string Label;
    public readonly string Description;

    public CheckboxAttribute(string label = null, string description = null)
    {
        Label = label;
        Description = description;
    }

    public List<Control> GetControls(string name, Func<object> propertyGetter, Action<object> propertySetter)
    {               
        var checkbox = new MyGuiControlCheckbox(toolTip: Tools.Tools.Wrap(Description, Control.ToolTipWidth))
        {
            IsChecked = (bool)propertyGetter(),
            IsCheckedChanged = x => propertySetter(x.IsChecked),
        };

        var nameLabel = new MyGuiControlLabel(text: Tools.Tools.GetLabelOrDefault(name, Label));

        // The description shares the row with the checkbox and the label, so it is wrapped to
        // whatever they leave of the row instead of running off the right edge of the dialog.
        // Both controls have measured themselves by now, and the layout gives the label the
        // wider of its text and the minimum, so the width left over is known here. A label
        // longer than the minimum simply leaves the description less room. The row grows in
        // height to fit, since the layout takes the height of its tallest control.
        var labelWidth = Math.Max(Control.LabelMinWidth, nameLabel.Size.X) + Control.LabelGap;
        var descriptionWidth = Control.RowRight - Control.RowLeft - checkbox.Size.X - labelWidth;
        var description = new MyGuiControlLabel(text: Tools.Tools.Wrap(Description ?? "", descriptionWidth));

        return new List<Control>
        {
            new Control(checkbox),
            new Control(nameLabel, minWidth: Control.LabelMinWidth, rightMargin: Control.LabelGap),
            new Control(description, minWidth: descriptionWidth),
        };
    }
    public List<Type> SupportedTypes { get; } = new List<Type>()
    {
        typeof(bool)
    };
}