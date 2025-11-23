using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Medieval.ObjectBuilders;
using Medieval.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Definitions;
using VRage;
using VRage.Data.Audio;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Debugging;
using VRage.Game.GUI;
using VRage.Game.GUI.TextPanel;
using VRage.Game.Gui;
using VRage.Game.ModAPI;
using VRage.Game.ObjectBuilders;
using VRage.Game.ObjectBuilders.AI;
using VRage.Game.ObjectBuilders.AI.Bot;
using VRage.Game.ObjectBuilders.Animation;
using VRage.Game.ObjectBuilders.Campaign;
using VRage.Game.ObjectBuilders.ComponentSystem;
using VRage.Game.ObjectBuilders.Components;
using VRage.Game.ObjectBuilders.Components.BankingAndCurrency;
using VRage.Game.ObjectBuilders.Components.Beacon;
using VRage.Game.ObjectBuilders.Components.Contracts;
using VRage.Game.ObjectBuilders.Components.Trading;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ObjectBuilders.Definitions.Factions;
using VRage.Game.ObjectBuilders.Definitions.Reputation;
using VRage.Game.ObjectBuilders.Definitions.SessionComponents;
using VRage.Game.ObjectBuilders.Gui;
using VRage.Game.ObjectBuilders.VisualScripting;
using VRage.Game.VisualScripting;
using VRage.Input;
using VRage.Library.Utils;
using VRage.ModAPI;
using VRage.ObjectBuilder;
using VRage.ObjectBuilders;
using VRage.ObjectBuilders.Components;
using VRage.ObjectBuilders.Definitions;
using VRage.ObjectBuilders.Definitions.Components;
using VRage.ObjectBuilders.Voxels;
using VRage.Profiler;
using VRage.Render.Particles;
using VRage.Serialization;
using VRage.Utils;
using VRageMath;
using VRageRender;
using VRageRender.Animations;
using VRageRender.Messages;

namespace Microsoft.Xml.Serialization.GeneratedAssembly;

