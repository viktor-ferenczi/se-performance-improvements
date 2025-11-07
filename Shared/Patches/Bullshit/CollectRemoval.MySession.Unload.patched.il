// 60e54853
ldarg.0
ldc.i4.1
call System.Void Sandbox.Game.World.MySession::set_IsUnloading(System.Boolean value)
ldsfld System.Action Sandbox.Game.World.MySession::OnUnloading
call static System.Void System.DelegateExtensions::InvokeIfNotNull(System.Action handler)
call static System.Void Sandbox.Engine.Networking.MyGameService::OnSessionUnloading()
ldarg.0
ldfld Sandbox.Game.Multiplayer.MyFactionCollection Sandbox.Game.World.MySession::Factions
ldarg.0
ldftn System.Void Sandbox.Game.World.MySession::OnFactionsStateChanged(VRage.Game.ModAPI.MyFactionStateChange change, System.Int64 fromFactionId, System.Int64 toFactionId, System.Int64 playerId, System.Int64 sender)
newobj System.Void System.Action`5<VRage.Game.ModAPI.MyFactionStateChange, System.Int64, System.Int64, System.Int64, System.Int64>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.Multiplayer.MyFactionCollection::remove_FactionStateChanged(System.Action`5<VRage.Game.ModAPI.MyFactionStateChange, System.Int64, System.Int64, System.Int64, System.Int64> value)
ldarg.0
ldfld Sandbox.Game.Multiplayer.MyPlayerCollection Sandbox.Game.World.MySession::Players
ldarg.0
ldftn System.Void Sandbox.Game.World.MySession::OnPlayersChanged(System.Boolean added, Sandbox.Game.World.PlayerId id)
newobj System.Void System.Action`2<System.Boolean, Sandbox.Game.World.PlayerId>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.Multiplayer.MyPlayerCollection::remove_PlayersChanged(System.Action`2<System.Boolean, Sandbox.Game.World.PlayerId> value)
call static System.Void Sandbox.Game.Entities.MyCubeGrid::Unload()
ldarg.0
ldnull
stfld System.Action`2<VRage.Game.ModAPI.Interfaces.IMyCameraController, VRage.Game.ModAPI.Interfaces.IMyCameraController> Sandbox.Game.World.MySession::CameraAttachedToChanged
ldsfld VRage.Utils.MyStringId Sandbox.Game.Localization.MySpaceTexts::ProgressScreen_UnloadingWorld
call static System.Text.StringBuilder VRage.MyTexts::Get(VRage.Utils.MyStringId id)
ldloca.s 1 (System.Nullable`1[VRage.Utils.MyStringId])
initobj System.Nullable`1[VRage.Utils.MyStringId]
ldloc.1
ldc.i4.1
ldc.i4.1
newobj System.Void Sandbox.Game.MyGuiScreenProgress::.ctor(System.Text.StringBuilder text, System.Nullable`1<VRage.Utils.MyStringId> cancelText, System.Boolean isTopMostScreen, System.Boolean enableBackgroundFade)
stloc.0
ldloc.0
call static System.Void Sandbox.Graphics.GUI.MyScreenManager::AddScreenNow(Sandbox.Graphics.GUI.MyGuiScreenBase screen)
ldloc.0
callvirt System.Void Sandbox.Game.MyGuiScreenProgress::DrawPaused()
L2:
[EX_BeginException]
call static ParallelTasks.IWorkScheduler ParallelTasks.Parallel::get_Scheduler()
ldc.r8 16
call static System.TimeSpan System.TimeSpan::FromMilliseconds(System.Double value)
callvirt abstract virtual System.Boolean ParallelTasks.IWorkScheduler::WaitForTasksToFinish(System.TimeSpan waitTimeout)
stloc.2
[EX_BeginException]
ldarg.0
ldc.i4 2147483647
stfld System.Int32 Sandbox.Game.World.MySession::m_gameplayFrameCounter
call static System.Void ParallelTasks.Parallel::RunCallbacks()
leave.s L0
[EX_BeginCatch]
stloc.3
call static VRage.Utils.MyLog VRage.Utils.MyLog::get_Default()
ldstr "Exception occurred while unloading session"
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
call static VRage.Utils.MyLog VRage.Utils.MyLog::get_Default()
ldloc.3
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.Exception ex)
[EX_EndException]
leave.s L1
L0:
L1:
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual System.Void VRage.IVRagePlatform::Update()
call static System.Void Sandbox.Engine.Networking.MyGameService::Update()
ldloc.2
ldsfld Sandbox.MySandboxGame Sandbox.MySandboxGame::Static
ldc.i4.1
callvirt System.Boolean Sandbox.MySandboxGame::ProcessInvoke(System.Boolean isClosing)
ldc.i4.0
ceq
and
stloc.2
ldloc.2
brfalse.s L2
call static Sandbox.Game.Gui.MyGuiScreenLoading Sandbox.Graphics.GUI.MyScreenManager::GetFirstScreenOfType()
dup
brtrue.s L3
pop
call static Sandbox.Game.MyGuiScreenProgress Sandbox.Graphics.GUI.MyScreenManager::GetFirstScreenOfType()
L3:
ldc.i4.1
call static System.Void Sandbox.Graphics.GUI.MyScreenManager::CloseAllScreensNowExcept(Sandbox.Graphics.GUI.MyGuiScreenBase dontRemove, System.Boolean isUnloading)
ldc.i4.s 16
call static System.Void Sandbox.Graphics.GUI.MyGuiSandbox::Update(System.Int32 totalTimeInMS)
call static Sandbox.Game.Gui.MyGuiScreenLoading Sandbox.Graphics.GUI.MyScreenManager::GetFirstScreenOfType()
dup
brtrue.s L4
pop
call static Sandbox.Game.MyGuiScreenProgress Sandbox.Graphics.GUI.MyScreenManager::GetFirstScreenOfType()
L4:
ldc.i4.1
call static System.Void Sandbox.Graphics.GUI.MyScreenManager::CloseAllScreensNowExcept(Sandbox.Graphics.GUI.MyGuiScreenBase dontRemove, System.Boolean isUnloading)
ldc.i4.0
call static System.Void Sandbox.MySandboxGame::set_IsPaused(System.Boolean value)
call static System.Boolean Sandbox.Game.Gui.MyHud::get_RotatingWheelVisible()
brfalse.s L5
call static System.Void Sandbox.Game.Gui.MyHud::PopRotatingWheelVisible()
L5:
ldc.i4.0
stsfld System.Boolean Sandbox.Engine.Platform.Game::EnableSimSpeedLocking
ldsfld Sandbox.Engine.Utils.MySpectatorCameraController Sandbox.Engine.Utils.MySpectatorCameraController::Static
callvirt System.Void Sandbox.Engine.Utils.MySpectatorCameraController::CleanLight()
call static Sandbox.Game.World.MySession Sandbox.Game.World.MySession::get_Static()
brfalse L6
call static Sandbox.Engine.Analytics.MySpaceAnalytics Sandbox.Engine.Analytics.MySpaceAnalytics::get_Instance()
brfalse.s L7
call static Sandbox.Engine.Analytics.MySpaceAnalytics Sandbox.Engine.Analytics.MySpaceAnalytics::get_Instance()
callvirt System.Void Sandbox.Engine.Analytics.MySpaceAnalytics::ReportMinedStats()
call static Sandbox.Engine.Analytics.MySpaceAnalytics Sandbox.Engine.Analytics.MySpaceAnalytics::get_Instance()
callvirt System.Void Sandbox.Engine.Analytics.MySpaceAnalytics::ReportEmotesStats()
call static Sandbox.Engine.Analytics.MySpaceAnalytics Sandbox.Engine.Analytics.MySpaceAnalytics::get_Instance()
callvirt System.Void Sandbox.Engine.Analytics.MySpaceAnalytics::ReportBlocksStats()
call static Sandbox.Engine.Analytics.MySpaceAnalytics Sandbox.Engine.Analytics.MySpaceAnalytics::get_Instance()
callvirt System.Void Sandbox.Engine.Analytics.MySpaceAnalytics::ReportNetworkStats()
call static Sandbox.Engine.Analytics.MySpaceAnalytics Sandbox.Engine.Analytics.MySpaceAnalytics::get_Instance()
callvirt System.Void Sandbox.Engine.Analytics.MySpaceAnalytics::ReportWorldEnd()
L7:
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "MySession::Unload START"
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
call static System.Void Sandbox.Game.World.MySessionSnapshot::WaitForSaving()
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "AutoSaveInMinutes: "
ldarg.0
call System.UInt32 Sandbox.Game.World.MySession::get_AutoSaveInMinutes()
box System.UInt32
call static System.String System.String::Concat(System.Object arg0, System.Object arg1)
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "MySandboxGame.IsDedicated: "
ldsflda System.Boolean Sandbox.Engine.Platform.Game::IsDedicated
call virtual System.String System.Boolean::ToString()
call static System.String System.String::Concat(System.String str0, System.String str1)
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "IsServer: "
call static System.Boolean Sandbox.Game.Multiplayer.Sync::get_IsServer()
stloc.s 4 (System.Boolean)
ldloca.s 4 (System.Boolean)
call virtual System.String System.Boolean::ToString()
call static System.String System.String::Concat(System.String str0, System.String str1)
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
call static Sandbox.Game.World.MySession Sandbox.Game.World.MySession::get_Static()
callvirt System.Boolean Sandbox.Game.World.MySession::get_LoadFailed()
brtrue.s L8
ldsfld System.Boolean Sandbox.Engine.Platform.Game::IsDedicated
brfalse.s L9
ldsfld VRage.Game.ModAPI.IMyConfigDedicated Sandbox.MySandboxGame::ConfigDedicated
callvirt abstract virtual System.Boolean VRage.Game.ModAPI.IMyConfigDedicated::get_RestartSave()
brfalse.s L10
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "Autosave in unload"
callvirt System.Void VRage.Utils.MyLog::WriteLineAndConsole(System.String msg)
ldarg.0
ldnull
ldnull
call System.Boolean Sandbox.Game.World.MySession::Save(System.String customSaveName, System.Action`1<Sandbox.Game.World.SaveProgress> progress)
pop
L8:
L9:
L10:
ldsfld Sandbox.MySandboxGame Sandbox.MySandboxGame::Static
callvirt System.Void Sandbox.MySandboxGame::ClearInvokeQueue()
call static System.Void Sandbox.Game.GameSystems.MyDroneAIDataStatic::Reset()
call static VRage.Audio.IMyAudio VRage.Audio.MyAudio::get_Static()
callvirt abstract virtual System.Void VRage.Audio.IMyAudio::StopUpdatingAll3DCues()
call static VRage.Audio.IMyAudio VRage.Audio.MyAudio::get_Static()
ldc.i4.1
callvirt abstract virtual System.Void VRage.Audio.IMyAudio::set_Mute(System.Boolean value)
call static VRage.Audio.IMyAudio VRage.Audio.MyAudio::get_Static()
callvirt abstract virtual System.Void VRage.Audio.IMyAudio::StopMusic()
call static VRage.Audio.IMyAudio VRage.Audio.MyAudio::get_Static()
ldc.r4 1
ldc.r4 0
callvirt abstract virtual System.Void VRage.Audio.IMyAudio::ChangeGlobalVolume(System.Single level, System.Single time)
call static VRage.Collections.ListReader`1<VRage.Data.Audio.MySoundData> VRage.Audio.MyAudioExtensions::GetSoundDataFromDefinitions()
call static VRage.Collections.ListReader`1<VRage.Data.Audio.MyAudioEffect> VRage.Audio.MyAudioExtensions::GetEffectData()
call static System.Void VRage.Audio.MyAudio::ReloadData(VRage.Collections.ListReader`1<VRage.Data.Audio.MySoundData> sounds, VRage.Collections.ListReader`1<VRage.Data.Audio.MyAudioEffect> effects)
ldsfld System.Collections.Concurrent.ConcurrentDictionary`2<VRage.Audio.MyCueId, Sandbox.Game.Entities.LastTimePlayingData> Sandbox.Game.Entities.MyEntity3DSoundEmitter::LastTimePlaying
callvirt virtual System.Void System.Collections.Concurrent.ConcurrentDictionary`2<VRage.Audio.MyCueId, Sandbox.Game.Entities.LastTimePlayingData>::Clear()
call static System.Void Sandbox.Game.Entities.MyEntity3DSoundEmitter::ClearEntityEmitters()
ldarg.0
ldc.i4.0
call System.Void Sandbox.Game.World.MySession::set_Ready(System.Boolean value)
ldarg.0
ldfld Sandbox.Game.Entities.MyVoxelMaps Sandbox.Game.World.MySession::VoxelMaps
callvirt System.Void Sandbox.Game.Entities.MyVoxelMaps::Clear()
ldsfld Sandbox.Engine.Utils.MyConfig Sandbox.MySandboxGame::Config
callvirt System.Void Sandbox.Engine.Utils.MyConfigBase::Save()
ldarg.0
call Sandbox.Game.World.MyPlayer Sandbox.Game.World.MySession::get_LocalHumanPlayer()
brfalse.s L11
ldarg.0
call Sandbox.Game.World.MyPlayer Sandbox.Game.World.MySession::get_LocalHumanPlayer()
callvirt Sandbox.Game.Entities.Character.MyCharacter Sandbox.Game.World.MyPlayer::get_Character()
dup
brtrue.s L12
pop
ldnull
br.s L13
L12:
call Sandbox.Game.Entities.IMyControllableEntity Sandbox.Game.Entities.Character.MyCharacter::get_CurrentRemoteControl()
L13:
brfalse.s L14
ldarg.0
call Sandbox.Game.World.MyPlayer Sandbox.Game.World.MySession::get_LocalHumanPlayer()
callvirt Sandbox.Game.Entities.Character.MyCharacter Sandbox.Game.World.MyPlayer::get_Character()
dup
brtrue.s L15
pop
ldnull
br.s L16
L15:
call Sandbox.Game.Entities.IMyControllableEntity Sandbox.Game.Entities.Character.MyCharacter::get_CurrentRemoteControl()
L16:
isinst Sandbox.Game.Entities.MyShipController
dup
stloc.s 5 (Sandbox.Game.Entities.MyShipController)
brfalse.s L17
ldloc.s 5 (Sandbox.Game.Entities.MyShipController)
callvirt System.Void Sandbox.Game.Entities.MyShipController::Use()
L14:
L17:
ldarg.0
call Sandbox.Game.World.MyPlayer Sandbox.Game.World.MySession::get_LocalHumanPlayer()
callvirt Sandbox.Game.World.MyEntityController Sandbox.Game.World.MyPlayer::get_Controller()
brfalse.s L18
ldarg.0
call Sandbox.Game.World.MyPlayer Sandbox.Game.World.MySession::get_LocalHumanPlayer()
callvirt Sandbox.Game.World.MyEntityController Sandbox.Game.World.MyPlayer::get_Controller()
callvirt System.Void Sandbox.Game.World.MyEntityController::SaveCamera()
L11:
L18:
call static System.Void VRage.Game.News.NewContentNotification.MyNewContentNotificationsBase::SaveViewedContentInfo()
call static System.Void VRageRender.MyRenderProxy::ClearPersistentBillboards()
ldarg.0
call System.Void Sandbox.Game.World.MySession::DisconnectMultiplayer()
ldarg.0
ldc.i4.0
call System.Void Sandbox.Game.World.MySession::UnloadDataComponents(System.Boolean beforeLoadWorld)
ldarg.0
call System.Void Sandbox.Game.World.MySession::UnloadMultiplayer()
call static System.Void Sandbox.Game.Gui.MyTerminalControlFactory::Unload()
call static Sandbox.Definitions.MyDefinitionManager Sandbox.Definitions.MyDefinitionManager::get_Static()
ldc.i4.0
callvirt System.Void Sandbox.Definitions.MyDefinitionManager::UnloadData(System.Boolean clearPreloaded)
call static Sandbox.Definitions.MyDefinitionManager Sandbox.Definitions.MyDefinitionManager::get_Static()
callvirt System.Void Sandbox.Definitions.MyDefinitionManager::PreloadDefinitions()
call static System.Boolean Sandbox.Game.Multiplayer.Sync::get_IsDedicated()
brtrue.s L19
call static System.Void Sandbox.Graphics.MyGuiManager::InitFonts()
L19:
call static VRage.Input.IMyInput VRage.Input.MyInput::get_Static()
callvirt abstract virtual System.Void VRage.Input.IMyInput::ClearBlacklist()
ldsfld System.Boolean Sandbox.Engine.Platform.Game::IsDedicated
brtrue.s L20
call static EmptyKeys.UserInterface.Engine EmptyKeys.UserInterface.Engine::get_Instance()
callvirt abstract virtual EmptyKeys.UserInterface.AssetManager EmptyKeys.UserInterface.Engine::get_AssetManager()
isinst VRage.UserInterface.MyAssetManager
stloc.s 6 (VRage.UserInterface.MyAssetManager)
ldloc.s 6 (VRage.UserInterface.MyAssetManager)
brfalse.s L21
ldloc.s 6 (VRage.UserInterface.MyAssetManager)
callvirt System.Void VRage.UserInterface.MyAssetManager::UnloadGeneratedTextures()
L20:
L21:
call static System.Void VRage.Game.MyDefinitionErrors::Clear()
call static System.Void Sandbox.Game.EntityComponents.MyResourceDistributorComponent::Clear()
ldsflda VRageMath.Matrix VRageMath.Matrix::Zero
call static VRageMath.MatrixD VRageMath.MatrixD::op_Implicit(VRageMath.Matrix& m)
ldsfld VRageMath.Matrix VRageMath.Matrix::Zero
ldsfld VRageMath.Matrix VRageMath.Matrix::Zero
ldc.r4 0
ldc.r4 0
ldc.r4 0
ldc.r4 0
ldc.r4 0
ldsfld VRageMath.Vector3D VRageMath.Vector3D::Zero
ldc.r4 0
ldc.r4 0
ldc.i4.1
ldc.i4.1
call static System.Void VRageRender.MyRenderProxy::SetCameraViewMatrix(VRageMath.MatrixD viewMatrix, VRageMath.Matrix projectionMatrix, VRageMath.Matrix projectionFarMatrix, System.Single fov, System.Single fovSkybox, System.Single nearPlane, System.Single farPlane, System.Single farFarPlane, VRageMath.Vector3D cameraPosition, System.Single projectionOffsetX, System.Single projectionOffsetY, System.Int32 lastMomentUpdateIndex, System.Boolean smooth)
call static System.Void VRageRender.MyRenderProxy::UnloadData()
ldsfld Sandbox.Game.Gui.MyHudQuestlog Sandbox.Game.Gui.MyHud::Questlog
callvirt System.Void Sandbox.Game.Gui.MyHudQuestlog::CleanDetails()
ldsfld Sandbox.Game.Gui.MyHudQuestlog Sandbox.Game.Gui.MyHud::Questlog
ldc.i4.0
callvirt System.Void Sandbox.Game.Gui.MyHudQuestlog::set_Visible(System.Boolean value)
ldsfld Sandbox.Game.Gui.MyHudQuestlog Sandbox.Game.Gui.MyHud::BuffInfo
callvirt System.Void Sandbox.Game.Gui.MyHudQuestlog::CleanDetails()
ldsfld Sandbox.Game.Gui.MyHudQuestlog Sandbox.Game.Gui.MyHud::BuffInfo
ldc.i4.0
callvirt System.Void Sandbox.Game.Gui.MyHudQuestlog::set_Visible(System.Boolean value)
call static System.Void Sandbox.ModAPI.MyAPIGateway::Clean()
call static System.Void Sandbox.Game.GameSystems.MyOxygenProviderSystem::ClearOxygenGenerators()
call static System.Void VRageMath.MyDynamicAABBTree::Dispose()
call static System.Void VRageMath.MyDynamicAABBTreeD::Dispose()
call static System.Void VRage.Render.Particles.MyParticleEffectsLibrary::Close()
ldarg.0
ldfld Sandbox.Game.Multiplayer.MyFactionCollection Sandbox.Game.World.MySession::Factions
callvirt System.Void Sandbox.Game.Multiplayer.MyFactionCollection::Clear()
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
callvirt abstract virtual System.Boolean VRage.IVRageSystem::get_IsMemoryLimited()
brfalse.s L22
call static System.Void VRage.Game.Models.MyModels::UnloadModdedModels()
L22:
call static System.Void VRage.Game.Entity.UseObject.MyUseObjectFactory::RestoreBaseGamePreset()
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
call static System.Int32 System.GC::get_MaxGeneration()
ldc.i4.1
nop
pop
pop
pop
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "MySession::Unload END"
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
ldsfld System.Collections.Generic.Dictionary`2<Sandbox.Game.World.PlayerId, System.Collections.Generic.List`1<VRageMath.Vector3>> Sandbox.Game.Entities.MyCubeBuilder::AllPlayersColors
brfalse.s L23
ldsfld System.Collections.Generic.Dictionary`2<Sandbox.Game.World.PlayerId, System.Collections.Generic.List`1<VRageMath.Vector3>> Sandbox.Game.Entities.MyCubeBuilder::AllPlayersColors
callvirt virtual System.Void System.Collections.Generic.Dictionary`2<Sandbox.Game.World.PlayerId, System.Collections.Generic.List`1<VRageMath.Vector3>>::Clear()
L23:
ldsfld Sandbox.Game.Gui.MyHudScreenEffects Sandbox.Game.Gui.MyHud::ScreenEffects
ldnull
stfld System.Action Sandbox.Game.Gui.MyHudScreenEffects::OnBlackscreenFadeFinishedCallback
ldsfld System.Action Sandbox.Game.World.MySession::OnUnloaded
call static System.Void System.DelegateExtensions::InvokeIfNotNull(System.Action handler)
call static System.Void Sandbox.Engine.Networking.MyGameService::OnSessionUnloaded()
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
callvirt abstract virtual System.Void VRage.IVRageSystem::OnSessionUnloaded()
call static ParallelTasks.IWorkScheduler ParallelTasks.Parallel::get_Scheduler()
ldc.i4.m1
conv.i8
newobj System.Void System.TimeSpan::.ctor(System.Int64 ticks)
callvirt abstract virtual System.Boolean ParallelTasks.IWorkScheduler::WaitForTasksToFinish(System.TimeSpan waitTimeout)
pop
call static System.Void ParallelTasks.Parallel::Clean()
call static System.Int32 Havok.HkHandle::GetHandlesAmount()
ldc.i4.0
ble.s L24
call static VRage.Utils.MyLog VRage.Utils.MyLog::get_Default()
ldstr "Shouldn\'t have any unclosed handles. Closing HkHandles {0}"
call static System.Int32 Havok.HkHandle::GetHandlesAmount()
box System.Int32
call static System.String System.String::Format(System.String format, System.Object arg0)
call static System.Object[] System.Array::Empty()
call static System.Void VRage.Utils.MyLogExtensions::Error(VRage.Utils.MyLog self, System.String message, System.Object[] args)
L24:
call static System.Void Sandbox.Game.Entities.Character.MyCharacter::ResetOnCharacterDiedEvent()
leave.s L25
L6:
ldsfld VRage.Utils.MyLog Sandbox.MySandboxGame::Log
ldstr "MySession::Game Unloaded unexpectedly. Suspected reason is power failure/Sleep mode"
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
leave.s L26
[EX_BeginFinally]
ldloc.0
ldc.i4.0
callvirt virtual System.Boolean Sandbox.Graphics.GUI.MyGuiScreenBase::CloseScreen(System.Boolean isUnloading)
pop
[EX_EndException]
endfinally
L25:
ldarg.0
ldc.i4.0
call System.Void Sandbox.Game.World.MySession::set_IsUnloading(System.Boolean value)
L26:
ret
