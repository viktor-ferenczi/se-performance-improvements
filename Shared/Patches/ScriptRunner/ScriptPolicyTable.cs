namespace Shared.Patches;

// The policy table of ScriptPolicy, one line per member, tab separated: the member's full name
// (namespace, type, member; generic types without their parameters), the policy of the getter
// or method, the policy of the setter ("-" for none) and a note. The policies are pass, queue
// and main, see MemberPolicy. A member missing here is main.
//
// It is code, not an embedded file, because the plugin loaders compile the plugin from its
// .cs files only. It has a row for every member of the game's PB whitelist that
// ScriptEligibility looks up (Tests/Shared/Patches/ScriptWhitelist.txt, made by
// Tools/PbProfile/whitelist.py), each classified from the decompiled game code (SE-626).
// A policy with @ names the mechanism it relies on (StandIn), see ScriptPolicy. Static members
// can only be pass or main, and pass only if they use a game object argument through its
// interfaces (a proxy fails a type test or cast to a game class) or a patch unwraps it.
internal static class ScriptPolicyTable
{
    public const string Text =
        @"Sandbox.Game.Entities.MyCompositeGameLogicComponent.GetAs	pass	-	MyCompositeGameLogicComponent.cs:84 foreach over own m_logicComponents with type test (string overload :226 FirstOrDefault); read only
Sandbox.Game.EntityComponents.MyResourceSinkComponent.AcceptedResources	pass	-	MyResourceSinkComponent.cs:168 new ListReader over m_resourceIds; read only
Sandbox.Game.EntityComponents.MyResourceSinkComponent.CurrentInput	pass	-	MyResourceSinkComponent.cs:163 -> CurrentInputByType :454 m_dataPerType array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.CurrentInputByType	pass	-	MyResourceSinkComponent.cs:454 m_dataPerType[GetTypeIndex] read; distributor writes it on main thread only
Sandbox.Game.EntityComponents.MyResourceSinkComponent.IsPowerAvailable	pass	-	MyResourceSinkComponent.cs:427 CurrentInput + distributor Sink_IsResourceAvailable (MyResourceDistributorComponent.cs:2852) dict/HashSet reads
Sandbox.Game.EntityComponents.MyResourceSinkComponent.IsPowered	pass	-	MyResourceSinkComponent.cs:166 -> IsPoweredByType :464 m_dataPerType array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.IsPoweredByType	pass	-	MyResourceSinkComponent.cs:464 m_dataPerType[GetTypeIndex] read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.MaxRequiredInputByType	pass	-	MyResourceSinkComponent.cs:444 array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.RequiredInput	pass	-	MyResourceSinkComponent.cs:157 -> RequiredInputByType :459 array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.RequiredInputByType	pass	-	MyResourceSinkComponent.cs:459 array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.SuppliedRatio	pass	-	MyResourceSinkComponent.cs:160 -> SuppliedRatioByType :494 array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.SuppliedRatioByType	pass	-	MyResourceSinkComponent.cs:494 m_dataPerType[GetTypeIndex] read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.CurrentOutput	pass	-	MyResourceSourceComponent.cs:112 FirstPair of m_resourceTypeToIndex -> CurrentOutputByType :226 array read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.CurrentOutputByType	pass	-	MyResourceSourceComponent.cs:226 array read (GetTypeIndex may throw KeyNotFound, read only)
Sandbox.Game.EntityComponents.MyResourceSourceComponent.DefinedOutput	pass	-	MyResourceSourceComponent.cs:116 -> DefinedOutputByType :304 array read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.DefinedOutputByType	pass	-	MyResourceSourceComponent.cs:304 m_dataPerType[GetTypeIndex] read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.HasCapacityRemaining	pass	-	MyResourceSourceComponent.cs:138 -> HasCapacityRemainingByType :314 RemainingCapacity, session CreativeMode and array reads
Sandbox.Game.EntityComponents.MyResourceSourceComponent.MaxOutput	pass	-	MyResourceSourceComponent.cs:114 -> MaxOutputByType :268 MaxOutputLimitedByCapacity pure math on m_dataPerType
Sandbox.Game.EntityComponents.MyResourceSourceComponent.MaxOutputByType	pass	-	MyResourceSourceComponent.cs:268 MaxOutputLimitedByCapacity: pure math on m_dataPerType
Sandbox.Game.EntityComponents.MyResourceSourceComponent.ProductionEnabled	pass	-	MyResourceSourceComponent.cs:118 -> ProductionEnabledByType :344 array read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.ProductionEnabledByType	pass	-	MyResourceSourceComponent.cs:344 m_dataPerType[GetTypeIndex].IsProducerEnabled read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.RemainingCapacity	pass	-	MyResourceSourceComponent.cs:120 -> RemainingCapacityByType :249 array read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.ResourceTypes	pass	-	MyResourceSourceComponent.cs:140 new ListReader over m_resourceIds
Sandbox.Game.Gui.TerminalActionExtensions.ApplyAction	pass@Terminal	-	TerminalActionExtensions.cs:14/19 GetActionWithName (TerminalStandIn) then Apply through the action proxy, queued
Sandbox.Game.Gui.TerminalActionExtensions.GetAction	pass@Terminal	-	TerminalActionExtensions.cs:9 GetActionWithName through the block proxy (TerminalStandIn)
Sandbox.ModAPI.Ingame.IMyAssembler.CooperativeMode	pass	queue	MyAssembler.cs:356 get IsSlave=Sync read; set m_slave.Value (Sync<T>.Value set (network event))
Sandbox.ModAPI.Ingame.IMyAssembler.CurrentProgress	pass	-	MyAssembler.cs:222 Sync read
Sandbox.ModAPI.Ingame.IMyAssembler.DisassembleEnabled	pass	-	MyAssembler.cs:338 -> DisassembleEnabled :307 m_disassembleEnabled field read
Sandbox.ModAPI.Ingame.IMyAssembler.Mode	pass	queue	MyAssembler.cs:340 get DisassembleEnabled field; set RequestDisassembleEnabled -> MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyAssembler.Repeating	pass	queue	MyAssembler.cs:368 get RepeatEnabled fields; set RequestRepeatEnabled -> RaiseEvent (MyAssembler.cs:1203)
Sandbox.ModAPI.Ingame.IMyAttachableTopBlock.Base	pass	-	MyAttachableTopBlockBase.cs:28 Stator => m_parentBlock field (mechanically connected, same grid group); proxy wraps result
Sandbox.ModAPI.Ingame.IMyAttachableTopBlock.IsAttached	pass	-	MyAttachableTopBlockBase.cs:26 Stator != null; MyPistonTop.cs:48 m_pistonBlock != null; field reads
Sandbox.ModAPI.Ingame.IMyAttackPatternComponent.AttackPatternId	pass	-	MyOffensiveCombatAbstractComponent.cs:167 auto-property read
Sandbox.ModAPI.Ingame.IMyAttackPatternComponent.AttackPatternName	pass	-	MyOffensiveCombatIntercept.cs:100 (and the other 3 patterns) static MySpaceTexts MyStringId read
Sandbox.ModAPI.Ingame.IMyAttackPatternComponent.AttackPatternTooltip	pass	-	MyOffensiveCombatIntercept.cs:98 (and the other 3 patterns) static MySpaceTexts MyStringId read
Sandbox.ModAPI.Ingame.IMyAttackPatternComponent.HasEnemy	pass	-	MyOffensiveCombatAbstractComponent.cs:102 field reads + FoundEnemy (MySearchEnemyComponent.cs:107, client caches own m_foundEnemy)
Sandbox.ModAPI.Ingame.IMyAttackPatternComponent.IsSelected	pass	-	MyOffensiveCombatAbstractComponent.cs:142 m_isSelected field read
Sandbox.ModAPI.Ingame.IMyAutopilotWaypoint.Matrix	pass	-	MyAutopilotWaypoint.cs:45 returns m_matrix
Sandbox.ModAPI.Ingame.IMyAutopilotWaypoint.Name	pass	-	MyAutopilotWaypoint.cs:187 returns m_name
Sandbox.ModAPI.Ingame.IMyAutopilotWaypoint.RelatedEntityId	pass	-	MyAutopilotWaypoint.cs:149 returns m_relatedEntityId
Sandbox.ModAPI.Ingame.IMyAutopilotWaypoint.RelativeMatrix	main	-	MyAutopilotWaypoint.cs:80 RelatedEntity -> UpdateRelatedEntity :264 MySandboxGame.Invoke on a worker thread; writes m_relatedMatrixCurrent, HUD GPS Coords
Sandbox.ModAPI.Ingame.IMyBasicMissionBlock.SelectedMissionId	pass	queue	MyBasicMissionBlock.cs:47 m_selectedMission Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyBasicMissionComponent.DetailedInfo	pass	-	MyBasicMissionFollowHome.cs:160 (FollowPlayer :130, Autopilot :154) get-only StringBuilder auto-property of own component
Sandbox.ModAPI.Ingame.IMyBasicMissionComponent.IsSelected	pass	queue	MyBasicMissionFollowHome.cs:162 field read; set (also FollowPlayer :134) changes AiBlockSystem waypoints, Sync sets, event subscriptions
Sandbox.ModAPI.Ingame.IMyBasicMissionComponent.MissionName	pass	-	MyBasicMissionFollowHome.cs:156 (FollowPlayer :126, Autopilot :187) static MySpaceTexts MyStringId
Sandbox.ModAPI.Ingame.IMyBasicMissionComponent.UniqueSelectionId	pass	-	MyBasicMissionFollowHome.cs:158 (FollowPlayer :128, Autopilot :152) auto-property read
Sandbox.ModAPI.Ingame.IMyBasicMissionFollowHome.MaxRange	pass	queue	MyBasicMissionFollowHome.cs:216 Sync read; set m_maxRange.Value only if IsSelected
Sandbox.ModAPI.Ingame.IMyBasicMissionFollowHome.MinRange	pass	queue	MyBasicMissionFollowHome.cs:201 Sync read; set m_minRange.Value only if IsSelected
Sandbox.ModAPI.Ingame.IMyBasicMissionFollowHome.Wander	pass	queue	MyBasicMissionFollowHome.cs:231 Sync read; set Sync (if IsSelected) + UpdateSbForFlightBlock
Sandbox.ModAPI.Ingame.IMyBasicMissionFollowPlayer.FollowDistance	pass	queue	MyBasicMissionFollowPlayer.cs:183 Sync read; set m_followDistance.Value only if IsSelected
Sandbox.ModAPI.Ingame.IMyBatteryBlock.ChargeMode	pass	queue	MyBatteryBlock.cs:286 Sync read / m_chargeMode.Value set
Sandbox.ModAPI.Ingame.IMyBatteryBlock.CurrentInput	pass	-	MyBatteryBlock.cs:211 ResourceSink.CurrentInputByType array read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.CurrentStoredPower	pass	-	MyBatteryBlock.cs:174 SourceComp.RemainingCapacityByType array read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.HasCapacityRemaining	pass	-	MyBatteryBlock.cs:155 SourceComp.HasCapacityRemainingByType array/session reads
Sandbox.ModAPI.Ingame.IMyBatteryBlock.IsCharging	pass	-	MyBatteryBlock.cs:235 CurrentInput/CurrentOutput resource array reads
Sandbox.ModAPI.Ingame.IMyBatteryBlock.MaxInput	pass	-	MyBatteryBlock.cs:223 ResourceSink.MaxRequiredInputByType array read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.MaxStoredPower	pass	-	MyBatteryBlock.cs:157 field read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.OnlyDischarge	pass	queue	MyBatteryBlock.cs:274 m_chargeMode Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyBatteryBlock.OnlyRecharge	pass	queue	MyBatteryBlock.cs:262 m_chargeMode Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyBatteryBlock.SemiautoEnabled	pass	queue	MyBatteryBlock.cs:247 m_chargeMode Sync read / Sync.Value set (only when true)
Sandbox.ModAPI.Ingame.IMyBeacon.HudText	pass	queue	MyBeacon.cs:173 HudText.ToString(); set SetHudText :290 CompareUpdate + MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyBeacon.Radius	pass	queue	MyBeacon.cs:161 get RadioBroadcaster.BroadcastRadius field; set rewrites global MyRadioBroadcasters tree (MyRadioBroadcaster.cs:80)
Sandbox.ModAPI.Ingame.IMyBlockGroup.GetBlocks	pass	-	MyBlockGroup.cs:54 iterates group-private HashSet, IsAccessibleForProgrammableBlock field; proxy must wrap list results and the collect callback args (raw blocks)
Sandbox.ModAPI.Ingame.IMyBlockGroup.GetBlocksOfType	pass	-	MyBlockGroup.cs:66/78 same as GetBlocks; proxy must wrap results and collect args
Sandbox.ModAPI.Ingame.IMyBlockGroup.Name	pass	-	MyBlockGroup.cs:19 Name.ToString() new string
Sandbox.ModAPI.Ingame.IMyBroadcastListener.IsActive	pass	-	BroadcastListener.cs:10 auto-property read
Sandbox.ModAPI.Ingame.IMyBroadcastListener.Tag	pass	-	BroadcastListener.cs auto-property read
Sandbox.ModAPI.Ingame.IMyCameraBlock.AvailableScanRange	pass	-	lazy accumulation in the own camera block. Agent: MyCameraBlock.cs:138 getter lazily accumulates into m_availableScanRange/m_lastUpdateTime (write). Replace: return m_availableScanRange + (TotalGamePlayTimeInMilliseconds - m_lastUpdateTime)*RaycastTimeMultiplier when IsWorking&&EnableRaycast, without storing
Sandbox.ModAPI.Ingame.IMyCameraBlock.CanScan	pass	-	reads AvailableScanRange, see there. Agent: MyCameraBlock.cs:813-860 all overloads read AvailableScanRange (lazy write, see AvailableScanRange); replace with the non-storing range calc + same limit/CheckAngleLimits math
Sandbox.ModAPI.Ingame.IMyCameraBlock.EnableRaycast	pass	queue	MyCameraBlock.cs:224 get auto-property; set writes m_lastUpdateTime and ResourceSink.Update()
Sandbox.ModAPI.Ingame.IMyCameraBlock.IsActive	pass	-	MyCameraBlock.cs:249 m_activeViewers Sync read
Sandbox.ModAPI.Ingame.IMyCameraBlock.Raycast	pass@Sensing	-	MyCameraBlock.cs:709 own scan range, CastRay under the Havok world lock; planet voxel test serialized (SensingStandIn)
Sandbox.ModAPI.Ingame.IMyCameraBlock.RaycastConeLimit	pass	-	MyCameraBlock.cs:241 definition read
Sandbox.ModAPI.Ingame.IMyCameraBlock.RaycastDistanceLimit	pass	-	MyCameraBlock.cs:243 definition read
Sandbox.ModAPI.Ingame.IMyCameraBlock.RaycastTimeMultiplier	pass	-	MyCameraBlock.cs:245 definition read
Sandbox.ModAPI.Ingame.IMyCameraBlock.TimeUntilScan	pass	-	reads AvailableScanRange, see there. Agent: MyCameraBlock.cs:862 reads AvailableScanRange (lazy write); replace with non-storing range calc
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.BroadcastTarget	pass	queue	MyChatBroadcastEntityComponent.cs:185 m_target Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.CustomName	pass	queue	MyChatBroadcastEntityComponent.cs:197 m_customName Sync read; set SetCustomName :926 truncates + Sync.Value set
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.GetMessage	pass	-	MyChatBroadcastEntityComponent.cs:343 m_messages array read
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.MaxMessageCount	pass	-	MyChatBroadcastEntityComponent.cs:221 constant 8
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.SendGps	queue	-	MyChatBroadcastEntityComponent.cs:383 builds MyGps text then SendMessage :365 MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.SendMessage	queue	-	MyChatBroadcastEntityComponent.cs:352/365 both overloads MyMultiplayer.RaiseEvent OnTransmitMessage
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.SendRandomMessage	queue	-	MyChatBroadcastEntityComponent.cs:373 shared Random via MyUtils.GetRandomInt, then RaiseEvent
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.SetMessage	queue	-	MyChatBroadcastEntityComponent.cs:328 writes m_messages and m_messagesSync.Value (network); GetMessage sees old text until replay
Sandbox.ModAPI.Ingame.IMyChatBroadcastControllerComponent.UseAntenna	pass	queue	MyChatBroadcastEntityComponent.cs:209 m_useAntenna Sync read / Sync.Value set (ValueChanged handler)
Sandbox.ModAPI.Ingame.IMyCockpit.OxygenCapacity	pass	-	MyCockpit.cs:477 definition read
Sandbox.ModAPI.Ingame.IMyCockpit.OxygenFilledRatio	pass	-	MyCockpit.cs:459 OxygenFillLevel Sync read
Sandbox.ModAPI.Ingame.IMyCollector.UseConveyorSystem	pass	queue	MyCollector.cs:130 m_useConveyorSystem Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyConveyorSorter.AddItem	queue	-	MyConveyorSorter.cs:828 ChangeListType/ChangeListId :503/:529 MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyConveyorSorter.DrainAll	pass	queue	MyConveyorSorter.cs:173 Sync read / m_drainAll.Value set
Sandbox.ModAPI.Ingame.IMyConveyorSorter.GetFilterList	pass	-	MyConveyorSorter.cs:810 reads m_inventoryConstraint sets, fills caller list with new filters
Sandbox.ModAPI.Ingame.IMyConveyorSorter.IsAllowed	pass	-	MyConveyorSorter.cs:219 Enabled/IsWorking/sink reads + m_inventoryConstraint.Check HashSet reads
Sandbox.ModAPI.Ingame.IMyConveyorSorter.Mode	pass	-	MyConveyorSorter.cs:207 m_inventoryConstraint.IsWhitelist read
Sandbox.ModAPI.Ingame.IMyConveyorSorter.RemoveItem	queue	-	MyConveyorSorter.cs:843 ChangeListType/ChangeListId MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyConveyorSorter.SetFilter	queue	-	MyConveyorSorter.cs:823 RaiseEvent DoSetupFilter; snapshot the items list at call time (script may reuse it)
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.CustomFleeCoordinates	pass	queue	MyDefensiveCombatBlock.cs:444 Sync reads; set writes m_customFleeCoordinates and m_useCustomFleeCoordinates Sync
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.Flee	queue	-	MyDefensiveCombatBlock.cs:515 -> Flee :1803 UpdateIsWorking, SetNextWaypoint / AiBlockSystem.SetMovementWaypoints, combat state writes
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.FleeBeaconEntityId	pass	-	MyDefensiveCombatBlock.cs:388 m_selectedBeaconEntityId Sync read
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.FleeCoordinates	pass	-	MyDefensiveCombatBlock.cs:404 GetFleeCoordinates: Sync reads, MyGpsCollection.GetGps dict reads, MyEntities.GetEntityById + WorldMatrix read
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.FleeGps	pass	-	MyDefensiveCombatBlock.cs:419 m_selectedGpsHash Sync read
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.FleeTrigger	pass	queue	MyDefensiveCombatBlock.cs:376 m_fleeTrigger Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.IsFleeing	pass	-	MyDefensiveCombatBlock.cs:358 m_currentFleeCoordinates Sync read
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.LockTarget	pass	queue	MyDefensiveCombatBlock.cs:469 m_lockTarget Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.SearchEnemyComponent	pass	-	MyDefensiveCombatBlock.cs:481 returns m_searchEnemyComponent field; proxy wraps result
Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock.WaypointZoneSize	pass	queue	MyDefensiveCombatBlock.cs:406 Sync read; set clamps to definition range + Sync.Value set
Sandbox.ModAPI.Ingame.IMyDoor.CloseDoor	queue	-	MyDoor.cs:168/MyAdvancedDoor.cs:229/MyAirtightDoorGeneric.cs:132 -> ToggleDoor: SetOpenRequest RaiseEvent or Open Sync set
Sandbox.ModAPI.Ingame.IMyDoor.Open	pass	-	MyDoorBase.cs:67 m_open Sync read
Sandbox.ModAPI.Ingame.IMyDoor.OpenDoor	queue	-	MyDoor.cs:156/MyAdvancedDoor.cs:217/MyAirtightDoorGeneric.cs:120 -> ToggleDoor network write
Sandbox.ModAPI.Ingame.IMyDoor.OpenRatio	pass	-	MyDoor.cs:129 / MyAdvancedDoor.cs:168 (loop over list) / MyAirtightDoorGeneric.cs:106 field reads
Sandbox.ModAPI.Ingame.IMyDoor.Status	pass	-	MyDoor.cs:109 / MyAdvancedDoor.cs:101 / MyAirtightDoorGeneric.cs:84 field reads
Sandbox.ModAPI.Ingame.IMyDoor.ToggleDoor	queue	-	MyDoor.cs:180 SetOpenRequest RaiseEvent; MyAirtightDoorGeneric.cs:144 Open Sync set
Sandbox.ModAPI.Ingame.IMyEventControllerBlock.IsAndModeEnabled	pass	queue	MyEventControllerBlock.cs:200 m_isAndModeEnabled Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyEventControllerBlock.IsLowerOrEqualCondition	pass	queue	MyEventControllerBlock.cs:188 m_conditionInvert Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyEventControllerBlock.Threshold	pass	queue	MyEventControllerBlock.cs:176 m_threshold Sync read; set Clamp(0,1) + Sync.Value set (pending get returns unclamped value)
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.AmountOfSeedsRequired	pass	-	MyFarmPlotLogic.cs:383 component definition read
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.GetDetailedInfoWithoutRequiredInput	standin@DetailedInfo	-	MyFarmPlotLogic.cs:885 new StringBuilder, shared MyValueFormatter; the stand-in holds FormatLock
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.IsAlive	pass	-	MyFarmPlotLogic.cs:335 m_plantHealthSync read
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.IsHarvestable	pass	-	MyFarmPlotLogic.cs:341 IsPlantPlanted/IsPlantFullyGrown/IsAlive field and Sync reads
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.IsPlantFullyGrown	pass	-	MyFarmPlotLogic.cs:337 m_growthProgressSync read
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.IsPlantPlanted	pass	-	MyFarmPlotLogic.cs:339 m_plantedSeed != null
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.OutputItem	pass	-	MyFarmPlotLogic.cs:357 m_plantedSeed definition field read
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.OutputItemAmount	pass	-	MyFarmPlotLogic.cs:371 Sync, seed definition and component definition reads
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.AlignToPGravity	pass	queue	MyFlightMovementBlock.cs:293 -> MyAutopilotComponent.IsAlignedToGravity Sync read/set
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.CollisionAvoidance	pass	queue	MyFlightMovementBlock.cs:217 get IsUsingCollisionAvoidanceForLogic fields; set Sync
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.CurrentWaypoint	pass	-	MyFlightMovementBlock.cs:312 returns m_currentWaypoint reference
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.FlightMode	pass	queue	MyFlightMovementBlock.cs:400 autopilot m_currentFlightMode Sync read; set ChangeFlightMode Sync set + SetAutoPilotEnabled
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.GetWaypoints	pass	-	MyFlightMovementBlock.cs:1545 iterates own autopilot Waypoints list into caller list; proxy wraps the raw MyAutopilotWaypoint results
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.IsAutoPilotEnabled	pass	-	MyFlightMovementBlock.cs:324 autopilot AutoPilotEnabled Sync read
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.LookAtPosition	pass	pass	MyFlightMovementBlock.cs:336 -> MyAutopilotComponent.cs:933 plain auto-property of own component, read/written by autopilot update on main only
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.MinimalAltitude	pass	queue	MyFlightMovementBlock.cs:255 MinHeightAboveTerrain Sync read/set
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.PrecisionMode	pass	queue	MyFlightMovementBlock.cs:274 IsDockingModeEnabled Sync read/set
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.SpeedLimit	pass	queue	MyFlightMovementBlock.cs:236 Sync read; set Sync + stuck detection
Sandbox.ModAPI.Ingame.IMyFunctionalBlock.Enabled	pass	queue	MyFunctionalBlock.cs:151 m_enabled Sync read/set (MyShipDrill/MyShipToolBase overrides same plus m_isManuallyActivated check)
Sandbox.ModAPI.Ingame.IMyFunctionalBlock.RequestEnable	queue	-	MyFunctionalBlock.cs:776 Enabled = enable -> m_enabled.Value Sync set
Sandbox.ModAPI.Ingame.IMyGasGenerator.AutoRefill	pass	queue	MyGasGenerator.cs:170 Sync read/set
Sandbox.ModAPI.Ingame.IMyGasGenerator.IsProducing	pass	-	MyGasGenerator.cs:194 CanProduce field/sink reads + GetIsProducing :673 definition and source array reads
Sandbox.ModAPI.Ingame.IMyGasGenerator.UseConveyorSystem	pass	queue	MyGasGenerator.cs:146 m_useConveyorSystem Sync read / Sync.Value set
Sandbox.ModAPI.Ingame.IMyGasTank.AutoRefillBottles	pass	queue	MyGasTank.cs:217 Sync read/set
Sandbox.ModAPI.Ingame.IMyGasTank.Capacity	pass	-	MyGasTank.cs:229 definition read
Sandbox.ModAPI.Ingame.IMyGasTank.FilledRatio	pass	-	MyGasTank.cs:233 double field read (gas tasks may write it; atomic on x64)
Sandbox.ModAPI.Ingame.IMyGasTank.RefillBottles	queue	-	MyGasTank.cs:318 IsWorking check then SendRefillRequest :1037 MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyGasTank.Stockpile	pass	queue	MyGasTank.cs:205 get IsStockpiling; set ChangeStockpileMode RaiseEvent + UpdateEmissivity (MyGasTank.cs:1000)
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.CurrentCallChainDepth	pass	-	MyProgrammableBlock.cs:142 -> IlInjector m_callChainDepth static, made [ThreadStatic] by IlInjectorPrepatch so it is the worker's own depth
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.CurrentInstructionCount	pass	-	the counter is per thread with FixScriptCounter. Agent: MyProgrammableBlock.cs:136 -> IlInjector InstructionCounterHandle reads process-wide static m_numInstructions (IlInjector.cs:87); with parallel scripts it must return the worker's own counter
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.LastRunTimeMs	pass	-	MyProgrammableBlock.cs:132 RuntimeInfo auto-property of this PB
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.LifetimeTicks	pass	-	MyProgrammableBlock.cs:162 GameplayFrameCounter - m_firstRunFrame
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.MaxCallChainDepth	pass	-	MyProgrammableBlock.cs:140 -> IlInjector.cs:32 constant 1000
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.MaxInstructionCount	pass	-	the counter is per thread with FixScriptCounter. Agent: MyProgrammableBlock.cs:134 -> static IlInjector m_numMaxInstructions rewritten at every run start by any thread; return the per-run limit from the worker's counter
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.TimeSinceLastRun	pass	-	MyProgrammableBlock.cs:130 frame counter math, new TimeSpan
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.UpdateFrequency	pass	queue	MyProgrammableBlock.cs:144 -> MyIngameScriptComponent.cs:52 setter changes component NeedsUpdate (entity update list registration); proxy should return the pending value on later gets
Sandbox.ModAPI.Ingame.IMyGridProgramWorldInfo.InventoryMultiplier	pass	-	MyProgrammableBlock.cs:105 session settings read
Sandbox.ModAPI.Ingame.IMyGridProgramWorldInfo.LargeShipMaxAngularSpeed	pass	-	MyProgrammableBlock.cs:109 MySector.EnvironmentDefinition read
Sandbox.ModAPI.Ingame.IMyGridProgramWorldInfo.LargeShipMaxSpeed	pass	-	MyProgrammableBlock.cs:107 MySector.EnvironmentDefinition read
Sandbox.ModAPI.Ingame.IMyGridProgramWorldInfo.PressurizationEnabled	pass	-	MyProgrammableBlock.cs:93 session settings reads
Sandbox.ModAPI.Ingame.IMyGridProgramWorldInfo.SmallShipMaxAngularSpeed	pass	-	MyProgrammableBlock.cs:113 MySector.EnvironmentDefinition read
Sandbox.ModAPI.Ingame.IMyGridProgramWorldInfo.SmallShipMaxSpeed	pass	-	MyProgrammableBlock.cs:111 MySector.EnvironmentDefinition read
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.CanAccess	standin@Access	-	MyProgrammableBlock.cs:252/275 reads; the grid overload's BigOwners lacks the parent fallback on a worker thread, AccessStandIn applies it
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.GetBlockGroupWithName	pass	-	MyGridTerminalSystem.cs:478 StringBuilder.CompareTo extension, read only
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.GetBlockGroups	pass	-	MyGridTerminalSystem.cs:417 iterates group-private BlockGroups list; proxy wraps results and collect args
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.GetBlockWithId	pass	-	MyGridTerminalSystem.cs:491 m_blockTable.TryGetValue read
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.GetBlockWithName	pass	-	MyGridTerminalSystem.cs:466 CustomName.CompareTo read
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.GetBlocks	pass	-	MyGridTerminalSystem.cs:405 iterates m_blocks, reads IsAccessibleForProgrammableBlock field
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.GetBlocksOfType	pass	-	MyGridTerminalSystem.cs:430/442 iterates m_blocks; collect delegate gets raw blocks, proxy must wrap it and the results
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.SearchBlocksOfName	pass	-	MyGridTerminalSystem.cs:454 CustomName.ToString() + Contains; allocations only
Sandbox.ModAPI.Ingame.IMyGyro.GyroOverride	pass	queue	MyGyro.cs:127 Sync read/set
Sandbox.ModAPI.Ingame.IMyGyro.GyroPower	pass	queue	MyGyro.cs:111 Sync read/set
Sandbox.ModAPI.Ingame.IMyGyro.Pitch	pass	queue	MyGyro.cs:158 get Sync read; set SetGyroTorquePitch (Sync), only if GyroOverride at apply time; keep call order with GyroOverride
Sandbox.ModAPI.Ingame.IMyGyro.Roll	pass	queue	MyGyro.cs:175 same as Pitch
Sandbox.ModAPI.Ingame.IMyGyro.Yaw	pass	queue	MyGyro.cs:141 same as Pitch
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.DisableBroadcastListener	queue	-	MyIntergridCommunicationContext.cs:276 unregisters from global MyIGCSystemSessionComponent; proxy should do the ArgumentException check at call time
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.GetBroadcastListeners	pass	-	MyIntergridCommunicationContext.cs:288 iterates PB-private m_broadcastListeners into caller list; proxy wraps results and collect args
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.IsEndpointReachable	pass@Igc	-	MyIntergridCommunicationContext.cs:207 IsConnectedTo: locked LRU per context, antenna walk under AntennaLock (IgcStandIn)
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.Me	pass	-	MyIntergridCommunicationContext.cs ProgrammableBlock.EntityId
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.RegisterBroadcastListener	pass@Igc	-	MyIntergridCommunicationContext.cs:248 own set + session component's listeners, locked in the phase (IgcStandIn)
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.SendBroadcastMessage	pass@Igc	-	MyIntergridCommunicationContext.cs:226 generic, so it cannot be queued; boxes the message and EnqueueMessage, locked in the phase (IgcStandIn)
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.SendUnicastMessage	pass@Igc	-	MyIntergridCommunicationContext.cs:232 IsConnectedTo (see IsEndpointReachable) and EnqueueMessage, locked in the phase (IgcStandIn)
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.UnicastListener	pass	-	MyIntergridCommunicationContext.cs auto-property read
Sandbox.ModAPI.Ingame.IMyJumpDrive.CurrentStoredPower	pass	-	MyJumpDrive.cs:350 Sync read
Sandbox.ModAPI.Ingame.IMyJumpDrive.JumpDistanceMeters	pass	queue	MyJumpDrive.cs:369 get ComputeMaxDistance: own JumpSystem drive loop, grid mass ([ThreadStatic] m_tmpGroupNodes); set m_jumpDistanceRatio Sync
Sandbox.ModAPI.Ingame.IMyJumpDrive.JumpDistanceRatio	pass	queue	MyJumpDrive.cs:354 m_jumpDistanceRatio Sync read; set clamps and sets Sync
Sandbox.ModAPI.Ingame.IMyJumpDrive.MaxJumpDistanceMeters	pass	-	MyJumpDrive.cs:348 JumpSystem Get{Min,Max}JumpDistance: loops own drives, GetCurrentMass with [ThreadStatic] scratch; read only
Sandbox.ModAPI.Ingame.IMyJumpDrive.MaxStoredPower	pass	-	MyJumpDrive.cs:352 definition read
Sandbox.ModAPI.Ingame.IMyJumpDrive.MinJumpDistanceMeters	pass	-	MyJumpDrive.cs:394 JumpSystem.GetMinJumpDistance: loops own drives, definition + relation reads
Sandbox.ModAPI.Ingame.IMyJumpDrive.Recharge	pass	queue	MyJumpDrive.cs:424 m_isRecharging Sync read/set
Sandbox.ModAPI.Ingame.IMyJumpDrive.Status	pass	-	MyJumpDrive.cs:408 IsJumping/IsWorking/m_storedPower reads
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.AIEnabled	pass	-	MyLargeTurretBase.cs:623 definition read
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.Azimuth	pass	queue	MyLargeTurretBase.cs:1211 get m_rotation; set SetManualAzimuth
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.CanControl	pass@Sensing	-	MyTurretController.cs:219 antenna CheckConnection under AntennaLock in the phase; a dedicated server returns before it
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.Elevation	pass	queue	MyLargeTurretBase.cs:1199 get m_elevation; set SetManualElevation
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.EnableIdleRotation	pass	queue	MyLargeTurretBase.cs:663 Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.GetTargetedEntity	pass@Sensing	-	MyLargeTurretBase.cs:3708 Target set by main; MyDetectedEntityInfoHelper.Create reads, lazy WorldAABB is an idempotent write (SensingStandIn)
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.GetTargetingGroup	pass	-	MyLargeTurretBase.cs:3771 m_targetingGroup Sync read, MyStringHash.String under shared lock
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.GetTargetingGroups	pass	-	MyLargeTurretBase.cs:3747 definition list copied and sorted in a new list; both overloads read only
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.HasTarget	pass	-	MyLargeTurretBase.cs:1191 m_targetingSystem.Target reference read (targeting tasks may swap it; atomic)
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.IsAimed	pass	-	MyLargeTurretBase.cs:1189 m_isAimed field read
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.IsUnderControl	pass	-	MyLargeTurretBase.cs:1237 ControllerInfo.Controller read
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.Range	pass	queue	MyLargeTurretBase.cs:1241 ShootRangeSimple Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.ResetTargetingToDefault	queue	-	MyLargeTurretBase.cs:3703 RaiseEvent ResetTargetParams
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SetManualAzimuthAndElevation	queue	-	MyLargeTurretBase.cs:3173 writes Rotation/Elevation, ResetTarget on targeting system, RotateModels
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SetTarget	queue	-	MyLargeTurretBase.cs:3681 MyMultiplayer.RaiseEvent SetTargetPosition
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SetTargetingGroup	queue	-	MyLargeTurretBase.cs:3781 m_targetingGroup Sync set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SyncAzimuth	queue	-	MyLargeTurretBase.cs:3717 SyncRotationAndOrientation (network)
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SyncElevation	queue	-	MyLargeTurretBase.cs:3722 SyncRotationAndOrientation: m_rotationAndElevationSync Sync set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SyncEnableIdleRotation	pass	-	MyLargeTurretBase.cs:3713 empty body
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetCharacters	pass	queue	MyLargeTurretBase.cs:895 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetEnemies	pass	queue	MyLargeTurretBase.cs:952 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetLargeGrids	pass	queue	MyLargeTurretBase.cs:876 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetMeteors	pass	queue	MyLargeTurretBase.cs:819 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetMissiles	pass	queue	MyLargeTurretBase.cs:838 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetNeutrals	pass	queue	MyLargeTurretBase.cs:933 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetSmallGrids	pass	queue	MyLargeTurretBase.cs:857 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetStations	pass	queue	MyLargeTurretBase.cs:914 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TrackTarget	queue	-	MyLargeTurretBase.cs:3676 MyMultiplayer.RaiseEvent SetTargetPosition
Sandbox.ModAPI.Ingame.IMyLaserAntenna.Connect	queue	-	MyLaserAntenna.cs:2042 ConnectToGps (state change/network)
Sandbox.ModAPI.Ingame.IMyLaserAntenna.IsOutsideLimits	pass	-	MyLaserAntenna.cs:365 m_outsideLimits field read
Sandbox.ModAPI.Ingame.IMyLaserAntenna.IsPermanent	pass	queue	MyLaserAntenna.cs:351 m_permanentConnection Sync read; set ChangePerm -> RaiseEvent or DoSetIsPerm
Sandbox.ModAPI.Ingame.IMyLaserAntenna.Range	pass	queue	MyLaserAntenna.cs:390 m_range Sync read/set
Sandbox.ModAPI.Ingame.IMyLaserAntenna.RequireLoS	pass	-	MyLaserAntenna.cs:363 m_needLineOfSight field read
Sandbox.ModAPI.Ingame.IMyLaserAntenna.SetTargetCoords	queue	-	MyLaserAntenna.cs:2034 PasteCoordinates (network)
Sandbox.ModAPI.Ingame.IMyLaserAntenna.Status	pass	-	MyLaserAntenna.cs:367 m_outsideLimits/m_state reads
Sandbox.ModAPI.Ingame.IMyLaserAntenna.TargetCoords	pass	-	MyLaserAntenna.cs:349 m_targetCoords field read (written only in main-thread updates)
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkIntervalSeconds	pass	queue	MyLightingBlock.cs:338 MyLightingLogic m_sync read; set Sync
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkLenght	pass	-	MyLightingBlock.cs:281 MyLightingLogic BlinkLengthSync read
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkLength	pass	queue	MyLightingBlock.cs:355 Sync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkOffset	pass	queue	MyLightingBlock.cs:372 Sync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.Color	pass	queue	MyLightingBlock.cs:389 MyLightingLogic m_color read; set LightColorSync
Sandbox.ModAPI.Ingame.IMyLightingBlock.Falloff	pass	queue	MyLightingBlock.cs:321 LightFalloffSync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.Intensity	pass	queue	MyLightingBlock.cs:304 IntensitySync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.Radius	pass	queue	MyLightingBlock.cs:283 m_radius/m_reflectorRadius read; set LightRadiusSync
Sandbox.ModAPI.Ingame.IMyLightingBlock.ReflectorRadius	pass	-	MyLightingBlock.cs:279 MyLightingLogic m_reflectorRadius read
Sandbox.ModAPI.Ingame.IMyLightingComponent.BlinkIntervalSeconds	pass	queue	MyLightingComponent.cs:234 MyLightingLogic Sync read; set Sync + UpdateEnabled
Sandbox.ModAPI.Ingame.IMyLightingComponent.BlinkLength	pass	queue	MyLightingComponent.cs:210 MyLightingLogic Sync read/set
Sandbox.ModAPI.Ingame.IMyLightingComponent.BlinkOffset	pass	queue	MyLightingComponent.cs:222 MyLightingLogic Sync read/set
Sandbox.ModAPI.Ingame.IMyLightingComponent.Color	pass	queue	MyLightingComponent.cs:174 m_color read; set writes logic, UpdateEmissivity/UpdateLightProperties (render)
Sandbox.ModAPI.Ingame.IMyLightingComponent.Falloff	pass	queue	MyLightingComponent.cs:246 m_falloff read; set UpdateIntensity/UpdateLightProperties (render)
Sandbox.ModAPI.Ingame.IMyLightingComponent.Intensity	pass	queue	MyLightingComponent.cs:258 IntensitySync read; set Sync + UpdateLightProperties
Sandbox.ModAPI.Ingame.IMyLightingComponent.Radius	pass	queue	MyLightingComponent.cs:186 m_radius read; set UpdateLightProperties (render)
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.Attach	queue	-	MyMechanicalConnectionBlockBase.cs:1516 writes m_lastAttachmentChangeRequestFrame and m_connectionState Sync
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.Detach	queue	-	MyMechanicalConnectionBlockBase.cs:1527 same as Attach
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.IsAttached	pass	-	MyMechanicalConnectionBlockBase.cs:246 field read
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.IsLocked	pass	-	MyMechanicalConnectionBlockBase.cs:409 m_isWelding/m_welded field reads
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.PendingAttachment	pass	-	MyMechanicalConnectionBlockBase.cs:421 m_connectionState Sync read
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.SafetyLock	pass	queue	MyMechanicalConnectionBlockBase.cs:390 m_isWelding/m_welded reads; set m_forceWeld Sync
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.SafetyLockSpeed	pass	queue	MyMechanicalConnectionBlockBase.cs:377 m_weldSpeed Sync read; set clamps and sets Sync
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.Top	pass	-	MyMechanicalConnectionBlockBase.cs:240 m_topBlock read
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.TopGrid	pass	-	MyMechanicalConnectionBlockBase.cs:228 m_topBlock.CubeGrid read
Sandbox.ModAPI.Ingame.IMyMessageProvider.AcceptMessage	pass@Igc	-	MyMessageListener.cs:83 dequeues the PB's own listener, filled by main before the simulation; an inactive broadcast listener drops from its own context
Sandbox.ModAPI.Ingame.IMyMessageProvider.DisableMessageCallback	queue	-	MyMessageListener.cs:101 UnregisterFromCallback -> global MyIGCSystemSessionComponent.UnregisterContextWithPendingCallbacks
Sandbox.ModAPI.Ingame.IMyMessageProvider.HasPendingMessage	pass	-	MyMessageListener.cs Queue.Count read
Sandbox.ModAPI.Ingame.IMyMessageProvider.MaxWaitingMessages	pass	-	MyMessageListener.cs:26 constant 25
Sandbox.ModAPI.Ingame.IMyMessageProvider.SetMessageCallback	queue	-	MyMessageListener.cs:92 writes m_callback; BroadcastListener.cs:18 throws if inactive, null throws: check at call time
Sandbox.ModAPI.Ingame.IMyMotorStator.Angle	pass	-	MyMotorStator.cs:336 m_currentAngle read
Sandbox.ModAPI.Ingame.IMyMotorStator.BrakingTorque	pass	queue	MyMotorStator.cs:351 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.Displacement	pass	queue	MyMotorStator.cs:454 get DummyDisplacement Sync read + constant; set clamps by definition, m_dummyDisplacement Sync
Sandbox.ModAPI.Ingame.IMyMotorStator.LowerLimitDeg	pass	queue	MyMotorStator.cs:406 Sync read; set SetSafeAngles (Sync writes, ScaleUp/Down)
Sandbox.ModAPI.Ingame.IMyMotorStator.LowerLimitRad	pass	queue	MyMotorStator.cs:390 Sync read; set SetSafeAngles
Sandbox.ModAPI.Ingame.IMyMotorStator.RotateToAngle	queue	-	MyMotorStator.cs:1771 MyMultiplayer.RaiseEvent RequestRotation
Sandbox.ModAPI.Ingame.IMyMotorStator.RotorLock	pass	queue	MyMotorStator.cs:482 m_rotorLock Sync + CubeGrid.ForceLockMotors read; set Sync
Sandbox.ModAPI.Ingame.IMyMotorStator.TargetVelocityRPM	pass	queue	MyMotorStator.cs:377 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.TargetVelocityRad	pass	queue	MyMotorStator.cs:364 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.Torque	pass	queue	MyMotorStator.cs:338 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.UpperLimitDeg	pass	queue	MyMotorStator.cs:438 Sync read; set SetSafeAngles
Sandbox.ModAPI.Ingame.IMyMotorStator.UpperLimitRad	pass	queue	MyMotorStator.cs:422 Sync read; set SetSafeAngles
Sandbox.ModAPI.Ingame.IMyMotorSuspension.AirShockEnabled	pass	queue	MyMotorSuspension.cs:588 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Brake	pass	queue	MyMotorSuspension.cs:746 BrakingEnabled Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Damping	pass	-	MyMotorSuspension.cs:771 constant 70 (obsolete)
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Friction	pass	queue	MyMotorSuspension.cs:696 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Height	pass	queue	MyMotorSuspension.cs:720 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.InvertPropulsion	pass	queue	MyMotorSuspension.cs:521 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.InvertSteer	pass	queue	MyMotorSuspension.cs:509 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.IsParkingEnabled	pass	queue	MyMotorSuspension.cs:758 -> :612 m_isParkingEnabled Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.MaxSteerAngle	pass	queue	MyMotorSuspension.cs:734 Sync read; set Sync + OnPerFrameUpdatePropertyChanged
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Power	pass	queue	MyMotorSuspension.cs:708 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Propulsion	pass	queue	MyMotorSuspension.cs:576 m_propulsion Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.PropulsionOverride	pass	queue	MyMotorSuspension.cs:624 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.SteerAngle	pass	-	MyMotorSuspension.cs:732 m_steerAngle field read
Sandbox.ModAPI.Ingame.IMyMotorSuspension.SteerReturnSpeed	pass	-	MyMotorSuspension.cs:777 constant 0.01 (obsolete)
Sandbox.ModAPI.Ingame.IMyMotorSuspension.SteerSpeed	pass	-	MyMotorSuspension.cs:774 constant 0.01 (obsolete)
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Steering	pass	queue	MyMotorSuspension.cs:564 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.SteeringOverride	pass	queue	MyMotorSuspension.cs:636 Sync read; set writes m_steeringChanged + Sync
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Strength	pass	queue	MyMotorSuspension.cs:684 Sync read (sqrt); set Sync
Sandbox.ModAPI.Ingame.IMyMotorSuspension.SuspensionTravel	pass	-	MyMotorSuspension.cs:780 constant 100 (obsolete)
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.GetAttackPatternIds	pass	-	MyOffensiveCombatBlock.cs:781 iterates own Components, adds AttackPatternId to caller collection (generic)
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.LastScan	pass	-	MyOffensiveCombatBlock.cs:180 m_lastScan field and game time math
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.SearchEnemyComponent	pass	-	MyOffensiveCombatBlock.cs:194 field read
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.SelectedAttackPattern	pass	queue	MyOffensiveCombatBlock.cs:198 Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.TargetPriority	pass	queue	MyOffensiveCombatBlock.cs:222 Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.TryGetSelectedAttackPattern	pass	-	MyOffensiveCombatBlock.cs:773/793 iterates own Components, IsSelected field read; proxy must wrap the out component
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.UpdateTargetInterval	pass	queue	MyOffensiveCombatBlock.cs:210 Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatCircleOrbit.CircleDistance	pass	queue	MyOffensiveCombatCircleOrbit.cs:86 m_circleDistance Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatCircleOrbit.CircleInPGravity	pass	queue	MyOffensiveCombatCircleOrbit.cs:98 m_circleInPGravity Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatCircleOrbit.Facing	pass	queue	MyOffensiveWithWeaponsAndFacingCombatComponent.cs:46 m_facing Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatCircleOrbit.IsOrbiting	pass	-	MyOffensiveCombatCircleOrbit.cs:110 m_isOrbiting field read
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.BreakOffDistance	pass	queue	MyOffensiveCombatHitAndRun.cs:163 m_breakOffDistance Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.GetSelectedWeapons	pass	-	MyOffensiveWithWeaponsCombatComponent.cs:856 AddRange of m_selectedWeapons Sync list into caller list
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.IsMovingAwayFromTarget	pass	-	MyOffensiveCombatHitAndRun.cs:113 m_state.CurrentState field read
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.IsMovingToTarget	pass	-	MyOffensiveCombatHitAndRun.cs:125 m_state.CurrentState field read
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.IsWeaponSelected	pass	-	MyOffensiveWithWeaponsCombatComponent.cs:870 m_selectedWeapons Sync list Contains
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.RetreatAngle	pass	queue	MyOffensiveCombatHitAndRun.cs:151 m_retreatAngle Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.RetreatDistance	pass	queue	MyOffensiveCombatHitAndRun.cs:139 m_retreatDistance Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.RetreatTimeout	pass	queue	MyOffensiveCombatHitAndRun.cs:127 m_retreatTimeout Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatHitAndRun.SetSelectedWeapons	queue	-	MyOffensiveWithWeaponsCombatComponent.cs:861 m_selectedWeapons Sync set to the caller's list; snapshot the list at call time
Sandbox.ModAPI.Ingame.IMyOffensiveCombatIntercept.GuidanceType	pass	queue	MyOffensiveCombatIntercept.cs:114 m_guidanceType Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatIntercept.ShouldOverrideCollisionAvoidance	pass	queue	MyOffensiveCombatIntercept.cs:102 m_overrideCollisionAvoidance Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatStayAtRange.EvasiveManeuverDistance	pass	-	MyOffensiveCombatStayAtRange.cs:103 field read
Sandbox.ModAPI.Ingame.IMyOffensiveCombatStayAtRange.EvasiveManeuvers	pass	queue	MyOffensiveCombatStayAtRange.cs:105 m_evasiveManeuvers Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatStayAtRange.FacingPriority	pass	queue	MyOffensiveCombatStayAtRange.cs:117 plain auto-property read/write (not synced)
Sandbox.ModAPI.Ingame.IMyOffensiveCombatStayAtRange.MaximalDistance	pass	queue	MyOffensiveCombatStayAtRange.cs:119 m_maximalDistance Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatStayAtRange.MinimalDistance	pass	queue	MyOffensiveCombatStayAtRange.cs:131 m_minimalDistance Sync read/set
Sandbox.ModAPI.Ingame.IMyOreDetector.BroadcastUsingAntennas	pass	queue	MyOreDetector.cs:113 component auto-property read; set writes component + RaisePropertiesChanged
Sandbox.ModAPI.Ingame.IMyOreDetector.Range	pass	-	MyOreDetector.cs:88 DetectionRadius / definition math
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.BeaconDisplayText	main	-	MyPathRecorderComponent.cs:326 -> Beacon getter :410 sets m_beaconEntityId.Value = 0 (Sync set, network) when the beacon is closed or missing
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.BeaconEntityId	pass	-	MyPathRecorderComponent.cs:431 m_beaconEntityId Sync read
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.BeaconWorldPosition	main	-	MyPathRecorderComponent.cs:314 -> Beacon getter :410 may set m_beaconEntityId Sync to 0 (network) on a closed beacon
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.IsPlaying	pass	-	MyPathRecorderComponent.cs:310 m_isPlaying Sync read
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.IsRecording	pass	-	MyPathRecorderComponent.cs:312 m_isRecording Sync read
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.MinimalDistance	pass	queue	MyPathRecorderComponent.cs:386 m_minimalDistance Sync read/set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.Play	queue	-	MyPathRecorderComponent.cs:1670 m_isPlaying Sync set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.Record	queue	-	MyPathRecorderComponent.cs:1680 m_isRecording Sync set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.RecordInterval	pass	queue	MyPathRecorderComponent.cs:374 m_recordInterval Sync read/set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.RemoveBeacon	queue	-	MyPathRecorderComponent.cs:1665 m_beaconEntityId Sync set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.RepeatPath	pass	queue	MyPathRecorderComponent.cs:338 m_repeatPath Sync read/set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.ShowPath	pass	queue	MyPathRecorderComponent.cs:350 m_showPath Sync read/set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.ShowSelectedPoints	pass	queue	MyPathRecorderComponent.cs:362 m_showSelectedPoints Sync read/set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.StopPlay	queue	-	MyPathRecorderComponent.cs:1675 m_isPlaying Sync set
Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.StopRecord	queue	-	MyPathRecorderComponent.cs:1685 m_isRecording Sync set
Sandbox.ModAPI.Ingame.IMyPistonBase.CurrentPosition	pass	-	MyPistonBase.cs:229 field read
Sandbox.ModAPI.Ingame.IMyPistonBase.Extend	queue	-	MyPistonBase.cs:1615 Velocity Sync set (sign flip); a later Velocity get in the run sees the old value
Sandbox.ModAPI.Ingame.IMyPistonBase.HighestPosition	pass	-	MyPistonBase.cs:323 definition read
Sandbox.ModAPI.Ingame.IMyPistonBase.LowestPosition	pass	-	MyPistonBase.cs:321 BlockDefinition.Minimum read
Sandbox.ModAPI.Ingame.IMyPistonBase.MaxLimit	pass	queue	MyPistonBase.cs:305 Sync<float> read; setter clamps and sets Sync (network)
Sandbox.ModAPI.Ingame.IMyPistonBase.MaxVelocity	pass	-	MyPistonBase.cs:287 definition read
Sandbox.ModAPI.Ingame.IMyPistonBase.MinLimit	pass	queue	MyPistonBase.cs:289 Sync<float> read; setter sets Sync (network)
Sandbox.ModAPI.Ingame.IMyPistonBase.MoveToPosition	queue	-	MyPistonBase.cs:1639 MyMultiplayer.RaiseEvent RequestMoveToPosition
Sandbox.ModAPI.Ingame.IMyPistonBase.NormalizedPosition	pass	-	MyPistonBase.cs:259 m_currentPos and limit Sync reads, math
Sandbox.ModAPI.Ingame.IMyPistonBase.Retract	queue	-	MyPistonBase.cs:1623 Velocity Sync set (sign flip); a later Velocity get in the run sees the old value
Sandbox.ModAPI.Ingame.IMyPistonBase.Reverse	queue	-	MyPistonBase.cs:1631 Velocity Sync set (sign flip); a later Velocity get in the run sees the old value
Sandbox.ModAPI.Ingame.IMyPistonBase.Status	pass	-	MyPistonBase.cs:231 compares Velocity/limits with m_currentPos, read only
Sandbox.ModAPI.Ingame.IMyPistonBase.Velocity	pass	queue	MyPistonBase.cs:271 Sync<float> read; setter clamps and sets Sync (network)
Sandbox.ModAPI.Ingame.IMyPowerProducer.CurrentOutput	pass	-	MyFueledPowerProducer.cs:94, MyBatteryBlock.cs:187, MyEnvironmentalPowerProducer.cs:59 -> MyResourceSourceComponent.CurrentOutputByType array read
Sandbox.ModAPI.Ingame.IMyPowerProducer.CurrentOutputRatio	pass	-	MyFueledPowerProducer.cs:98, MyBatteryBlock.cs:303, MyEnvironmentalPowerProducer.cs:65 CurrentOutput (source array read) / definition or MaxOutput
Sandbox.ModAPI.Ingame.IMyPowerProducer.MaxOutput	pass	-	MyFueledPowerProducer.cs:96 definition, MyReactor.cs:85 *multiplier, battery/env -> MyResourceSourceComponent.MaxOutputByType reads
Sandbox.ModAPI.Ingame.IMyProductionBlock.AddQueueItem	queue	-	MyProductionBlock.cs:1101 GetBlueprintDefinition + AddQueueItemRequest -> RaiseEvent (MyAssembler.cs:1434 override also UpdateCurrentState); void. Null blueprint NRE becomes deferred
Sandbox.ModAPI.Ingame.IMyProductionBlock.CanUseBlueprint	pass	-	MyProductionBlock.cs:1091/449 definition dictionary + BlueprintClass HashSet lookups
Sandbox.ModAPI.Ingame.IMyProductionBlock.ClearQueue	queue	-	MyProductionBlock.cs:528 ClearQueueRequest raises OnRemoveQueueItem events; void
Sandbox.ModAPI.Ingame.IMyProductionBlock.GetQueue	pass	-	MyProductionBlock.cs:1157 reads m_queue, fills caller list with new MyProductionItem
Sandbox.ModAPI.Ingame.IMyProductionBlock.InputInventory	pass	-	MyProductionBlock.cs:175/283 returns m_inputInventory
Sandbox.ModAPI.Ingame.IMyProductionBlock.InsertQueueItem	queue	-	MyProductionBlock.cs:1119 -> InsertQueueItemRequest -> AddQueueItemRequest RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyProductionBlock.IsProducing	pass	-	MyProductionBlock.cs:219 Sync<bool> read
Sandbox.ModAPI.Ingame.IMyProductionBlock.IsQueueEmpty	pass	-	MyProductionBlock.cs:213 m_queue.Count
Sandbox.ModAPI.Ingame.IMyProductionBlock.MoveQueueItemRequest	queue	-	MyProductionBlock.cs:511 RaiseEvent OnMoveQueueItemCallback; void
Sandbox.ModAPI.Ingame.IMyProductionBlock.NextItemId	pass@Inventory	-	MyProductionBlock.cs:243 m_nextItemId++ on its own block, in the script's group
Sandbox.ModAPI.Ingame.IMyProductionBlock.OutputInventory	pass	-	MyProductionBlock.cs:194/285 returns m_outputInventory
Sandbox.ModAPI.Ingame.IMyProductionBlock.RemoveQueueItem	queue	-	MyProductionBlock.cs:1137 -> RemoveQueueItemRequest :546 RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyProductionBlock.UseConveyorSystem	pass	queue	MyProductionBlock.cs:245 Sync<bool> read; setter sets Sync
Sandbox.ModAPI.Ingame.IMyProgrammableBlock.IsRunning	pass	-	MyProgrammableBlock.cs:484 m_isRunning field
Sandbox.ModAPI.Ingame.IMyProgrammableBlock.TerminalRunArgument	pass	-	MyProgrammableBlock.cs:472 returns m_terminalRunArgument (setter private)
Sandbox.ModAPI.Ingame.IMyProgrammableBlock.TryRun	standin@TryRun	-	MyProgrammableBlock.cs:1431 runs the other PB on the same thread if it may run on a worker thread in the same group, else false (TryRunStandIn)
Sandbox.ModAPI.Ingame.IMyProjector.BuildableBlocksCount	pass	-	MyProjectorBase.cs:844 int field m_buildableBlocksCount (set by projector work OnComplete on main)
Sandbox.ModAPI.Ingame.IMyProjector.IsProjecting	pass	-	MyProjectorBase.cs:836/896 m_clipboard.IsActive auto-property (MyGridClipboard.cs:179)
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionOffset	pass	queue	MyProjectorBase.cs:794 plain field m_projectionOffset; setter queued to keep call order with the queued UpdateOffsetAndRotation
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionOffsetX	pass	-	MyProjectorBase.cs:819 m_projectionOffset.X
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionOffsetY	pass	-	MyProjectorBase.cs:822 m_projectionOffset.Y
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionOffsetZ	pass	-	MyProjectorBase.cs:825 m_projectionOffset.Z
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionRotX	pass	-	MyProjectorBase.cs:828 m_projectionRotation.X * 90
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionRotY	pass	-	MyProjectorBase.cs:831 m_projectionRotation.Y * 90
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionRotZ	pass	-	MyProjectorBase.cs:834 m_projectionRotation.Z * 90
Sandbox.ModAPI.Ingame.IMyProjector.ProjectionRotation	pass	queue	MyProjectorBase.cs:806 plain field m_projectionRotation; setter queued to keep call order with the queued UpdateOffsetAndRotation
Sandbox.ModAPI.Ingame.IMyProjector.RemainingArmorBlocks	pass	-	MyProjectorBase.cs:842 int field (UpdateStats on main)
Sandbox.ModAPI.Ingame.IMyProjector.RemainingBlocks	pass	-	MyProjectorBase.cs:838 int field m_remainingBlocks
Sandbox.ModAPI.Ingame.IMyProjector.RemainingBlocksPerType	pass	-	MyProjectorBase.cs:846 copies m_remainingBlocksPerType into a new Dictionary; written only by UpdateStats :1430 on main
Sandbox.ModAPI.Ingame.IMyProjector.ShowOnlyBuildable	pass	queue	MyProjectorBase.cs:859 plain field m_showOnlyBuildable; setter queued to keep order with UpdateOffsetAndRotation (sent by SendNewOffset)
Sandbox.ModAPI.Ingame.IMyProjector.TotalBlocks	pass	-	MyProjectorBase.cs:840 int field m_totalBlocks
Sandbox.ModAPI.Ingame.IMyProjector.UpdateOffsetAndRotation	queue	-	MyProjectorBase.cs:2473 -> OnOffsetsChanged :908 SendNewOffset (RaiseEvent) + Remap; void
Sandbox.ModAPI.Ingame.IMyRadioAntenna.EnableBroadcasting	pass	queue	MyRadioAntenna.cs:224 Sync<bool> read; setter sets Sync (network, ValueChanged handlers)
Sandbox.ModAPI.Ingame.IMyRadioAntenna.HudText	pass	queue	MyRadioAntenna.cs:210 HudText.ToString(); setter SetHudText :243 CompareUpdate + RaiseEvent
Sandbox.ModAPI.Ingame.IMyRadioAntenna.IsBroadcasting	pass	-	MyRadioAntenna.cs:791 RadioBroadcaster.WantsToBeEnabled read (Components.Get dictionary lookup)
Sandbox.ModAPI.Ingame.IMyRadioAntenna.Radius	pass	queue	MyRadioAntenna.cs:198/284 RadioBroadcaster.BroadcastRadius field; setter sets m_radius Sync
Sandbox.ModAPI.Ingame.IMyRadioAntenna.ShowShipName	pass	queue	MyRadioAntenna.cs:179 m_showShipName Sync read/set
Sandbox.ModAPI.Ingame.IMyReactor.UseConveyorSystem	pass	queue	MyReactor.cs:101/87 Sync<bool>
Sandbox.ModAPI.Ingame.IMyRemoteControl.AddWaypoint	queue	-	MyRemoteControl.cs:1042/1047 -> MyAutopilotComponent.AddWaypoint :3613 RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.ClearWaypoints	queue	-	MyRemoteControl.cs:915 -> MyAutopilotComponent.ClearWaypoints :3589 RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.CurrentWaypoint	pass	-	MyRemoteControl.cs:332 new MyWaypointInfo from MyAutopilotComponent.m_currentWaypoint Name/Coords
Sandbox.ModAPI.Ingame.IMyRemoteControl.Direction	pass	queue	MyRemoteControl.cs:320 -> MyAutopilotComponent.CurrentDirection :786 Sync read/set
Sandbox.ModAPI.Ingame.IMyRemoteControl.FlightMode	pass	queue	MyRemoteControl.cs:308 -> MyAutopilotComponent.CurrentFlightMode :771 Sync read/set
Sandbox.ModAPI.Ingame.IMyRemoteControl.GetNearestPlayer	pass@Sensing	-	MyRemoteControl.cs:899 online players snapshot (ConcurrentDictionary) and their matrices
Sandbox.ModAPI.Ingame.IMyRemoteControl.GetWaypointInfo	pass	-	MyRemoteControl.cs:1029 copies m_autopilotComponent.Waypoints into caller list (new MyWaypointInfo)
Sandbox.ModAPI.Ingame.IMyRemoteControl.IsAutoPilotEnabled	pass	-	MyRemoteControl.cs:270 AutoPilotEnabled Sync read
Sandbox.ModAPI.Ingame.IMyRemoteControl.SetAutoPilotEnabled	queue	-	MyRemoteControl.cs:884 -> MyAutopilotComponent.SetAutoPilotEnabled :3003 ClearMovementControl + Sync set; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.SetCollisionAvoidance	queue	-	MyRemoteControl.cs:889 -> MyAutopilotComponent.IsUsingCollisionAvoidance :695 Sync set; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.SetDockingMode	queue	-	MyRemoteControl.cs:894 -> MyAutopilotComponent.IsDockingModeEnabled :737 Sync set; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.SpeedLimit	pass	queue	MyRemoteControl.cs:296 -> MyAutopilotComponent.SpeedLimit :752 Sync read; setter Sync + stuck detection reset
Sandbox.ModAPI.Ingame.IMyRemoteControl.WaitForFreeWay	pass	queue	MyRemoteControl.cs:344 -> MyAutopilotComponent.WaitForFreeWay :801 Sync read/set
Sandbox.ModAPI.Ingame.IMyResourceStorageComponent.FilledRatio	pass	-	MyResourceStorageComponent.cs:108 m_filledRatio double field
Sandbox.ModAPI.Ingame.IMyResourceStorageComponent.IsResourceStorage	pass	-	MyResourceStorageComponent.cs:315 loops m_sourceComponent.ResourceTypes (new ListReader), compare
Sandbox.ModAPI.Ingame.IMyResourceStorageComponent.ResourceCapacity	pass	-	MyResourceStorageComponent.cs:131 definition Capacity
Sandbox.ModAPI.Ingame.IMySearchEnemyComponent.FoundEnemyId	pass	-	MySearchEnemyComponent.cs:173/107 server path returns m_foundEnemy (read only); client path caches m_foundEnemy but PBs execute only on the server
Sandbox.ModAPI.Ingame.IMySearchEnemyComponent.SubsystemsToDestroy	pass	queue	MySearchEnemyComponent.cs:187 Sync read; setter sets Sync; set MySearchEnemyComponent.cs:195 Sync<T>.Value
Sandbox.ModAPI.Ingame.IMySearchEnemyComponent.TargetingLockOptions	pass	queue	MySearchEnemyComponent.cs:175 Sync read; setter sets Sync
Sandbox.ModAPI.Ingame.IMySearchlight.AimingRadius	pass	queue	MySearchlight.cs:514 ShootRangeSimple :1031 m_shootingRange Sync read; setter clamps + Sync set
Sandbox.ModAPI.Ingame.IMySearchlight.BlinkInterval	pass	queue	MySearchlight.cs:687 -> MyLightingLogic.BlinkIntervalSeconds :139 Sync read; setter Sync set
Sandbox.ModAPI.Ingame.IMySearchlight.BlinkLength	pass	queue	MySearchlight.cs:699 -> MyLightingLogic.BlinkLength :271 Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.BlinkOffset	pass	queue	MySearchlight.cs:711 -> MyLightingLogic.BlinkOffset :291 Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.Color	pass	queue	MySearchlight.cs:639 MyLightingLogic m_color read; setter MyLightingLogic.cs:78 UpdateEmissivity, UpdateLightProperties (render), event
Sandbox.ModAPI.Ingame.IMySearchlight.EnableIdleMovement	pass	queue	MySearchlight.cs:533 -> EnableIdleRotation :1223 m_enableIdleRotation Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.Intensity	pass	queue	MySearchlight.cs:663 IntensitySync read; setter clamps + Sync set
Sandbox.ModAPI.Ingame.IMySearchlight.Offset	pass	queue	MySearchlight.cs:675 LightOffsetSync read; setter clamps + Sync set
Sandbox.ModAPI.Ingame.IMySearchlight.Radius	pass	queue	MySearchlight.cs:651 LightRadiusSync read; setter clamps + Sync set
Sandbox.ModAPI.Ingame.IMySearchlight.SetManualAzimuthAndElevation	queue	-	MySearchlight.cs:1282 writes Rotation/Elevation, targeting ResetTarget, RotateModels (matrices/render); void
Sandbox.ModAPI.Ingame.IMySearchlight.SetTargetOptions	queue	-	MySearchlight.cs:1277 -> MyLargeTurretTargetingSystem.SetCurrentTargetingModeIndex :2030 SetTargetingMode (Sync); void
Sandbox.ModAPI.Ingame.IMySearchlight.TargetCharacters	pass	queue	MySearchlight.cs:911 TargetFlags m_targetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetEnemy	pass	queue	MySearchlight.cs:600 -> TargetEnemies :968 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetFriends	pass	queue	MySearchlight.cs:581 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetLargeShips	pass	queue	MySearchlight.cs:569 -> TargetLargeGrids :892 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetLockEnemy	pass	queue	MySearchlight.cs:612 -> TargetLocking :1239 m_targetLocking Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetMeteors	pass	queue	MySearchlight.cs:835 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetNeutrals	pass	queue	MySearchlight.cs:949 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetOptions	pass	-	MySearchlight.cs:624 m_targetingGroup Sync read, Enum.Parse; GetTargetingGroupDefinitions makes a new list from definitions (discarded)
Sandbox.ModAPI.Ingame.IMySearchlight.TargetRockets	pass	queue	MySearchlight.cs:545 -> TargetMissiles :854 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetSmallShips	pass	queue	MySearchlight.cs:557 -> TargetSmallGrids :873 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySearchlight.TargetStations	pass	queue	MySearchlight.cs:930 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMySensorBlock.BackExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.BottomExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectAsteroids	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectEnemy	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectFloatingObjects	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectFriendly	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectLargeShips	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectNeutral	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectOwner	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectPlayers	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectSmallShips	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectStations	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectSubgrids	pass	queue	MySensorBlock.cs:252-460 bit test on Filters Sync; setter is read-modify-write of Filters (:228), correct when applied in call order
Sandbox.ModAPI.Ingame.IMySensorBlock.DetectedEntities	pass	-	MySensorBlock.cs:1700 AddRange from MyConcurrentHashSet m_detectedEntities (own spin lock, enumerator released) into caller list
Sandbox.ModAPI.Ingame.IMySensorBlock.FrontExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.IsActive	pass	-	MySensorBlock.cs:791/178 Sync<bool> m_active read
Sandbox.ModAPI.Ingame.IMySensorBlock.LastDetectedEntity	pass@Sensing	-	MySensorBlock.cs:793 entity set by the sensor's main thread update; Create as for turrets
Sandbox.ModAPI.Ingame.IMySensorBlock.LeftExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.MaxRange	pass	-	MySensorBlock.cs:226 definition read
Sandbox.ModAPI.Ingame.IMySensorBlock.PlayProximitySound	pass	queue	MySensorBlock.cs:635/240 m_playProximitySound Sync read/set
Sandbox.ModAPI.Ingame.IMySensorBlock.RightExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.TopExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMyShipConnector.CollectAll	pass	queue	MyShipConnector.cs:545 Sync<bool>
Sandbox.ModAPI.Ingame.IMyShipConnector.Connect	queue	-	MyShipConnector.cs:2513 TryConnect (constraints, events); void
Sandbox.ModAPI.Ingame.IMyShipConnector.Disconnect	queue	-	MyShipConnector.cs:2521 TryDisconnect; void
Sandbox.ModAPI.Ingame.IMyShipConnector.IsConnected	pass	-	MyShipConnector.cs:585 m_connected field
Sandbox.ModAPI.Ingame.IMyShipConnector.IsLocked	pass	-	MyShipConnector.cs:573 IsWorking && m_constraint != null
Sandbox.ModAPI.Ingame.IMyShipConnector.IsParkingEnabled	pass	queue	MyShipConnector.cs:587/436 m_isParkingEnabled Sync read/set
Sandbox.ModAPI.Ingame.IMyShipConnector.OtherConnector	pass	-	MyShipConnector.cs:615 returns m_other (proxy must wrap it)
Sandbox.ModAPI.Ingame.IMyShipConnector.PullStrength	pass	queue	MyShipConnector.cs:557 Strength Sync read; setter clamps + Sync set
Sandbox.ModAPI.Ingame.IMyShipConnector.Status	pass	-	MyShipConnector.cs:599 reads m_connected, IsWorking, m_constraint
Sandbox.ModAPI.Ingame.IMyShipConnector.ThrowOut	pass	queue	MyShipConnector.cs:533 Sync<bool>
Sandbox.ModAPI.Ingame.IMyShipConnector.ToggleConnect	queue	-	MyShipConnector.cs:2529 TrySwitch; void
Sandbox.ModAPI.Ingame.IMyShipController.CalculateShipMass	pass	-	MyShipController.cs:3245 -> MyCubeGrid.GetCurrentMass :5557: group nodes into [ThreadStatic] m_tmpGroupNodes, MyGridShape mass fields, pilot/inventory mass reads
Sandbox.ModAPI.Ingame.IMyShipController.CanControlShip	pass	-	MyShipController.cs:864 m_enableShipControl
Sandbox.ModAPI.Ingame.IMyShipController.CenterOfMass	pass	-	MyShipController.cs:787 Physics.CenterOfMassWorld (Havok read + Offset)
Sandbox.ModAPI.Ingame.IMyShipController.ControlThrusters	pass	queue	MyShipController.cs:738 Sync read; setter checks IsMainCockpitFree then Sync set
Sandbox.ModAPI.Ingame.IMyShipController.ControlWheels	pass	queue	MyShipController.cs:723 Sync read; setter checks then Sync set
Sandbox.ModAPI.Ingame.IMyShipController.DampenersOverride	pass	queue	MyShipController.cs:768 grid MyEntityThrustComponent.DampenersEnabled auto-prop; setter CubeGrid.EnableDampingInternal
Sandbox.ModAPI.Ingame.IMyShipController.GetArtificialGravity	pass@Gravity	-	MyShipController.cs:3221 generator matrices and boxes, brought up to date on main before the phase (GravityStandIn)
Sandbox.ModAPI.Ingame.IMyShipController.GetNaturalGravity	pass	-	planet gravity is thread-safe; the controller's lazy matrix is own-group. Agent: MyShipController.cs:3216 MyGravityProviderSystem.CalculateNaturalGravityInPoint is thread-safe (shared lock, pure planet math) but base.WorldMatrix of the controller block: block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags). Replace: translation = Vector3D.Transform(PositionComp.LocalMatrixRef.Translation, CubeGrid.PositionComp.WorldMatrixRef), then call it
Sandbox.ModAPI.Ingame.IMyShipController.GetShipSpeed	pass	-	MyShipController.cs:3231 Parent.Physics.LinearVelocity (Havok read)
Sandbox.ModAPI.Ingame.IMyShipController.GetShipVelocities	pass	-	MyShipController.cs:3237 Parent.Physics linear/angular velocity (Havok read)
Sandbox.ModAPI.Ingame.IMyShipController.GetTotalGravity	pass@Gravity	-	MyShipController.cs:3226 natural + artificial, see GetArtificialGravity
Sandbox.ModAPI.Ingame.IMyShipController.HandBrake	pass	queue	MyShipController.cs:753 GridSystems.WheelSystem.HandBrake field; setter re-checks state and calls SwitchHandbrake (toggle), fine applied in order
Sandbox.ModAPI.Ingame.IMyShipController.HasWheels	pass	-	MyShipController.cs:866 WheelSystem.m_wheels.Count
Sandbox.ModAPI.Ingame.IMyShipController.IsMainCockpit	pass	queue	MyShipController.cs:706 Sync read; set MyShipController.cs:714 IsMainCockpitFree/CanBeMainCockpit check, then Sync<T>.Value
Sandbox.ModAPI.Ingame.IMyShipController.IsUnderControl	pass	-	MyShipController.cs:721 ControllerInfo.Controller != null
Sandbox.ModAPI.Ingame.IMyShipController.MoveIndicator	pass	-	MyShipController.cs:883 auto-property read
Sandbox.ModAPI.Ingame.IMyShipController.RollIndicator	pass	-	MyShipController.cs:887 auto-property read
Sandbox.ModAPI.Ingame.IMyShipController.RotationIndicator	pass	-	MyShipController.cs:885 auto-property read
Sandbox.ModAPI.Ingame.IMyShipController.ShowHorizonIndicator	pass	queue	MyShipController.cs:868/528 m_horizonIndicatorEnabled Sync + CanHaveHorizon definition read; setter Sync set
Sandbox.ModAPI.Ingame.IMyShipController.TryGetPlanetElevation	pass	-	as GetNaturalGravity. Agent: MyShipController.cs:3253 PositionComp.GetPosition and WorldAABB of the controller block: block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags). GetClosestPlanet ([ThreadStatic] list, locked AABB tree), planet GetPosition (top-level) and ProjectToSurface (cacheless heightmap) are read only. Replace: compute world matrix/AABB as LocalMatrix x grid WorldMatrixRef without caching, rest unchanged
Sandbox.ModAPI.Ingame.IMyShipController.TryGetPlanetPosition	pass	-	as GetNaturalGravity. Agent: MyShipController.cs:3284 same lazy GetPosition/WorldAABB of the controller block as TryGetPlanetElevation; same replacement
Sandbox.ModAPI.Ingame.IMyShipDrill.TerrainClearingMode	pass	queue	MyShipDrill.cs:235 m_terrainClearingMode Sync read/set
Sandbox.ModAPI.Ingame.IMyShipToolBase.IsActivated	pass	-	MyShipToolBase.cs:182 m_isActivated; MyShipDrill.cs:277 m_wantsToDrill
Sandbox.ModAPI.Ingame.IMyShipToolBase.UseConveyorSystem	pass	queue	MyShipToolBase.cs:170/144, MyShipDrill.cs:265/223 Sync<bool>
Sandbox.ModAPI.Ingame.IMyShipWelder.HelpOthers	pass	pass	MyShipWelder.cs:65 getter returns false, setter is empty
Sandbox.ModAPI.Ingame.IMySmallGatlingGun.UseConveyorSystem	pass	-	MySmallGatlingGun.cs:239 m_useConveyorSystem Sync read
Sandbox.ModAPI.Ingame.IMySmallMissileLauncher.UseConveyorSystem	pass	-	MySmallMissileLauncher.cs:256 m_useConveyorSystem Sync read
Sandbox.ModAPI.Ingame.IMySolarFoodGenerator.ItemsPerMinute	pass	-	MySolarFoodGenerator.cs:112 m_itemsPerMinute Sync read
Sandbox.ModAPI.Ingame.IMySolarFoodGenerator.TimeRemainingUntilNextBatch	pass	-	MySolarFoodGenerator.cs:114 m_timeRemainingUntilNextBatch Sync read
Sandbox.ModAPI.Ingame.IMySolarOccludable.GetEntityId	pass	-	MySolarOccludableComponent.cs:26, MySolarPanel.cs:191, MyOxygenFarm.cs:296 EntityId field
Sandbox.ModAPI.Ingame.IMySolarOccludable.IsSolarOccluded	pass	-	MySolarOccludableComponent.cs:17, MySolarPanel.cs:38, MyOxygenFarm.cs:85 -> MyCubeGrid.IsSolarOccluded :3408 Sync read
Sandbox.ModAPI.Ingame.IMyStoreBlock.CancelStoreItem	main	-	MyStoreBlock.cs:1790 runs item OnCancel callback, PlayerItems.Remove; bool result false when id unknown (cannot be assumed true)
Sandbox.ModAPI.Ingame.IMyStoreBlock.GetPlayerStoreItems	pass	-	MyStoreBlock.cs:2118 copies PlayerItems into caller list (new MyStoreQueryItem)
Sandbox.ModAPI.Ingame.IMyStoreBlock.InsertOffer	main	-	MyStoreBlock.cs:2001 out id; CreateNewOffer_Internal creates store item (AllocateId, inventory/listing fee)
Sandbox.ModAPI.Ingame.IMyStoreBlock.InsertOrder	main	-	MyStoreBlock.cs:2026 out id; CreateNewOrder_Internal creates store item (AllocateId, PlayerItems.Add)
Sandbox.ModAPI.Ingame.IMyStoreBlock.Searchable	pass	queue	MyStoreBlock.cs:334 m_searchable Sync read/set
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomData	pass	queue	MyTerminalBlock.cs:290 Storage.TryGetValue read; setter SetCustomData_Internal :543 may create Storage component, RaiseCustomDataChanged network event
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomInfo	pass	-	MyTerminalBlock.cs:433 CustomInfo.ToString()
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomName	pass	queue	lazy default name of the own block; DisplayNameText of the shared definition is made thread-safe by ScriptRunner. Agent: MyTerminalBlock.cs:417/252 getter lazily fills m_defaultCustomName via LoadDefaultCustomName :1364 when the block has no custom name (common for unnamed blocks). Replace: m_customName?.ToString() ?? (m_defaultCustomName.Length>0 ? it.ToString() : BlockDefinition.DisplayNameText + (NumberInGrid>1 ? """" """"+NumberInGrid : """""""")) without caching. Setter SetCustomName :668 events + RaiseEvent
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomNameWithFaction	pass	-	MyTerminalBlock.cs:429 CustomNameWithFaction.ToString(); the builder is refilled by GetHudParams :971 on main
Sandbox.ModAPI.Ingame.IMyTerminalBlock.DetailedInfo	standin@DetailedInfo	-	MyTerminalBlock.cs:393 refresh if dirty with the shared formatter; DetailedInfoStandIn holds FormatLock, defers five unsafe block types to main
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetActionWithName	pass@Terminal	-	MyTerminalBlock.cs:1403 -> helper :67. Lookup with per-thread scratch lists, control lists made under a lock (TerminalStandIn)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetActions	pass@Terminal	-	MyTerminalBlock.cs:1393 -> helper :33. Lookup with per-thread scratch lists, control lists made under a lock (TerminalStandIn)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetProperties	pass@Terminal	-	MyTerminalBlock.cs:1413 -> helper :105. Lookup with per-thread scratch lists, control lists made under a lock (TerminalStandIn)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetProperty	pass@Terminal	-	MyTerminalBlock.cs:1408 -> helper :86. Lookup with per-thread scratch lists, control lists made under a lock (TerminalStandIn)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.HasLocalPlayerAccess	pass	-	MyTerminalBlock.cs:894 LocalPlayerId -> HasPlayerAccessReason :933 admin settings and faction dictionary reads (MyIDModule.cs:33)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.HasNobodyPlayerAccessToBlock	pass	-	MyTerminalBlock.cs:847 IDModule ShareMode/Owner reads
Sandbox.ModAPI.Ingame.IMyTerminalBlock.HasPlayerAccess	pass	-	MyTerminalBlock.cs:917 HasAdminUseTerminals dictionary reads + IDModule.GetUserRelationToOwner faction dictionary reads
Sandbox.ModAPI.Ingame.IMyTerminalBlock.HasPlayerAccessWithNobodyCheck	pass	-	MyTerminalBlock.cs:899 as HasPlayerAccess; isForPB path reads own grid BigOwners (recalc on a worker thread only under IsAsyncUpdateInProgress)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.IsSameConstructAs	pass	-	MyTerminalBlock.cs:1418 -> MyCubeGrid.IsSameConstructAs :16421 MyCubeGridGroups.Mechanical.GetGroup dictionary reads
Sandbox.ModAPI.Ingame.IMyTerminalBlock.SearchActionsOfName	pass@Terminal	-	MyTerminalBlock.cs:1398 -> helper :50. Lookup with per-thread scratch lists, control lists made under a lock (TerminalStandIn)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.SetCustomName	queue	-	MyTerminalBlock.cs:668/684 UpdateCustomName (events) + RaiseEvent; void; snapshot the StringBuilder overload's text
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowInInventory	pass	queue	MyTerminalBlock.cs:338 Sync<bool> read; setter Sync + event; set MyTerminalBlock.cs:344 Sync<T>.Value + RaiseShowInInventoryChanged
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowInTerminal	pass	queue	MyTerminalBlock.cs:322 Sync<bool> read; setter Sync + event; set MyTerminalBlock.cs:328 Sync<T>.Value + RaiseShowInTerminalChanged
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowInToolbarConfig	pass	queue	MyTerminalBlock.cs:354 m_showInToolbarConfig Sync read; setter Sync + RaiseShowInToolbarConfigChanged
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowOnHUD	pass	queue	MyTerminalBlock.cs:306 Sync<bool> read; setter Sync + RaiseShowOnHUDChanged
Sandbox.ModAPI.Ingame.IMyTextPanel.GetPublicTitle	pass	-	MyTextPanel.cs:1269 m_publicTitleHelper.ToString()
Sandbox.ModAPI.Ingame.IMyTextPanel.WritePublicTitle	queue	-	as WriteText. Agent: MyTextPanel.cs:1274 writes m_publicTitleHelper and RaiseEvent (SendChangeTitleMessage), returns !m_isTextPanelOpen. Replace: result = !m_isTextPanelOpen (read), queue the real call
Sandbox.ModAPI.Ingame.IMyTextSurface.AddImageToSelection	queue	-	MyTextPanel.cs:1351 / MyTextPanelComponent.cs:2171 reads lists then AddImagesToSelectionRequest RaiseEvent + OnSelectedTextureChanged; void
Sandbox.ModAPI.Ingame.IMyTextSurface.AddImagesToSelection	queue	-	MyTextPanel.cs:1378 / MyTextPanelComponent.cs:2202 reads lists then AddImagesToSelectionRequest RaiseEvent; void; copy the ids list
Sandbox.ModAPI.Ingame.IMyTextSurface.Alignment	pass	queue	MyTextPanel.cs:452 / MyTextPanelComponent.cs:970/780 m_fontData Sync read; setter Sync set + NeedsUpdate
Sandbox.ModAPI.Ingame.IMyTextSurface.BackgroundAlpha	pass	queue	MyTextPanel.cs:391 / MyTextPanelComponent.cs:709 m_contentData Sync read; setter Sync + m_backgroundChanged + UpdateIsStaticContent
Sandbox.ModAPI.Ingame.IMyTextSurface.BackgroundColor	pass	queue	MyTextPanel.cs:376 / MyTextPanelComponent.cs:690 m_contentData Sync read; setter Sync + flags
Sandbox.ModAPI.Ingame.IMyTextSurface.ChangeInterval	pass	queue	MyTextPanel.cs:406 / MyTextPanelComponent.cs:875 m_contentData Sync read; setter OnUpdateInterval + Sync set
Sandbox.ModAPI.Ingame.IMyTextSurface.ClearImagesFromSelection	queue	-	MyTextPanel.cs:1491 / MyTextPanelComponent.cs:2316 RemoveImagesFromSelectionRequest RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyTextSurface.ContentType	pass	queue	MyTextPanel.cs:486/167 / MyTextPanelComponent.cs:994/615 m_contentData Sync read; setter Sync + UpdateIsStaticContent
Sandbox.ModAPI.Ingame.IMyTextSurface.CurrentlyShownImage	pass	-	MyTextPanel.cs:421 / MyTextPanelComponent.cs:939 reads SelectedTexturesToDraw and m_currentSelectedTexture
Sandbox.ModAPI.Ingame.IMyTextSurface.DisplayName	pass	-	MyTextPanel.cs:564 / MyTextPanelComponent.cs:1060 m_displayName
Sandbox.ModAPI.Ingame.IMyTextSurface.DrawFrame	pass@Sprites	-	MyTextPanelComponent.cs:1951 new frame with DispatchSprites as callback; the frame is the script's own until Dispose
Sandbox.ModAPI.Ingame.IMyTextSurface.Font	pass	queue	MyTextPanel.cs:437 / MyTextPanelComponent.cs:955/745 new MyDefinitionId(type, name) -> MyStringHash.GetOrCompute (internally locked, thread-safe); setter definition lookup + Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.FontColor	pass	queue	MyTextPanel.cs:361 / MyTextPanelComponent.cs:728 m_fontData Sync read; setter Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.FontSize	pass	queue	MyTextPanel.cs:346 / MyTextPanelComponent.cs:762 m_fontData Sync read; setter rounds + Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.GetFonts	pass	-	MyTextPanel.cs:1525 / MyTextPanelComponent.cs:2348 font definitions into caller list (GetObjectBuilderType cache is ConcurrentDictionary)
Sandbox.ModAPI.Ingame.IMyTextSurface.GetScripts	pass	-	MyTextPanel.cs:1542 / MyTextPanelComponent.cs:1736 MyTextSurfaceScriptFactory.Scripts keys into caller list
Sandbox.ModAPI.Ingame.IMyTextSurface.GetSelectedImages	pass	-	MyTextPanel.cs:1513 / MyTextPanelComponent.cs:2340 SelectedTexturesToDraw subtype names into caller list
Sandbox.ModAPI.Ingame.IMyTextSurface.GetSprites	pass	-	MyTextPanel.cs:1537 / MyTextPanelComponent.cs:2360 copies m_definitions subtype names to caller list
Sandbox.ModAPI.Ingame.IMyTextSurface.GetText	pass	-	MyTextPanel.cs:1322 m_publicDescription.ToString(); MyTextPanelComponent.cs:2138 m_textHelper.ToString() (sees queued WriteText only after apply)
Sandbox.ModAPI.Ingame.IMyTextSurface.MeasureStringInPixels	pass	-	MyTextPanel.cs:1552 / MyTextPanelComponent.cs:2378 -> MyFontDefinition.MeasureStringRaw: m_fontsById lookup, MyFont.MeasureString glyph/kern dictionary reads; MyStringHash.GetOrCompute is lock-protected (adds entry only for unknown font names)
Sandbox.ModAPI.Ingame.IMyTextSurface.Name	pass	-	MyTextPanel.cs:562 / MyTextPanelComponent.cs:1058 m_name field
Sandbox.ModAPI.Ingame.IMyTextSurface.PreserveAspectRatio	pass	queue	MyTextPanel.cs:502 / MyTextPanelComponent.cs:895 m_contentData Sync; setter Sync + UpdateIsStaticContent
Sandbox.ModAPI.Ingame.IMyTextSurface.ReadText	pass	-	MyTextPanel.cs:1342 appends m_publicDescription; MyTextPanelComponent.cs:2162 appends Text, into caller buffer
Sandbox.ModAPI.Ingame.IMyTextSurface.RemoveImageFromSelection	queue	-	MyTextPanel.cs:1418 / MyTextPanelComponent.cs:2243 RemoveImagesFromSelectionRequest RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyTextSurface.RemoveImagesFromSelection	queue	-	MyTextPanel.cs:1453 / MyTextPanelComponent.cs:2278 RemoveImagesFromSelectionRequest RaiseEvent; void; copy the ids list
Sandbox.ModAPI.Ingame.IMyTextSurface.Script	pass	queue	MyTextPanel.cs:467 / MyTextPanelComponent.cs:982/798 m_scriptData Sync read; setter SetScriptData + NeedsUpdate
Sandbox.ModAPI.Ingame.IMyTextSurface.ScriptBackgroundColor	pass	queue	MyTextPanel.cs:532 / MyTextPanelComponent.cs:835 m_scriptData Sync read; setter SetScriptData + flags
Sandbox.ModAPI.Ingame.IMyTextSurface.ScriptForegroundColor	pass	queue	MyTextPanel.cs:547 / MyTextPanelComponent.cs:854 m_scriptData Sync read; setter SetScriptData
Sandbox.ModAPI.Ingame.IMyTextSurface.SurfaceSize	pass	-	MyTextPanel.cs:498 / MyTextPanelComponent.cs:1006 m_textureSize x static pure CalcAspectFactor (MyRenderComponentScreenAreas.cs:148)
Sandbox.ModAPI.Ingame.IMyTextSurface.TextPadding	pass	queue	MyTextPanel.cs:517 / MyTextPanelComponent.cs:913 m_contentData Sync read; setter rounds + Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.TextureSize	pass	-	MyTextPanel.cs:500 / MyTextPanelComponent.cs:1008 m_textureSize
Sandbox.ModAPI.Ingame.IMyTextSurface.WriteText	queue	-	queued; returns true, the game returns false only while a player edits the panel text. Agent: MyTextPanel.cs:1303/1327 writes m_publicDescriptionHelper + m_descriptionPublicDirty, returns !m_isTextPanelOpen; MyTextPanelComponent.cs:2123/2143 writes m_textHelper + m_textHelperDirty, returns true. Replace: compute the result from those reads, copy the string/StringBuilder, queue the real call in order
Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider.GetSurface	pass	-	MyTextPanel.cs:1562 PanelComponent; MyFunctionalBlock.cs:612, MyCockpit.cs:2847, MyProjectorBase.cs:2507 -> MyMultiTextPanelComponent.GetSurface :863 m_panels[index] (proxy must wrap the MyTextPanelComponent)
Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider.SurfaceCount	pass	-	MyTextPanel.cs:566 constant 1; MyFunctionalBlock.cs:163, MyCockpit.cs:619, MyProjectorBase.cs:871 -> m_panels.Count
Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider.UseGenericLcd	pass	-	MyFunctionalBlock.cs:139, MyTextPanel.cs:159, MyCockpit.cs:631 constants
Sandbox.ModAPI.Ingame.IMyThrust.CurrentThrust	pass	-	MyThrust.cs:374 m_currentStrength x definition x multiplier
Sandbox.ModAPI.Ingame.IMyThrust.CurrentThrustPercentage	pass	-	MyThrust.cs:376 CurrentThrust / MaxThrust math (both pass)
Sandbox.ModAPI.Ingame.IMyThrust.GridThrustDirection	pass	-	MyThrust.cs:378/276 reads session controlled entity / grid ControlSystem.CurrentShipController and block Orientation
Sandbox.ModAPI.Ingame.IMyThrust.MaxEffectiveThrust	pass	-	MyThrust.cs:362 -> MyEntityThrustComponent.GetLastThrustMultiplier :2173 -> MyThrusterBlockThrustComponent.CalculateForceMultiplier :378 field/definition math
Sandbox.ModAPI.Ingame.IMyThrust.MaxThrust	pass	-	MyThrust.cs:360 definition x m_thrustMultiplier
Sandbox.ModAPI.Ingame.IMyThrust.ThrustOverride	pass	queue	MyThrust.cs:294/195 Sync x multiplier x definition; setter converts + Sync
Sandbox.ModAPI.Ingame.IMyThrust.ThrustOverridePercentage	pass	queue	MyThrust.cs:306 Sync read; setter clamps + Sync
Sandbox.ModAPI.Ingame.IMyUpgradableBlock.FillUpgradesDictionary	pass	-	MyCubeBlock.cs:2103 copies UpgradeValues (lazy new dict of the own block :359) into caller dictionary
Sandbox.ModAPI.Ingame.IMyUpgradableBlock.GetUpgrades	pass	-	MyCubeBlock.cs:2115 out new Dictionary copy of UpgradeValues
Sandbox.ModAPI.Ingame.IMyUpgradableBlock.UpgradeCount	pass	-	MyCubeBlock.cs:393 UpgradeValues.Count (lazy own dict)
Sandbox.ModAPI.Ingame.IMyUpgradeModule.Connections	pass	-	MyUpgradeModule.cs:55 counts m_connectedBlocks values
Sandbox.ModAPI.Ingame.IMyUpgradeModule.FillUpgradeList	pass	-	MyUpgradeModule.cs:450 copies m_upgrades array into caller list
Sandbox.ModAPI.Ingame.IMyUpgradeModule.GetUpgradeList	pass	-	MyUpgradeModule.cs:462 out new list copy of m_upgrades
Sandbox.ModAPI.Ingame.IMyUpgradeModule.UpgradeCount	pass	-	MyUpgradeModule.cs:53 m_upgrades.Length
Sandbox.ModAPI.Ingame.IMyUserControllableGun.IsShooting	pass	-	MyUserControllableGun.cs:79 Sync<bool> m_isShooting
Sandbox.ModAPI.Ingame.IMyUserControllableGun.Shoot	pass	queue	MyUserControllableGun.cs:81 m_forceShoot read; setter SetShooting
Sandbox.ModAPI.Ingame.IMyUserControllableGun.ShootOnce	queue	-	MyUserControllableGun.cs:305 OnShootOncePressed; void
Sandbox.ModAPI.Ingame.IMyWarhead.Detonate	queue	-	MyWarhead.cs:644/386 Explode (damage, entity removal); void
Sandbox.ModAPI.Ingame.IMyWarhead.DetonationTime	pass	queue	MyWarhead.cs:197/183 m_countdownMs Sync; setter Sync
Sandbox.ModAPI.Ingame.IMyWarhead.IsArmed	pass	queue	MyWarhead.cs:165 Sync<bool>; setter Sync + emissive handler
Sandbox.ModAPI.Ingame.IMyWarhead.IsCountingDown	pass	-	MyWarhead.cs:195/143 auto-property
Sandbox.ModAPI.Ingame.IMyWarhead.StartCountdown	main	-	MyWarhead.cs:338 writes IsCountingDown, MyWarheads.AddWarhead (static list), events; returns bool
Sandbox.ModAPI.Ingame.IMyWarhead.StopCountdown	main	-	MyWarhead.cs:351 writes IsCountingDown, MyWarheads.RemoveWarhead (static list); returns bool (false when not counting down)
Sandbox.ModAPI.Ingame.IMyWindTurbine.Effectivity	pass	-	MyWindTurbine.cs:116 returns m_totalEffectivity field (written by UpdateEffectivity on main)
Sandbox.ModAPI.Ingame.IMyWindTurbine.PlacementEffectivity	pass	-	MyWindTurbine.cs:102 returns m_placementEffectivity field
Sandbox.ModAPI.Ingame.IMyWindTurbine.WindEffectivity	pass	-	MyWindTurbine.cs:98 own grid's MySharedWindComponent.WindSpeedModifier; create branch (:307) only for unregistered turbine, own grid
Sandbox.ModAPI.Ingame.IUserCustomizableTerminalAction.FetchAllBlocks	pass	-	MyToolbarItemTerminalBlock.cs:45 / MyToolbarItemTerminalGroup.cs:462 entity lookup, group HashSet read; adds to caller's list (wrap results)
Sandbox.ModAPI.Ingame.IUserCustomizableTerminalAction.GetBlock	pass	-	MyToolbarItemTerminalBlock.cs:40 returns m_block; MyToolbarItemTerminalGroup.cs:452 TryGetEntityById + BlockGroups read, new list
Sandbox.ModAPI.Ingame.IUserCustomizableTerminalAction.Parameters	pass	-	MyToolbarItemActions.cs:42 returns m_parameters list (read; script gets the item's own mutable list, as on main)
Sandbox.ModAPI.Ingame.IUserCustomizableTerminalAction.SetCustomIconTitle	queue	-	MyToolbarItemActions.cs:92 writes m_customIconTitle of a toolbar item (toolbar may be a player's or another grid's, read by HUD). Void
Sandbox.ModAPI.Ingame.MyGridProgram.Echo	pass	pass	Getter returns the delegate set in MyProgrammableBlock.CreateInstance (MyProgrammableBlock.cs:1195). Invoking the default delegate EchoTextToDetailInfo (:1217) only appends to this PB's own m_echoOutput, which only this PB's run touches (read back after the run in RunSandboxedProgramActionCore). Setter is the script assigning its own MyGridProgram field.
Sandbox.ModAPI.Ingame.MyGridProgram.GridTerminalSystem	pass	-	Auto-property on the script instance, set to the PB's MyGridTerminalWrapper by RunSandboxedProgramAction (MyProgrammableBlock.cs:819) and nulled after the run. Reading it writes nothing; the returned wrapper must be proxied.
Sandbox.ModAPI.Ingame.MyGridProgram.IGC	main	-	MyGridProgram.IGC calls m_IGC_ContextGetter, a lambda (MyProgrammableBlock.cs:1198) that lazily caches a context from MyIGCSystemSessionComponent.GetOrMakeContextFor (MyIGCSystemSessionComponent.cs:171), which adds to the shared m_perPBCommContexts Dictionary. Replace: call GetOrMakeContextFor for each PB on main before the phase and set IMyGridProgram.IGC_ContextGetter to return the cached (proxied) context; then the getter is a plain read.
Sandbox.ModAPI.Ingame.MyGridProgram.Me	pass	pass	Auto-property set to the PB in CreateInstance (MyProgrammableBlock.cs:1194). Read only; setter is the script assigning its own field (no game effect).
Sandbox.ModAPI.Ingame.MyGridProgram.Runtime	pass	-	Auto-property holding the PB's RuntimeInfo (set in CreateInstance, MyProgrammableBlock.cs:1191). Getter writes nothing; RuntimeInfo members are classified separately.
Sandbox.ModAPI.Ingame.MyGridProgram.Storage	pass	pass	MyGridProgram.cs:55 reads/writes the script instance's own m_storage. Only read by the PB on main (save, MyProgrammableBlock.cs:698/1108), which cannot run during the phase.
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.ApplyAction	pass@Terminal	-	TerminalBlockExtentions.cs:15/20 GetActionWithName then Apply, both through proxies (Apply queued)
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.GetId	pass	-	Returns block.EntityId (MyEntity.m_entityId field).
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.GetInventory	pass@Inventory	-	TerminalBlockExtentions.cs:84 reads; block unwrapped, the inventory wrapped (InventoryStandIn)
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.GetInventoryCount	pass@Inventory	-	TerminalBlockExtentions.cs:103 component read; block unwrapped (InventoryStandIn)
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.GetUseConveyorSystem	pass@Inventory	-	TerminalBlockExtentions.cs:118 Sync read; block unwrapped (InventoryStandIn)
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.HasAction	pass@Terminal	-	TerminalBlockExtentions.cs:25 GetActionWithName through the block proxy != null
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.HasInventory	pass@Inventory	-	TerminalBlockExtentions.cs:64 type tests; the block is unwrapped while proxies are on (InventoryStandIn)
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.SetUseConveyorSystem	pass@Inventory	-	TerminalBlockExtentions.cs:133 Sync write, made on main after the phase (InventoryStandIn)
Sandbox.ModAPI.Interfaces.ITerminalAction.Apply	queue	-	MyTerminalAction<TBlock>.Apply (MyTerminalAction.cs:244/253) runs the action delegate (Sync writes, RaiseEvent). Void; queue and copy the ListReader<TerminalActionParameter> argument.
Sandbox.ModAPI.Interfaces.ITerminalAction.Icon	pass	-	MyTerminalAction.cs:88 returns m_icon field
Sandbox.ModAPI.Interfaces.ITerminalAction.Id	pass	-	MyTerminalAction.Id returns readonly m_id (MyTerminalAction.cs:80).
Sandbox.ModAPI.Interfaces.ITerminalAction.IsEnabled	pass@Terminal	-	MyTerminalAction.cs:273 Enabled and Callable delegates, reads of the block (sampled ~30)
Sandbox.ModAPI.Interfaces.ITerminalAction.Name	pass	-	Returns the action's shared m_name StringBuilder (MyTerminalAction.cs:84); a read, but the script gets a shared mutable object (same as on main).
Sandbox.ModAPI.Interfaces.ITerminalAction.WriteValue	standin@Terminal	-	MyTerminalAction.cs:314 Writer formats with the shared MyValueFormatter; the stand-in holds ScriptStandIns.FormatLock
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetDefaultValue	pass@Terminal	-	MyTerminalValueControl.cs:127 DefaultValueGetter; combobox -> GetMinimum, serialized (TerminalStandIn)
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetMaximum	pass@Terminal	-	Slider Denormalizer lambdas; combobox fills its static m_handlerItems, serialized in the phase (TerminalStandIn)
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetMinimum	pass@Terminal	-	Same as GetMaximum
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetMininum	pass@Terminal	-	MyTerminalValueControl.cs:133 -> GetMinimum
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetValue	pass@Terminal	-	MyTerminalValueControl.cs:79 Getter delegate, reads of the block (sampled ~40)
Sandbox.ModAPI.Interfaces.ITerminalProperty.Id	pass	-	MyTerminalValueControl: returns the control's Id field.
Sandbox.ModAPI.Interfaces.ITerminalProperty.SetValue	queue	-	MyTerminalValueControl.SetValue (MyTerminalValueControl.cs:84) calls Setter (Sync writes) and block.NotifyTerminalValueChanged; slider/color/combobox overrides clamp first. Void, queue.
Sandbox.ModAPI.Interfaces.ITerminalProperty.TypeName	pass	-	MyTerminalValueControl: typeof(TValue).Name.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.As	pass	-	property as ITerminalProperty<TValue>; pure type test.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.AsBool	pass	-	property as ITerminalProperty<bool>; pure type test (proxy must implement the generic interface).
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.AsColor	pass	-	property as ITerminalProperty<Color>; pure type test.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.AsFloat	pass	-	property as ITerminalProperty<float>; pure type test.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.Cast	pass	-	As<TValue>() plus an exception built from property.Id/TypeName; reads only.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetDefaultValue	pass@Terminal	-	TerminalPropertyExtensions.cs:168 GetProperty and GetDefaultValue through proxies
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetMaximum	pass@Terminal	-	GetProperty and GetMaximum through proxies
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetMinimum	pass@Terminal	-	GetProperty and GetMinimum through proxies
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetMininum	pass@Terminal	-	TerminalPropertyExtensions.cs:181 GetProperty and GetMininum through proxies
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValue	pass@Terminal	-	TerminalPropertyExtensions.cs:156 GetProperty, Cast and GetValue through proxies
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueBool	pass@Terminal	-	GetValue<bool>: see GetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueColor	pass@Terminal	-	GetValue<Color>: see GetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueFloat	pass@Terminal	-	GetValue<float>: see GetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.Is	pass	-	TerminalPropertyExtensions.cs:49 property is ITerminalProperty<TValue>; pure type test
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValue	pass@Terminal	-	TerminalPropertyExtensions.cs:217 GetProperty and Cast through proxies, then the property's SetValue, queued
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueBool	pass@Terminal	-	SetValue<bool>: see SetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueColor	pass@Terminal	-	SetValue<Color>: see SetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueFloat	pass@Terminal	-	SetValue<float>: see SetValue extension.
Sandbox.MyInitializer.MyNativeException.StackTrace	pass	-	MyInitializer.cs:53 m_trace.ToString() of the exception's own StackTrace
Sandbox.MyInitializer.MyOutOfMemoryException.StackTrace	pass	-	MyInitializer.cs:40 returns string.Empty
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.CanPressurize	pass	-	lazy matrices of the own vent and grid. Agent: MyAirVent.CanPressurize (MyAirVent.cs:265) -> GetOxygenBlock (:1015): reads base.WorldMatrix of the vent (block: lazy RecalculateWorldMatrixHRIfNeeded writes, MyPositionComponentBase.cs:442) and CubeGrid.WorldToGridInteger (grid WorldMatrixNormalizedInv lazy cache, :294). Replace: same math with non-caching matrices (LocalMatrixRef * grid WorldMatrixRef, fresh inverse), then GasSystem.GetOxygenBlockAt and Room.IsAirtight (reads; breathing data is a ConcurrentDictionary).
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.Depressurize	pass	queue	Getter reads m_isDepressurizing Sync (MyAirVent.cs:224); setter writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.GetOxygenLevel	pass	-	lazy matrices of the own vent and grid. Agent: MyAirVent.GetOxygenLevel (MyAirVent.cs:1101) uses GetOxygenBlock (:1015) with the same lazy vent WorldMatrix and grid WorldMatrixNormalizedInv writes as CanPressurize. Replace with the non-caching matrix version, then MyOxygenBlock.OxygenLevel/Room.EnvironmentOxygen (reads).
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.IsDepressurizing	pass	-	MyAirVent.cs:224 m_isDepressurizing Sync read
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.IsPressurized	pass	-	MyAirVent.cs:1096 returns CanPressurize: same lazy own vent/grid matrices as CanPressurize, see there
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.PressurizationEnabled	pass	-	MySession.Static.Settings.EnableOxygenPressurization (MyAirVent.cs:320).
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.Status	pass	-	Auto-property Status (MyAirVent.cs:236), written only by UpdateStatus on main.
SpaceEngineers.Game.ModAPI.Ingame.IMyButtonPanel.AnyoneCanUse	pass	queue	MyButtonPanel.cs:116 m_anyoneCanUse Sync read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyButtonPanel.ClearCustomButtonName	queue	-	MyButtonPanel.cs:578 SetCustomButtonName(null) -> MyMultiplayer.RaiseEvent SetButtonName (:523). Void
SpaceEngineers.Game.ModAPI.Ingame.IMyButtonPanel.GetButtonName	pass	-	MyButtonPanel.cs:536 custom name dictionary read, else toolbar item DisplayName.ToString() or MyTexts; reads only
SpaceEngineers.Game.ModAPI.Ingame.IMyButtonPanel.HasCustomButtonName	pass	-	MyButtonPanel.cs:518 m_customButtonNames.Dictionary.ContainsKey
SpaceEngineers.Game.ModAPI.Ingame.IMyButtonPanel.IsButtonAssigned	pass	-	MyButtonPanel.cs:513 Toolbar.GetItemAtIndex(pos) != null, array read
SpaceEngineers.Game.ModAPI.Ingame.IMyButtonPanel.SetCustomButtonName	queue	-	MyButtonPanel.cs:573 -> SetCustomButtonName (:523) MyMultiplayer.RaiseEvent SetButtonName. Void
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGenerator.FieldDepth	pass	-	MyGravityGenerator.cs:93 m_fieldSize Sync read (.Z)
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGenerator.FieldHeight	pass	-	MyGravityGenerator.cs:91 m_fieldSize Sync read (.Y)
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGenerator.FieldSize	pass	queue	MyGravityGenerator.cs:95/58 Sync read; setter clamps to definition and writes m_fieldSize Sync (UpdateFieldShape on change)
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGenerator.FieldWidth	pass	-	MyGravityGenerator.cs:89 m_fieldSize Sync read (.X)
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGeneratorBase.Gravity	pass	-	MyGravityGeneratorBase.cs:87 / MyGravityGeneratorSphere.cs:74 GravityAcceleration Sync read (/9.81)
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGeneratorBase.GravityAcceleration	pass	queue	MyGravityGeneratorBase.cs:89: getter reads m_gravityAcceleration Sync; setter clamps and writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGeneratorSphere.Radius	pass	queue	MyGravityGeneratorSphere.cs:88/60 m_radius Sync read; setter clamps to definition and writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.AutoLock	pass	queue	MyLandingGear.AutoLock (MyLandingGear.cs:375): Sync read; setter writes m_autoLock Sync.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.IsBreakable	pass	-	MyLandingGear.cs:387/330 m_breakForce field compare
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.IsLocked	pass	-	LockMode == Locked (MyLandingGear.cs:328), field read.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.IsParkingEnabled	pass	queue	MyLandingGear.cs:403/295 m_isParkingEnabled Sync read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.Lock	queue	-	RequestLandingGearLock -> RequestLock -> MyMultiplayer.RaiseEvent AttachRequest (MyLandingGear.cs:597/1534). Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.LockMode	pass	-	Returns m_lockMode field (MyLandingGear.cs:307).
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.ResetAutoLock	queue	-	MyLandingGear.cs:1742/1108 writes m_autoLockTimer and SetEmissiveStateWorking (render emissivity). Void
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.ToggleLock	queue	-	RequestLandingGearSwitch (MyLandingGear.cs:585) -> RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.Unlock	queue	-	RequestLandingGearUnlock -> RequestLock -> RaiseEvent (MyLandingGear.cs:605). Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyLargeConveyorTurretBase.UseConveyorSystem	pass	-	MyLargeConveyorTurretBase.cs:53 m_useConveyorSystem Sync read
SpaceEngineers.Game.ModAPI.Ingame.IMyOxygenFarm.CanProduce	pass	-	MyOxygenFarm.cs:89/56 session settings, Enabled Sync, ResourceSink.IsPoweredByType array read, IsWorking; reads only
SpaceEngineers.Game.ModAPI.Ingame.IMyOxygenFarm.GetOutput	pass	-	MyOxygenFarm.cs:306: IsWorking ? SolarComponent.MaxOutput : 0; field reads.
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.Atmosphere	pass	-	MyParachute.cs:242 lazy m_atmosphereDensityCache of the own block + planet GetAirDensity (pure math); own block WorldMatrix
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.AutoDeploy	pass	queue	MyParachute.cs:193 m_autoDeploy Sync read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.AutoDeployHeight	pass	queue	MyParachute.cs:178 DeployHeight (m_deployHeight Sync) read; setter clamps and writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.CloseDoor	queue	-	MyParachute.cs:297 -> ToggleDoor -> MyDoorBase.SetOpenRequest RaiseEvent (MyDoorBase.cs:170). Void
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.GetArtificialGravity	pass@Gravity	-	MyParachute.cs:972 see IMyShipController.GetArtificialGravity
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.GetNaturalGravity	pass	-	MyParachute.cs:967 CalculateNaturalGravityInPoint (thread-safe planet math) at own block WorldMatrix (lazy, own group)
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.GetTotalGravity	pass@Gravity	-	MyParachute.cs:977 see IMyShipController.GetArtificialGravity
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.GetVelocity	pass	-	MyParachute.cs:957 Parent.Physics.GetVelocityAtPoint (Havok read) at own block position
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.OpenDoor	queue	-	MyParachute.cs:289 -> ToggleDoor -> MyDoorBase.SetOpenRequest RaiseEvent (MyDoorBase.cs:170). Void
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.OpenRatio	pass	-	MyParachute.cs:166 base.Open Sync read
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.Status	pass	-	MyParachute.cs:154 base.Open Sync read
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.ToggleDoor	queue	-	MyParachute.cs:305 SetOpenRequest -> MyMultiplayer.RaiseEvent OpenRequest (MyDoorBase.cs:170). Void
SpaceEngineers.Game.ModAPI.Ingame.IMyParachute.TryGetClosestPoint	pass	-	MyParachute.cs:943 sets own m_nearPlanetCache; GetClosestPlanet ([ThreadStatic] list, tree lock), cacheless ProjectToSurface; out param
SpaceEngineers.Game.ModAPI.Ingame.IMyShipMergeBlock.IsConnected	pass	-	MyShipMergeBlock.IsLocked: m_mergeState Sync read (MyShipMergeBlock.cs:160).
SpaceEngineers.Game.ModAPI.Ingame.IMyShipMergeBlock.State	pass	-	MyShipMergeBlock.cs:168 m_mergeState Sync read
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.GetSounds	pass	-	MySoundBlock.cs:837 definitions dictionary read (GetSoundCategoryDefinitions new list); clears and fills the caller's list
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.IsSoundSelected	pass	-	!string.IsNullOrEmpty(m_cueIdString) (MySoundBlock.cs:192).
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.LoopPeriod	pass	queue	MySoundBlock.LoopPeriod (MySoundBlock.cs:166): Sync read; setter writes m_loopPeriod Sync.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.Play	queue	-	RequestPlaySound -> MyMultiplayer.RaiseEvent PlaySound (MySoundBlock.cs:481). Void.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.Range	pass	queue	MySoundBlock.cs:230/136 m_soundRadius Sync read; setter clamps and writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.SelectedSound	pass	queue	Getter returns m_cueIdString (MySoundBlock.cs:256); setter SelectSound writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.Stop	queue	-	RequestStopSound (MySoundBlock.cs:569): RaiseEvent plus StopSoundInternal on the sound emitter. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.Volume	pass	queue	MySoundBlock.cs:218/151 m_volume Sync read; setter clamps and writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMySpaceBall.Broadcasting	pass	queue	MySpaceBall.cs:219/139 RadioBroadcaster.Enabled read; setter writes m_broadcastSync (BroadcastChanged on change)
SpaceEngineers.Game.ModAPI.Ingame.IMySpaceBall.Friction	pass	queue	MySpaceBall.cs:193/103 m_friction Sync read; setter clamps and writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMySpaceBall.IsBroadcasting	pass	-	MySpaceBall.cs:217/139 Components.Get<MyDataBroadcaster>().Enabled read
SpaceEngineers.Game.ModAPI.Ingame.IMySpaceBall.Restitution	pass	queue	MySpaceBall.cs:205/127 m_restitution Sync read; setter clamps and writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMySpaceBall.VirtualMass	pass	queue	MySpaceBall.cs:181/453 GetMass reads m_virtualMass Sync; setter writes Sync (RefreshPhysicsBody on change)
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.IsCountingDown	pass	-	Private auto-property (MyTimerBlock.cs:119).
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.Silent	pass	queue	MyTimerBlock.cs:159/121 m_silent Sync read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.StartCountdown	queue	-	StartBtn (MyTimerBlock.cs:576): RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.StopCountdown	queue	-	StopBtn (MyTimerBlock.cs:581): RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.Trigger	queue	-	OnTrigger (MyTimerBlock.cs:487): RaiseEvent Trigger. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.TriggerDelay	pass	queue	MyTimerBlock.cs:133: getter rounds m_timerSync; setter writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyTransponder.AllowSignalsFrom	pass	queue	MyTransponderBlock.cs:48 -> MySignalReceiverEntityComponent.cs:75 m_receiveFrom Sync read/set
SpaceEngineers.Game.ModAPI.Ingame.IMyTransponder.Channel	pass	queue	MyTransponderBlock.cs:29 -> MySignalChannelEntityComponent.cs:63 m_channel Sync read/set
SpaceEngineers.Game.ModAPI.Ingame.IMyTransponder.SendSignal	queue	-	MyTransponderBlock.cs:103 -> MySignalSenderEntityComponent.cs:89 frame stamp write, antenna reachability, triggers receivers on other grids. Void
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AIEnabled	pass	queue	m_allowAI Sync read/write (MyTurretControlBlock.cs:841).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AddTool	queue	-	MyTurretControlBlock.AddTool (MyTurretControlBlock.cs:3939) -> SyncToolSelection RaiseEvent. Void; unwrap the proxied tool argument when applying.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AddTools	queue	-	MyTurretControlBlock.cs:3899 -> SyncToolSelection RaiseEvent (:2520). Void; snapshot the list and unwrap proxied tools at call time
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AngleDeviation	pass	queue	MyTurretControlBlock.cs:638: ToDegrees(m_angleDeviation) read; setter writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AzimuthRotor	pass	queue	Getter returns m_azimuthor (MyTurretControlBlock.cs:1186); setter writes m_boundAzimuth Sync after IsInSameLogicalGroupAs (unwrap the proxied rotor).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.Camera	pass	queue	Getter returns m_camera (MyTurretControlBlock.cs:1224); setter writes m_boundCamera Sync (unwrap the proxied camera).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.ClearTools	queue	-	MyTurretControlBlock.cs:3955: SyncToolUnselection RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.ElevationRotor	pass	queue	Getter returns m_elevator (MyTurretControlBlock.cs:1205); setter writes m_boundElevation Sync (unwrap the proxied rotor).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetDirectionSource	pass	-	Returns m_directionBlock (MyTurretControlBlock.cs:3973).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetShootDirection	pass	-	ShootDirection (MyTurretControlBlock.cs:1044): WorldMatrix.Forward of the controller, its camera or its bound weapon (MySmallGatlingGun.cs:643 and the others), all on its own construct
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetTargetedEntity	pass@Sensing	-	MyTurretControlBlock.cs:3845 see IMyLargeTurretBase.GetTargetedEntity
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetTargetingGroup	pass	-	MyTurretControlBlock.cs:3868 m_targetingGroup Sync read, MyStringHash.String (locked table read)
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetTargetingGroups	pass	-	MyTurretControlBlock.cs:3849 targeting group definitions read; fills the caller's list (obsolete overload returns a new list)
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetTools	pass	-	Clears the script's list and adds values from m_boundTools (MyTurretControlBlock.cs:3883); reads game state, writes only the caller's list.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.HasTarget	pass	-	m_targetingSystem.Target != null (MyTurretControlBlock.cs:1184); a read (targeting may update it in its own background task, as in vanilla).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.IsAimed	pass	-	MyTurretControlBlock.cs:719 m_isAimed field read (setter not in the ingame interface)
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.IsSunTrackerEnabled	pass	queue	MySunTrackingComponent.m_isSunTrackerEnabled read (MyTurretControlBlock.cs:853). Setter (RaiseEvent) unused by scripts.; set MySunTrackingComponent.cs:50 Sync<T>.Value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.IsUnderControl	pass	-	ControllerInfo.Controller != null (MyTurretControlBlock.cs:1170).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.MoveIndicator	pass	-	Auto-property (MyTurretControlBlock.cs:1245).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.Range	pass	queue	ShootRangeSimple (MyTurretControlBlock.cs:1122): m_shootingRange Sync read; setter clamps and writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.RemoveTool	queue	-	MyTurretControlBlock.cs:3947 -> SyncToolUnselection RaiseEvent (:2515). Void; unwrap the proxied tool
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.RemoveTools	queue	-	MyTurretControlBlock.cs:3919 -> SyncToolUnselection RaiseEvent (:2515). Void; snapshot the list and unwrap proxied tools
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.RollIndicator	pass	-	Auto-property (MyTurretControlBlock.cs:1247).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.RotationIndicator	pass	-	Auto-property (MyTurretControlBlock.cs:1243).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.SetTargetingGroup	queue	-	MyTurretControlBlock.cs:3878 m_targetingGroup Sync set; MyStringHash.Get throws on unknown id (then on main). Void
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetCharacters	pass	queue	MyTurretControlBlock.cs:945 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetFriends	pass	queue	MyTurretControlBlock.cs:1021 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetLargeGrids	pass	queue	MyTurretControlBlock.cs:926 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetMeteors	pass	queue	MyTurretControlBlock.cs:869 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetMissiles	pass	queue	MyTurretControlBlock.cs:888 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetNeutrals	pass	queue	MyTurretControlBlock.cs:983 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetSmallGrids	pass	queue	MyTurretControlBlock.cs:907 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.TargetStations	pass	queue	MyTurretControlBlock.cs:964 TargetFlags (m_targetFlags Sync, :829) bit read; setter writes the Sync value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.VelocityMultiplierAzimuthRpm	pass	queue	Sync read times GetAzimuthSpeedMax (rotor MaxRotorAngularVelocity from MyGridPhysics settings) (MyTurretControlBlock.cs:785).; set MyTurretControlBlock.cs:793 Sync<T>.Value
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.VelocityMultiplierElevationRpm	pass	queue	Sync read times GetElevatorSpeedMax (MyTurretControlBlock.cs:797).; set MyTurretControlBlock.cs:805 Sync<T>.Value
SpaceEngineers.Game.ModAPI.Ingame.IMyVirtualMass.VirtualMass	pass	-	MyVirtualMass.cs:38 definition read; MySpaceBall.cs:115 m_virtualMass Sync read
System.ArrayExtensions.Contains	pass	-	ArrayExtensions.cs:281 loop over the given array with EqualityComparer<T>.Default; read only
System.Collections.Generic.DictionaryExtensions.AssertEmpty	pass	-	DictionaryExtensions.cs:90 Clear on the given dictionary; only touches the script's own argument collections
System.Collections.Generic.DictionaryExtensions.FirstPair	pass	-	DictionaryExtensions.cs:58 enumerator read of the given dictionary
System.Collections.Generic.DictionaryExtensions.GetOrAdd	pass	-	DictionaryExtensions.cs:79 ConcurrentDictionary TryGetValue/GetOrAdd plus the script's activator; only touches the script's own argument collections
System.Collections.Generic.DictionaryExtensions.GetValueOrDefault	pass	-	DictionaryExtensions.cs:43/49/65 TryGetValue on the given dictionary; read only
System.Collections.Generic.DictionaryExtensions.GetValueOrNew	pass	-	DictionaryExtensions.cs:33 TryGetValue, new V, Add to the given dictionary; only touches the script's own argument collections
System.Collections.Generic.DictionaryExtensions.GetWith	pass	-	DictionaryExtensions.cs:9 nested loop over the given lists; read only
System.Collections.Generic.DictionaryExtensions.Remove	pass	-	DictionaryExtensions.cs:74 TryRemove on the given ConcurrentDictionary; only touches the script's own argument collections
System.Collections.Generic.HashSetExtensions.FirstElement	pass	-	HashSetExtensions.cs:5 enumerator read of the given set
System.Collections.Generic.ListExtensions.AddArray	pass	-	ListExtensions.cs:174/179 Capacity + Add on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.AddHashsetCasting	pass	-	ListExtensions.cs:97 foreach set, Add to the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.AddList	pass	-	ListExtensions.cs:159 Capacity + Add on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.AddOrInsert	pass	-	ListExtensions.cs:85 Add/Insert on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.AssertEmpty	pass	-	ListExtensions.cs:337 Clear on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.AtMod	pass	-	ListExtensions.cs:377/382 indexer read of List/ListReader
System.Collections.Generic.ListExtensions.BinaryIntervalSearch	pass	-	ListExtensions.cs:198/247/287 reads; Comparison overload uses the [ThreadStatic] FunctorComparer instance
System.Collections.Generic.ListExtensions.ClearAndTrim	pass	-	ListExtensions.cs:449 Clear + Capacity on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.EnsureCapacity	pass	-	ListExtensions.cs:347 Capacity on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.FunctorComparer..ctor	pass	-	ListExtensions.cs:9 implicit ctor, no state beyond the instance
System.Collections.Generic.ListExtensions.FunctorComparer.Compare	pass	-	ListExtensions.cs:16 calls the stored Comparison delegate
System.Collections.Generic.ListExtensions.FunctorComparer.Forget	pass	-	ListExtensions.cs:38 nulls the instance's delegate field; instance is [ThreadStatic]
System.Collections.Generic.ListExtensions.FunctorComparer.Get	pass	-	ListExtensions.cs:22 m_Instance is [ThreadStatic], so each worker has its own comparer
System.Collections.Generic.ListExtensions.GetClearToken	pass	-	ListExtensions.cs:44 new ClearToken struct over the given list; Dispose clears that list only
System.Collections.Generic.ListExtensions.GetInternalArray	pass	-	ListExtensions.cs:80 list.ToArray copy
System.Collections.Generic.ListExtensions.InsertInOrder	pass	-	ListExtensions.cs:308/318 BinarySearch + Insert on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.IsSorted	pass	-	ListExtensions.cs:323 comparer loop; read only
System.Collections.Generic.ListExtensions.IsValidIndex	pass	-	ListExtensions.cs:123 Count compare
System.Collections.Generic.ListExtensions.MaxBy	pass	-	ListExtensions.cs:392/415 enumerates the source with the script's selector; read only
System.Collections.Generic.ListExtensions.MinBy	pass	-	ListExtensions.cs:387 MaxBy with a negating closure; read only
System.Collections.Generic.ListExtensions.Move	pass	-	ListExtensions.cs:109 element shifts in the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.Pop	pass	-	ListExtensions.cs:355 RemoveAt on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.RemoveAtFast	pass	-	ListExtensions.cs:59/69 swap + RemoveAt on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.RemoveIndices	pass	-	ListExtensions.cs:135 compaction + RemoveRange on the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.SortNoAlloc	pass	-	ListExtensions.cs:364 list.Sort with the [ThreadStatic] FunctorComparer (per thread, not shared)
System.Collections.Generic.ListExtensions.Swap	pass	-	ListExtensions.cs:152 two element writes in the given list; only touches the script's own argument collections
System.Collections.Generic.ListExtensions.ToArray	pass	-	ListExtensions.cs:438 new array via the script's selector; read only
System.Collections.Generic.QueueExtensions.TryDequeue	pass	-	QueueExtensions.cs:20 Dequeue on the given queue; only touches the script's own argument collections
System.Collections.Generic.QueueExtensions.TryDequeueSync	pass	-	QueueExtensions.cs:31 lock on the given queue's SyncRoot + Dequeue; only touches the script's own argument collections
System.Text.StringBuilderExtensions_Format.AppendStringBuilder	pass	-	StringBuilderExtensions_Format.cs:7 appends chars to the script's StringBuilder
System.Text.StringBuilderExtensions_Format.AppendSubstring	pass	-	StringBuilderExtensions_Format.cs:17 appends chars to the script's StringBuilder
System.Text.StringBuilderExtensions_Format.ConcatFormat	pass	-	StringBuilderExtensions_Format.cs:47 invariant NumberFormatInfo; Concat helpers read only static readonly ms_digits
System.Text.StringBuilderExtensions_Format.FirstLetterUpperCase	pass	-	StringBuilderExtensions_Format.cs:226 char write in the script's StringBuilder
System.Text.StringBuilderExtensions_Format.ToLower	pass	-	StringBuilderExtensions_Format.cs:217 char writes in the script's StringBuilder
System.Text.StringBuilderExtensions_Format.ToUpper	pass	-	StringBuilderExtensions_Format.cs:208 char writes in the script's StringBuilder
VRage.Game.Components.Interfaces.IMyComponentContainer.Get	pass	-	MyComponentContainer.Get<T>: Dictionary TryGetValue on m_components. Returned components (e.g. MyResourceSinkComponent) are classes, not ingame interfaces: their members are not intercepted by proxies.
VRage.Game.Components.Interfaces.IMyComponentContainer.Has	pass	-	MyComponentContainer.cs:209 m_components ContainsKey/Count read (only implementer, MyEntityComponentContainer inherits it)
VRage.Game.Components.Interfaces.IMyComponentContainer.TryGet	pass	-	MyComponentContainer.TryGet<T>: m_components lookup plus MyComponentTypeFactory.TryGetComponentTypeFromModApiInterface (static dict read). Same caveat: returned component classes (MyResourceSinkComponent etc.) bypass the proxies.
VRage.Game.Components.MyComponentBase.ContainerBase	pass	-	MyComponentBase.cs:15 returns m_container field
VRage.Game.Components.MyComponentBase.GetAs	pass	-	MyComponentBase.cs:45 'this as T'; override MyCompositeGameLogicComponent.cs:84 loops its list, read only
VRage.Game.Components.MyResourceSinkComponentBase.AcceptedResources	pass	-	MyResourceSinkComponent.cs:168 (only subclass) new ListReader over m_resourceIds; read only
VRage.Game.Components.MyResourceSinkComponentBase.CurrentInputByType	pass	-	MyResourceSinkComponent.cs:454 m_dataPerType array read
VRage.Game.Components.MyResourceSinkComponentBase.IsPowerAvailable	pass	-	MyResourceSinkComponent.cs:427 CurrentInput + distributor Sink_IsResourceAvailable (MyResourceDistributorComponent.cs:2852) reads
VRage.Game.Components.MyResourceSinkComponentBase.IsPoweredByType	pass	-	MyResourceSinkComponent.cs:464 m_dataPerType array read
VRage.Game.Components.MyResourceSinkComponentBase.MaxRequiredInputByType	pass	-	MyResourceSinkComponent.cs:444 m_dataPerType array read
VRage.Game.Components.MyResourceSinkComponentBase.RequiredInputByType	pass	-	MyResourceSinkComponent.cs:459 m_dataPerType array read
VRage.Game.Components.MyResourceSinkComponentBase.SuppliedRatioByType	pass	-	MyResourceSinkComponent.cs:494 m_dataPerType array read
VRage.Game.Components.MyResourceSourceComponentBase.CurrentOutputByType	pass	-	MyResourceSourceComponent.cs:226 (only subclass) m_dataPerType array read
VRage.Game.Components.MyResourceSourceComponentBase.DefinedOutputByType	pass	-	MyResourceSourceComponent.cs:304 m_dataPerType array read
VRage.Game.Components.MyResourceSourceComponentBase.MaxOutputByType	pass	-	MyResourceSourceComponent.cs:268 MaxOutputLimitedByCapacity: math on m_dataPerType
VRage.Game.Components.MyResourceSourceComponentBase.ProductionEnabledByType	pass	-	MyResourceSourceComponent.cs:344 m_dataPerType array read
VRage.Game.GUI.TextPanel.MySpriteDrawFrame..ctor	pass	-	MySpriteDrawFrame.cs:29 stores the callback, list from PoolManager (concurrent pool); the callback runs only in Dispose
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.Add	pass	-	Struct method, not interceptable: appends to the frame's own pooled MyList (m_sprites from PoolManager in the ctor), no shared state (MySpriteDrawFrame.cs:45).
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.AddRange	pass	-	Struct method: m_sprites.AddRange of the script's enumerable into the frame's own list (MySpriteDrawFrame.cs:50).
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.AddToList	pass	-	MySpriteDrawFrame.cs:69 copies the frame's own m_sprites into the caller's list
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.Clip	pass	-	MySpriteDrawFrame.cs:39 Add of a clip sprite to the frame's own list, returns a token struct
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.Dispose	pass@Sprites	-	MySpriteDrawFrame.cs:77 callback then pool return (thread-safe PoolManager); DispatchSprites :1956 writes only its surface's fields, sent and rendered by the main thread later
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.ToCollection	pass	-	MySpriteDrawFrame.cs:55 copies the frame's own list into a new array
VRage.Game.ModAPI.Ingame.IMyCubeBlock.BlockDefinition	pass	-	SlimBlock.BlockDefinition.Id (MyCubeBlock.cs:371).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.CubeGrid	pass	-	SlimBlock.CubeGrid field (MyCubeBlock.cs:302/375).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.DefinitionDisplayNameText	pass	-	DisplayNameText of the shared definition is made thread-safe by ScriptRunner. Agent: BlockDefinition.DisplayNameText; MyCubeBlockDefinition.DisplayNameText (MyCubeBlockDefinition.cs:416) for definitions with DisplayNameVariant lazily creates and reuses the shared m_displayNameTextCache StringBuilder (definition shared by all blocks of that type in all groups). Replace: DisplayNameVariant == null ? base text : base text + """" """" + MyTexts.GetString(variant), no shared builder.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.DisassembleRatio	pass	-	MyCubeBlock.cs:325 definition read; MyDoor.cs:107 / MyAdvancedDoor.cs:99 multiply, Open Sync read
VRage.Game.ModAPI.Ingame.IMyCubeBlock.DisplayNameText	pass	-	MyTerminalBlock.DisplayNameText = CustomName.ToString() (MyTerminalBlock.cs:276), new string; other blocks use the MyEntity auto-property.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.GetOwnerFactionTag	pass	-	MyCubeBlock.GetOwnerFactionTag (MyCubeBlock.cs:443): IDModule.Owner and MyFactionCollection.GetPlayerFaction (two Dictionary reads).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.GetPlayerRelationToOwner	pass	-	MyCubeBlock.cs:474 LocalHumanPlayer + MyIDModule.GetUserRelationToOwner: faction/relation dictionary reads (MyIDModule.cs:33)
VRage.Game.ModAPI.Ingame.IMyCubeBlock.GetUserRelationToOwner	pass	-	MyCubeBlock.cs:461 -> MyIDModule.GetRelationPlayerBlock: MyFactionCollection dictionary reads, Sync.Players.IdentityIsNpc HashSet read
VRage.Game.ModAPI.Ingame.IMyCubeBlock.IsBeingHacked	pass@Sensing	-	MyTerminalBlock.cs:370 writes its own previous value, raises IsBeingHackedChanged (no game subscribers)
VRage.Game.ModAPI.Ingame.IMyCubeBlock.IsFunctional	pass	-	SlimBlock.ComponentStack.IsFunctional: integrity field comparisons (MyComponentStack.cs:131).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.IsWorking	pass	-	Auto-property IsWorking (MyCubeBlock.cs:327).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Mass	pass	-	GetMass() = BlockDefinition.Mass (MyCubeBlock.cs:1722); MySpaceBall override reads VirtualMass.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Max	pass	-	SlimBlock.Max field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Min	pass	-	SlimBlock.Min field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.NumberInGrid	pass	-	Auto-property (MyCubeBlock.cs:319).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Orientation	pass	-	SlimBlock.Orientation field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.OwnerId	pass	-	IDModule?.Owner (MyCubeBlock.cs:266).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Position	pass	-	SlimBlock.Position field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.UpdateIsWorking	queue	-	MyCubeBlock.cs:507 writes IsWorking, raises IsWorkingChanged (resource/conveyor handlers), CheckEmissiveState
VRage.Game.ModAPI.Ingame.IMyCubeBlock.UpdateVisual	queue	-	MyCubeBlock.cs:1135 OnBlockUpdateVisual event, skin/model change, Render writes; many overrides (MyBatteryBlock.cs:541 etc.) do render/emissive work
VRage.Game.ModAPI.Ingame.IMyCubeGrid.CubeExists	pass	-	m_cubes.ContainsKey (MyCubeGrid.cs:8737).
VRage.Game.ModAPI.Ingame.IMyCubeGrid.CustomName	pass	queue	Getter returns DisplayName field; setter calls ChangeDisplayNameRequest (network event) (MyCubeGrid.cs:3652).
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GetCubeBlock	pass	-	m_cubes.TryGetValue plus FatBlock/IsAccessibleForProgrammableBlock field reads (MyCubeGrid.cs:7933/16382).
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GridIntegerToWorld	pass	-	Transform with base.WorldMatrix (MyCubeGrid.cs:6051); grids have no position parent, so RecalculateWorldMatrixHRIfNeeded returns without writing.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GridSize	pass	-	Auto-property GridSize.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GridSizeEnum	pass	-	m_gridSizeEnum field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.IsSameConstructAs	pass	-	MyCubeGridGroups.Static.Mechanical.GetGroup x2 (MyGroups.GetGroup: Dictionary read) (MyCubeGrid.cs:16421). Casts the argument to MyCubeGrid: unwrap the proxy.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.IsStatic	pass	-	m_isStatic field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.LinearVelocity	pass	-	MyCubeGrid.cs:3059 Physics.LinearVelocityUnsafe (Havok read; MP client adds parent velocity via MyEntities.GetEntityById read)
VRage.Game.ModAPI.Ingame.IMyCubeGrid.Max	pass	-	m_max field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.Min	pass	-	m_min field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.Speed	pass	-	MyCubeGrid.cs:3075 m_speed field
VRage.Game.ModAPI.Ingame.IMyCubeGrid.WorldToGridInteger	pass	-	lazy inverse matrix of the own grid. Agent: Uses PositionComp.WorldMatrixNormalizedInv (MyCubeGrid.cs:6036), which lazily writes m_normalizedWorldMatrixInv and its dirty flag (MyPositionComponentBase.cs:294). Low use; a fresh MatrixD.Invert(Normalize(WorldMatrix)) would make it pass.
VRage.Game.ModAPI.Ingame.IMyEntity.Closed	pass	-	MyEntity.Closed auto-property.
VRage.Game.ModAPI.Ingame.IMyEntity.Components	pass	-	MyEntity.Components auto-property; returned container should be proxied (see IMyComponentContainer).
VRage.Game.ModAPI.Ingame.IMyEntity.DisplayName	pass	-	m_displayName field.
VRage.Game.ModAPI.Ingame.IMyEntity.EntityId	pass	-	m_entityId field.
VRage.Game.ModAPI.Ingame.IMyEntity.GetInventory	pass	-	GetInventoryBase(index): Components.TryGet<MyInventoryBase> then IterateInventory (MyInventoryAggregate ChildList read), or MyProductionBlock override returning Input/OutputInventory; reads only.
VRage.Game.ModAPI.Ingame.IMyEntity.GetPosition	pass	-	same as WorldMatrix. Agent: PositionComp.GetPosition (MyPositionComponentBase.cs:504) calls RecalculateWorldMatrixHRIfNeeded (:442): for blocks (position parent = grid) it lazily rewrites m_worldMatrix, counters and dirty flags. Replace: root entity -> WorldMatrixRef.Translation (no write); child -> (LocalMatrixRef * parent world matrix, computed recursively without caching).Translation.
VRage.Game.ModAPI.Ingame.IMyEntity.HasInventory	pass	-	InventoryCount > 0: Components.TryGet + GetInventoryCount (reads).
VRage.Game.ModAPI.Ingame.IMyEntity.InventoryCount	pass	-	Components.TryGet<MyInventoryBase> + GetInventoryCount (MyInventory returns 1, aggregate returns its counter).
VRage.Game.ModAPI.Ingame.IMyEntity.Name	pass	-	m_name field.
VRage.Game.ModAPI.Ingame.IMyEntity.WorldAABB	pass	-	lazy AABB of an own-group entity. Agent: PositionComp.WorldAABB (MyPositionComponentBase.cs:105) lazily recomputes m_worldAABB and clears m_worldAABBDirty, for grids too, plus the HR world matrix for blocks. Replace: LocalAABB.Transform(non-caching world matrix).
VRage.Game.ModAPI.Ingame.IMyEntity.WorldAABBHr	pass	-	same as IMyEntity.WorldAABB: MyEntity.cs:1243 -> PositionComp.WorldAABB (lazy recompute of an own-group entity)
VRage.Game.ModAPI.Ingame.IMyEntity.WorldMatrix	pass	-	lazy world matrix of an own-group entity; the grid does not move during the phase. Agent: PositionComp.WorldMatrixRef (MyPositionComponentBase.cs:58) -> RecalculateWorldMatrixHRIfNeeded (:442) lazily writes m_worldMatrix/counter/dirty flags for blocks. Root entities (grids) read without writes. Replace: child -> LocalMatrixRef * parent world matrix computed without caching.
VRage.Game.ModAPI.Ingame.IMyEntity.WorldVolume	pass	-	lazy volume of an own-group entity. Agent: PositionComp.WorldVolume (MyPositionComponentBase.cs:132) lazily recomputes m_worldVolume and clears its dirty flag (also for grids). Replace: center = Transform(LocalVolume.Center, non-caching world matrix), radius = LocalVolume.Radius.
VRage.Game.ModAPI.Ingame.IMyEntity.WorldVolumeHr	pass	-	same as IMyEntity.WorldVolume: MyEntity.cs:1252 -> PositionComp.WorldVolume (lazy recompute of an own-group entity)
VRage.Game.ModAPI.Ingame.IMyInventory.CanItemsBeAdded	pass	-	MyInventory.CanItemsBeAdded (MyInventory.cs:619): MyInventoryItemAdapter.Static is [ThreadStatic], definition lookups, Constraint.Check (HashSet reads).
VRage.Game.ModAPI.Ingame.IMyInventory.CanPutItems	pass	-	MyInventory.cs:508 MyInventoryBase.CanPutItems field
VRage.Game.ModAPI.Ingame.IMyInventory.CanTransferItemTo	pass@Transfers	-	MyInventory.cs:2776 see IsConnectedTo
VRage.Game.ModAPI.Ingame.IMyInventory.ContainItems	pass	-	MyInventory.cs:2695 -> ContainItems :841 GetItemAmount sum over m_items
VRage.Game.ModAPI.Ingame.IMyInventory.CurrentMass	pass	-	m_currentMass + ExternalMass fields (MyInventory.cs:428).
VRage.Game.ModAPI.Ingame.IMyInventory.CurrentVolume	pass	-	m_currentVolume field (MyInventory.cs:414).
VRage.Game.ModAPI.Ingame.IMyInventory.FindItem	pass	-	FindFirstPositionOfType scans m_items; MakeAPIItem builds a new MyInventoryItem (MyInventory.cs:2714/754).
VRage.Game.ModAPI.Ingame.IMyInventory.GetAcceptedItems	pass	-	Reads Constraint ids or MyDefinitionManager.GetInventoryItemDefinitions (thread-safe Lazy); writes only the caller's list (MyInventory.cs:2783).
VRage.Game.ModAPI.Ingame.IMyInventory.GetItemAmount	pass	-	MyInventory.GetItemAmount (MyInventory.cs:736): sums over m_items.
VRage.Game.ModAPI.Ingame.IMyInventory.GetItemAt	pass	-	IsItemAt + m_items[index].MakeAPIItem (MyInventory.cs:2700).
VRage.Game.ModAPI.Ingame.IMyInventory.GetItemByID	pass	-	MyInventory.cs:2709 GetItemByID :1405 scans m_items, MakeAPIItem builds a new struct
VRage.Game.ModAPI.Ingame.IMyInventory.GetItems	pass	-	Iterates m_items, MakeAPIItem, filter, adds to the caller's list (MyInventory.cs:2724).
VRage.Game.ModAPI.Ingame.IMyInventory.IsConnectedTo	pass@Transfers	-	MyInventory.cs:2765 CanTransferTo: locked per inventory LRU, ComputeCanTransfer under the Pathfinding lock
VRage.Game.ModAPI.Ingame.IMyInventory.IsFull	pass	-	Compares m_currentVolume/m_currentMass with MaxVolume/MaxMass (MyInventory.cs:472).
VRage.Game.ModAPI.Ingame.IMyInventory.IsItemAt	pass	-	MyInventory.cs:1628 m_items.IsValidIndex
VRage.Game.ModAPI.Ingame.IMyInventory.ItemCount	pass	-	m_items.Count (MyInventory.cs:506).
VRage.Game.ModAPI.Ingame.IMyInventory.MaxVolume	pass	-	m_maxVolume times session multipliers (MyInventory.cs:347).
VRage.Game.ModAPI.Ingame.IMyInventory.Owner	pass	-	base.Entity as MyEntity (MyInventory.cs:430).
VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemFrom	standin@Transfers	-	See TransferItemTo
VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemTo	standin@Transfers	-	MyInventory.cs:2942 reachability on the worker for the game's result, the move by item id on main after the phase (TransferStandIn)
VRage.Game.ModAPI.Ingame.IMyInventory.VolumeFillFactor	pass	-	MyInventory.cs:416 CurrentVolume / MaxVolume, field reads
VRage.Game.ModAPI.Ingame.IMyInventoryItem.Amount	pass	pass	MyPhysicalInventoryItem.cs:149 struct field get/set; the interface boxes a copy, setter writes only the script's box
VRage.Game.ModAPI.Ingame.IMyInventoryItem.Content	pass	pass	MyPhysicalInventoryItem.cs:179 Content auto-property of the boxed struct copy; setter writes only the script's box
VRage.Game.ModAPI.Ingame.IMyInventoryItem.ItemId	pass	pass	MyPhysicalInventoryItem.cs:194 struct field get/set on the script's boxed copy
VRage.Game.ModAPI.Ingame.IMyInventoryItem.Scale	pass	pass	MyPhysicalInventoryItem.cs:164 struct field get/set on the script's boxed copy
VRage.Game.ModAPI.Ingame.IMyInventoryOwner.EntityId	pass	-	MyCargoContainer.cs:90 etc. base.EntityId field
VRage.Game.ModAPI.Ingame.IMyInventoryOwner.GetInventory	pass	-	MyCargoContainer.cs:173 this.GetInventory / MyEntityExtensions.GetInventory -> GetInventoryBase (as IMyEntity.GetInventory)
VRage.Game.ModAPI.Ingame.IMyInventoryOwner.HasInventory	pass	-	MyCargoContainer.cs:92 etc. base.HasInventory (as IMyEntity.HasInventory)
VRage.Game.ModAPI.Ingame.IMyInventoryOwner.InventoryCount	pass	-	MyCargoContainer.cs:88 etc. base.InventoryCount (as IMyEntity.InventoryCount)
VRage.Game.ModAPI.Ingame.IMyInventoryOwner.UseConveyorSystem	pass	main	Sync read in most impls; set throws NotImplementedException in MyLargeTurretBase, MyConveyorSorter, MyShipConnector, MyCharacter, so not replayed
VRage.Game.ModAPI.Ingame.IMySlimBlock.AccumulatedDamage	pass	-	MySlimBlock.cs:862 m_accumulatedDamage field
VRage.Game.ModAPI.Ingame.IMySlimBlock.BlockDefinition	pass	-	BlockDefinition.Id (MySlimBlock.cs:1114).
VRage.Game.ModAPI.Ingame.IMySlimBlock.BuildIntegrity	pass	-	m_componentStack.BuildIntegrity field (MySlimBlock.cs:957).
VRage.Game.ModAPI.Ingame.IMySlimBlock.BuildLevelRatio	pass	-	m_componentStack.BuildRatio (MySlimBlock.cs:955).
VRage.Game.ModAPI.Ingame.IMySlimBlock.ColorMaskHSV	pass	-	MySlimBlock.cs:910 m_colorMaskHSV field
VRage.Game.ModAPI.Ingame.IMySlimBlock.CubeGrid	pass	-	m_cubeGrid field (MySlimBlock.cs:884).
VRage.Game.ModAPI.Ingame.IMySlimBlock.CurrentDamage	pass	-	BuildIntegrity - Integrity (MySlimBlock.cs:969).
VRage.Game.ModAPI.Ingame.IMySlimBlock.DamageRatio	pass	-	2 - BuildIntegrity/MaxIntegrity (MySlimBlock.cs:971).
VRage.Game.ModAPI.Ingame.IMySlimBlock.FatBlock	pass	-	Auto-property FatBlock (MySlimBlock.cs:878).
VRage.Game.ModAPI.Ingame.IMySlimBlock.GetMissingComponents	pass	-	MySlimBlock.cs:1769 -> MyComponentStack.cs:386 definition + stockpile reads, writes only the caller's dictionary
VRage.Game.ModAPI.Ingame.IMySlimBlock.HasDeformation	pass	-	MySlimBlock.cs:987 MyGridSkeleton.IsDeformed :448 ConcurrentDictionary reads; scratch m_tempAffectedCubes is [ThreadStatic]
VRage.Game.ModAPI.Ingame.IMySlimBlock.IsDestroyed	pass	-	MySlimBlock.cs:961 m_componentStack.m_integrity compare (MyComponentStack.cs:67)
VRage.Game.ModAPI.Ingame.IMySlimBlock.IsFullIntegrity	pass	-	m_componentStack.IsFullIntegrity (MySlimBlock.cs:943).
VRage.Game.ModAPI.Ingame.IMySlimBlock.IsFullyDismounted	pass	-	MySlimBlock.cs:959 m_componentStack.m_integrity compare (MyComponentStack.cs:65)
VRage.Game.ModAPI.Ingame.IMySlimBlock.Mass	pass	-	MySlimBlock.cs:2770 GetMass: FatBlock.GetMass or BlockDefinition.Mass
VRage.Game.ModAPI.Ingame.IMySlimBlock.MaxDeformation	pass	-	MySlimBlock.cs:999 m_cachedMaxDeformation field
VRage.Game.ModAPI.Ingame.IMySlimBlock.MaxIntegrity	pass	-	Definition MaxIntegrity (MySlimBlock.cs:967).
VRage.Game.ModAPI.Ingame.IMySlimBlock.OwnerId	pass	-	MySlimBlock.cs:1028 FatBlock.OwnerId or grid ownership component lookup (Components.TryGet, no implementer in game)
VRage.Game.ModAPI.Ingame.IMySlimBlock.Position	pass	-	MySlimBlock.cs:1079 Position field
VRage.Game.ModAPI.Ingame.IMySlimBlock.ShowParts	pass	-	MySlimBlock.cs:939 auto-property
VRage.Game.ModAPI.Ingame.IMySlimBlock.SkinSubtypeId	pass	-	MySlimBlock.cs:815 SkinSubtypeId field
VRage.Game.ModAPI.Ingame.IMySlimBlock.StockpileAllocated	pass	-	MySlimBlock.cs:973 m_stockpile != null
VRage.Game.ModAPI.Ingame.IMySlimBlock.StockpileEmpty	pass	-	MySlimBlock.cs:975 m_stockpile.IsEmpty: m_items.Count (MyConstructionStockpile.cs:80)
VRage.Game.ModAPI.Ingame.MyInventoryItemExtension.GetDefinitionId	pass	-	MyInventoryItemExtension.cs:5 Content.GetObjectId / TypeId+SubtypeId reads, new MyDefinitionId
VRage.Game.ModAPI.Ingame.MyPhysicalInventoryItemExtensions_ModAPI.GetItemInfo	pass	-	MyDefinitionManager.GetPhysicalItemDefinition (dictionary reads; missing definitions only log via MyLog) and a new MyItemInfo.
VRage.Game.MyRelationsBetweenPlayerAndBlockExtensions.IsFriendly	pass	-	MyRelationsBetweenPlayerAndBlockExtensions.cs:5 pure enum comparison
VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_GasProperties..ctor	pass	-	MyObjectBuilder_GasProperties.cs:13 new object builder, field initializers only (MyObjectBuilder_DefinitionBase.cs:231)
VRage.MyTupleComparer..ctor	pass	-	MyTupleComparer.cs:8 stateless comparer, default ctor
VRage.ObjectBuilders.MyObjectBuilder_Base.SubtypeId	pass	-	MyObjectBuilder_Base.cs:83 m_subtypeId field
VRage.ObjectBuilders.MyObjectBuilder_Base.TypeId	pass	-	MyObjectBuilder_Base.cs:116 GetType() wrapped in a new MyObjectBuilderType
VRage.Stats.MyNameComparer.Compare	pass	-	MyNameComparer.cs:7 string CompareTo of the two keys
VRage.Stats.MyPriorityComparer.Compare	pass	-	MyPriorityComparer.cs:7 compares MyStat.Priority (m_priority field reads)";
}