public class XmlSerializationWriter1 : XmlSerializationWriter
{
private static readonly Dictionary<Type, Action<XmlSerializationWriter1, string, string, object, bool>> Write2977_Object_TypeHandlers_1 =
        new Dictionary<Type, Action<XmlSerializationWriter1, string, string, object, bool>>
        {
            { typeof(MyAtmosphereSettings), (w, n, ns, o, nt) => { Write3076_MyAtmosphereSettings(n, ns, (MyAtmosphereSettings)o, needType: nt); } },
            { typeof(MyObjectBuilder_Planet.SavedSector), (w, n, ns, o, nt) => { Write3075_SavedSector(n, ns, (MyObjectBuilder_Planet.SavedSector)o, needType: nt); } },
            { typeof(Vector3B), (w, n, ns, o, nt) => { Write3074_Vector3B(n, ns, (Vector3B)o, needType: nt); } },
            { typeof(Vector3S), (w, n, ns, o, nt) => { Write3073_Vector3S(n, ns, (Vector3S)o, needType: nt); } },
            { typeof(MyObjectBuilder_ComponentContainer.ComponentData), (w, n, ns, o, nt) => { Write3069_ComponentData(n, ns, (MyObjectBuilder_ComponentContainer.ComponentData)o, true, needType: nt); } },
            { typeof(BoundingBoxD), (w, n, ns, o, nt) => { Write3067_BoundingBoxD(n, ns, (BoundingBoxD)o, needType: nt); } },
            { typeof(SerializableDictionary<string, byte[]>), (w, n, ns, o, nt) => { Write3066_Item(n, ns, (SerializableDictionary<string, byte[]>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<string, byte[]>.Entry), (w, n, ns, o, nt) => { Write3065_EntryOfStringArrayOfByte(n, ns, (SerializableDictionary<string, byte[]>.Entry)o, needType: nt); } },
            { typeof(Vector3I), (w, n, ns, o, nt) => { Write3059_Vector3I(n, ns, (Vector3I)o, needType: nt); } },
            { typeof(SerializableDictionary<ulong, int>), (w, n, ns, o, nt) => { Write3055_Item(n, ns, (SerializableDictionary<ulong, int>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<ulong, int>.Entry), (w, n, ns, o, nt) => { Write3054_EntryOfUInt64Int32(n, ns, (SerializableDictionary<ulong, int>.Entry)o, needType: nt); } },
            { typeof(MyObjectBuilder_Checkpoint.PlayerItem), (w, n, ns, o, nt) => { Write3053_PlayerItem(n, ns, (MyObjectBuilder_Checkpoint.PlayerItem)o, needType: nt); } },
            { typeof(SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, long>), (w, n, ns, o, nt) => { Write3052_Item(n, ns, (SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, long>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, long>.Entry), (w, n, ns, o, nt) => { Write3051_EntryOfPlayerIdInt64(n, ns, (SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, long>.Entry)o, needType: nt); } },
            { typeof(SerializableBoundingBoxD), (w, n, ns, o, nt) => { Write3050_SerializableBoundingBoxD(n, ns, (SerializableBoundingBoxD)o, needType: nt); } },
            { typeof(SerializableDictionary<ulong, List<long>>), (w, n, ns, o, nt) => { Write3049_Item(n, ns, (SerializableDictionary<ulong, List<long>>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<ulong, List<long>>.Entry), (w, n, ns, o, nt) => { Write3048_EntryOfUInt64ListOfInt64(n, ns, (SerializableDictionary<ulong, List<long>>.Entry)o, needType: nt); } },
            { typeof(SerializableDictionary<long, MyObjectBuilder_Gps>), (w, n, ns, o, nt) => { Write3047_Item(n, ns, (SerializableDictionary<long, MyObjectBuilder_Gps>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<long, MyObjectBuilder_Gps>.Entry), (w, n, ns, o, nt) => { Write3046_Item(n, ns, (SerializableDictionary<long, MyObjectBuilder_Gps>.Entry)o, needType: nt); } },
            { typeof(MyObjectBuilder_Gps.Entry), (w, n, ns, o, nt) => { Write3044_Entry(n, ns, (MyObjectBuilder_Gps.Entry)o, needType: nt); } },
            { typeof(Color), (w, n, ns, o, nt) => { Write3043_Color(n, ns, (Color)o, needType: nt); } },
            { typeof(SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, List<Vector3>>), (w, n, ns, o, nt) => { Write3035_Item(n, ns, (SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, List<Vector3>>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, List<Vector3>>.Entry), (w, n, ns, o, nt) => { Write3034_EntryOfPlayerIdListOfVector3(n, ns, (SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, List<Vector3>>.Entry)o, needType: nt); } },
            { typeof(SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, MyObjectBuilder_Player>), (w, n, ns, o, nt) => { Write3033_Item(n, ns, (SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, MyObjectBuilder_Player>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, MyObjectBuilder_Player>.Entry), (w, n, ns, o, nt) => { Write3032_Item(n, ns, (SerializableDictionary<MyObjectBuilder_Checkpoint.PlayerId, MyObjectBuilder_Player>.Entry)o, needType: nt); } },
            { typeof(SerializableDictionary<long, CameraControllerSettings>), (w, n, ns, o, nt) => { Write3030_Item(n, ns, (SerializableDictionary<long, CameraControllerSettings>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<long, CameraControllerSettings>.Entry), (w, n, ns, o, nt) => { Write3029_Item(n, ns, (SerializableDictionary<long, CameraControllerSettings>.Entry)o, needType: nt); } },
            { typeof(CameraControllerSettings), (w, n, ns, o, nt) => { Write3028_CameraControllerSettings(n, ns, (CameraControllerSettings)o, true, needType: nt); } },
            { typeof(Vector3D), (w, n, ns, o, nt) => { Write3024_Vector3D(n, ns, (Vector3D)o, needType: nt); } },
            { typeof(MyObjectBuilder_Checkpoint.RespawnCooldownItem), (w, n, ns, o, nt) => { Write3023_RespawnCooldownItem(n, ns, (MyObjectBuilder_Checkpoint.RespawnCooldownItem)o, needType: nt); } },
            { typeof(SerializableDictionary<ulong, MyPromoteLevel>), (w, n, ns, o, nt) => { Write3022_Item(n, ns, (SerializableDictionary<ulong, MyPromoteLevel>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<ulong, MyPromoteLevel>.Entry), (w, n, ns, o, nt) => { Write3021_EntryOfUInt64MyPromoteLevel(n, ns, (SerializableDictionary<ulong, MyPromoteLevel>.Entry)o, needType: nt); } },
            { typeof(MyObjectBuilder_Checkpoint.ModItem), (w, n, ns, o, nt) => { Write3019_ModItem(n, ns, (MyObjectBuilder_Checkpoint.ModItem)o, needType: nt); } },
            { typeof(SerializableDictionary<ulong, long>), (w, n, ns, o, nt) => { Write3018_Item(n, ns, (SerializableDictionary<ulong, long>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<ulong, long>.Entry), (w, n, ns, o, nt) => { Write3017_EntryOfUInt64Int64(n, ns, (SerializableDictionary<ulong, long>.Entry)o, needType: nt); } },
            { typeof(MyObjectBuilder_FactionsVisEntry), (w, n, ns, o, nt) => { Write3015_Item(n, ns, (MyObjectBuilder_FactionsVisEntry)o, needType: nt); } },
            { typeof(MyObjectBuilder_FactionRequests), (w, n, ns, o, nt) => { Write3014_Item(n, ns, (MyObjectBuilder_FactionRequests)o, needType: nt); } },
            { typeof(MyObjectBuilder_PlayerFactionRelation), (w, n, ns, o, nt) => { Write3013_Item(n, ns, (MyObjectBuilder_PlayerFactionRelation)o, needType: nt); } },
            { typeof(MyObjectBuilder_FactionRelation), (w, n, ns, o, nt) => { Write3012_Item(n, ns, (MyObjectBuilder_FactionRelation)o, needType: nt); } },
            { typeof(SerializableDictionary<long, long>), (w, n, ns, o, nt) => { Write3010_Item(n, ns, (SerializableDictionary<long, long>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<long, long>.Entry), (w, n, ns, o, nt) => { Write3009_EntryOfInt64Int64(n, ns, (SerializableDictionary<long, long>.Entry)o, needType: nt); } },
            { typeof(MyObjectBuilder_Faction), (w, n, ns, o, nt) => { Write3008_MyObjectBuilder_Faction(n, ns, (MyObjectBuilder_Faction)o, true, needType: nt); } },
            { typeof(SerializableDictionary<long, float>), (w, n, ns, o, nt) => { Write3007_Item(n, ns, (SerializableDictionary<long, float>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<long, float>.Entry), (w, n, ns, o, nt) => { Write3006_EntryOfInt64Single(n, ns, (SerializableDictionary<long, float>.Entry)o, needType: nt); } },
            { typeof(WorkshopId), (w, n, ns, o, nt) => { Write3005_WorkshopId(n, ns, (WorkshopId)o, needType: nt); } },
            { typeof(MyObjectBuilder_Station), (w, n, ns, o, nt) => { Write3004_MyObjectBuilder_Station(n, ns, (MyObjectBuilder_Station)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_StoreItem), (w, n, ns, o, nt) => { Write3003_MyObjectBuilder_StoreItem(n, ns, (MyObjectBuilder_StoreItem)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_FactionMember), (w, n, ns, o, nt) => { Write2998_MyObjectBuilder_FactionMember(n, ns, (MyObjectBuilder_FactionMember)o, needType: nt); } },
            { typeof(SerializableDictionary<string, object>), (w, n, ns, o, nt) => { Write2996_Item(n, ns, (SerializableDictionary<string, object>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<string, object>.Entry), (w, n, ns, o, nt) => { Write2995_EntryOfStringObject(n, ns, (SerializableDictionary<string, object>.Entry)o, needType: nt); } },
            { typeof(SerializableDefinitionId), (w, n, ns, o, nt) => { Write2993_SerializableDefinitionId(n, ns, (SerializableDefinitionId)o, needType: nt); } },
            { typeof(SerializableDictionary<long, MyObjectBuilder_Checkpoint.PlayerId>), (w, n, ns, o, nt) => { Write2992_Item(n, ns, (SerializableDictionary<long, MyObjectBuilder_Checkpoint.PlayerId>)o, true, needType: nt); } },
            { typeof(SerializableDictionary<long, MyObjectBuilder_Checkpoint.PlayerId>.Entry), (w, n, ns, o, nt) => { Write2991_EntryOfInt64PlayerId(n, ns, (SerializableDictionary<long, MyObjectBuilder_Checkpoint.PlayerId>.Entry)o, needType: nt); } },
            { typeof(MyObjectBuilder_Checkpoint.PlayerId), (w, n, ns, o, nt) => { Write2990_PlayerId(n, ns, (MyObjectBuilder_Checkpoint.PlayerId)o, needType: nt); } },
            { typeof(Vector3), (w, n, ns, o, nt) => { Write2988_Vector3(n, ns, (Vector3)o, needType: nt); } },
            { typeof(MyObjectBuilder_Toolbar.Slot), (w, n, ns, o, nt) => { Write2987_Slot(n, ns, (MyObjectBuilder_Toolbar.Slot)o, needType: nt); } },
            { typeof(SerializableVector2), (w, n, ns, o, nt) => { Write2984_SerializableVector2(n, ns, (SerializableVector2)o, needType: nt); } },
            { typeof(MyPositionAndOrientation), (w, n, ns, o, nt) => { Write2983_MyPositionAndOrientation(n, ns, (MyPositionAndOrientation)o, needType: nt); } },
            { typeof(Quaternion), (w, n, ns, o, nt) => { Write2982_Quaternion(n, ns, (Quaternion)o, needType: nt); } },
            { typeof(SerializableVector3), (w, n, ns, o, nt) => { Write2981_SerializableVector3(n, ns, (SerializableVector3)o, needType: nt); } },
            { typeof(SerializableVector3D), (w, n, ns, o, nt) => { Write2980_SerializableVector3D(n, ns, (SerializableVector3D)o, needType: nt); } },
            { typeof(SerializableVector3I), (w, n, ns, o, nt) => { Write2979_SerializableVector3I(n, ns, (SerializableVector3I)o, needType: nt); } },
            { typeof(MyObjectBuilder_Base), (w, n, ns, o, nt) => { Write2978_MyObjectBuilder_Base(n, ns, (MyObjectBuilder_Base)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_ComponentContainer), (w, n, ns, o, nt) => { Write3070_Item(n, ns, (MyObjectBuilder_ComponentContainer)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_EntityBase), (w, n, ns, o, nt) => { Write3071_MyObjectBuilder_EntityBase(n, ns, (MyObjectBuilder_EntityBase)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_VoxelMap), (w, n, ns, o, nt) => { Write3072_MyObjectBuilder_VoxelMap(n, ns, (MyObjectBuilder_VoxelMap)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Planet), (w, n, ns, o, nt) => { Write3077_MyObjectBuilder_Planet(n, ns, (MyObjectBuilder_Planet)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_EnvironmentSettings), (w, n, ns, o, nt) => { Write3063_Item(n, ns, (MyObjectBuilder_EnvironmentSettings)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_GlobalEventBase), (w, n, ns, o, nt) => { Write3061_Item(n, ns, (MyObjectBuilder_GlobalEventBase)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_GlobalEvents), (w, n, ns, o, nt) => { Write3062_MyObjectBuilder_GlobalEvents(n, ns, (MyObjectBuilder_GlobalEvents)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Sector), (w, n, ns, o, nt) => { Write3064_MyObjectBuilder_Sector(n, ns, (MyObjectBuilder_Sector)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Gps), (w, n, ns, o, nt) => { Write3045_MyObjectBuilder_Gps(n, ns, (MyObjectBuilder_Gps)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_FactionChatItem), (w, n, ns, o, nt) => { Write3041_Item(n, ns, (MyObjectBuilder_FactionChatItem)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_FactionChatHistory), (w, n, ns, o, nt) => { Write3042_Item(n, ns, (MyObjectBuilder_FactionChatHistory)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_GlobalChatItem), (w, n, ns, o, nt) => { Write3038_MyObjectBuilder_GlobalChatItem(n, ns, (MyObjectBuilder_GlobalChatItem)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_GlobalChatHistory), (w, n, ns, o, nt) => { Write3039_Item(n, ns, (MyObjectBuilder_GlobalChatHistory)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_PlayerChatItem), (w, n, ns, o, nt) => { Write3036_MyObjectBuilder_PlayerChatItem(n, ns, (MyObjectBuilder_PlayerChatItem)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_PlayerChatHistory), (w, n, ns, o, nt) => { Write3037_Item(n, ns, (MyObjectBuilder_PlayerChatHistory)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_ChatHistory), (w, n, ns, o, nt) => { Write3040_MyObjectBuilder_ChatHistory(n, ns, (MyObjectBuilder_ChatHistory)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Player), (w, n, ns, o, nt) => { Write3031_MyObjectBuilder_Player(n, ns, (MyObjectBuilder_Player)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Client), (w, n, ns, o, nt) => { Write3026_MyObjectBuilder_Client(n, ns, (MyObjectBuilder_Client)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Identity), (w, n, ns, o, nt) => { Write3025_MyObjectBuilder_Identity(n, ns, (MyObjectBuilder_Identity)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_FactionCollection), (w, n, ns, o, nt) => { Write3016_Item(n, ns, (MyObjectBuilder_FactionCollection)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_SessionComponent), (w, n, ns, o, nt) => { Write2994_Item(n, ns, (MyObjectBuilder_SessionComponent)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_ScriptManager), (w, n, ns, o, nt) => { Write2997_MyObjectBuilder_ScriptManager(n, ns, (MyObjectBuilder_ScriptManager)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Toolbar), (w, n, ns, o, nt) => { Write2989_MyObjectBuilder_Toolbar(n, ns, (MyObjectBuilder_Toolbar)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_Checkpoint), (w, n, ns, o, nt) => { Write3058_MyObjectBuilder_Checkpoint(n, ns, (MyObjectBuilder_Checkpoint)o, true, needType: nt); } },
            { typeof(MyObjectBuilder_World), (w, n, ns, o, nt) => { Write3078_MyObjectBuilder_World(n, ns, (MyObjectBuilder_World)o, true, needType: nt); } },
            { typeof(MyGuiDrawAlignEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiDrawAlignEnum", "");
					base.Writer.WriteString(Write5_MyGuiDrawAlignEnum((MyGuiDrawAlignEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyVariableIdentifier>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyVariableIdentifier", "");
					List<MyVariableIdentifier> list = (List<MyVariableIdentifier>)o;
					if (list != null)
					{
						for (int i = 0; i < ((ICollection)list).Count; i++)
						{
							Write19_MyVariableIdentifier("MyVariableIdentifier", "", list[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<IdentifierList>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfIdentifierList", "");
					List<IdentifierList> list2 = (List<IdentifierList>)o;
					if (list2 != null)
					{
						for (int i = 0; i < ((ICollection)list2).Count; i++)
						{
							Write20_IdentifierList("IdentifierList", "", list2[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("string", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_VoxelPostprocessingDecimate.Settings>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSettings", "");
					List<MyObjectBuilder_VoxelPostprocessingDecimate.Settings> list4 = (List<MyObjectBuilder_VoxelPostprocessingDecimate.Settings>)o;
					if (list4 != null)
					{
						for (int i = 0; i < ((ICollection)list4).Count; i++)
						{
							Write42_Settings("Settings", "", list4[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiDrawAlignEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiDrawAlignEnum", "");
					base.Writer.WriteString(Write47_MyGuiDrawAlignEnum((MyGuiDrawAlignEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_StatVisualStyle[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyAbstractXmlSerializerOfMyObjectBuilder_StatVisualStyle", "");
					MyObjectBuilder_StatVisualStyle[] array = (MyObjectBuilder_StatVisualStyle[])o;
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							WriteSerializable((MyAbstractXmlSerializer<MyObjectBuilder_StatVisualStyle>)array[i], "StatStyle", "", true: true, wrapped: true);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyCubeTopology), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyCubeTopology", "");
					base.Writer.WriteString(Write50_MyCubeTopology((MyCubeTopology)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.Side[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSide", "");
					MyObjectBuilder_CubeBlockDefinition.Side[] array2 = (MyObjectBuilder_CubeBlockDefinition.Side[])o;
					if (array2 != null)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							Write52_Side("Side", "", array2[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(VoxelPlacementMode), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("VoxelPlacementMode", "");
					base.Writer.WriteString(Write59_VoxelPlacementMode((VoxelPlacementMode)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyCubeSize), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyCubeSize", "");
					base.Writer.WriteString(Write62_MyCubeSize((MyCubeSize)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockTopology), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockTopology", "");
					base.Writer.WriteString(Write63_MyBlockTopology((MyBlockTopology)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyCubeTopology), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyCubeTopology", "");
					base.Writer.WriteString(Write66_MyCubeTopology((MyCubeTopology)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.Side[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSide", "");
					MyObjectBuilder_CubeBlockDefinition.Side[] array2 = (MyObjectBuilder_CubeBlockDefinition.Side[])o;
					if (array2 != null)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							Write67_Side("Side", "", array2[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfCubeBlockComponent", "");
					MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] array3 = (MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[])o;
					if (array3 != null)
					{
						for (int i = 0; i < array3.Length; i++)
						{
							Write69_CubeBlockComponent("Component", "", array3[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.CubeBlockEffect[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfCubeBlockEffect", "");
					MyObjectBuilder_CubeBlockDefinition.CubeBlockEffect[] array4 = (MyObjectBuilder_CubeBlockDefinition.CubeBlockEffect[])o;
					if (array4 != null)
					{
						for (int i = 0; i < array4.Length; i++)
						{
							Write70_CubeBlockEffect("ParticleEffect", "", array4[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.CubeBlockEffectBase[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfCubeBlockEffectBase", "");
					MyObjectBuilder_CubeBlockDefinition.CubeBlockEffectBase[] array5 = (MyObjectBuilder_CubeBlockDefinition.CubeBlockEffectBase[])o;
					if (array5 != null)
					{
						for (int i = 0; i < array5.Length; i++)
						{
							Write71_CubeBlockEffectBase("Effect", "", array5[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(BlockSideEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("BlockSideEnum", "");
					base.Writer.WriteString(Write73_BlockSideEnum((BlockSideEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.MountPoint[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMountPoint", "");
					MyObjectBuilder_CubeBlockDefinition.MountPoint[] array6 = (MyObjectBuilder_CubeBlockDefinition.MountPoint[])o;
					if (array6 != null)
					{
						for (int i = 0; i < array6.Length; i++)
						{
							Write74_MountPoint("MountPoint", "", array6[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.Variant[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfVariant", "");
					MyObjectBuilder_CubeBlockDefinition.Variant[] array7 = (MyObjectBuilder_CubeBlockDefinition.Variant[])o;
					if (array7 != null)
					{
						for (int i = 0; i < array7.Length; i++)
						{
							Write75_Variant("Variant", "", array7[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.EntityComponentDefinition[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfEntityComponentDefinition", "");
					MyObjectBuilder_CubeBlockDefinition.EntityComponentDefinition[] array8 = (MyObjectBuilder_CubeBlockDefinition.EntityComponentDefinition[])o;
					if (array8 != null)
					{
						for (int i = 0; i < array8.Length; i++)
						{
							Write76_EntityComponentDefinition("Component", "", array8[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyPhysicsOption), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyPhysicsOption", "");
					base.Writer.WriteString(Write77_MyPhysicsOption((MyPhysicsOption)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_CubeBlockDefinition.BuildProgressModel>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfBuildProgressModel", "");
					List<MyObjectBuilder_CubeBlockDefinition.BuildProgressModel> list5 = (List<MyObjectBuilder_CubeBlockDefinition.BuildProgressModel>)o;
					if (list5 != null)
					{
						for (int i = 0; i < ((ICollection)list5).Count; i++)
						{
							Write78_BuildProgressModel("Model", "", list5[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MySymmetryAxisEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MySymmetryAxisEnum", "");
					base.Writer.WriteString(Write79_MySymmetryAxisEnum((MySymmetryAxisEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyAutorotateMode), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyAutorotateMode", "");
					base.Writer.WriteString(Write80_MyAutorotateMode((MyAutorotateMode)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDefinitionId[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSerializableDefinitionId", "");
					SerializableDefinitionId[] array9 = (SerializableDefinitionId[])o;
					if (array9 != null)
					{
						for (int i = 0; i < array9.Length; i++)
						{
							Write56_SerializableDefinitionId("BlockVariant", "", array9[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockDirection), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockDirection", "");
					base.Writer.WriteString(Write81_MyBlockDirection((MyBlockDirection)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockRotation), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockRotation", "");
					base.Writer.WriteString(Write82_MyBlockRotation((MyBlockRotation)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDefinitionId[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSerializableDefinitionId1", "");
					SerializableDefinitionId[] array9 = (SerializableDefinitionId[])o;
					if (array9 != null)
					{
						for (int i = 0; i < array9.Length; i++)
						{
							Write56_SerializableDefinitionId("GeneratedBlock", "", array9[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<BoneInfo>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfBoneInfo", "");
					List<BoneInfo> list6 = (List<BoneInfo>)o;
					if (list6 != null)
					{
						for (int i = 0; i < ((ICollection)list6).Count; i++)
						{
							Write84_BoneInfo("BoneInfo", "", list6[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MySerializableList<uint>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfUnsignedInt", "");
					MySerializableList<uint> mySerializableList = (MySerializableList<uint>)o;
					if (mySerializableList != null)
					{
						for (int i = 0; i < ((ICollection)mySerializableList).Count; i++)
						{
							WriteElementStringRaw("unsignedInt", "", XmlConvert.ToString(mySerializableList[i]));
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(string[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					string[] array10 = (string[])o;
					if (array10 != null)
					{
						for (int i = 0; i < array10.Length; i++)
						{
							WriteNullableStringLiteral("string", "", array10[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<ScreenArea>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfScreenArea", "");
					List<ScreenArea> list7 = (List<ScreenArea>)o;
					if (list7 != null)
					{
						for (int i = 0; i < ((ICollection)list7).Count; i++)
						{
							Write88_ScreenArea("ScreenArea", "", list7[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<SerializableDefinitionId>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSerializableDefinitionId", "");
					List<SerializableDefinitionId> list8 = (List<SerializableDefinitionId>)o;
					if (list8 != null)
					{
						for (int i = 0; i < ((ICollection)list8).Count; i++)
						{
							Write94_SerializableDefinitionId("AssetModifier", "", list8[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyPlanetAnimal[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyPlanetAnimal", "");
					MyPlanetAnimal[] array11 = (MyPlanetAnimal[])o;
					if (array11 != null)
					{
						for (int i = 0; i < array11.Length; i++)
						{
							Write97_MyPlanetAnimal("Animal", "", array11[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(string[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					string[] array10 = (string[])o;
					if (array10 != null)
					{
						for (int i = 0; i < array10.Length; i++)
						{
							WriteNullableStringLiteral("Model", "", array10[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_ConsumableItemDefinition.StatValue[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfStatValue", "");
					MyObjectBuilder_ConsumableItemDefinition.StatValue[] array12 = (MyObjectBuilder_ConsumableItemDefinition.StatValue[])o;
					if (array12 != null)
					{
						for (int i = 0; i < array12.Length; i++)
						{
							Write108_StatValue("Stat", "", array12[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiDrawAlignEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiDrawAlignEnum", "");
					base.Writer.WriteString(Write114_MyGuiDrawAlignEnum((MyGuiDrawAlignEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyItemFlags), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyItemFlags", "");
					base.Writer.WriteString(Write138_MyItemFlags((MyItemFlags)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("string", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_ScriptNode>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyAbstractXmlSerializerOfMyObjectBuilder_ScriptNode", "");
					List<MyObjectBuilder_ScriptNode> list9 = (List<MyObjectBuilder_ScriptNode>)o;
					if (list9 != null)
					{
						for (int i = 0; i < ((ICollection)list9).Count; i++)
						{
							WriteSerializable((MyAbstractXmlSerializer<MyObjectBuilder_ScriptNode>)list9[i], "MyObjectBuilder_ScriptNode", "", true: true, wrapped: true);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(string[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					string[] array10 = (string[])o;
					if (array10 != null)
					{
						for (int i = 0; i < array10.Length; i++)
						{
							WriteNullableStringLiteral("string", "", array10[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MySafeZoneAction), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MySafeZoneAction", "");
					base.Writer.WriteString(Write166_MySafeZoneAction((MySafeZoneAction)o));
					base.Writer.WriteEndElement(); } },
            { typeof(string[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					string[] array10 = (string[])o;
					if (array10 != null)
					{
						for (int i = 0; i < array10.Length; i++)
						{
							WriteNullableStringLiteral("string", "", array10[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyContractChancePair[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyAbstractXmlSerializerOfMyContractChancePair", "");
					MyContractChancePair[] array13 = (MyContractChancePair[])o;
					if (array13 != null)
					{
						for (int i = 0; i < array13.Length; i++)
						{
							WriteSerializable((MyAbstractXmlSerializer<MyContractChancePair>)array13[i], "ContractChance", "", true: true, wrapped: true);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_DurabilityComponentDefinition.HitDefinition[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfHitDefinition", "");
					MyObjectBuilder_DurabilityComponentDefinition.HitDefinition[] array14 = (MyObjectBuilder_DurabilityComponentDefinition.HitDefinition[])o;
					if (array14 != null)
					{
						for (int i = 0; i < array14.Length; i++)
						{
							Write188_HitDefinition("Hit", "", array14[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<int>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfInt", "");
					List<int> list10 = (List<int>)o;
					if (list10 != null)
					{
						for (int i = 0; i < ((ICollection)list10).Count; i++)
						{
							WriteElementStringRaw("int", "", XmlConvert.ToString(list10[i]));
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyVariableIdentifier>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyVariableIdentifier", "");
					List<MyVariableIdentifier> list = (List<MyVariableIdentifier>)o;
					if (list != null)
					{
						for (int i = 0; i < ((ICollection)list).Count; i++)
						{
							Write197_MyVariableIdentifier("MyVariableIdentifier", "", list[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyPersistentEntityFlags2), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyPersistentEntityFlags2", "");
					base.Writer.WriteString(Write201_MyPersistentEntityFlags2((MyPersistentEntityFlags2)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_ComponentContainer.ComponentData>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfComponentData", "");
					List<MyObjectBuilder_ComponentContainer.ComponentData> list11 = (List<MyObjectBuilder_ComponentContainer.ComponentData>)o;
					if (list11 != null)
					{
						for (int i = 0; i < ((ICollection)list11).Count; i++)
						{
							Write206_ComponentData("ComponentData", "", list11[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_BatteryRegenerationEffect>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyObjectBuilder_BatteryRegenerationEffect", "");
					List<MyObjectBuilder_BatteryRegenerationEffect> list12 = (List<MyObjectBuilder_BatteryRegenerationEffect>)o;
					if (list12 != null)
					{
						for (int i = 0; i < ((ICollection)list12).Count; i++)
						{
							Write217_Item("MyObjectBuilder_BatteryRegenerationEffect", "", list12[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyRelationsBetweenFactions), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyRelationsBetweenFactions", "");
					base.Writer.WriteString(Write223_MyRelationsBetweenFactions((MyRelationsBetweenFactions)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyPersistentEntityFlags2), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyPersistentEntityFlags2", "");
					base.Writer.WriteString(Write243_MyPersistentEntityFlags2((MyPersistentEntityFlags2)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_ComponentContainer.ComponentData>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfComponentData", "");
					List<MyObjectBuilder_ComponentContainer.ComponentData> list11 = (List<MyObjectBuilder_ComponentContainer.ComponentData>)o;
					if (list11 != null)
					{
						for (int i = 0; i < ((ICollection)list11).Count; i++)
						{
							Write248_ComponentData("ComponentData", "", list11[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_EnvironmentItems.MyOBEnvironmentItemData[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyOBEnvironmentItemData", "");
					MyObjectBuilder_EnvironmentItems.MyOBEnvironmentItemData[] array15 = (MyObjectBuilder_EnvironmentItems.MyOBEnvironmentItemData[])o;
					if (array15 != null)
					{
						for (int i = 0; i < array15.Length; i++)
						{
							Write252_MyOBEnvironmentItemData("Item", "", array15[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_AnimationLayer.MyLayerMode), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyLayerMode", "");
					base.Writer.WriteString(Write259_MyLayerMode((MyObjectBuilder_AnimationLayer.MyLayerMode)o));
					base.Writer.WriteEndElement(); } },
            { typeof(VoxelPlacementMode), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("VoxelPlacementMode", "");
					base.Writer.WriteString(Write265_VoxelPlacementMode((VoxelPlacementMode)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyCubeSize), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyCubeSize", "");
					base.Writer.WriteString(Write268_MyCubeSize((MyCubeSize)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockTopology), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockTopology", "");
					base.Writer.WriteString(Write269_MyBlockTopology((MyBlockTopology)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyCubeTopology), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyCubeTopology", "");
					base.Writer.WriteString(Write272_MyCubeTopology((MyCubeTopology)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.Side[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSide", "");
					MyObjectBuilder_CubeBlockDefinition.Side[] array2 = (MyObjectBuilder_CubeBlockDefinition.Side[])o;
					if (array2 != null)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							Write273_Side("Side", "", array2[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfCubeBlockComponent", "");
					MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] array3 = (MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[])o;
					if (array3 != null)
					{
						for (int i = 0; i < array3.Length; i++)
						{
							Write275_CubeBlockComponent("Component", "", array3[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.CubeBlockEffect[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfCubeBlockEffect", "");
					MyObjectBuilder_CubeBlockDefinition.CubeBlockEffect[] array4 = (MyObjectBuilder_CubeBlockDefinition.CubeBlockEffect[])o;
					if (array4 != null)
					{
						for (int i = 0; i < array4.Length; i++)
						{
							Write276_CubeBlockEffect("ParticleEffect", "", array4[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.CubeBlockEffectBase[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfCubeBlockEffectBase", "");
					MyObjectBuilder_CubeBlockDefinition.CubeBlockEffectBase[] array5 = (MyObjectBuilder_CubeBlockDefinition.CubeBlockEffectBase[])o;
					if (array5 != null)
					{
						for (int i = 0; i < array5.Length; i++)
						{
							Write277_CubeBlockEffectBase("Effect", "", array5[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(BlockSideEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("BlockSideEnum", "");
					base.Writer.WriteString(Write279_BlockSideEnum((BlockSideEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.MountPoint[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMountPoint", "");
					MyObjectBuilder_CubeBlockDefinition.MountPoint[] array6 = (MyObjectBuilder_CubeBlockDefinition.MountPoint[])o;
					if (array6 != null)
					{
						for (int i = 0; i < array6.Length; i++)
						{
							Write280_MountPoint("MountPoint", "", array6[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.Variant[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfVariant", "");
					MyObjectBuilder_CubeBlockDefinition.Variant[] array7 = (MyObjectBuilder_CubeBlockDefinition.Variant[])o;
					if (array7 != null)
					{
						for (int i = 0; i < array7.Length; i++)
						{
							Write281_Variant("Variant", "", array7[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_CubeBlockDefinition.EntityComponentDefinition[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfEntityComponentDefinition", "");
					MyObjectBuilder_CubeBlockDefinition.EntityComponentDefinition[] array8 = (MyObjectBuilder_CubeBlockDefinition.EntityComponentDefinition[])o;
					if (array8 != null)
					{
						for (int i = 0; i < array8.Length; i++)
						{
							Write282_EntityComponentDefinition("Component", "", array8[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyPhysicsOption), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyPhysicsOption", "");
					base.Writer.WriteString(Write283_MyPhysicsOption((MyPhysicsOption)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_CubeBlockDefinition.BuildProgressModel>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfBuildProgressModel", "");
					List<MyObjectBuilder_CubeBlockDefinition.BuildProgressModel> list5 = (List<MyObjectBuilder_CubeBlockDefinition.BuildProgressModel>)o;
					if (list5 != null)
					{
						for (int i = 0; i < ((ICollection)list5).Count; i++)
						{
							Write284_BuildProgressModel("Model", "", list5[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MySymmetryAxisEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MySymmetryAxisEnum", "");
					base.Writer.WriteString(Write285_MySymmetryAxisEnum((MySymmetryAxisEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyAutorotateMode), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyAutorotateMode", "");
					base.Writer.WriteString(Write286_MyAutorotateMode((MyAutorotateMode)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDefinitionId[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSerializableDefinitionId", "");
					SerializableDefinitionId[] array9 = (SerializableDefinitionId[])o;
					if (array9 != null)
					{
						for (int i = 0; i < array9.Length; i++)
						{
							Write262_SerializableDefinitionId("BlockVariant", "", array9[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockDirection), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockDirection", "");
					base.Writer.WriteString(Write287_MyBlockDirection((MyBlockDirection)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockRotation), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockRotation", "");
					base.Writer.WriteString(Write288_MyBlockRotation((MyBlockRotation)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDefinitionId[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSerializableDefinitionId1", "");
					SerializableDefinitionId[] array9 = (SerializableDefinitionId[])o;
					if (array9 != null)
					{
						for (int i = 0; i < array9.Length; i++)
						{
							Write262_SerializableDefinitionId("GeneratedBlock", "", array9[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<BoneInfo>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfBoneInfo", "");
					List<BoneInfo> list6 = (List<BoneInfo>)o;
					if (list6 != null)
					{
						for (int i = 0; i < ((ICollection)list6).Count; i++)
						{
							Write290_BoneInfo("BoneInfo", "", list6[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MySerializableList<uint>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfUnsignedInt", "");
					MySerializableList<uint> mySerializableList = (MySerializableList<uint>)o;
					if (mySerializableList != null)
					{
						for (int i = 0; i < ((ICollection)mySerializableList).Count; i++)
						{
							WriteElementStringRaw("unsignedInt", "", XmlConvert.ToString(mySerializableList[i]));
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(string[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					string[] array10 = (string[])o;
					if (array10 != null)
					{
						for (int i = 0; i < array10.Length; i++)
						{
							WriteNullableStringLiteral("string", "", array10[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<ScreenArea>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfScreenArea", "");
					List<ScreenArea> list7 = (List<ScreenArea>)o;
					if (list7 != null)
					{
						for (int i = 0; i < ((ICollection)list7).Count; i++)
						{
							Write294_ScreenArea("ScreenArea", "", list7[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyVariableIdentifier>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyVariableIdentifier", "");
					List<MyVariableIdentifier> list = (List<MyVariableIdentifier>)o;
					if (list != null)
					{
						for (int i = 0; i < ((ICollection)list).Count; i++)
						{
							Write301_MyVariableIdentifier("MyVariableIdentifier", "", list[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<IdentifierList>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfIdentifierList", "");
					List<IdentifierList> list2 = (List<IdentifierList>)o;
					if (list2 != null)
					{
						for (int i = 0; i < ((ICollection)list2).Count; i++)
						{
							Write302_IdentifierList("IdentifierList", "", list2[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("string", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyWeaponRule>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyWeaponRule", "");
					List<MyWeaponRule> list13 = (List<MyWeaponRule>)o;
					if (list13 != null)
					{
						for (int i = 0; i < ((ICollection)list13).Count; i++)
						{
							Write314_MyWeaponRule("WeaponRule", "", list13[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("Weapon", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyWeaponBehavior>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyWeaponBehavior", "");
					List<MyWeaponBehavior> list14 = (List<MyWeaponBehavior>)o;
					if (list14 != null)
					{
						for (int i = 0; i < ((ICollection)list14).Count; i++)
						{
							Write315_MyWeaponBehavior("WeaponBehavior", "", list14[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiDrawAlignEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiDrawAlignEnum", "");
					base.Writer.WriteString(Write321_MyGuiDrawAlignEnum((MyGuiDrawAlignEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGameModeEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGameModeEnum", "");
					base.Writer.WriteString(Write323_MyGameModeEnum((MyGameModeEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyOnlineModeEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyOnlineModeEnum", "");
					base.Writer.WriteString(Write324_MyOnlineModeEnum((MyOnlineModeEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockLimitsEnabledEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockLimitsEnabledEnum", "");
					base.Writer.WriteString(Write325_MyBlockLimitsEnabledEnum((MyBlockLimitsEnabledEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyEnvironmentHostilityEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyEnvironmentHostilityEnum", "");
					base.Writer.WriteString(Write326_MyEnvironmentHostilityEnum((MyEnvironmentHostilityEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDictionary<string, short>.Entry[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfEntryOfStringInt16", "");
					SerializableDictionary<string, short>.Entry[] array16 = (SerializableDictionary<string, short>.Entry[])o;
					if (array16 != null)
					{
						for (int i = 0; i < array16.Length; i++)
						{
							Write327_EntryOfStringInt16("item", "", array16[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("Warning", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_SessionSettings.LimitBlocksByOption), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("LimitBlocksByOption", "");
					base.Writer.WriteString(Write329_LimitBlocksByOption((MyObjectBuilder_SessionSettings.LimitBlocksByOption)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlButtonStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlButtonStyleEnum", "");
					base.Writer.WriteString(Write333_MyGuiControlButtonStyleEnum((MyGuiControlButtonStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlCheckboxStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlCheckboxStyleEnum", "");
					base.Writer.WriteString(Write335_MyGuiControlCheckboxStyleEnum((MyGuiControlCheckboxStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlGridStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlGridStyleEnum", "");
					base.Writer.WriteString(Write340_MyGuiControlGridStyleEnum((MyGuiControlGridStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(CheckStateEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("CheckStateEnum", "");
					base.Writer.WriteString(Write344_CheckStateEnum((CheckStateEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlIndeterminateCheckboxStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlIndeterminateCheckboxStyleEnum", "");
					base.Writer.WriteString(Write345_Item((MyGuiControlIndeterminateCheckboxStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_GuiControlBase>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyObjectBuilder_GuiControlBase", "");
					List<MyObjectBuilder_GuiControlBase> list15 = (List<MyObjectBuilder_GuiControlBase>)o;
					if (list15 != null)
					{
						for (int i = 0; i < ((ICollection)list15).Count; i++)
						{
							Write322_MyObjectBuilder_GuiControlBase("MyObjectBuilder_GuiControlBase", "", list15[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlListStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlListStyleEnum", "");
					base.Writer.WriteString(Write350_MyGuiControlListStyleEnum((MyGuiControlListStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlListboxStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlListboxStyleEnum", "");
					base.Writer.WriteString(Write352_MyGuiControlListboxStyleEnum((MyGuiControlListboxStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlRadioButtonStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlRadioButtonStyleEnum", "");
					base.Writer.WriteString(Write358_Item((MyGuiControlRadioButtonStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_GuiControlSeparatorList.Separator>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSeparator", "");
					List<MyObjectBuilder_GuiControlSeparatorList.Separator> list16 = (List<MyObjectBuilder_GuiControlSeparatorList.Separator>)o;
					if (list16 != null)
					{
						for (int i = 0; i < ((ICollection)list16).Count; i++)
						{
							Write361_Separator("Separator", "", list16[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyPersistentEntityFlags2), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyPersistentEntityFlags2", "");
					base.Writer.WriteString(Write381_MyPersistentEntityFlags2((MyPersistentEntityFlags2)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_ComponentContainer.ComponentData>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfComponentData", "");
					List<MyObjectBuilder_ComponentContainer.ComponentData> list11 = (List<MyObjectBuilder_ComponentContainer.ComponentData>)o;
					if (list11 != null)
					{
						for (int i = 0; i < ((ICollection)list11).Count; i++)
						{
							Write386_ComponentData("ComponentData", "", list11[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_EnvironmentItems.MyOBEnvironmentItemData[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyOBEnvironmentItemData", "");
					MyObjectBuilder_EnvironmentItems.MyOBEnvironmentItemData[] array15 = (MyObjectBuilder_EnvironmentItems.MyOBEnvironmentItemData[])o;
					if (array15 != null)
					{
						for (int i = 0; i < array15.Length; i++)
						{
							Write390_MyOBEnvironmentItemData("Item", "", array15[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDefinitionId[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSerializableDefinitionId", "");
					SerializableDefinitionId[] array9 = (SerializableDefinitionId[])o;
					if (array9 != null)
					{
						for (int i = 0; i < array9.Length; i++)
						{
							Write406_SerializableDefinitionId("Block", "", array9[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("string", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<double>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfDouble", "");
					List<double> list17 = (List<double>)o;
					if (list17 != null)
					{
						for (int i = 0; i < ((ICollection)list17).Count; i++)
						{
							WriteElementStringRaw("double", "", XmlConvert.ToString(list17[i]));
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiDrawAlignEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiDrawAlignEnum", "");
					base.Writer.WriteString(Write424_MyGuiDrawAlignEnum((MyGuiDrawAlignEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGameModeEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGameModeEnum", "");
					base.Writer.WriteString(Write426_MyGameModeEnum((MyGameModeEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyOnlineModeEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyOnlineModeEnum", "");
					base.Writer.WriteString(Write427_MyOnlineModeEnum((MyOnlineModeEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockLimitsEnabledEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockLimitsEnabledEnum", "");
					base.Writer.WriteString(Write428_MyBlockLimitsEnabledEnum((MyBlockLimitsEnabledEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyEnvironmentHostilityEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyEnvironmentHostilityEnum", "");
					base.Writer.WriteString(Write429_MyEnvironmentHostilityEnum((MyEnvironmentHostilityEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDictionary<string, short>.Entry[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfEntryOfStringInt16", "");
					SerializableDictionary<string, short>.Entry[] array16 = (SerializableDictionary<string, short>.Entry[])o;
					if (array16 != null)
					{
						for (int i = 0; i < array16.Length; i++)
						{
							Write430_EntryOfStringInt16("item", "", array16[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("Warning", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_SessionSettings.LimitBlocksByOption), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("LimitBlocksByOption", "");
					base.Writer.WriteString(Write432_LimitBlocksByOption((MyObjectBuilder_SessionSettings.LimitBlocksByOption)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlButtonStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlButtonStyleEnum", "");
					base.Writer.WriteString(Write436_MyGuiControlButtonStyleEnum((MyGuiControlButtonStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlCheckboxStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlCheckboxStyleEnum", "");
					base.Writer.WriteString(Write438_MyGuiControlCheckboxStyleEnum((MyGuiControlCheckboxStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_GuiControlBase>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyObjectBuilder_GuiControlBase", "");
					List<MyObjectBuilder_GuiControlBase> list15 = (List<MyObjectBuilder_GuiControlBase>)o;
					if (list15 != null)
					{
						for (int i = 0; i < ((ICollection)list15).Count; i++)
						{
							Write425_MyObjectBuilder_GuiControlBase("MyObjectBuilder_GuiControlBase", "", list15[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlGridStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlGridStyleEnum", "");
					base.Writer.WriteString(Write447_MyGuiControlGridStyleEnum((MyGuiControlGridStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(CheckStateEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("CheckStateEnum", "");
					base.Writer.WriteString(Write451_CheckStateEnum((CheckStateEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlIndeterminateCheckboxStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlIndeterminateCheckboxStyleEnum", "");
					base.Writer.WriteString(Write452_Item((MyGuiControlIndeterminateCheckboxStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlListStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlListStyleEnum", "");
					base.Writer.WriteString(Write455_MyGuiControlListStyleEnum((MyGuiControlListStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlListboxStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlListboxStyleEnum", "");
					base.Writer.WriteString(Write457_MyGuiControlListboxStyleEnum((MyGuiControlListboxStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiControlRadioButtonStyleEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiControlRadioButtonStyleEnum", "");
					base.Writer.WriteString(Write463_Item((MyGuiControlRadioButtonStyleEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_GuiControlSeparatorList.Separator>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfSeparator", "");
					List<MyObjectBuilder_GuiControlSeparatorList.Separator> list16 = (List<MyObjectBuilder_GuiControlSeparatorList.Separator>)o;
					if (list16 != null)
					{
						for (int i = 0; i < ((ICollection)list16).Count; i++)
						{
							Write466_Separator("Separator", "", list16[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_AutomaticBehaviour.DroneTargetSerializable>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfDroneTargetSerializable", "");
					List<MyObjectBuilder_AutomaticBehaviour.DroneTargetSerializable> list18 = (List<MyObjectBuilder_AutomaticBehaviour.DroneTargetSerializable>)o;
					if (list18 != null)
					{
						for (int i = 0; i < ((ICollection)list18).Count; i++)
						{
							Write478_DroneTargetSerializable("DroneTargetSerializable", "", list18[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<long>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfLong", "");
					List<long> list19 = (List<long>)o;
					if (list19 != null)
					{
						for (int i = 0; i < ((ICollection)list19).Count; i++)
						{
							WriteElementStringRaw("long", "", XmlConvert.ToString(list19[i]));
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(TargetPrioritization), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("TargetPrioritization", "");
					base.Writer.WriteString(Write479_TargetPrioritization((TargetPrioritization)o));
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyObjectBuilder_PlayerChatItem>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyObjectBuilder_PlayerChatItem", "");
					List<MyObjectBuilder_PlayerChatItem> list20 = (List<MyObjectBuilder_PlayerChatItem>)o;
					if (list20 != null)
					{
						for (int i = 0; i < ((ICollection)list20).Count; i++)
						{
							Write485_MyObjectBuilder_PlayerChatItem("PCI", "", list20[i], true: true, needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyProfilerBlockKey>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyProfilerBlockKey", "");
					List<MyProfilerBlockKey> list21 = (List<MyProfilerBlockKey>)o;
					if (list21 != null)
					{
						for (int i = 0; i < ((ICollection)list21).Count; i++)
						{
							Write491_MyProfilerBlockKey("MyProfilerBlockKey", "", list21[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyVariableIdentifier>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyVariableIdentifier", "");
					List<MyVariableIdentifier> list = (List<MyVariableIdentifier>)o;
					if (list != null)
					{
						for (int i = 0; i < ((ICollection)list).Count; i++)
						{
							Write1290_MyVariableIdentifier("MyVariableIdentifier", "", list[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<MyVariableIdentifier>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfMyVariableIdentifier", "");
					List<MyVariableIdentifier> list = (List<MyVariableIdentifier>)o;
					if (list != null)
					{
						for (int i = 0; i < ((ICollection)list).Count; i++)
						{
							Write1296_MyVariableIdentifier("MyVariableIdentifier", "", list[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyGuiDrawAlignEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGuiDrawAlignEnum", "");
					base.Writer.WriteString(Write1303_MyGuiDrawAlignEnum((MyGuiDrawAlignEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyGameModeEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyGameModeEnum", "");
					base.Writer.WriteString(Write1305_MyGameModeEnum((MyGameModeEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyOnlineModeEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyOnlineModeEnum", "");
					base.Writer.WriteString(Write1306_MyOnlineModeEnum((MyOnlineModeEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyBlockLimitsEnabledEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyBlockLimitsEnabledEnum", "");
					base.Writer.WriteString(Write1307_MyBlockLimitsEnabledEnum((MyBlockLimitsEnabledEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyEnvironmentHostilityEnum), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyEnvironmentHostilityEnum", "");
					base.Writer.WriteString(Write1308_MyEnvironmentHostilityEnum((MyEnvironmentHostilityEnum)o));
					base.Writer.WriteEndElement(); } },
            { typeof(SerializableDictionary<string, short>.Entry[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfEntryOfStringInt16", "");
					SerializableDictionary<string, short>.Entry[] array16 = (SerializableDictionary<string, short>.Entry[])o;
					if (array16 != null)
					{
						for (int i = 0; i < array16.Length; i++)
						{
							Write1309_EntryOfStringInt16("item", "", array16[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(List<string>), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					List<string> list3 = (List<string>)o;
					if (list3 != null)
					{
						for (int i = 0; i < ((ICollection)list3).Count; i++)
						{
							WriteNullableStringLiteral("Warning", "", list3[i]);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_SessionSettings.LimitBlocksByOption), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("LimitBlocksByOption", "");
					base.Writer.WriteString(Write1311_LimitBlocksByOption((MyObjectBuilder_SessionSettings.LimitBlocksByOption)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyPersistentEntityFlags2), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("MyPersistentEntityFlags2", "");
					base.Writer.WriteString(Write1322_MyPersistentEntityFlags2((MyPersistentEntityFlags2)o));
					base.Writer.WriteEndElement(); } },
            { typeof(MyObjectBuilder_BoardScreen.BoardRow[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfBoardRow", "");
					MyObjectBuilder_BoardScreen.BoardRow[] array241 = (MyObjectBuilder_BoardScreen.BoardRow[])o;
					if (array241 != null)
					{
						for (int i = 0; i < array241.Length; i++)
						{
							Write5959_BoardRow("BoardRow", "", array241[i], needType: nt);
						}
					}
					base.Writer.WriteEndElement(); } },
            { typeof(string[]), (w, n, ns, o, nt) => { base.Writer.WriteStartElement(n, ns);
					WriteXsiType("ArrayOfString", "");
					string[] array10 = (string[])o;
					if (array10 != null)
					{
						for (int i = 0; i < array10.Length; i++)
						{
							WriteNullableStringLiteral("string", "", array10[i]);
						}
					}
					base.Writer.WriteEndElement(); } }
        };


	private void Write2977_Object(string n, string ns, object o, bool isNullable, bool needType)
	{
		if (o == null)
		{
			if (isNullable)
			{
				WriteNullTagLiteral(n, ns);
			}
			return;
		}
		if (!needType)
		{
			Type type = o.GetType();
			if (!(type == typeof(object)))
			{
				if (Write2977_Object_TypeHandlers_1.TryGetValue(type, out var handler))
{
handler(this, n, ns, o, needType);
return;
}


				else
				{
					WriteTypedPrimitive(n, ns, o, xsiType: true);
				}
				return;
			}
		}
		WriteStartElement(n, ns, o, writePrefixed: false, null);
		WriteEndElement(o);
	}
}
