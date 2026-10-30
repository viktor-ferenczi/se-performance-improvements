using System;
using System.Collections.Generic;

namespace ClientPlugin.Settings.Elements;

internal interface IElement
{
    List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    );
    List<Type> SupportedTypes { get; }

    // Name of a bool property of Config; the controls are disabled while it is false
    string EnabledBy { get; }
}
