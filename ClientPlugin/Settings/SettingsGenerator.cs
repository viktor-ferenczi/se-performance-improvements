using ClientPlugin.Settings.Elements;
using ClientPlugin.Settings.Layouts;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace ClientPlugin.Settings;

internal class AttributeInfo
{
    public IElement ElementType;
    public string Name;
    public Func<object> Getter;
    public Action<object> Setter;
    public Func<bool> IsEnabled;
}

internal class SettingsGenerator
{
    public readonly string Name;

    private readonly List<AttributeInfo> attributes;
    private List<List<Control>> controls;
    private List<(Func<bool> IsEnabled, List<Control> Row)> enabledRows = new();
    public SettingsScreen Dialog { get; private set; }
    public Layout ActiveLayout { get; private set; }

    private static bool ValidateType(Type type, List<Type> typesList)
    {
        return typesList.Any(t => t.IsAssignableFrom(type));
    }

    private static Delegate GetDelegate(MethodInfo methodInfo)
    {
        // Reconstruct the type
        Type[] methodArgs = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        Type type = Expression.GetDelegateType(methodArgs.Concat(new[] { methodInfo.ReturnType }).ToArray());

        // Create a delegate
        return Delegate.CreateDelegate(type, null, methodInfo);
    }

    public SettingsGenerator()
    {
        attributes = ExtractAttributes();
        Name = Config.Current.Title;
        ActiveLayout = new None(()=>controls);
        Dialog = new SettingsScreen(Name, OnRecreateControls, UpdateEnabled, size: ActiveLayout.SettingsPanelSize);
    }

    private List<MyGuiControlBase> OnRecreateControls()
    {
        CreateConfigControls();
        var controlsToRecreate = ActiveLayout.RecreateControls();
        ActiveLayout.LayoutControls();
        return controlsToRecreate;
    }

    public void SetLayout<T>() where T : Layout
    {
        ActiveLayout = (T)Activator.CreateInstance(typeof(T), (Func<List<List<Control>>>)(() => controls));
        Dialog.UpdateSize(ActiveLayout.SettingsPanelSize);
    }

    public void RefreshLayout()
    {
        ActiveLayout.LayoutControls();
    }

    private void CreateConfigControls()
    {
        controls = new List<List<Control>>();
        enabledRows = new List<(Func<bool>, List<Control>)>();

        foreach (AttributeInfo info in attributes)
        {
            var row = info.ElementType.GetControls(info.Name, info.Getter, info.Setter);
            controls.Add(row);

            if (info.IsEnabled != null)
                enabledRows.Add((info.IsEnabled, row));
        }

        UpdateEnabled();
    }

    // Called on every update of the dialog, so a row follows its enabling option while the dialog is open
    private void UpdateEnabled()
    {
        foreach (var (isEnabled, row) in enabledRows)
        {
            var enabled = isEnabled();
            foreach (var control in row)
                control.GuiControl.Enabled = enabled;
        }
    }

    private static Func<bool> GetIsEnabled(IElement element, string name)
    {
        if (element.EnabledBy == null)
            return null;

        var propertyInfo = typeof(Config).GetProperty(element.EnabledBy);
        if (propertyInfo?.PropertyType != typeof(bool))
        {
            throw new Exception(
                $"Element {element.GetType().Name} for {name} is enabled by "
                + $"{element.EnabledBy}, which is not a bool property of Config");
        }

        return () => (bool)propertyInfo.GetValue(Config.Current);
    }

    private static List<AttributeInfo> ExtractAttributes()
    {
        var config = new List<AttributeInfo>();

        foreach (var propertyInfo in typeof(Config).GetProperties())
        {
            var name = propertyInfo.Name;
            foreach (var attribute in propertyInfo.GetCustomAttributes())
            {
                if (attribute is IElement element)
                {
                    if (!ValidateType(propertyInfo.PropertyType, element.SupportedTypes))
                    {
                        throw new Exception(
                            $"Element {element.GetType().Name} for {name} expects "
                            + $"{string.Join("/", element.SupportedTypes)} but "
                            + $"recieved {propertyInfo.PropertyType.FullName}");
                    }

                    var info = new AttributeInfo()
                    {
                        ElementType = element,
                        Name = name,
                        Getter = Getter,
                        Setter = Setter,
                        IsEnabled = GetIsEnabled(element, name),
                    };
                    config.Add(info);
                }
            }

            continue;

            object Getter() => propertyInfo.GetValue(Config.Current);
            void Setter(object value) => propertyInfo.SetValue(Config.Current, value);
        }

        foreach (var methodInfo in typeof(Config).GetMethods())
        {
            string name = methodInfo.Name;
            Delegate method = GetDelegate(methodInfo);

            foreach (var attribute in methodInfo.GetCustomAttributes())
            {
                if (attribute is IElement element)
                {
                    if (!ValidateType(typeof(Delegate), element.SupportedTypes))
                    {
                        throw new Exception(
                            $"Element {element.GetType().Name} for {name} expects "
                            + $"{string.Join("/", element.SupportedTypes)} but "
                            + $"recieved {typeof(Delegate).FullName}");
                    }

                    var info = new AttributeInfo()
                    {
                        ElementType = element,
                        Name = name,
                        Getter = () => method,
                        Setter = null,
                        IsEnabled = GetIsEnabled(element, name),
                    };
                    config.Add(info);
                }
            }
        }

        return config;
    }
}