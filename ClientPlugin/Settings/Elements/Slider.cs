using Sandbox;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using VRage.Utils;

namespace ClientPlugin.Settings.Elements;

internal class SliderAttribute : Attribute, IElement
{
    public enum SliderType
    {
        Integer,
        Float,
    }

    public readonly float Min;
    public readonly float Max;
    public readonly float Step;
    public readonly SliderType Type;
    public readonly string Label;
    public readonly string Description;

    // Name of a bool property of Config; the slider is disabled while it is false
    public readonly string EnabledBy;

    public SliderAttribute(float min, float max, float step = 1f, SliderType type = SliderType.Float, string label = null, string description = null, string enabledBy = null)
    {
        Min = min;
        Max = max;
        Step = step;
        Type = type;
        Label = label;
        Description = description;
        EnabledBy = enabledBy;
    }

    // Follows the config on every update, so a value the config changes on its own (or an
    // option which disables the slider) shows up while the dialog is open
    private class ConfigSlider : MyGuiControlSlider
    {
        public Func<object> Getter;
        public Func<bool> IsEnabled;

        public ConfigSlider(string toolTip, float defaultValue, float minValue, float maxValue, bool intValue)
            : base(toolTip: toolTip, defaultValue: defaultValue, minValue: minValue, maxValue: maxValue, intValue: intValue)
        {
        }

        public override void Update()
        {
            if (IsEnabled != null)
                Enabled = IsEnabled();

            var value = Convert.ToSingle(Getter());
            if (Value != value)
                Value = value;

            base.Update();
        }
    }

    public List<Control> GetControls(string name, Func<object> propertyGetter, Action<object> propertySetter)
    {
        var valueLabel = new MyGuiControlLabel();

        void ValueUpdate(MyGuiControlSlider element)
        {
            switch (Type)
            {
                case SliderType.Integer:
                    int intValue = Convert.ToInt32(element.Value);
                    propertySetter(intValue);
                    valueLabel.Text = intValue.ToString();
                    break;

                case SliderType.Float:
                    propertySetter(element.Value);
                    valueLabel.Text = MyValueFormatter.GetFormatedFloat(element.Value, element.LabelDecimalPlaces);
                    break;
            }
        }

        bool SpecifyValue(MyGuiControlSlider element)
        {
            MyGuiScreenDialogAmount screen = new MyGuiScreenDialogAmount(
                Min,
                Max,
                MyCommonTexts.DialogAmount_SetValueCaption,
                defaultAmount: Convert.ToSingle(propertyGetter()),
                parseAsInteger: Type == SliderType.Integer,
                backgroundTransition: MySandboxGame.Config.UIBkOpacity,
                guiTransition: MySandboxGame.Config.UIOpacity);

            screen.OnConfirmed += (value) => element.Value = value;

            // Hide the settings screen behind this dialog. The CanHideOthers
            // setter is protected, so reflection is needed. Use ?. so the dialog
            // still opens (just without the hide effect) on game versions where
            // the property has been renamed or removed.
            typeof(MyGuiScreenBase)
                .GetProperty("CanHideOthers", BindingFlags.Public | BindingFlags.Instance)
                ?.SetValue(screen, true);

            MyGuiSandbox.AddScreen(screen);
            return true;
        }

        var enabledBy = EnabledBy == null ? null : typeof(Config).GetProperty(EnabledBy);
        var slider = new ConfigSlider(
            toolTip: Tools.Tools.Wrap(Description, Control.ToolTipWidth),
            defaultValue: Convert.ToSingle(propertyGetter()),
            minValue: Min,
            maxValue: Max,
            intValue: Type == SliderType.Integer)
        {
            MinimumStepOverride = Step,
            Getter = propertyGetter,
            IsEnabled = enabledBy == null ? null : () => (bool)enabledBy.GetValue(Config.Current),
        };

        if (Type == SliderType.Float)
        {
            slider.LabelDecimalPlaces = (int)Math.Max(1, Math.Ceiling(-Math.Log10(2f * Step)));
        }

        slider.ValueChanged += ValueUpdate;
        slider.SliderSetValueManual = SpecifyValue;

        ValueUpdate(slider);

        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return new List<Control>()
        {
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(slider, fillFactor: 1f, rightMargin: 0.005f),
            new Control(valueLabel, minWidth: 0.06f),
        };
    }

    public List<Type> SupportedTypes { get; } = new List<Type>()
    {
        typeof(float),
        typeof(int),
    };
}