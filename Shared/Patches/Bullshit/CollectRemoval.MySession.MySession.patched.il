// d0d4ee38
ldarg.0
newobj System.Void VRage.Collections.CachingDictionary`2<System.Type, VRage.Game.Components.MySessionComponentBase>::.ctor()
stfld VRage.Collections.CachingDictionary`2<System.Type, VRage.Game.Components.MySessionComponentBase> Sandbox.Game.World.MySession::m_sessionComponents
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.Int32, System.Collections.Generic.SortedSet`1<VRage.Game.Components.MySessionComponentBase>>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.Int32, System.Collections.Generic.SortedSet`1<VRage.Game.Components.MySessionComponentBase>> Sandbox.Game.World.MySession::m_sessionComponentsForUpdate
ldarg.0
newobj System.Void System.Collections.Generic.List`1<VRage.Game.Components.MySessionComponentBase>::.ctor()
stfld System.Collections.Generic.List`1<VRage.Game.Components.MySessionComponentBase> Sandbox.Game.World.MySession::m_sessionComponentForDraw
ldarg.0
newobj System.Void System.Collections.Generic.List`1<VRage.Game.Components.MySessionComponentBase>::.ctor()
stfld System.Collections.Generic.List`1<VRage.Game.Components.MySessionComponentBase> Sandbox.Game.World.MySession::m_sessionComponentForDrawAsync
ldarg.0
newobj System.Void System.Collections.Generic.HashSet`1<System.String>::.ctor()
stfld System.Collections.Generic.HashSet`1<System.String> Sandbox.Game.World.MySession::SessionComponentEnabled
ldarg.0
newobj System.Void System.Collections.Generic.HashSet`1<System.String>::.ctor()
stfld System.Collections.Generic.HashSet`1<System.String> Sandbox.Game.World.MySession::SessionComponentDisabled
ldarg.0
newobj System.Void Sandbox.Engine.Utils.MySpectatorCameraController::.ctor()
stfld Sandbox.Engine.Utils.MySpectatorCameraController Sandbox.Game.World.MySession::Spectator
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.String, System.Int16>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.String, System.Int16> Sandbox.Game.World.MySession::EmptyBlockTypeLimitDictionary
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, System.Int64>::.ctor()
newobj System.Void VRage.Serialization.SerializableDictionary`2<System.UInt64, System.Int64>::.ctor(System.Collections.Generic.Dictionary`2<System.UInt64, System.Int64> dict)
stfld VRage.Serialization.SerializableDictionary`2<System.UInt64, System.Int64> Sandbox.Game.World.MySession::PlayersDeadInCryopods
ldarg.0
ldc.i4.s 9
stfld System.Int32 Sandbox.Game.World.MySession::<RequiresDX>k__BackingField
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, VRage.Game.ModAPI.MyPromoteLevel>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, VRage.Game.ModAPI.MyPromoteLevel> Sandbox.Game.World.MySession::PromotedUsers
ldarg.0
newobj System.Void Sandbox.Game.Entities.MyVoxelMaps::.ctor()
stfld Sandbox.Game.Entities.MyVoxelMaps Sandbox.Game.World.MySession::VoxelMaps
ldarg.0
newobj System.Void Sandbox.Game.Multiplayer.MyFactionCollection::.ctor()
stfld Sandbox.Game.Multiplayer.MyFactionCollection Sandbox.Game.World.MySession::Factions
ldarg.0
newobj System.Void Sandbox.Game.Multiplayer.MyPlayerCollection::.ctor()
stfld Sandbox.Game.Multiplayer.MyPlayerCollection Sandbox.Game.World.MySession::Players
ldarg.0
newobj System.Void System.Collections.Generic.List`1<Sandbox.Game.Entities.MySingleOwnershipRequest>::.ctor()
stfld System.Collections.Generic.List`1<Sandbox.Game.Entities.MySingleOwnershipRequest> Sandbox.Game.World.MySession::OwnershipChangeRequests
ldarg.0
newobj System.Void System.Collections.Generic.List`1<System.Int64>::.ctor()
stfld System.Collections.Generic.List`1<System.Int64> Sandbox.Game.World.MySession::BlocksIdsToRemoveFromOwnershipTransferRequests
ldarg.0
newobj System.Void Sandbox.Game.Multiplayer.MyPerPlayerData::.ctor()
stfld Sandbox.Game.Multiplayer.MyPerPlayerData Sandbox.Game.World.MySession::PerPlayerData
ldarg.0
newobj System.Void Sandbox.Game.Multiplayer.MyToolBarCollection::.ctor()
stfld Sandbox.Game.Multiplayer.MyToolBarCollection Sandbox.Game.World.MySession::Toolbars
ldarg.0
newobj System.Void Sandbox.Engine.Multiplayer.MyVirtualClients::.ctor()
stfld Sandbox.Engine.Multiplayer.MyVirtualClients Sandbox.Game.World.MySession::VirtualClients
ldarg.0
newobj System.Void Sandbox.Game.Multiplayer.MyCameraCollection::.ctor()
stfld Sandbox.Game.Multiplayer.MyCameraCollection Sandbox.Game.World.MySession::Cameras
ldarg.0
newobj System.Void Sandbox.Game.Multiplayer.MyGpsCollection::.ctor()
stfld Sandbox.Game.Multiplayer.MyGpsCollection Sandbox.Game.World.MySession::Gpss
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, Sandbox.Game.World.AdminSettingsEnum>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, Sandbox.Game.World.AdminSettingsEnum> Sandbox.Game.World.MySession::m_remoteAdminSettings
ldarg.0
newobj System.Void System.Collections.Generic.List`1<Sandbox.Game.World.MyUpdateCallback>::.ctor()
stfld System.Collections.Generic.List`1<Sandbox.Game.World.MyUpdateCallback> Sandbox.Game.World.MySession::m_updateCallbacks
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.String, VRage.MyFixedPoint>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.String, VRage.MyFixedPoint> Sandbox.Game.World.MySession::AmountMined
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32> Sandbox.Game.World.MySession::NumberFoodConsumed
ldarg.0
newobj System.Void System.Collections.Generic.HashSet`1<VRage.Game.MyEncounterId>::.ctor()
stfld System.Collections.Generic.HashSet`1<VRage.Game.MyEncounterId> Sandbox.Game.World.MySession::LocallyInvokedEncounters
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32> Sandbox.Game.World.MySession::NumberHarvestedItems
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32> Sandbox.Game.World.MySession::NumberCollectorHarvestedItems
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32> Sandbox.Game.World.MySession::NumberForagedItems
ldarg.0
newobj System.Void System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32>::.ctor()
stfld System.Collections.Generic.Dictionary`2<System.UInt64, System.Int32> Sandbox.Game.World.MySession::NumberCollectorForagedItems
ldarg.0
ldsfld Sandbox.Engine.Utils.MySpectatorCameraController Sandbox.Engine.Utils.MySpectatorCameraController::Static
stfld VRage.Game.ModAPI.Interfaces.IMyCameraController Sandbox.Game.World.MySession::m_cameraController
ldarg.0
newobj System.Void System.Collections.Generic.HashSet`1<System.UInt64>::.ctor()
stfld System.Collections.Generic.HashSet`1<System.UInt64> Sandbox.Game.World.MySession::CreativeTools
ldarg.0
newobj System.Void System.Collections.Generic.List`1<VRage.Game.Components.MySessionComponentBase>::.ctor()
stfld System.Collections.Generic.List`1<VRage.Game.Components.MySessionComponentBase> Sandbox.Game.World.MySession::m_loadOrder
ldarg.0
ldsfld System.DateTime System.DateTime::MaxValue
stfld System.DateTime Sandbox.Game.World.MySession::m_streamingIndicatorShowTime
ldarg.0
ldc.i4.1
stfld System.Boolean Sandbox.Game.World.MySession::<HighSimulationQuality>k__BackingField
ldarg.0
newobj System.Void Sandbox.Game.GameSystems.MyOxygenProviderSystemHelper::.ctor()
stfld Sandbox.Game.GameSystems.MyOxygenProviderSystemHelper Sandbox.Game.World.MySession::m_oxygenHelper
ldarg.0
call System.Void System.Object::.ctor()
ldarg.1
brtrue.s L0
call static VRage.Utils.MyLog VRage.Utils.MyLog::get_Default()
ldstr "MySession.Static.MySession() - sync layer is null"
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
L0:
ldarg.0
ldarg.1
call System.Void Sandbox.Game.World.MySession::set_SyncLayer(Sandbox.Game.Multiplayer.MySyncLayer value)
ldarg.0
ldloca.s 0 (System.TimeSpan)
initobj System.TimeSpan
ldloc.0
call System.Void Sandbox.Game.World.MySession::set_ElapsedGameTime(System.TimeSpan value)
ldarg.0
ldfld Sandbox.Engine.Utils.MySpectatorCameraController Sandbox.Game.World.MySession::Spectator
callvirt System.Void VRage.MySpectator::Reset()
call static System.Void Sandbox.Game.Entities.MyCubeGrid::ResetInfoGizmos()
ldarg.0
ldsfld VRage.Library.Utils.MyTimeSpan VRage.Library.Utils.MyTimeSpan::Zero
stfld VRage.Library.Utils.MyTimeSpan Sandbox.Game.World.MySession::m_timeOfSave
ldarg.0
ldloca.s 0 (System.TimeSpan)
initobj System.TimeSpan
ldloc.0
call System.Void Sandbox.Game.World.MySession::set_ElapsedGameTime(System.TimeSpan value)
ldarg.0
ldc.i4.0
call System.Void Sandbox.Game.World.MySession::set_Ready(System.Boolean value)
ldarg.0
ldc.r8 0
call System.Void Sandbox.Game.World.MySession::set_MultiplayerLastMsg(System.Double value)
ldarg.0
ldc.i4.1
call System.Void Sandbox.Game.World.MySession::set_MultiplayerAlive(System.Boolean value)
ldarg.0
ldc.i4.1
call System.Void Sandbox.Game.World.MySession::set_MultiplayerDirect(System.Boolean value)
ldarg.0
ldsfld VRage.Utils.MyVersion VRage.Game.MyFinalBuildConstants::APP_VERSION
call static System.Int32 VRage.Utils.MyVersion::op_Implicit(VRage.Utils.MyVersion version)
call System.Void Sandbox.Game.World.MySession::set_AppVersionFromSave(System.Int32 value)
ldarg.0
ldfld Sandbox.Game.Multiplayer.MyFactionCollection Sandbox.Game.World.MySession::Factions
ldarg.0
ldftn System.Void Sandbox.Game.World.MySession::OnFactionsStateChanged(VRage.Game.ModAPI.MyFactionStateChange change, System.Int64 fromFactionId, System.Int64 toFactionId, System.Int64 playerId, System.Int64 sender)
newobj System.Void System.Action`5<VRage.Game.ModAPI.MyFactionStateChange, System.Int64, System.Int64, System.Int64, System.Int64>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.Multiplayer.MyFactionCollection::add_FactionStateChanged(System.Action`5<VRage.Game.ModAPI.MyFactionStateChange, System.Int64, System.Int64, System.Int64, System.Int64> value)
ldarg.0
ldfld Sandbox.Game.Multiplayer.MyPlayerCollection Sandbox.Game.World.MySession::Players
ldarg.0
ldftn System.Void Sandbox.Game.World.MySession::OnPlayersChanged(System.Boolean added, Sandbox.Game.World.PlayerId id)
newobj System.Void System.Action`2<System.Boolean, Sandbox.Game.World.PlayerId>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.Multiplayer.MyPlayerCollection::add_PlayersChanged(System.Action`2<System.Boolean, Sandbox.Game.World.PlayerId> value)
call static System.Boolean Sandbox.Game.Multiplayer.Sync::get_IsServer()
brfalse.s L1
ldarg.0
ldfld Sandbox.Game.Multiplayer.MyPlayerCollection Sandbox.Game.World.MySession::Players
ldnull
ldftn static System.Void Sandbox.Game.Entities.MyCubeGrid::Players_PlayerRemoved(Sandbox.Game.World.PlayerId obj)
newobj System.Void System.Action`1<Sandbox.Game.World.PlayerId>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.Multiplayer.MyPlayerCollection::add_PlayerRemoved(System.Action`1<Sandbox.Game.World.PlayerId> value)
L1:
ldarg.0
newobj System.Void Sandbox.Game.World.MyScriptManager::.ctor()
stfld Sandbox.Game.World.MyScriptManager Sandbox.Game.World.MySession::ScriptManager
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
ldc.i4.2
ldc.i4.1
nop
pop
pop
pop
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "GC Memory: {0} B"
ldc.i4.0
call static System.Int64 System.GC::GetTotalMemory(System.Boolean forceFullCollection)
stloc.1
ldloca.s 1 (System.Int64)
ldstr "##,#"
call System.String System.Int64::ToString(System.String format)
call static System.String System.String::Format(System.String format, System.Object arg0)
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
call static System.OperatingSystem System.Environment::get_OSVersion()
callvirt System.PlatformID System.OperatingSystem::get_Platform()
ldc.i4.4
beq.s L2
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "Process Memory: {0} B"
call static System.Diagnostics.Process System.Diagnostics.Process::GetCurrentProcess()
callvirt System.Int64 System.Diagnostics.Process::get_PrivateMemorySize64()
stloc.1
ldloca.s 1 (System.Int64)
ldstr "##,#"
call System.String System.Int64::ToString(System.String format)
call static System.String System.String::Format(System.String format, System.Object arg0)
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
L2:
ldarg.0
newobj System.Void Sandbox.Game.GameSystems.ContextHandling.MyGameFocusManager::.ctor()
call System.Void Sandbox.Game.World.MySession::set_GameFocusManager(Sandbox.Game.GameSystems.ContextHandling.MyGameFocusManager value)
ret
