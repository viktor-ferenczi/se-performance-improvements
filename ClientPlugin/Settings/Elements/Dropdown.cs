using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sandbox.Graphics.GUI;

namespace ClientPlugin.Settings.Elements;

internal class DropdownAttribute : Attribute, IElement
{
    private const float DropdownWidth = 0.2f;

    public readonly int VisibleRows;
    public readonly string Label;
    public readonly string Description;
    public string EnabledBy { get; }

    private static string UnCamelCase(string str)
    {
        return Regex.Replace(
            Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    public DropdownAttribute(
        int visibleRows = 20,
        string label = null,
        string description = null,
        string enabledBy = null
    )
    {
        VisibleRows = visibleRows;
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
        object selectedEnum = propertyGetter();
        Type choiceEnum = selectedEnum.GetType();

        var dropdown = new MyGuiControlCombobox(
            toolTip: Tools.Tools.Wrap(Description, Control.ToolTipWidth)
        );
        string[] elements = Enum.GetNames(choiceEnum);

        for (int i = 0; i < elements.Length; i++)
        {
            dropdown.AddItem(i, UnCamelCase(elements[i]));
        }

        void OnItemSelect()
        {
            long key = dropdown.GetSelectedKey();
            string value = elements[key];

            object enumValue = Enum.Parse(choiceEnum, value);
            propertySetter(enumValue);
        }

        dropdown.ItemSelected += OnItemSelect;
        dropdown.SelectItemByIndex(Convert.ToInt32(selectedEnum));

        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return new List<Control>()
        {
            Control.Indent(),
            new Control(
                new MyGuiControlLabel(text: label),
                minWidth: Control.LabelMinWidth,
                rightMargin: Control.LabelGap
            ),
            // A fixed, narrow width: filling the row would stretch the box far wider than the
            // few words of an enum value need. Widen this if a longer value ever gets clipped.
            new Control(dropdown, fixedWidth: DropdownWidth),
        };
    }

    public List<Type> SupportedTypes { get; } = new List<Type>() { typeof(Enum) };
}
