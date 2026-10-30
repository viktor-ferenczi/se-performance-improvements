using System;
using System.Collections.Generic;
using Sandbox.Graphics.GUI;

namespace ClientPlugin.Settings.Elements;

internal class TextboxAttribute : Attribute, IElement
{
    public readonly string Label;
    public readonly string Description;
    public string EnabledBy { get; }

    public TextboxAttribute(string label = null, string description = null, string enabledBy = null)
    {
        Label = label;
        Description = description;
        EnabledBy = enabledBy;
    }

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        var textBox = new MyGuiControlTextbox(defaultText: (string)propertyGetter());
        textBox.TextChanged += box => propertySetter(box.Text);
        textBox.SetToolTip(Tools.Tools.Wrap(Description, Control.ToolTipWidth));

        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return new List<Control>()
        {
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(textBox, fillFactor: 1f),
        };
    }

    public List<Type> SupportedTypes { get; } = new List<Type>() { typeof(string) };
}
