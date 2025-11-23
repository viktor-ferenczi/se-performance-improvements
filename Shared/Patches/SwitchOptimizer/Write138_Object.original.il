// 09b4d512
nop
ldarg.3
ldnull
ceq
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L0
nop
ldarg.s 4
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L1
ldarg.0
ldarg.1
ldarg.2
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullTagLiteral(System.String name, System.String ns)
nop
L1:
br L2
L0:
ldarg.s 5
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L3
nop
ldarg.3
callvirt System.Type System.Object::GetType()
stloc.0
ldloc.0
ldtoken System.Object
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L4
nop
nop
br L5
L4:
ldloc.0
ldtoken Sandbox.Game.GUI.DebugInputComponents.MyMultiDebugInputComponent+MultidebugData
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L6
nop
ldarg.0
ldarg.1
ldarg.2
ldarg.3
unbox.any Sandbox.Game.GUI.DebugInputComponents.MyMultiDebugInputComponent+MultidebugData
ldc.i4.1
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write139_MultidebugData(System.String n, System.String ns, Sandbox.Game.GUI.DebugInputComponents.MultidebugData o, System.Boolean needType)
nop
br L7
L6:
ldloc.0
ldtoken VRage.Utils.MyGuiDrawAlignEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L8
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyGuiDrawAlignEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Utils.MyGuiDrawAlignEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write1_MyGuiDrawAlignEnum(VRage.Utils.MyGuiDrawAlignEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L9
L8:
ldloc.0
ldtoken Sandbox.Game.Entities.MyPlaybackCommand
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L10
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPlaybackCommand"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.Entities.MyPlaybackCommand
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write10_MyPlaybackCommand(Sandbox.Game.Entities.MyPlaybackCommand v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L11
L10:
ldloc.0
ldtoken Sandbox.Game.Entities.MyBlendOption
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L12
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyBlendOption"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.Entities.MyBlendOption
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write11_MyBlendOption(Sandbox.Game.Entities.MyBlendOption v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L13
L12:
ldloc.0
ldtoken Sandbox.Game.Entities.MyFrameOption
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L14
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyFrameOption"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.Entities.MyFrameOption
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write12_MyFrameOption(Sandbox.Game.Entities.MyFrameOption v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L15
L14:
ldloc.0
ldtoken VRage.Game.GUI.TextPanel.ContentType
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L16
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ContentType"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.GUI.TextPanel.ContentType
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write15_ContentType(VRage.Game.GUI.TextPanel.ContentType v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L17
L16:
ldloc.0
ldtoken Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L18
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfModule"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module[]
stloc.1
ldloc.1
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L19
nop
ldc.i4.0
stloc.2
br.s L20
L21:
nop
ldarg.0
ldstr "Module"
ldstr ""
ldloc.1
ldloc.2
ldelema Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module
ldobj Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write23_Module(System.String n, System.String ns, Sandbox.Game.WorldEnvironment.ObjectBuilders.Module o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L20:
ldloc.2
ldloc.1
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L21
nop
L19:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L22
L18:
ldloc.0
ldtoken System.Collections.Generic.List`1[System.String]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L23
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[System.String]
stloc.3
ldloc.3
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L24
nop
ldc.i4.0
stloc.2
br.s L25
L26:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.3
ldloc.2
callvirt virtual System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32 index)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L25:
ldloc.2
ldloc.3
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L26
nop
L24:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L27
L23:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.WorkshopId]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L28
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfWorkshopId"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.WorkshopId]
stloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L29
nop
ldc.i4.0
stloc.2
br.s L30
L31:
nop
ldarg.0
ldstr "WorkshopId"
ldstr ""
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldloc.2
callvirt virtual VRage.Game.WorkshopId System.Collections.Generic.List`1<VRage.Game.WorkshopId>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write29_WorkshopId(System.String n, System.String ns, VRage.Game.WorkshopId o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L30:
ldloc.2
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L31
nop
L29:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L32
L28:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L33
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfByteString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][]
stloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L34
nop
ldc.i4.0
stloc.2
br.s L35
L36:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write30_EntryOfByteString(System.String n, System.String ns, VRage.Serialization.Entry<System.Byte, System.String> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L35:
ldloc.2
ldloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L36
nop
L34:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L37
L33:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L38
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][]
stloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L39
nop
ldc.i4.0
stloc.2
br.s L40
L41:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write33_EntryOfStringString(System.String n, System.String ns, VRage.Serialization.Entry<System.String, System.String> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L40:
ldloc.2
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L41
nop
L39:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L42
L38:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L43
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringMyDebugInputData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][]
stloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L44
nop
ldc.i4.0
stloc.2
br.s L45
L46:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write35_EntryOfStringMyDebugInputData(System.String n, System.String ns, VRage.Serialization.Entry<System.String, Sandbox.Engine.Utils.MyDebugInputData> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L45:
ldloc.2
ldloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L46
nop
L44:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L47
L43:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L48
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringSerializableDictionaryOfStringString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][]
stloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L49
nop
ldc.i4.0
stloc.2
br.s L50
L51:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write37_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.String, VRage.Serialization.SerializableDictionary`2<System.String, System.String>> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L50:
ldloc.2
ldloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L51
nop
L49:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L52
L48:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L53
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringSingle"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][]
stloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L54
nop
ldc.i4.0
stloc.2
br.s L55
L56:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write39_EntryOfStringSingle(System.String n, System.String ns, VRage.Serialization.Entry<System.String, System.Single> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L55:
ldloc.2
ldloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L56
nop
L54:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L57
L53:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L58
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfInt32SerializableDictionaryOfStringSingle"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][]
stloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L59
nop
ldc.i4.0
stloc.2
br.s L60
L61:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write41_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.Int32, VRage.Serialization.SerializableDictionary`2<System.String, System.Single>> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L60:
ldloc.2
ldloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L61
nop
L59:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L62
L58:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L63
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringInnerData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData][]
stloc.s 11 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData][])
ldloc.s 11 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L64
nop
ldc.i4.0
stloc.2
br.s L65
L66:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 11 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write44_EntryOfStringInnerData(System.String n, System.String ns, VRage.Serialization.Entry<System.String, Sandbox.Engine.Utils.InnerData> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L65:
ldloc.2
ldloc.s 11 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfigBase+MyObjectBuilder_ConfigData+InnerData][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L66
nop
L64:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L67
L63:
ldloc.0
ldtoken Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L68
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyEnvironmentItemTypeDefinition"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition[]
stloc.s 12 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition[])
ldloc.s 12 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L69
nop
ldc.i4.0
stloc.2
br.s L70
L71:
nop
ldarg.0
ldstr "Item"
ldstr ""
ldloc.s 12 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition[])
ldloc.2
ldelem.ref
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write71_Item(System.String n, System.String ns, Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L70:
ldloc.2
ldloc.s 12 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyEnvironmentItemTypeDefinition[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L71
nop
L69:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L72
L68:
ldloc.0
ldtoken Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L73
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyProceduralEnvironmentMapping"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping[]
stloc.s 13 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping[])
ldloc.s 13 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L74
nop
ldc.i4.0
stloc.2
br.s L75
L76:
nop
ldarg.0
ldstr "Mapping"
ldstr ""
ldloc.s 13 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping[])
ldloc.2
ldelem.ref
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write75_MyProceduralEnvironmentMapping(System.String n, System.String ns, Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L75:
ldloc.2
ldloc.s 13 (Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralEnvironmentMapping[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L76
nop
L74:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L77
L73:
ldloc.0
ldtoken Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralScanningMethod
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L78
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyProceduralScanningMethod"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralScanningMethod
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write76_MyProceduralScanningMethod(Sandbox.Game.WorldEnvironment.ObjectBuilders.MyProceduralScanningMethod v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L79
L78:
ldloc.0
ldtoken System.TypeCode
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L80
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "TypeCode"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any System.TypeCode
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write86_TypeCode(System.TypeCode v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L81
L80:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ToolbarItemActionParameter]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L82
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_ToolbarItemActionParameter"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ToolbarItemActionParameter]
stloc.s 14 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ToolbarItemActionParameter])
ldloc.s 14 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ToolbarItemActionParameter])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L83
nop
ldc.i4.0
stloc.2
br.s L84
L85:
nop
ldarg.0
ldstr "MyObjectBuilder_ToolbarItemActionParameter"
ldstr ""
ldloc.s 14 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ToolbarItemActionParameter])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_ToolbarItemActionParameter System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_ToolbarItemActionParameter>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write87_Item(System.String n, System.String ns, VRage.Game.MyObjectBuilder_ToolbarItemActionParameter o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L84:
ldloc.2
ldloc.s 14 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ToolbarItemActionParameter])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L85
nop
L83:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L86
L82:
ldloc.0
ldtoken System.Collections.Generic.List`1[System.String]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L87
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[System.String]
stloc.3
ldloc.3
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L88
nop
ldc.i4.0
stloc.2
br.s L89
L90:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.3
ldloc.2
callvirt virtual System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32 index)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L89:
ldloc.2
ldloc.3
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L90
nop
L88:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L91
L87:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.WorkshopId]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L92
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfWorkshopId"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.WorkshopId]
stloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L93
nop
ldc.i4.0
stloc.2
br.s L94
L95:
nop
ldarg.0
ldstr "WorkshopId"
ldstr ""
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldloc.2
callvirt virtual VRage.Game.WorkshopId System.Collections.Generic.List`1<VRage.Game.WorkshopId>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write94_WorkshopId(System.String n, System.String ns, VRage.Game.WorkshopId o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L94:
ldloc.2
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L95
nop
L93:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L96
L92:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L97
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfByteString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][]
stloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L98
nop
ldc.i4.0
stloc.2
br.s L99
L100:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write95_EntryOfByteString(System.String n, System.String ns, VRage.Serialization.Entry<System.Byte, System.String> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L99:
ldloc.2
ldloc.s 5 (VRage.Serialization.SerializableDictionary`2+Entry[System.Byte,System.String][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L100
nop
L98:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L101
L97:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L102
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][]
stloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L103
nop
ldc.i4.0
stloc.2
br.s L104
L105:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write98_EntryOfStringString(System.String n, System.String ns, VRage.Serialization.Entry<System.String, System.String> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L104:
ldloc.2
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L105
nop
L103:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L106
L102:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L107
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringMyDebugInputData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][]
stloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L108
nop
ldc.i4.0
stloc.2
br.s L109
L110:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write100_EntryOfStringMyDebugInputData(System.String n, System.String ns, VRage.Serialization.Entry<System.String, Sandbox.Engine.Utils.MyDebugInputData> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L109:
ldloc.2
ldloc.s 7 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,Sandbox.Engine.Utils.MyConfig+MyDebugInputData][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L110
nop
L108:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L111
L107:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L112
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringSerializableDictionaryOfStringString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][]
stloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L113
nop
ldc.i4.0
stloc.2
br.s L114
L115:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write102_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.String, VRage.Serialization.SerializableDictionary`2<System.String, System.String>> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L114:
ldloc.2
ldloc.s 8 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,VRage.Serialization.SerializableDictionary`2[System.String,System.String]][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L115
nop
L113:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L116
L112:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L117
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringSingle"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][]
stloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L118
nop
ldc.i4.0
stloc.2
br.s L119
L120:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write104_EntryOfStringSingle(System.String n, System.String ns, VRage.Serialization.Entry<System.String, System.Single> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L119:
ldloc.2
ldloc.s 9 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Single][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L120
nop
L118:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L121
L117:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L122
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfInt32SerializableDictionaryOfStringSingle"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][]
stloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L123
nop
ldc.i4.0
stloc.2
br.s L124
L125:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write106_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.Int32, VRage.Serialization.SerializableDictionary`2<System.String, System.Single>> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L124:
ldloc.2
ldloc.s 10 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int32,VRage.Serialization.SerializableDictionary`2[System.String,System.Single]][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L125
nop
L123:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L126
L122:
ldloc.0
ldtoken Sandbox.Game.Entities.Blocks.MyStoreSellItemResults
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L127
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyStoreSellItemResults"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.Entities.Blocks.MyStoreSellItemResults
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write117_MyStoreSellItemResults(Sandbox.Game.Entities.Blocks.MyStoreSellItemResults v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L128
L127:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L129
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSerializableOrientedBoundingBoxD"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD]
stloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L130
nop
ldc.i4.0
stloc.2
br.s L131
L132:
nop
ldarg.0
ldstr "SerializableOrientedBoundingBoxD"
ldstr ""
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldloc.2
callvirt virtual VRage.SerializableOrientedBoundingBoxD System.Collections.Generic.List`1<VRage.SerializableOrientedBoundingBoxD>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write128_Item(System.String n, System.String ns, VRage.SerializableOrientedBoundingBoxD o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L131:
ldloc.2
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L132
nop
L130:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L133
L129:
ldloc.0
ldtoken System.Object[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L134
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfAnyType"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Object[]
stloc.s 16 (System.Object[])
ldloc.s 16 (System.Object[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L135
nop
ldc.i4.0
stloc.2
br.s L136
L137:
nop
ldarg.0
ldstr "anyType"
ldstr ""
ldloc.s 16 (System.Object[])
ldloc.2
ldelem.ref
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write138_Object(System.String n, System.String ns, System.Object o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L136:
ldloc.2
ldloc.s 16 (System.Object[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L137
nop
L135:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L138
L134:
ldloc.0
ldtoken VRage.ObjectBuilders.MyPersistentEntityFlags2
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L139
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPersistentEntityFlags2"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.ObjectBuilders.MyPersistentEntityFlags2
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write142_MyPersistentEntityFlags2(VRage.ObjectBuilders.MyPersistentEntityFlags2 v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L140
L139:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L141
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfComponentData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData]
stloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L142
nop
ldc.i4.0
stloc.2
br.s L143
L144:
nop
ldarg.0
ldstr "ComponentData"
ldstr ""
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.ComponentSystem.ComponentData System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.ComponentSystem.ComponentData>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write147_ComponentData(System.String n, System.String ns, VRage.Game.ObjectBuilders.ComponentSystem.ComponentData o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L143:
ldloc.2
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L144
nop
L142:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L145
L141:
ldloc.0
ldtoken System.Collections.Generic.List`1[Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L146
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_Waypoint"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint]
stloc.s 18 (System.Collections.Generic.List`1[Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint])
ldloc.s 18 (System.Collections.Generic.List`1[Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L147
nop
ldc.i4.0
stloc.2
br.s L148
L149:
nop
ldarg.0
ldstr "MyObjectBuilder_Waypoint"
ldstr ""
ldloc.s 18 (System.Collections.Generic.List`1[Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint])
ldloc.2
callvirt virtual Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint System.Collections.Generic.List`1<Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write151_MyObjectBuilder_Waypoint(System.String n, System.String ns, Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L148:
ldloc.2
ldloc.s 18 (System.Collections.Generic.List`1[Sandbox.Common.ObjectBuilders.MyObjectBuilder_Waypoint])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L149
nop
L147:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L150
L146:
ldloc.0
ldtoken System.String[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L151
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.String[]
stloc.s 19 (System.String[])
ldloc.s 19 (System.String[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L152
nop
ldc.i4.0
stloc.2
br.s L153
L154:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.s 19 (System.String[])
ldloc.2
ldelem.ref
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L153:
ldloc.2
ldloc.s 19 (System.String[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L154
nop
L152:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L155
L151:
ldloc.0
ldtoken VRage.Game.MyToolbarType
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L156
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyToolbarType"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyToolbarType
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write157_MyToolbarType(VRage.Game.MyToolbarType v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L157
L156:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L158
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSlot"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot]
stloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L159
nop
ldc.i4.0
stloc.2
br.s L160
L161:
nop
ldarg.0
ldstr "Slot"
ldstr ""
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldloc.2
callvirt virtual VRage.Game.Slot System.Collections.Generic.List`1<VRage.Game.Slot>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write158_Slot(System.String n, System.String ns, VRage.Game.Slot o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L160:
ldloc.2
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L161
nop
L159:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L162
L158:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRageMath.Vector3]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L163
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfVector3"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRageMath.Vector3]
stloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L164
nop
ldc.i4.0
stloc.2
br.s L165
L166:
nop
ldarg.0
ldstr "Vector3"
ldstr ""
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldloc.2
callvirt virtual VRageMath.Vector3 System.Collections.Generic.List`1<VRageMath.Vector3>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write159_Vector3(System.String n, System.String ns, VRageMath.Vector3 o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L165:
ldloc.2
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L166
nop
L164:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L167
L163:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L168
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfCameraControllerSettings"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings]
stloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L169
nop
ldc.i4.0
stloc.2
br.s L170
L171:
nop
ldarg.0
ldstr "CameraControllerSettings"
ldstr ""
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldloc.2
callvirt virtual VRage.Game.CameraControllerSettings System.Collections.Generic.List`1<VRage.Game.CameraControllerSettings>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write162_CameraControllerSettings(System.String n, System.String ns, VRage.Game.CameraControllerSettings o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L170:
ldloc.2
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L171
nop
L169:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L172
L168:
ldloc.0
ldtoken VRage.Game.ModAPI.MyPromoteLevel
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L173
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPromoteLevel"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyPromoteLevel
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write163_MyPromoteLevel(VRage.Game.ModAPI.MyPromoteLevel v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L174
L173:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L175
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfInt64CameraControllerSettings"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][]
stloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L176
nop
ldc.i4.0
stloc.2
br.s L177
L178:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write164_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.Int64, VRage.Game.CameraControllerSettings> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L177:
ldloc.2
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L178
nop
L176:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L179
L175:
ldloc.0
ldtoken VRage.Utils.MyGuiDrawAlignEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L180
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyGuiDrawAlignEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Utils.MyGuiDrawAlignEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write170_MyGuiDrawAlignEnum(VRage.Utils.MyGuiDrawAlignEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L181
L180:
ldloc.0
ldtoken VRage.Game.MyToolbarType
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L182
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyToolbarType"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyToolbarType
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write194_MyToolbarType(VRage.Game.MyToolbarType v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L183
L182:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L184
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSlot"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot]
stloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L185
nop
ldc.i4.0
stloc.2
br.s L186
L187:
nop
ldarg.0
ldstr "Slot"
ldstr ""
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldloc.2
callvirt virtual VRage.Game.Slot System.Collections.Generic.List`1<VRage.Game.Slot>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write195_Slot(System.String n, System.String ns, VRage.Game.Slot o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L186:
ldloc.2
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L187
nop
L185:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L188
L184:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRageMath.Vector3]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L189
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfVector3"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRageMath.Vector3]
stloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L190
nop
ldc.i4.0
stloc.2
br.s L191
L192:
nop
ldarg.0
ldstr "Vector3"
ldstr ""
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldloc.2
callvirt virtual VRageMath.Vector3 System.Collections.Generic.List`1<VRageMath.Vector3>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write196_Vector3(System.String n, System.String ns, VRageMath.Vector3 o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L191:
ldloc.2
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L192
nop
L190:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L193
L189:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L194
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfCameraControllerSettings"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings]
stloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L195
nop
ldc.i4.0
stloc.2
br.s L196
L197:
nop
ldarg.0
ldstr "CameraControllerSettings"
ldstr ""
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldloc.2
callvirt virtual VRage.Game.CameraControllerSettings System.Collections.Generic.List`1<VRage.Game.CameraControllerSettings>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write199_CameraControllerSettings(System.String n, System.String ns, VRage.Game.CameraControllerSettings o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L196:
ldloc.2
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L197
nop
L195:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L198
L194:
ldloc.0
ldtoken VRage.Game.ModAPI.MyPromoteLevel
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L199
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPromoteLevel"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyPromoteLevel
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write200_MyPromoteLevel(VRage.Game.ModAPI.MyPromoteLevel v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L200
L199:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L201
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfInt64CameraControllerSettings"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][]
stloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L202
nop
ldc.i4.0
stloc.2
br.s L203
L204:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write201_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.Int64, VRage.Game.CameraControllerSettings> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L203:
ldloc.2
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L204
nop
L202:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L205
L201:
ldloc.0
ldtoken System.String[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L206
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.String[]
stloc.s 19 (System.String[])
ldloc.s 19 (System.String[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L207
nop
ldc.i4.0
stloc.2
br.s L208
L209:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.s 19 (System.String[])
ldloc.2
ldelem.ref
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L208:
ldloc.2
ldloc.s 19 (System.String[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L209
nop
L207:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L210
L206:
ldloc.0
ldtoken VRage.Library.Utils.MyGameModeEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L211
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyGameModeEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Library.Utils.MyGameModeEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write212_MyGameModeEnum(VRage.Library.Utils.MyGameModeEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L212
L211:
ldloc.0
ldtoken VRage.Game.MyOnlineModeEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L213
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyOnlineModeEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyOnlineModeEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write213_MyOnlineModeEnum(VRage.Game.MyOnlineModeEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L214
L213:
ldloc.0
ldtoken VRage.Game.MyBlockLimitsEnabledEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L215
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyBlockLimitsEnabledEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyBlockLimitsEnabledEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write214_MyBlockLimitsEnabledEnum(VRage.Game.MyBlockLimitsEnabledEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L216
L215:
ldloc.0
ldtoken VRage.Game.MyEnvironmentHostilityEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L217
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyEnvironmentHostilityEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyEnvironmentHostilityEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write215_MyEnvironmentHostilityEnum(VRage.Game.MyEnvironmentHostilityEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L218
L217:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L219
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringInt16"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16][]
stloc.s 24 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16][])
ldloc.s 24 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L220
nop
ldc.i4.0
stloc.2
br.s L221
L222:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 24 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write216_EntryOfStringInt16(System.String n, System.String ns, VRage.Serialization.Entry<System.String, System.Int16> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L221:
ldloc.2
ldloc.s 24 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.Int16][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L222
nop
L220:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L223
L219:
ldloc.0
ldtoken System.Collections.Generic.List`1[System.String]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L224
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[System.String]
stloc.3
ldloc.3
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L225
nop
ldc.i4.0
stloc.2
br.s L226
L227:
nop
ldarg.0
ldstr "Warning"
ldstr ""
ldloc.3
ldloc.2
callvirt virtual System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32 index)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L226:
ldloc.2
ldloc.3
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L227
nop
L225:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L228
L224:
ldloc.0
ldtoken VRage.Game.MyObjectBuilder_SessionSettings+LimitBlocksByOption
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L229
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "LimitBlocksByOption"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyObjectBuilder_SessionSettings+LimitBlocksByOption
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write218_LimitBlocksByOption(VRage.Game.LimitBlocksByOption v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L230
L229:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.WorkshopId]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L231
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfWorkshopId"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.WorkshopId]
stloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L232
nop
ldc.i4.0
stloc.2
br.s L233
L234:
nop
ldarg.0
ldstr "WorkshopId"
ldstr ""
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
ldloc.2
callvirt virtual VRage.Game.WorkshopId System.Collections.Generic.List`1<VRage.Game.WorkshopId>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write220_WorkshopId(System.String n, System.String ns, VRage.Game.WorkshopId o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L233:
ldloc.2
ldloc.s 4 (System.Collections.Generic.List`1[VRage.Game.WorkshopId])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L234
nop
L232:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L235
L231:
ldloc.0
ldtoken System.Collections.Generic.List`1[System.String]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L236
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString1"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[System.String]
stloc.3
ldloc.3
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L237
nop
ldc.i4.0
stloc.2
br.s L238
L239:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.3
ldloc.2
callvirt virtual System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32 index)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L238:
ldloc.2
ldloc.3
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L239
nop
L237:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L240
L236:
ldloc.0
ldtoken VRageMath.Base6Directions+Direction
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L241
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "Direction"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRageMath.Base6Directions+Direction
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write235_Direction(VRageMath.Direction v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L242
L241:
ldloc.0
ldtoken System.Collections.Generic.HashSet`1[System.Int64]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L243
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfLong"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.HashSet`1[System.Int64]
stloc.s 25 (System.Collections.Generic.HashSet`1[System.Int64])
ldloc.s 25 (System.Collections.Generic.HashSet`1[System.Int64])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L244
nop
ldloc.s 25 (System.Collections.Generic.HashSet`1[System.Int64])
callvirt System.Collections.Generic.Enumerator<System.Int64> System.Collections.Generic.HashSet`1<System.Int64>::GetEnumerator()
box System.Collections.Generic.HashSet`1+Enumerator[System.Int64]
stloc.s 26 (System.Collections.IEnumerator)
ldloc.s 26 (System.Collections.IEnumerator)
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L245
br.s L246
L247:
nop
ldloc.s 26 (System.Collections.IEnumerator)
callvirt abstract virtual System.Object System.Collections.IEnumerator::get_Current()
unbox.any System.Int64
stloc.s 27 (System.Int64)
ldarg.0
ldstr "long"
ldstr ""
ldloc.s 27 (System.Int64)
call static System.String System.Xml.XmlConvert::ToString(System.Int64 value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
L246:
ldloc.s 26 (System.Collections.IEnumerator)
callvirt abstract virtual System.Boolean System.Collections.IEnumerator::MoveNext()
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L247
L245:
nop
L244:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L248
L243:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Identity]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L249
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_Identity"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Identity]
stloc.s 28 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Identity])
ldloc.s 28 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Identity])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L250
nop
ldc.i4.0
stloc.2
br.s L251
L252:
nop
ldarg.0
ldstr "MyObjectBuilder_Identity"
ldstr ""
ldloc.s 28 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Identity])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_Identity System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_Identity>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write241_MyObjectBuilder_Identity(System.String n, System.String ns, VRage.Game.MyObjectBuilder_Identity o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L251:
ldloc.2
ldloc.s 28 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Identity])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L252
nop
L250:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L253
L249:
ldloc.0
ldtoken VRage.Game.MyToolbarType
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L254
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyToolbarType"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyToolbarType
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write242_MyToolbarType(VRage.Game.MyToolbarType v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L255
L254:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L256
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSlot"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot]
stloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L257
nop
ldc.i4.0
stloc.2
br.s L258
L259:
nop
ldarg.0
ldstr "Slot"
ldstr ""
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
ldloc.2
callvirt virtual VRage.Game.Slot System.Collections.Generic.List`1<VRage.Game.Slot>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write243_Slot(System.String n, System.String ns, VRage.Game.Slot o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L258:
ldloc.2
ldloc.s 20 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Toolbar+Slot])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L259
nop
L257:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L260
L256:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRageMath.Vector3]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L261
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfVector3"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRageMath.Vector3]
stloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L262
nop
ldc.i4.0
stloc.2
br.s L263
L264:
nop
ldarg.0
ldstr "Vector3"
ldstr ""
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
ldloc.2
callvirt virtual VRageMath.Vector3 System.Collections.Generic.List`1<VRageMath.Vector3>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write244_Vector3(System.String n, System.String ns, VRageMath.Vector3 o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L263:
ldloc.2
ldloc.s 21 (System.Collections.Generic.List`1[VRageMath.Vector3])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L264
nop
L262:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L265
L261:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L266
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfCameraControllerSettings"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings]
stloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L267
nop
ldc.i4.0
stloc.2
br.s L268
L269:
nop
ldarg.0
ldstr "CameraControllerSettings"
ldstr ""
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
ldloc.2
callvirt virtual VRage.Game.CameraControllerSettings System.Collections.Generic.List`1<VRage.Game.CameraControllerSettings>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write247_CameraControllerSettings(System.String n, System.String ns, VRage.Game.CameraControllerSettings o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L268:
ldloc.2
ldloc.s 22 (System.Collections.Generic.List`1[VRage.Game.CameraControllerSettings])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L269
nop
L267:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L270
L266:
ldloc.0
ldtoken VRage.Game.ModAPI.MyPromoteLevel
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L271
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPromoteLevel"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyPromoteLevel
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write248_MyPromoteLevel(VRage.Game.ModAPI.MyPromoteLevel v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L272
L271:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L273
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfInt64CameraControllerSettings"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][]
stloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L274
nop
ldc.i4.0
stloc.2
br.s L275
L276:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write249_Item(System.String n, System.String ns, VRage.Serialization.Entry<System.Int64, VRage.Game.CameraControllerSettings> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L275:
ldloc.2
ldloc.s 23 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,VRage.Game.CameraControllerSettings][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L276
nop
L274:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L277
L273:
ldloc.0
ldtoken System.Collections.Generic.List`1[Sandbox.Game.Multiplayer.MyPlayerCollection+AllPlayerData]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L278
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfAllPlayerData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[Sandbox.Game.Multiplayer.MyPlayerCollection+AllPlayerData]
stloc.s 29 (System.Collections.Generic.List`1[Sandbox.Game.Multiplayer.MyPlayerCollection+AllPlayerData])
ldloc.s 29 (System.Collections.Generic.List`1[Sandbox.Game.Multiplayer.MyPlayerCollection+AllPlayerData])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L279
nop
ldc.i4.0
stloc.2
br.s L280
L281:
nop
ldarg.0
ldstr "AllPlayerData"
ldstr ""
ldloc.s 29 (System.Collections.Generic.List`1[Sandbox.Game.Multiplayer.MyPlayerCollection+AllPlayerData])
ldloc.2
callvirt virtual Sandbox.Game.Multiplayer.AllPlayerData System.Collections.Generic.List`1<Sandbox.Game.Multiplayer.AllPlayerData>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write252_AllPlayerData(System.String n, System.String ns, Sandbox.Game.Multiplayer.AllPlayerData o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L280:
ldloc.2
ldloc.s 29 (System.Collections.Generic.List`1[Sandbox.Game.Multiplayer.MyPlayerCollection+AllPlayerData])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L281
nop
L279:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L282
L278:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_FactionMember]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L283
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_FactionMember"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_FactionMember]
stloc.s 30 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_FactionMember])
ldloc.s 30 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_FactionMember])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L284
nop
ldc.i4.0
stloc.2
br.s L285
L286:
nop
ldarg.0
ldstr "MyObjectBuilder_FactionMember"
ldstr ""
ldloc.s 30 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_FactionMember])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_FactionMember System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_FactionMember>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write253_MyObjectBuilder_FactionMember(System.String n, System.String ns, VRage.Game.MyObjectBuilder_FactionMember o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L285:
ldloc.2
ldloc.s 30 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_FactionMember])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L286
nop
L284:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L287
L283:
ldloc.0
ldtoken VRage.Game.MyFactionTypes
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L288
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyFactionTypes"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyFactionTypes
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write254_MyFactionTypes(VRage.Game.MyFactionTypes v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L289
L288:
ldloc.0
ldtoken VRage.Game.MyStationTypeEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L290
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyStationTypeEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyStationTypeEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write256_MyStationTypeEnum(VRage.Game.MyStationTypeEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L291
L290:
ldloc.0
ldtoken VRage.Game.ObjectBuilders.Definitions.ItemTypes
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L292
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ItemTypes"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ObjectBuilders.Definitions.ItemTypes
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write258_ItemTypes(VRage.Game.ObjectBuilders.Definitions.ItemTypes v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L293
L292:
ldloc.0
ldtoken VRage.Game.ObjectBuilders.Definitions.StoreItemTypes
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L294
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "StoreItemTypes"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ObjectBuilders.Definitions.StoreItemTypes
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write259_StoreItemTypes(VRage.Game.ObjectBuilders.Definitions.StoreItemTypes v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L295
L294:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L296
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_StoreItem"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem]
stloc.s 31 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem])
ldloc.s 31 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L297
nop
ldc.i4.0
stloc.2
br.s L298
L299:
nop
ldarg.0
ldstr "MyObjectBuilder_StoreItem"
ldstr ""
ldloc.s 31 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write260_MyObjectBuilder_StoreItem(System.String n, System.String ns, VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L298:
ldloc.2
ldloc.s 31 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_StoreItem])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L299
nop
L297:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L300
L296:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Station]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L301
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_Station"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Station]
stloc.s 32 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Station])
ldloc.s 32 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Station])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L302
nop
ldc.i4.0
stloc.2
br.s L303
L304:
nop
ldarg.0
ldstr "MyObjectBuilder_Station"
ldstr ""
ldloc.s 32 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Station])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_Station System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_Station>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write261_MyObjectBuilder_Station(System.String n, System.String ns, VRage.Game.MyObjectBuilder_Station o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L303:
ldloc.2
ldloc.s 32 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Station])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L304
nop
L302:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L305
L301:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L306
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfInt64Single"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single][]
stloc.s 33 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single][])
ldloc.s 33 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L307
nop
ldc.i4.0
stloc.2
br.s L308
L309:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 33 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write263_EntryOfInt64Single(System.String n, System.String ns, VRage.Serialization.Entry<System.Int64, System.Single> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L308:
ldloc.2
ldloc.s 33 (VRage.Serialization.SerializableDictionary`2+Entry[System.Int64,System.Single][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L309
nop
L307:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L310
L306:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Faction]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L311
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_Faction"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Faction]
stloc.s 34 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Faction])
ldloc.s 34 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Faction])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L312
nop
ldc.i4.0
stloc.2
br.s L313
L314:
nop
ldarg.0
ldstr "MyObjectBuilder_Faction"
ldstr ""
ldloc.s 34 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Faction])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_Faction System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_Faction>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write265_MyObjectBuilder_Faction(System.String n, System.String ns, VRage.Game.MyObjectBuilder_Faction o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L313:
ldloc.2
ldloc.s 34 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Faction])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L314
nop
L312:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L315
L311:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Client]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L316
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_Client"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Client]
stloc.s 35 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Client])
ldloc.s 35 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Client])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L317
nop
ldc.i4.0
stloc.2
br.s L318
L319:
nop
ldarg.0
ldstr "MyObjectBuilder_Client"
ldstr ""
ldloc.s 35 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Client])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_Client System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_Client>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write266_MyObjectBuilder_Client(System.String n, System.String ns, VRage.Game.MyObjectBuilder_Client o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L318:
ldloc.2
ldloc.s 35 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_Client])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L319
nop
L317:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L320
L316:
ldloc.0
ldtoken System.Collections.Generic.HashSet`1[System.Int32]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L321
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfInt"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.HashSet`1[System.Int32]
stloc.s 36 (System.Collections.Generic.HashSet`1[System.Int32])
ldloc.s 36 (System.Collections.Generic.HashSet`1[System.Int32])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L322
nop
ldloc.s 36 (System.Collections.Generic.HashSet`1[System.Int32])
callvirt System.Collections.Generic.Enumerator<System.Int32> System.Collections.Generic.HashSet`1<System.Int32>::GetEnumerator()
box System.Collections.Generic.HashSet`1+Enumerator[System.Int32]
stloc.s 26 (System.Collections.IEnumerator)
ldloc.s 26 (System.Collections.IEnumerator)
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L323
br.s L324
L325:
nop
ldloc.s 26 (System.Collections.IEnumerator)
callvirt abstract virtual System.Object System.Collections.IEnumerator::get_Current()
unbox.any System.Int32
stloc.s 37 (System.Int32)
ldarg.0
ldstr "int"
ldstr ""
ldloc.s 37 (System.Int32)
call static System.String System.Xml.XmlConvert::ToString(System.Int32 value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
L324:
ldloc.s 26 (System.Collections.IEnumerator)
callvirt abstract virtual System.Boolean System.Collections.IEnumerator::MoveNext()
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L325
L323:
nop
L322:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L326
L321:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L327
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSerializableOrientedBoundingBoxD"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD]
stloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L328
nop
ldc.i4.0
stloc.2
br.s L329
L330:
nop
ldarg.0
ldstr "SerializableOrientedBoundingBoxD"
ldstr ""
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldloc.2
callvirt virtual VRage.SerializableOrientedBoundingBoxD System.Collections.Generic.List`1<VRage.SerializableOrientedBoundingBoxD>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write273_Item(System.String n, System.String ns, VRage.SerializableOrientedBoundingBoxD o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L329:
ldloc.2
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L330
nop
L328:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L331
L327:
ldloc.0
ldtoken VRage.Game.GUI.TextPanel.TextAlignment
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L332
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "TextAlignment"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.GUI.TextPanel.TextAlignment
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write289_TextAlignment(VRage.Game.GUI.TextPanel.TextAlignment v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L333
L332:
ldloc.0
ldtoken Sandbox.Game.Entities.Blocks.MyStoreBuyItemResults
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L334
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyStoreBuyItemResults"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.Entities.Blocks.MyStoreBuyItemResults
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write291_MyStoreBuyItemResults(Sandbox.Game.Entities.Blocks.MyStoreBuyItemResults v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L335
L334:
ldloc.0
ldtoken Sandbox.Game.SessionComponents.MySessionComponentWarningSystem+Category
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L336
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "Category"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.SessionComponents.MySessionComponentWarningSystem+Category
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write300_Category(Sandbox.Game.SessionComponents.Category v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L337
L336:
ldloc.0
ldtoken VRage.Game.ObjectBuilders.Definitions.ItemTypes
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L338
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ItemTypes"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ObjectBuilders.Definitions.ItemTypes
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write307_ItemTypes(VRage.Game.ObjectBuilders.Definitions.ItemTypes v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L339
L338:
ldloc.0
ldtoken VRage.Game.ObjectBuilders.Definitions.StoreItemTypes
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L340
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "StoreItemTypes"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ObjectBuilders.Definitions.StoreItemTypes
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write308_StoreItemTypes(VRage.Game.ObjectBuilders.Definitions.StoreItemTypes v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L341
L340:
ldloc.0
ldtoken VRage.Game.ModAPI.MyUpdateTiersPlayerPresence
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L342
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyUpdateTiersPlayerPresence"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyUpdateTiersPlayerPresence
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write314_MyUpdateTiersPlayerPresence(VRage.Game.ModAPI.MyUpdateTiersPlayerPresence v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L343
L342:
ldloc.0
ldtoken VRage.Game.ModAPI.MyUpdateTiersGridPresence
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L344
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyUpdateTiersGridPresence"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyUpdateTiersGridPresence
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write315_MyUpdateTiersGridPresence(VRage.Game.ModAPI.MyUpdateTiersGridPresence v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L345
L344:
ldloc.0
ldtoken Sandbox.Game.Gui.ChatChannel
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L346
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ChatChannel"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.Gui.ChatChannel
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write317_ChatChannel(Sandbox.Game.Gui.ChatChannel v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L347
L346:
ldloc.0
ldtoken VRage.Game.MyOwnershipShareModeEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L348
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyOwnershipShareModeEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyOwnershipShareModeEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write329_MyOwnershipShareModeEnum(VRage.Game.MyOwnershipShareModeEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L349
L348:
ldloc.0
ldtoken VRage.ObjectBuilders.MyPersistentEntityFlags2
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L350
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPersistentEntityFlags2"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.ObjectBuilders.MyPersistentEntityFlags2
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write336_MyPersistentEntityFlags2(VRage.ObjectBuilders.MyPersistentEntityFlags2 v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L351
L350:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L352
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfComponentData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData]
stloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L353
nop
ldc.i4.0
stloc.2
br.s L354
L355:
nop
ldarg.0
ldstr "ComponentData"
ldstr ""
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.ComponentSystem.ComponentData System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.ComponentSystem.ComponentData>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write341_ComponentData(System.String n, System.String ns, VRage.Game.ObjectBuilders.ComponentSystem.ComponentData o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L354:
ldloc.2
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L355
nop
L353:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L356
L352:
ldloc.0
ldtoken Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L357
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfCutOut"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut[]
stloc.s 38 (Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut[])
ldloc.s 38 (Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L358
nop
ldc.i4.0
stloc.2
br.s L359
L360:
nop
ldarg.0
ldstr "CutOut"
ldstr ""
ldloc.s 38 (Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut[])
ldloc.2
ldelema Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut
ldobj Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write352_CutOut(System.String n, System.String ns, Sandbox.Game.GameSystems.CutOut o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L359:
ldloc.2
ldloc.s 38 (Sandbox.Game.GameSystems.MyShipMiningSystem+NetworkCutoutData+CutOut[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L360
nop
L358:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L361
L357:
ldloc.0
ldtoken Sandbox.ModAPI.MyExplosionTypeEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L362
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyExplosionTypeEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.ModAPI.MyExplosionTypeEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write362_MyExplosionTypeEnum(Sandbox.ModAPI.MyExplosionTypeEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L363
L362:
ldloc.0
ldtoken Sandbox.Game.MyExplosionFlags
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L364
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyExplosionFlags"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any Sandbox.Game.MyExplosionFlags
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write363_MyExplosionFlags(Sandbox.Game.MyExplosionFlags v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L365
L364:
ldloc.0
ldtoken VRage.ObjectBuilders.MyPersistentEntityFlags2
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L366
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyPersistentEntityFlags2"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.ObjectBuilders.MyPersistentEntityFlags2
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write371_MyPersistentEntityFlags2(VRage.ObjectBuilders.MyPersistentEntityFlags2 v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L367
L366:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L368
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfComponentData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData]
stloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L369
nop
ldc.i4.0
stloc.2
br.s L370
L371:
nop
ldarg.0
ldstr "ComponentData"
ldstr ""
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.ComponentSystem.ComponentData System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.ComponentSystem.ComponentData>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write376_ComponentData(System.String n, System.String ns, VRage.Game.ObjectBuilders.ComponentSystem.ComponentData o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L370:
ldloc.2
ldloc.s 17 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer+ComponentData])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L371
nop
L369:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L372
L368:
ldloc.0
ldtoken VRage.Game.MyCubeSize
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L373
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyCubeSize"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyCubeSize
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write380_MyCubeSize(VRage.Game.MyCubeSize v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L374
L373:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeBlock]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L375
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyAbstractXmlSerializerOfMyObjectBuilder_CubeBlock"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeBlock]
stloc.s 39 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeBlock])
ldloc.s 39 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeBlock])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L376
nop
ldc.i4.0
stloc.2
br.s L377
L378:
nop
ldarg.0
ldloc.s 39 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeBlock])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_CubeBlock System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_CubeBlock>::get_Item(System.Int32 index)
call static VRage.MyAbstractXmlSerializer`1<VRage.Game.MyObjectBuilder_CubeBlock> VRage.MyAbstractXmlSerializer`1<VRage.Game.MyObjectBuilder_CubeBlock>::op_Implicit(VRage.Game.MyObjectBuilder_CubeBlock builder)
ldstr "MyObjectBuilder_CubeBlock"
ldstr ""
ldc.i4.1
ldc.i4.1
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteSerializable(System.Xml.Serialization.IXmlSerializable serializable, System.String name, System.String ns, System.Boolean isNullable, System.Boolean wrapped)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L377:
ldloc.2
ldloc.s 39 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeBlock])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L378
nop
L376:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L379
L375:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.BoneInfo]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L380
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfBoneInfo"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.BoneInfo]
stloc.s 40 (System.Collections.Generic.List`1[VRage.Game.BoneInfo])
ldloc.s 40 (System.Collections.Generic.List`1[VRage.Game.BoneInfo])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L381
nop
ldc.i4.0
stloc.2
br.s L382
L383:
nop
ldarg.0
ldstr "BoneInfo"
ldstr ""
ldloc.s 40 (System.Collections.Generic.List`1[VRage.Game.BoneInfo])
ldloc.2
callvirt virtual VRage.Game.BoneInfo System.Collections.Generic.List`1<VRage.Game.BoneInfo>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write383_BoneInfo(System.String n, System.String ns, VRage.Game.BoneInfo o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L382:
ldloc.2
ldloc.s 40 (System.Collections.Generic.List`1[VRage.Game.BoneInfo])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L383
nop
L381:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L384
L380:
ldloc.0
ldtoken VRageMath.Base6Directions+Direction
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L385
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "Direction"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRageMath.Base6Directions+Direction
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write384_Direction(VRageMath.Direction v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L386
L385:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorPacket]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L387
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_ConveyorPacket"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorPacket]
stloc.s 41 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorPacket])
ldloc.s 41 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorPacket])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L388
nop
ldc.i4.0
stloc.2
br.s L389
L390:
nop
ldarg.0
ldstr "MyObjectBuilder_ConveyorPacket"
ldstr ""
ldloc.s 41 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorPacket])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_ConveyorPacket System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_ConveyorPacket>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write386_MyObjectBuilder_ConveyorPacket(System.String n, System.String ns, VRage.Game.MyObjectBuilder_ConveyorPacket o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L389:
ldloc.2
ldloc.s 41 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorPacket])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L390
nop
L388:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L391
L387:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.SerializableLineSectionInformation]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L392
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSerializableLineSectionInformation"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.SerializableLineSectionInformation]
stloc.s 42 (System.Collections.Generic.List`1[VRage.Game.SerializableLineSectionInformation])
ldloc.s 42 (System.Collections.Generic.List`1[VRage.Game.SerializableLineSectionInformation])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L393
nop
ldc.i4.0
stloc.2
br.s L394
L395:
nop
ldarg.0
ldstr "Section"
ldstr ""
ldloc.s 42 (System.Collections.Generic.List`1[VRage.Game.SerializableLineSectionInformation])
ldloc.2
callvirt virtual VRage.Game.SerializableLineSectionInformation System.Collections.Generic.List`1<VRage.Game.SerializableLineSectionInformation>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write387_Item(System.String n, System.String ns, VRage.Game.SerializableLineSectionInformation o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L394:
ldloc.2
ldloc.s 42 (System.Collections.Generic.List`1[VRage.Game.SerializableLineSectionInformation])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L395
nop
L393:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L396
L392:
ldloc.0
ldtoken VRage.Game.MyObjectBuilder_ConveyorLine+LineType
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L397
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "LineType"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyObjectBuilder_ConveyorLine+LineType
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write388_LineType(VRage.Game.LineType v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L398
L397:
ldloc.0
ldtoken VRage.Game.MyObjectBuilder_ConveyorLine+LineConductivity
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L399
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "LineConductivity"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyObjectBuilder_ConveyorLine+LineConductivity
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write389_LineConductivity(VRage.Game.LineConductivity v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L400
L399:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorLine]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L401
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_ConveyorLine"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorLine]
stloc.s 43 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorLine])
ldloc.s 43 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorLine])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L402
nop
ldc.i4.0
stloc.2
br.s L403
L404:
nop
ldarg.0
ldstr "MyObjectBuilder_ConveyorLine"
ldstr ""
ldloc.s 43 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorLine])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_ConveyorLine System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_ConveyorLine>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write390_MyObjectBuilder_ConveyorLine(System.String n, System.String ns, VRage.Game.MyObjectBuilder_ConveyorLine o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L403:
ldloc.2
ldloc.s 43 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_ConveyorLine])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L404
nop
L402:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L405
L401:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRageMath.Vector3I]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L406
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfVector3I"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRageMath.Vector3I]
stloc.s 44 (System.Collections.Generic.List`1[VRageMath.Vector3I])
ldloc.s 44 (System.Collections.Generic.List`1[VRageMath.Vector3I])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L407
nop
ldc.i4.0
stloc.2
br.s L408
L409:
nop
ldarg.0
ldstr "Vector3I"
ldstr ""
ldloc.s 44 (System.Collections.Generic.List`1[VRageMath.Vector3I])
ldloc.2
callvirt virtual VRageMath.Vector3I System.Collections.Generic.List`1<VRageMath.Vector3I>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write391_Vector3I(System.String n, System.String ns, VRageMath.Vector3I o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L408:
ldloc.2
ldloc.s 44 (System.Collections.Generic.List`1[VRageMath.Vector3I])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L409
nop
L407:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L410
L406:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_BlockGroup]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L411
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_BlockGroup"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_BlockGroup]
stloc.s 45 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_BlockGroup])
ldloc.s 45 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_BlockGroup])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L412
nop
ldc.i4.0
stloc.2
br.s L413
L414:
nop
ldarg.0
ldstr "MyObjectBuilder_BlockGroup"
ldstr ""
ldloc.s 45 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_BlockGroup])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_BlockGroup System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_BlockGroup>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write392_MyObjectBuilder_BlockGroup(System.String n, System.String ns, VRage.Game.MyObjectBuilder_BlockGroup o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L413:
ldloc.2
ldloc.s 45 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_BlockGroup])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L414
nop
L412:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L415
L411:
ldloc.0
ldtoken System.Single[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L416
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfFloat"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Single[]
stloc.s 46 (System.Single[])
ldloc.s 46 (System.Single[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L417
nop
ldc.i4.0
stloc.2
br.s L418
L419:
nop
ldarg.0
ldstr "float"
ldstr ""
ldloc.s 46 (System.Single[])
ldloc.2
ldelem.r4
conv.r4
conv.r4
call static System.String System.Xml.XmlConvert::ToString(System.Single value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L418:
ldloc.2
ldloc.s 46 (System.Single[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L419
nop
L417:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L420
L416:
ldloc.0
ldtoken System.Collections.Generic.List`1[System.Int64]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L421
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfLong"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[System.Int64]
stloc.s 47 (System.Collections.Generic.List`1[System.Int64])
ldloc.s 47 (System.Collections.Generic.List`1[System.Int64])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L422
nop
ldc.i4.0
stloc.2
br.s L423
L424:
nop
ldarg.0
ldstr "long"
ldstr ""
ldloc.s 47 (System.Collections.Generic.List`1[System.Int64])
ldloc.2
callvirt virtual System.Int64 System.Collections.Generic.List`1<System.Int64>::get_Item(System.Int32 index)
call static System.String System.Xml.XmlConvert::ToString(System.Int64 value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L423:
ldloc.2
ldloc.s 47 (System.Collections.Generic.List`1[System.Int64])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L424
nop
L422:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L425
L421:
ldloc.0
ldtoken VRage.Game.OxygenRoom[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L426
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfOxygenRoom"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.OxygenRoom[]
stloc.s 48 (VRage.Game.OxygenRoom[])
ldloc.s 48 (VRage.Game.OxygenRoom[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L427
nop
ldc.i4.0
stloc.2
br.s L428
L429:
nop
ldarg.0
ldstr "OxygenRoom"
ldstr ""
ldloc.s 48 (VRage.Game.OxygenRoom[])
ldloc.2
ldelema VRage.Game.OxygenRoom
ldobj VRage.Game.OxygenRoom
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write394_OxygenRoom(System.String n, System.String ns, VRage.Game.OxygenRoom o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L428:
ldloc.2
ldloc.s 48 (VRage.Game.OxygenRoom[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L429
nop
L427:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L430
L426:
ldloc.0
ldtoken VRage.Game.ModAPI.MyUpdateTiersGridPresence
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L431
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyUpdateTiersGridPresence"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyUpdateTiersGridPresence
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write395_MyUpdateTiersGridPresence(VRage.Game.ModAPI.MyUpdateTiersGridPresence v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L432
L431:
ldloc.0
ldtoken VRage.Game.ModAPI.MyUpdateTiersPlayerPresence
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L433
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyUpdateTiersPlayerPresence"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.ModAPI.MyUpdateTiersPlayerPresence
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write396_MyUpdateTiersPlayerPresence(VRage.Game.ModAPI.MyUpdateTiersPlayerPresence v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L434
L433:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeGrid]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L435
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_CubeGrid"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeGrid]
stloc.s 49 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeGrid])
ldloc.s 49 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeGrid])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L436
nop
ldc.i4.0
stloc.2
br.s L437
L438:
nop
ldarg.0
ldstr "MyObjectBuilder_CubeGrid"
ldstr ""
ldloc.s 49 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeGrid])
ldloc.2
callvirt virtual VRage.Game.MyObjectBuilder_CubeGrid System.Collections.Generic.List`1<VRage.Game.MyObjectBuilder_CubeGrid>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write397_MyObjectBuilder_CubeGrid(System.String n, System.String ns, VRage.Game.MyObjectBuilder_CubeGrid o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L437:
ldloc.2
ldloc.s 49 (System.Collections.Generic.List`1[VRage.Game.MyObjectBuilder_CubeGrid])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L438
nop
L436:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L439
L435:
ldloc.0
ldtoken System.Collections.Generic.List`1[System.UInt64]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L440
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfUnsignedLong"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[System.UInt64]
stloc.s 50 (System.Collections.Generic.List`1[System.UInt64])
ldloc.s 50 (System.Collections.Generic.List`1[System.UInt64])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L441
nop
ldc.i4.0
stloc.2
br.s L442
L443:
nop
ldarg.0
ldstr "unsignedLong"
ldstr ""
ldloc.s 50 (System.Collections.Generic.List`1[System.UInt64])
ldloc.2
callvirt virtual System.UInt64 System.Collections.Generic.List`1<System.UInt64>::get_Item(System.Int32 index)
call static System.String System.Xml.XmlConvert::ToString(System.UInt64 value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L442:
ldloc.2
ldloc.s 50 (System.Collections.Generic.List`1[System.UInt64])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L443
nop
L441:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L444
L440:
ldloc.0
ldtoken VRageMath.Base6Directions+Direction
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L445
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "Direction"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRageMath.Base6Directions+Direction
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write403_Direction(VRageMath.Direction v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L446
L445:
ldloc.0
ldtoken Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L447
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfModule"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module[]
stloc.1
ldloc.1
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L448
nop
ldc.i4.0
stloc.2
br.s L449
L450:
nop
ldarg.0
ldstr "Module"
ldstr ""
ldloc.1
ldloc.2
ldelema Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module
ldobj Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_ProceduralEnvironmentSector+Module
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write407_Module(System.String n, System.String ns, Sandbox.Game.WorldEnvironment.ObjectBuilders.Module o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L449:
ldloc.2
ldloc.1
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L450
nop
L448:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L451
L447:
ldloc.0
ldtoken VRageMath.Base6Directions+Direction
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L452
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "Direction"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRageMath.Base6Directions+Direction
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write412_Direction(VRageMath.Direction v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L453
L452:
ldloc.0
ldtoken VRage.Game.MyItemFlags
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L454
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyItemFlags"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Game.MyItemFlags
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write422_MyItemFlags(VRage.Game.MyItemFlags v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L455
L454:
ldloc.0
ldtoken VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L456
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_AreaTrigger"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger[]
stloc.s 51 (VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger[])
ldloc.s 51 (VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L457
nop
ldc.i4.0
stloc.2
br.s L458
L459:
nop
ldarg.0
ldstr "MyObjectBuilder_AreaTrigger"
ldstr ""
ldloc.s 51 (VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger[])
ldloc.2
ldelem.ref
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write434_MyObjectBuilder_AreaTrigger(System.String n, System.String ns, VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L458:
ldloc.2
ldloc.s 51 (VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_AreaTrigger[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L459
nop
L457:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L460
L456:
ldloc.0
ldtoken System.Int64[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L461
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfLong"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Int64[]
stloc.s 52 (System.Int64[])
ldloc.s 52 (System.Int64[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L462
nop
ldc.i4.0
stloc.2
br.s L463
L464:
nop
ldarg.0
ldstr "long"
ldstr ""
ldloc.s 52 (System.Int64[])
ldloc.2
ldelem.i8
call static System.String System.Xml.XmlConvert::ToString(System.Int64 value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L463:
ldloc.2
ldloc.s 52 (System.Int64[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L464
nop
L462:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L465
L461:
ldloc.0
ldtoken System.String[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L466
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.String[]
stloc.s 19 (System.String[])
ldloc.s 19 (System.String[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L467
nop
ldc.i4.0
stloc.2
br.s L468
L469:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.s 19 (System.String[])
ldloc.2
ldelem.ref
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L468:
ldloc.2
ldloc.s 19 (System.String[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L469
nop
L467:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L470
L466:
ldloc.0
ldtoken VRage.Game.VisualScripting.MyDebuggingNodeLog[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L471
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyDebuggingNodeLog"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.VisualScripting.MyDebuggingNodeLog[]
stloc.s 53 (VRage.Game.VisualScripting.MyDebuggingNodeLog[])
ldloc.s 53 (VRage.Game.VisualScripting.MyDebuggingNodeLog[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L472
nop
ldc.i4.0
stloc.2
br.s L473
L474:
nop
ldarg.0
ldstr "MyDebuggingNodeLog"
ldstr ""
ldloc.s 53 (VRage.Game.VisualScripting.MyDebuggingNodeLog[])
ldloc.2
ldelema VRage.Game.VisualScripting.MyDebuggingNodeLog
ldobj VRage.Game.VisualScripting.MyDebuggingNodeLog
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write455_MyDebuggingNodeLog(System.String n, System.String ns, VRage.Game.VisualScripting.MyDebuggingNodeLog o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L473:
ldloc.2
ldloc.s 53 (VRage.Game.VisualScripting.MyDebuggingNodeLog[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L474
nop
L472:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L475
L471:
ldloc.0
ldtoken VRage.Game.VisualScripting.MyDebuggingStateMachine[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L476
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyDebuggingStateMachine"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.VisualScripting.MyDebuggingStateMachine[]
stloc.s 54 (VRage.Game.VisualScripting.MyDebuggingStateMachine[])
ldloc.s 54 (VRage.Game.VisualScripting.MyDebuggingStateMachine[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L477
nop
ldc.i4.0
stloc.2
br.s L478
L479:
nop
ldarg.0
ldstr "MyDebuggingStateMachine"
ldstr ""
ldloc.s 54 (VRage.Game.VisualScripting.MyDebuggingStateMachine[])
ldloc.2
ldelema VRage.Game.VisualScripting.MyDebuggingStateMachine
ldobj VRage.Game.VisualScripting.MyDebuggingStateMachine
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write456_MyDebuggingStateMachine(System.String n, System.String ns, VRage.Game.VisualScripting.MyDebuggingStateMachine o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L478:
ldloc.2
ldloc.s 54 (VRage.Game.VisualScripting.MyDebuggingStateMachine[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L479
nop
L477:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L480
L476:
ldloc.0
ldtoken VRageMath.Base6Directions+Direction
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L481
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "Direction"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRageMath.Base6Directions+Direction
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write462_Direction(VRageMath.Direction v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L482
L481:
ldloc.0
ldtoken System.String[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L483
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.String[]
stloc.s 19 (System.String[])
ldloc.s 19 (System.String[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L484
nop
ldc.i4.0
stloc.2
br.s L485
L486:
nop
ldarg.0
ldstr "FilePath"
ldstr ""
ldloc.s 19 (System.String[])
ldloc.2
ldelem.ref
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L485:
ldloc.2
ldloc.s 19 (System.String[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L486
nop
L484:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L487
L483:
ldloc.0
ldtoken VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L488
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_ScriptSMCursor"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor[]
stloc.s 55 (VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor[])
ldloc.s 55 (VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L489
nop
ldc.i4.0
stloc.2
br.s L490
L491:
nop
ldarg.0
ldstr "MyObjectBuilder_ScriptSMCursor"
ldstr ""
ldloc.s 55 (VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor[])
ldloc.2
ldelem.ref
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write468_MyObjectBuilder_ScriptSMCursor(System.String n, System.String ns, VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L490:
ldloc.2
ldloc.s 55 (VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptSMCursor[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L491
nop
L489:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L492
L488:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptStateMachineManager+CursorStruct]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L493
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfCursorStruct"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptStateMachineManager+CursorStruct]
stloc.s 56 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptStateMachineManager+CursorStruct])
ldloc.s 56 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptStateMachineManager+CursorStruct])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L494
nop
ldc.i4.0
stloc.2
br.s L495
L496:
nop
ldarg.0
ldstr "CursorStruct"
ldstr ""
ldloc.s 56 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptStateMachineManager+CursorStruct])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.VisualScripting.CursorStruct System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.VisualScripting.CursorStruct>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write469_CursorStruct(System.String n, System.String ns, VRage.Game.ObjectBuilders.VisualScripting.CursorStruct o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L495:
ldloc.2
ldloc.s 56 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.VisualScripting.MyObjectBuilder_ScriptStateMachineManager+CursorStruct])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L496
nop
L494:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L497
L493:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MultilineData]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L498
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMultilineData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MultilineData]
stloc.s 57 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MultilineData])
ldloc.s 57 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MultilineData])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L499
nop
ldc.i4.0
stloc.2
br.s L500
L501:
nop
ldarg.0
ldstr "MultilineData"
ldstr ""
ldloc.s 57 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MultilineData])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.Gui.MultilineData System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.Gui.MultilineData>::get_Item(System.Int32 index)
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write471_MultilineData(System.String n, System.String ns, VRage.Game.ObjectBuilders.Gui.MultilineData o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L500:
ldloc.2
ldloc.s 57 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MultilineData])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L501
nop
L499:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L502
L498:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MyHighlightData]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L503
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyHighlightData"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MyHighlightData]
stloc.s 58 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MyHighlightData])
ldloc.s 58 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MyHighlightData])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L504
nop
ldc.i4.0
stloc.2
br.s L505
L506:
nop
ldarg.0
ldstr "MyHighlightData"
ldstr ""
ldloc.s 58 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MyHighlightData])
ldloc.2
callvirt virtual VRage.Game.ObjectBuilders.Gui.MyHighlightData System.Collections.Generic.List`1<VRage.Game.ObjectBuilders.Gui.MyHighlightData>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write474_MyHighlightData(System.String n, System.String ns, VRage.Game.ObjectBuilders.Gui.MyHighlightData o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L505:
ldloc.2
ldloc.s 58 (System.Collections.Generic.List`1[VRage.Game.ObjectBuilders.Gui.MyHighlightData])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L506
nop
L504:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L507
L503:
ldloc.0
ldtoken System.String[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L508
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfString1"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.String[]
stloc.s 19 (System.String[])
ldloc.s 19 (System.String[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L509
nop
ldc.i4.0
stloc.2
br.s L510
L511:
nop
ldarg.0
ldstr "string"
ldstr ""
ldloc.s 19 (System.String[])
ldloc.2
ldelem.ref
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteNullableStringLiteral(System.String name, System.String ns, System.String value)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L510:
ldloc.2
ldloc.s 19 (System.String[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L511
nop
L509:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L512
L508:
ldloc.0
ldtoken VRage.Utils.MyGuiDrawAlignEnum
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L513
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "MyGuiDrawAlignEnum"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.0
ldarg.3
unbox.any VRage.Utils.MyGuiDrawAlignEnum
call System.String Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write477_MyGuiDrawAlignEnum(VRage.Utils.MyGuiDrawAlignEnum v)
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteString(System.String text)
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L514
L513:
ldloc.0
ldtoken VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L515
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfBoardColumn"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn[]
stloc.s 59 (VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn[])
ldloc.s 59 (VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L516
nop
ldc.i4.0
stloc.2
br.s L517
L518:
nop
ldarg.0
ldstr "BoardColumn"
ldstr ""
ldloc.s 59 (VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn[])
ldloc.2
ldelema VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn
ldobj VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write478_BoardColumn(System.String n, System.String ns, VRage.Game.BoardColumn o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L517:
ldloc.2
ldloc.s 59 (VRage.Game.MyObjectBuilder_BoardScreen+BoardColumn[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L518
nop
L516:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L519
L515:
ldloc.0
ldtoken VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L520
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfEntryOfStringString"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][]
stloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L521
nop
ldc.i4.0
stloc.2
br.s L522
L523:
nop
ldarg.0
ldstr "item"
ldstr ""
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldloc.2
ldelema VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String]
ldobj VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String]
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write479_EntryOfStringString(System.String n, System.String ns, VRage.Serialization.Entry<System.String, System.String> o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L522:
ldloc.2
ldloc.s 6 (VRage.Serialization.SerializableDictionary`2+Entry[System.String,System.String][])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L523
nop
L521:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L524
L520:
ldloc.0
ldtoken VRage.Game.MyObjectBuilder_BoardScreen+BoardRow[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L525
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfBoardRow"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.MyObjectBuilder_BoardScreen+BoardRow[]
stloc.s 60 (VRage.Game.MyObjectBuilder_BoardScreen+BoardRow[])
ldloc.s 60 (VRage.Game.MyObjectBuilder_BoardScreen+BoardRow[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L526
nop
ldc.i4.0
stloc.2
br.s L527
L528:
nop
ldarg.0
ldstr "BoardRow"
ldstr ""
ldloc.s 60 (VRage.Game.MyObjectBuilder_BoardScreen+BoardRow[])
ldloc.2
ldelema VRage.Game.MyObjectBuilder_BoardScreen+BoardRow
ldobj VRage.Game.MyObjectBuilder_BoardScreen+BoardRow
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write481_BoardRow(System.String n, System.String ns, VRage.Game.BoardRow o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L527:
ldloc.2
ldloc.s 60 (VRage.Game.MyObjectBuilder_BoardScreen+BoardRow[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L528
nop
L526:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L529
L525:
ldloc.0
ldtoken VRage.Game.MyObjectBuilder_BoardScreen[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L530
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfMyObjectBuilder_BoardScreen"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass VRage.Game.MyObjectBuilder_BoardScreen[]
stloc.s 61 (VRage.Game.MyObjectBuilder_BoardScreen[])
ldloc.s 61 (VRage.Game.MyObjectBuilder_BoardScreen[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L531
nop
ldc.i4.0
stloc.2
br.s L532
L533:
nop
ldarg.0
ldstr "MyObjectBuilder_BoardScreen"
ldstr ""
ldloc.s 61 (VRage.Game.MyObjectBuilder_BoardScreen[])
ldloc.2
ldelem.ref
ldc.i4.1
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write482_MyObjectBuilder_BoardScreen(System.String n, System.String ns, VRage.Game.MyObjectBuilder_BoardScreen o, System.Boolean isNullable, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L532:
ldloc.2
ldloc.s 61 (VRage.Game.MyObjectBuilder_BoardScreen[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L533
nop
L531:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L534
L530:
ldloc.0
ldtoken System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L535
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSerializableOrientedBoundingBoxD"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD]
stloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L536
nop
ldc.i4.0
stloc.2
br.s L537
L538:
nop
ldarg.0
ldstr "SerializableOrientedBoundingBoxD"
ldstr ""
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
ldloc.2
callvirt virtual VRage.SerializableOrientedBoundingBoxD System.Collections.Generic.List`1<VRage.SerializableOrientedBoundingBoxD>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write490_Item(System.String n, System.String ns, VRage.SerializableOrientedBoundingBoxD o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L537:
ldloc.2
ldloc.s 15 (System.Collections.Generic.List`1[VRage.SerializableOrientedBoundingBoxD])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L538
nop
L536:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L539
L535:
ldloc.0
ldtoken System.Collections.Generic.List`1[Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_PlanetEnvironmentComponent+ObstructingBox]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L540
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfObstructingBox"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.List`1[Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_PlanetEnvironmentComponent+ObstructingBox]
stloc.s 62 (System.Collections.Generic.List`1[Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_PlanetEnvironmentComponent+ObstructingBox])
ldloc.s 62 (System.Collections.Generic.List`1[Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_PlanetEnvironmentComponent+ObstructingBox])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L541
nop
ldc.i4.0
stloc.2
br.s L542
L543:
nop
ldarg.0
ldstr "Sector"
ldstr ""
ldloc.s 62 (System.Collections.Generic.List`1[Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_PlanetEnvironmentComponent+ObstructingBox])
ldloc.2
callvirt virtual Sandbox.Game.WorldEnvironment.ObjectBuilders.ObstructingBox System.Collections.Generic.List`1<Sandbox.Game.WorldEnvironment.ObjectBuilders.ObstructingBox>::get_Item(System.Int32 index)
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write491_ObstructingBox(System.String n, System.String ns, Sandbox.Game.WorldEnvironment.ObjectBuilders.ObstructingBox o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L542:
ldloc.2
ldloc.s 62 (System.Collections.Generic.List`1[Sandbox.Game.WorldEnvironment.ObjectBuilders.MyObjectBuilder_PlanetEnvironmentComponent+ObstructingBox])
callvirt abstract virtual System.Int32 System.Collections.ICollection::get_Count()
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L543
nop
L541:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L544
L540:
ldloc.0
ldtoken System.Collections.Generic.HashSet`1[System.Int32]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue L545
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfInt"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass System.Collections.Generic.HashSet`1[System.Int32]
stloc.s 36 (System.Collections.Generic.HashSet`1[System.Int32])
ldloc.s 36 (System.Collections.Generic.HashSet`1[System.Int32])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L546
nop
ldloc.s 36 (System.Collections.Generic.HashSet`1[System.Int32])
callvirt System.Collections.Generic.Enumerator<System.Int32> System.Collections.Generic.HashSet`1<System.Int32>::GetEnumerator()
box System.Collections.Generic.HashSet`1+Enumerator[System.Int32]
stloc.s 26 (System.Collections.IEnumerator)
ldloc.s 26 (System.Collections.IEnumerator)
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L547
br.s L548
L549:
nop
ldloc.s 26 (System.Collections.IEnumerator)
callvirt abstract virtual System.Object System.Collections.IEnumerator::get_Current()
unbox.any System.Int32
stloc.s 37 (System.Int32)
ldarg.0
ldstr "int"
ldstr ""
ldloc.s 37 (System.Int32)
call static System.String System.Xml.XmlConvert::ToString(System.Int32 value)
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteElementStringRaw(System.String localName, System.String ns, System.String value)
nop
nop
L548:
ldloc.s 26 (System.Collections.IEnumerator)
callvirt abstract virtual System.Boolean System.Collections.IEnumerator::MoveNext()
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L549
L547:
nop
L546:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br L550
L545:
ldloc.0
ldtoken Sandbox.Game.Debugging.SimpleEntityInfo[]
call static System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle handle)
call static System.Boolean System.Type::op_Equality(System.Type left, System.Type right)
ldc.i4.0
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L551
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
ldarg.1
ldarg.2
callvirt System.Void System.Xml.XmlWriter::WriteStartElement(System.String localName, System.String ns)
nop
ldarg.0
ldstr "ArrayOfSimpleEntityInfo"
ldstr ""
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteXsiType(System.String name, System.String ns)
nop
nop
ldarg.3
castclass Sandbox.Game.Debugging.SimpleEntityInfo[]
stloc.s 63 (Sandbox.Game.Debugging.SimpleEntityInfo[])
ldloc.s 63 (Sandbox.Game.Debugging.SimpleEntityInfo[])
ldnull
ceq
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L552
nop
ldc.i4.0
stloc.2
br.s L553
L554:
nop
ldarg.0
ldstr "SimpleEntityInfo"
ldstr ""
ldloc.s 63 (Sandbox.Game.Debugging.SimpleEntityInfo[])
ldloc.2
ldelema Sandbox.Game.Debugging.SimpleEntityInfo
ldobj Sandbox.Game.Debugging.SimpleEntityInfo
ldc.i4.0
call System.Void Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1::Write524_SimpleEntityInfo(System.String n, System.String ns, Sandbox.Game.Debugging.SimpleEntityInfo o, System.Boolean needType)
nop
nop
ldloc.2
ldc.i4.1
add
stloc.2
L553:
ldloc.2
ldloc.s 63 (Sandbox.Game.Debugging.SimpleEntityInfo[])
ldlen
conv.i4
clt
stloc.s 64 (System.Boolean)
ldloc.s 64 (System.Boolean)
brtrue.s L554
nop
L552:
nop
ldarg.0
call System.Xml.XmlWriter System.Xml.Serialization.XmlSerializationWriter::get_Writer()
callvirt abstract virtual System.Void System.Xml.XmlWriter::WriteEndElement()
nop
br.s L555
L551:
nop
ldarg.0
ldarg.1
ldarg.2
ldarg.3
ldc.i4.1
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteTypedPrimitive(System.String name, System.String ns, System.Object o, System.Boolean xsiType)
nop
br.s L556
L5:
nop
L3:
ldarg.0
ldarg.1
ldarg.2
ldarg.3
ldc.i4.0
ldnull
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteStartElement(System.String name, System.String ns, System.Object o, System.Boolean writePrefixed, System.Xml.Serialization.XmlSerializerNamespaces xmlns)
nop
ldarg.0
ldarg.3
call System.Void System.Xml.Serialization.XmlSerializationWriter::WriteEndElement(System.Object o)
nop
L2:
L7:
L9:
L11:
L13:
L15:
L17:
L22:
L27:
L32:
L37:
L42:
L47:
L52:
L57:
L62:
L67:
L72:
L77:
L79:
L81:
L86:
L91:
L96:
L101:
L106:
L111:
L116:
L121:
L126:
L128:
L133:
L138:
L140:
L145:
L150:
L155:
L157:
L162:
L167:
L172:
L174:
L179:
L181:
L183:
L188:
L193:
L198:
L200:
L205:
L210:
L212:
L214:
L216:
L218:
L223:
L228:
L230:
L235:
L240:
L242:
L248:
L253:
L255:
L260:
L265:
L270:
L272:
L277:
L282:
L287:
L289:
L291:
L293:
L295:
L300:
L305:
L310:
L315:
L320:
L326:
L331:
L333:
L335:
L337:
L339:
L341:
L343:
L345:
L347:
L349:
L351:
L356:
L361:
L363:
L365:
L367:
L372:
L374:
L379:
L384:
L386:
L391:
L396:
L398:
L400:
L405:
L410:
L415:
L420:
L425:
L430:
L432:
L434:
L439:
L444:
L446:
L451:
L453:
L455:
L460:
L465:
L470:
L475:
L480:
L482:
L487:
L492:
L497:
L502:
L507:
L512:
L514:
L519:
L524:
L529:
L534:
L539:
L544:
L550:
L555:
L556:
ret
