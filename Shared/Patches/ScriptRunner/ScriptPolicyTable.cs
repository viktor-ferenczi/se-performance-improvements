namespace Shared.Patches;

// The policy table of ScriptPolicy, one line per member, tab separated: the member's full name
// (namespace, type, member; generic types without their parameters), the policy of the getter
// or method, the policy of the setter ("-" for none) and a note. The policies are pass, queue
// and main, see MemberPolicy. A member missing here is main.
//
// It is code, not an embedded file, because the plugin loaders compile the plugin from its
// .cs files only. The table was made from the API members used by the scripts of the
// se-dev-script corpus, classified from the decompiled game code (SE-626); members the
// classification marked as needing a worker-safe stand-in are main until they have one.
internal static class ScriptPolicyTable
{
    public const string Text =
        @"Sandbox.Game.EntityComponents.MyResourceSinkComponent.AcceptedResources	pass	-	MyResourceSinkComponent.cs:168 new ListReader over m_resourceIds; read only
Sandbox.Game.EntityComponents.MyResourceSinkComponent.CurrentInputByType	pass	-	MyResourceSinkComponent.cs:454 m_dataPerType[GetTypeIndex] read; distributor writes it on main thread only
Sandbox.Game.EntityComponents.MyResourceSinkComponent.MaxRequiredInputByType	pass	-	MyResourceSinkComponent.cs:444 array read
Sandbox.Game.EntityComponents.MyResourceSinkComponent.RequiredInputByType	pass	-	MyResourceSinkComponent.cs:459 array read
Sandbox.Game.EntityComponents.MyResourceSourceComponent.CurrentOutputByType	pass	-	MyResourceSourceComponent.cs:226 array read (GetTypeIndex may throw KeyNotFound, read only)
Sandbox.Game.EntityComponents.MyResourceSourceComponent.MaxOutputByType	pass	-	MyResourceSourceComponent.cs:268 MaxOutputLimitedByCapacity: pure math on m_dataPerType
Sandbox.Game.EntityComponents.MyResourceSourceComponent.ResourceTypes	pass	-	MyResourceSourceComponent.cs:140 new ListReader over m_resourceIds
Sandbox.ModAPI.Ingame.IMyAssembler.CooperativeMode	pass	queue	MyAssembler.cs:356 get IsSlave=Sync read; set m_slave.Value (Sync<T>.Value set (network event))
Sandbox.ModAPI.Ingame.IMyAssembler.CurrentProgress	pass	-	MyAssembler.cs:222 Sync read
Sandbox.ModAPI.Ingame.IMyAssembler.Mode	pass	queue	MyAssembler.cs:340 get DisassembleEnabled field; set RequestDisassembleEnabled -> MyMultiplayer.RaiseEvent
Sandbox.ModAPI.Ingame.IMyAssembler.Repeating	pass	queue	MyAssembler.cs:368 get RepeatEnabled fields; set RequestRepeatEnabled -> RaiseEvent (MyAssembler.cs:1203)
Sandbox.ModAPI.Ingame.IMyAutopilotWaypoint.Matrix	pass	-	MyAutopilotWaypoint.cs:45 returns m_matrix
Sandbox.ModAPI.Ingame.IMyBatteryBlock.ChargeMode	pass	queue	MyBatteryBlock.cs:286 Sync read / m_chargeMode.Value set
Sandbox.ModAPI.Ingame.IMyBatteryBlock.CurrentInput	pass	-	MyBatteryBlock.cs:211 ResourceSink.CurrentInputByType array read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.CurrentStoredPower	pass	-	MyBatteryBlock.cs:174 SourceComp.RemainingCapacityByType array read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.MaxInput	pass	-	MyBatteryBlock.cs:223 ResourceSink.MaxRequiredInputByType array read
Sandbox.ModAPI.Ingame.IMyBatteryBlock.MaxStoredPower	pass	-	MyBatteryBlock.cs:157 field read
Sandbox.ModAPI.Ingame.IMyBeacon.Radius	pass	queue	MyBeacon.cs:161 get RadioBroadcaster.BroadcastRadius field; set rewrites global MyRadioBroadcasters tree (MyRadioBroadcaster.cs:80)
Sandbox.ModAPI.Ingame.IMyBlockGroup.GetBlocks	pass	-	MyBlockGroup.cs:54 iterates group-private HashSet, IsAccessibleForProgrammableBlock field; proxy must wrap list results and the collect callback args (raw blocks)
Sandbox.ModAPI.Ingame.IMyBlockGroup.GetBlocksOfType	pass	-	MyBlockGroup.cs:66/78 same as GetBlocks; proxy must wrap results and collect args
Sandbox.ModAPI.Ingame.IMyBlockGroup.Name	pass	-	MyBlockGroup.cs:19 Name.ToString() new string
Sandbox.ModAPI.Ingame.IMyBroadcastListener.Tag	pass	-	BroadcastListener.cs auto-property read
Sandbox.ModAPI.Ingame.IMyCameraBlock.AvailableScanRange	pass	-	lazy accumulation in the own camera block. Agent: MyCameraBlock.cs:138 getter lazily accumulates into m_availableScanRange/m_lastUpdateTime (write). Replace: return m_availableScanRange + (TotalGamePlayTimeInMilliseconds - m_lastUpdateTime)*RaycastTimeMultiplier when IsWorking&&EnableRaycast, without storing
Sandbox.ModAPI.Ingame.IMyCameraBlock.CanScan	pass	-	reads AvailableScanRange, see there. Agent: MyCameraBlock.cs:813-860 all overloads read AvailableScanRange (lazy write, see AvailableScanRange); replace with the non-storing range calc + same limit/CheckAngleLimits math
Sandbox.ModAPI.Ingame.IMyCameraBlock.EnableRaycast	pass	queue	MyCameraBlock.cs:224 get auto-property; set writes m_lastUpdateTime and ResourceSink.Update()
Sandbox.ModAPI.Ingame.IMyCameraBlock.IsActive	pass	-	MyCameraBlock.cs:249 m_activeViewers Sync read
Sandbox.ModAPI.Ingame.IMyCameraBlock.Raycast	main	-	MyCameraBlock.cs:709 writes AvailableScanRange (lazy + decrement), m_debugRenderComponent.SetRaycastInfo, voxel Storage.GetGeometry().Intersect; result via MyDetectedEntityInfoHelper.Create (lazy WorldAABB/BigOwners writes)
Sandbox.ModAPI.Ingame.IMyCameraBlock.RaycastTimeMultiplier	pass	-	MyCameraBlock.cs:245 definition read
Sandbox.ModAPI.Ingame.IMyCameraBlock.TimeUntilScan	pass	-	reads AvailableScanRange, see there. Agent: MyCameraBlock.cs:862 reads AvailableScanRange (lazy write); replace with non-storing range calc
Sandbox.ModAPI.Ingame.IMyCockpit.OxygenCapacity	pass	-	MyCockpit.cs:477 definition read
Sandbox.ModAPI.Ingame.IMyCockpit.OxygenFilledRatio	pass	-	MyCockpit.cs:459 OxygenFillLevel Sync read
Sandbox.ModAPI.Ingame.IMyConveyorSorter.DrainAll	pass	queue	MyConveyorSorter.cs:173 Sync read / m_drainAll.Value set
Sandbox.ModAPI.Ingame.IMyConveyorSorter.GetFilterList	pass	-	MyConveyorSorter.cs:810 reads m_inventoryConstraint sets, fills caller list with new filters
Sandbox.ModAPI.Ingame.IMyConveyorSorter.Mode	pass	-	MyConveyorSorter.cs:207 m_inventoryConstraint.IsWhitelist read
Sandbox.ModAPI.Ingame.IMyConveyorSorter.SetFilter	queue	-	MyConveyorSorter.cs:823 RaiseEvent DoSetupFilter; snapshot the items list at call time (script may reuse it)
Sandbox.ModAPI.Ingame.IMyDoor.CloseDoor	queue	-	MyDoor.cs:168/MyAdvancedDoor.cs:229/MyAirtightDoorGeneric.cs:132 -> ToggleDoor: SetOpenRequest RaiseEvent or Open Sync set
Sandbox.ModAPI.Ingame.IMyDoor.Open	pass	-	MyDoorBase.cs:67 m_open Sync read
Sandbox.ModAPI.Ingame.IMyDoor.OpenDoor	queue	-	MyDoor.cs:156/MyAdvancedDoor.cs:217/MyAirtightDoorGeneric.cs:120 -> ToggleDoor network write
Sandbox.ModAPI.Ingame.IMyDoor.OpenRatio	pass	-	MyDoor.cs:129 / MyAdvancedDoor.cs:168 (loop over list) / MyAirtightDoorGeneric.cs:106 field reads
Sandbox.ModAPI.Ingame.IMyDoor.Status	pass	-	MyDoor.cs:109 / MyAdvancedDoor.cs:101 / MyAirtightDoorGeneric.cs:84 field reads
Sandbox.ModAPI.Ingame.IMyDoor.ToggleDoor	queue	-	MyDoor.cs:180 SetOpenRequest RaiseEvent; MyAirtightDoorGeneric.cs:144 Open Sync set
Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.GetDetailedInfoWithoutRequiredInput	main	-	MyFarmPlotLogic.cs:885 UpdateDetailedInfo uses MyValueFormatter.GetFormatedFloat which mutates shared static m_numberFormatInfoHelper (MyValueFormatter.cs:101)
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.AlignToPGravity	pass	queue	MyFlightMovementBlock.cs:293 -> MyAutopilotComponent.IsAlignedToGravity Sync read/set
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.CollisionAvoidance	pass	queue	MyFlightMovementBlock.cs:217 get IsUsingCollisionAvoidanceForLogic fields; set Sync
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.CurrentWaypoint	pass	-	MyFlightMovementBlock.cs:312 returns m_currentWaypoint reference
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.MinimalAltitude	pass	queue	MyFlightMovementBlock.cs:255 MinHeightAboveTerrain Sync read/set
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.PrecisionMode	pass	queue	MyFlightMovementBlock.cs:274 IsDockingModeEnabled Sync read/set
Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.SpeedLimit	pass	queue	MyFlightMovementBlock.cs:236 Sync read; set Sync + stuck detection
Sandbox.ModAPI.Ingame.IMyFunctionalBlock.Enabled	pass	queue	MyFunctionalBlock.cs:151 m_enabled Sync read/set (MyShipDrill/MyShipToolBase overrides same plus m_isManuallyActivated check)
Sandbox.ModAPI.Ingame.IMyGasGenerator.AutoRefill	pass	queue	MyGasGenerator.cs:170 Sync read/set
Sandbox.ModAPI.Ingame.IMyGasTank.AutoRefillBottles	pass	queue	MyGasTank.cs:217 Sync read/set
Sandbox.ModAPI.Ingame.IMyGasTank.Capacity	pass	-	MyGasTank.cs:229 definition read
Sandbox.ModAPI.Ingame.IMyGasTank.FilledRatio	pass	-	MyGasTank.cs:233 double field read (gas tasks may write it; atomic on x64)
Sandbox.ModAPI.Ingame.IMyGasTank.Stockpile	pass	queue	MyGasTank.cs:205 get IsStockpiling; set ChangeStockpileMode RaiseEvent + UpdateEmissivity (MyGasTank.cs:1000)
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.CurrentInstructionCount	pass	-	the counter is per thread with FixScriptCounter. Agent: MyProgrammableBlock.cs:136 -> IlInjector InstructionCounterHandle reads process-wide static m_numInstructions (IlInjector.cs:87); with parallel scripts it must return the worker's own counter
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.LastRunTimeMs	pass	-	MyProgrammableBlock.cs:132 RuntimeInfo auto-property of this PB
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.LifetimeTicks	pass	-	MyProgrammableBlock.cs:162 GameplayFrameCounter - m_firstRunFrame
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.MaxInstructionCount	pass	-	the counter is per thread with FixScriptCounter. Agent: MyProgrammableBlock.cs:134 -> static IlInjector m_numMaxInstructions rewritten at every run start by any thread; return the per-run limit from the worker's counter
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.TimeSinceLastRun	pass	-	MyProgrammableBlock.cs:130 frame counter math, new TimeSpan
Sandbox.ModAPI.Ingame.IMyGridProgramRuntimeInfo.UpdateFrequency	pass	queue	MyProgrammableBlock.cs:144 -> MyIngameScriptComponent.cs:52 setter changes component NeedsUpdate (entity update list registration); proxy should return the pending value on later gets
Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.CanAccess	main	-	MyProgrammableBlock.cs:252/275 (MyGridTerminalWrapper); grid overload reads MyCubeGrid.BigOwners which lazily recalculates owners (MyCubeGrid.cs:3195) on possibly foreign grids; could become replace if BigOwners read only when !NeedRecalculateOwners
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
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.IsEndpointReachable	main	-	MyIntergridCommunicationContext.cs:207 -> IsConnectedTo writes m_connectionCache LRU, EvaluateConnectionTo uses static scratch m_broadcasters (line 158) and antenna system
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.Me	pass	-	MyIntergridCommunicationContext.cs ProgrammableBlock.EntityId
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.RegisterBroadcastListener	main	-	MyIntergridCommunicationContext.cs:248 writes m_broadcastListeners and global MyIGCSystemSessionComponent.RegisterBroadcastListener (line 193). Replace: if an active listener with the tag exists return it (read only); else create the BroadcastListener object and queue the global registration (delivery only happens on main in UpdateBeforeSimulation)
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.SendBroadcastMessage	queue	-	MyIntergridCommunicationContext.cs:226 enqueues into global MyIGCSystemSessionComponent queue; run BoxMessage type check at call time so the exception reaches the script
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.SendUnicastMessage	main	-	MyIntergridCommunicationContext.cs:232 returns bool from IsConnectedTo (LRU cache write, static m_broadcasters scratch) then enqueues globally
Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem.UnicastListener	pass	-	MyIntergridCommunicationContext.cs auto-property read
Sandbox.ModAPI.Ingame.IMyJumpDrive.CurrentStoredPower	pass	-	MyJumpDrive.cs:350 Sync read
Sandbox.ModAPI.Ingame.IMyJumpDrive.MaxStoredPower	pass	-	MyJumpDrive.cs:352 definition read
Sandbox.ModAPI.Ingame.IMyJumpDrive.Status	pass	-	MyJumpDrive.cs:408 IsJumping/IsWorking/m_storedPower reads
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.AIEnabled	pass	-	MyLargeTurretBase.cs:623 definition read
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.Azimuth	pass	queue	MyLargeTurretBase.cs:1211 get m_rotation; set SetManualAzimuth
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.Elevation	pass	queue	MyLargeTurretBase.cs:1199 get m_elevation; set SetManualElevation
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.EnableIdleRotation	pass	queue	MyLargeTurretBase.cs:663 Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.GetTargetedEntity	main	-	MyLargeTurretBase.cs:3708 -> MyDetectedEntityInfoHelper.Create: PositionComp.WorldAABB lazy recompute (MyPositionComponentBase.cs:105) and grid BigOwners lazy recalc (MyCubeGrid.cs:3195). Replace: copy of Create computing AABB from LocalAABB*WorldMatrix and BigOwners only when not dirty (else main); read Target once
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.HasTarget	pass	-	MyLargeTurretBase.cs:1191 m_targetingSystem.Target reference read (targeting tasks may swap it; atomic)
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.IsUnderControl	pass	-	MyLargeTurretBase.cs:1237 ControllerInfo.Controller read
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.Range	pass	queue	MyLargeTurretBase.cs:1241 ShootRangeSimple Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.ResetTargetingToDefault	queue	-	MyLargeTurretBase.cs:3703 RaiseEvent ResetTargetParams
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.SyncAzimuth	queue	-	MyLargeTurretBase.cs:3717 SyncRotationAndOrientation (network)
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetCharacters	pass	queue	MyLargeTurretBase.cs:895 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetLargeGrids	pass	queue	MyLargeTurretBase.cs:876 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetMeteors	pass	queue	MyLargeTurretBase.cs:819 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetMissiles	pass	queue	MyLargeTurretBase.cs:838 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetSmallGrids	pass	queue	MyLargeTurretBase.cs:857 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLargeTurretBase.TargetStations	pass	queue	MyLargeTurretBase.cs:914 TargetFlags Sync read/set
Sandbox.ModAPI.Ingame.IMyLaserAntenna.Connect	queue	-	MyLaserAntenna.cs:2042 ConnectToGps (state change/network)
Sandbox.ModAPI.Ingame.IMyLaserAntenna.SetTargetCoords	queue	-	MyLaserAntenna.cs:2034 PasteCoordinates (network)
Sandbox.ModAPI.Ingame.IMyLaserAntenna.Status	pass	-	MyLaserAntenna.cs:367 m_outsideLimits/m_state reads
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkIntervalSeconds	pass	queue	MyLightingBlock.cs:338 MyLightingLogic m_sync read; set Sync
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkLength	pass	queue	MyLightingBlock.cs:355 Sync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.BlinkOffset	pass	queue	MyLightingBlock.cs:372 Sync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.Color	pass	queue	MyLightingBlock.cs:389 MyLightingLogic m_color read; set LightColorSync
Sandbox.ModAPI.Ingame.IMyLightingBlock.Falloff	pass	queue	MyLightingBlock.cs:321 LightFalloffSync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.Intensity	pass	queue	MyLightingBlock.cs:304 IntensitySync read/set
Sandbox.ModAPI.Ingame.IMyLightingBlock.Radius	pass	queue	MyLightingBlock.cs:283 m_radius/m_reflectorRadius read; set LightRadiusSync
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.Attach	queue	-	MyMechanicalConnectionBlockBase.cs:1516 writes m_lastAttachmentChangeRequestFrame and m_connectionState Sync
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.Detach	queue	-	MyMechanicalConnectionBlockBase.cs:1527 same as Attach
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.IsAttached	pass	-	MyMechanicalConnectionBlockBase.cs:246 field read
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.Top	pass	-	MyMechanicalConnectionBlockBase.cs:240 m_topBlock read
Sandbox.ModAPI.Ingame.IMyMechanicalConnectionBlock.TopGrid	pass	-	MyMechanicalConnectionBlockBase.cs:228 m_topBlock.CubeGrid read
Sandbox.ModAPI.Ingame.IMyMessageProvider.AcceptMessage	main	-	MyMessageListener.cs:83 dequeues the PB-private m_pendingMessages (written only by main in MyIGCSystemSessionComponent.UpdateBeforeSimulation); BroadcastListener.cs:27 may DisposeBroadcastListener when inactive+empty. Replace: dequeue on worker, queue the dispose step
Sandbox.ModAPI.Ingame.IMyMessageProvider.HasPendingMessage	pass	-	MyMessageListener.cs Queue.Count read
Sandbox.ModAPI.Ingame.IMyMessageProvider.SetMessageCallback	queue	-	MyMessageListener.cs:92 writes m_callback; BroadcastListener.cs:18 throws if inactive, null throws: check at call time
Sandbox.ModAPI.Ingame.IMyMotorStator.Angle	pass	-	MyMotorStator.cs:336 m_currentAngle read
Sandbox.ModAPI.Ingame.IMyMotorStator.BrakingTorque	pass	queue	MyMotorStator.cs:351 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.LowerLimitDeg	pass	queue	MyMotorStator.cs:406 Sync read; set SetSafeAngles (Sync writes, ScaleUp/Down)
Sandbox.ModAPI.Ingame.IMyMotorStator.LowerLimitRad	pass	queue	MyMotorStator.cs:390 Sync read; set SetSafeAngles
Sandbox.ModAPI.Ingame.IMyMotorStator.RotorLock	pass	queue	MyMotorStator.cs:482 m_rotorLock Sync + CubeGrid.ForceLockMotors read; set Sync
Sandbox.ModAPI.Ingame.IMyMotorStator.TargetVelocityRPM	pass	queue	MyMotorStator.cs:377 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.TargetVelocityRad	pass	queue	MyMotorStator.cs:364 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.Torque	pass	queue	MyMotorStator.cs:338 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorStator.UpperLimitDeg	pass	queue	MyMotorStator.cs:438 Sync read; set SetSafeAngles
Sandbox.ModAPI.Ingame.IMyMotorStator.UpperLimitRad	pass	queue	MyMotorStator.cs:422 Sync read; set SetSafeAngles
Sandbox.ModAPI.Ingame.IMyMotorSuspension.AirShockEnabled	pass	queue	MyMotorSuspension.cs:588 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Brake	pass	queue	MyMotorSuspension.cs:746 BrakingEnabled Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Friction	pass	queue	MyMotorSuspension.cs:696 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Height	pass	queue	MyMotorSuspension.cs:720 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.InvertPropulsion	pass	queue	MyMotorSuspension.cs:521 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.InvertSteer	pass	queue	MyMotorSuspension.cs:509 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.MaxSteerAngle	pass	queue	MyMotorSuspension.cs:734 Sync read; set Sync + OnPerFrameUpdatePropertyChanged
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Power	pass	queue	MyMotorSuspension.cs:708 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.PropulsionOverride	pass	queue	MyMotorSuspension.cs:624 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Steering	pass	queue	MyMotorSuspension.cs:564 Sync read/set
Sandbox.ModAPI.Ingame.IMyMotorSuspension.SteeringOverride	pass	queue	MyMotorSuspension.cs:636 Sync read; set writes m_steeringChanged + Sync
Sandbox.ModAPI.Ingame.IMyMotorSuspension.Strength	pass	queue	MyMotorSuspension.cs:684 Sync read (sqrt); set Sync
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.SearchEnemyComponent	pass	-	MyOffensiveCombatBlock.cs:194 field read
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.SelectedAttackPattern	pass	queue	MyOffensiveCombatBlock.cs:198 Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.TargetPriority	pass	queue	MyOffensiveCombatBlock.cs:222 Sync read/set
Sandbox.ModAPI.Ingame.IMyOffensiveCombatBlock.UpdateTargetInterval	pass	queue	MyOffensiveCombatBlock.cs:210 Sync read/set
Sandbox.ModAPI.Ingame.IMyPistonBase.CurrentPosition	pass	-	MyPistonBase.cs:229 field read
Sandbox.ModAPI.Ingame.IMyPistonBase.HighestPosition	pass	-	MyPistonBase.cs:323 definition read
Sandbox.ModAPI.Ingame.IMyPistonBase.LowestPosition	pass	-	MyPistonBase.cs:321 BlockDefinition.Minimum read
Sandbox.ModAPI.Ingame.IMyPistonBase.MaxLimit	pass	queue	MyPistonBase.cs:305 Sync<float> read; setter clamps and sets Sync (network)
Sandbox.ModAPI.Ingame.IMyPistonBase.MaxVelocity	pass	-	MyPistonBase.cs:287 definition read
Sandbox.ModAPI.Ingame.IMyPistonBase.MinLimit	pass	queue	MyPistonBase.cs:289 Sync<float> read; setter sets Sync (network)
Sandbox.ModAPI.Ingame.IMyPistonBase.Status	pass	-	MyPistonBase.cs:231 compares Velocity/limits with m_currentPos, read only
Sandbox.ModAPI.Ingame.IMyPistonBase.Velocity	pass	queue	MyPistonBase.cs:271 Sync<float> read; setter clamps and sets Sync (network)
Sandbox.ModAPI.Ingame.IMyPowerProducer.CurrentOutput	pass	-	MyFueledPowerProducer.cs:94, MyBatteryBlock.cs:187, MyEnvironmentalPowerProducer.cs:59 -> MyResourceSourceComponent.CurrentOutputByType array read
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
Sandbox.ModAPI.Ingame.IMyProductionBlock.NextItemId	main	-	MyProductionBlock.cs:243 getter is m_nextItemId++ (writes block state and returns it)
Sandbox.ModAPI.Ingame.IMyProductionBlock.OutputInventory	pass	-	MyProductionBlock.cs:194/285 returns m_outputInventory
Sandbox.ModAPI.Ingame.IMyProductionBlock.RemoveQueueItem	queue	-	MyProductionBlock.cs:1137 -> RemoveQueueItemRequest :546 RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyProductionBlock.UseConveyorSystem	pass	queue	MyProductionBlock.cs:245 Sync<bool> read; setter sets Sync
Sandbox.ModAPI.Ingame.IMyProgrammableBlock.TerminalRunArgument	pass	-	MyProgrammableBlock.cs:472 returns m_terminalRunArgument (setter private)
Sandbox.ModAPI.Ingame.IMyProgrammableBlock.TryRun	main	-	MyProgrammableBlock.cs:1431 executes another PB script synchronously (ExecuteCode, SetDetailedInfo), returns bool
Sandbox.ModAPI.Ingame.IMyProjector.RemainingBlocks	pass	-	MyProjectorBase.cs:838 int field m_remainingBlocks
Sandbox.ModAPI.Ingame.IMyProjector.TotalBlocks	pass	-	MyProjectorBase.cs:840 int field m_totalBlocks
Sandbox.ModAPI.Ingame.IMyRadioAntenna.EnableBroadcasting	pass	queue	MyRadioAntenna.cs:224 Sync<bool> read; setter sets Sync (network, ValueChanged handlers)
Sandbox.ModAPI.Ingame.IMyRadioAntenna.HudText	pass	queue	MyRadioAntenna.cs:210 HudText.ToString(); setter SetHudText :243 CompareUpdate + RaiseEvent
Sandbox.ModAPI.Ingame.IMyRadioAntenna.IsBroadcasting	pass	-	MyRadioAntenna.cs:791 RadioBroadcaster.WantsToBeEnabled read (Components.Get dictionary lookup)
Sandbox.ModAPI.Ingame.IMyRadioAntenna.Radius	pass	queue	MyRadioAntenna.cs:198/284 RadioBroadcaster.BroadcastRadius field; setter sets m_radius Sync
Sandbox.ModAPI.Ingame.IMyReactor.UseConveyorSystem	pass	queue	MyReactor.cs:101/87 Sync<bool>
Sandbox.ModAPI.Ingame.IMyRemoteControl.AddWaypoint	queue	-	MyRemoteControl.cs:1042/1047 -> MyAutopilotComponent.AddWaypoint :3613 RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.ClearWaypoints	queue	-	MyRemoteControl.cs:915 -> MyAutopilotComponent.ClearWaypoints :3589 RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyRemoteControl.FlightMode	pass	queue	MyRemoteControl.cs:308 -> MyAutopilotComponent.CurrentFlightMode :771 Sync read/set
Sandbox.ModAPI.Ingame.IMyRemoteControl.GetNearestPlayer	main	-	MyRemoteControl.cs:899/938 reads base.WorldMatrix of the RC and of the player's controlled entity (a cockpit block of any grid): block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags); returns bool
Sandbox.ModAPI.Ingame.IMyRemoteControl.GetWaypointInfo	pass	-	MyRemoteControl.cs:1029 copies m_autopilotComponent.Waypoints into caller list (new MyWaypointInfo)
Sandbox.ModAPI.Ingame.IMyRemoteControl.IsAutoPilotEnabled	pass	-	MyRemoteControl.cs:270 AutoPilotEnabled Sync read
Sandbox.ModAPI.Ingame.IMyRemoteControl.SetAutoPilotEnabled	queue	-	MyRemoteControl.cs:884 -> MyAutopilotComponent.SetAutoPilotEnabled :3003 ClearMovementControl + Sync set; void
Sandbox.ModAPI.Ingame.IMySearchEnemyComponent.FoundEnemyId	pass	-	MySearchEnemyComponent.cs:173/107 server path returns m_foundEnemy (read only); client path caches m_foundEnemy but PBs execute only on the server
Sandbox.ModAPI.Ingame.IMySearchEnemyComponent.SubsystemsToDestroy	pass	-	MySearchEnemyComponent.cs:187 Sync read; setter sets Sync
Sandbox.ModAPI.Ingame.IMySearchEnemyComponent.TargetingLockOptions	pass	queue	MySearchEnemyComponent.cs:175 Sync read; setter sets Sync
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
Sandbox.ModAPI.Ingame.IMySensorBlock.LastDetectedEntity	main	-	MySensorBlock.cs:793 builds MyDetectedEntityInfoHelper.Create (MyDetectedEntityInfoHelper.cs:22) on read: PositionComp.WorldAABB/WorldMatrix of the detected entity (any grid/character): block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags); forageables may load models
Sandbox.ModAPI.Ingame.IMySensorBlock.LeftExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.RightExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMySensorBlock.TopExtend	pass	queue	MySensorBlock.cs:461-560 reads m_fieldMin/m_fieldMax Sync; setter clamps then FieldMin/FieldMax Sync set + ResourceSink update
Sandbox.ModAPI.Ingame.IMyShipConnector.CollectAll	pass	queue	MyShipConnector.cs:545 Sync<bool>
Sandbox.ModAPI.Ingame.IMyShipConnector.Connect	queue	-	MyShipConnector.cs:2513 TryConnect (constraints, events); void
Sandbox.ModAPI.Ingame.IMyShipConnector.Disconnect	queue	-	MyShipConnector.cs:2521 TryDisconnect; void
Sandbox.ModAPI.Ingame.IMyShipConnector.OtherConnector	pass	-	MyShipConnector.cs:615 returns m_other (proxy must wrap it)
Sandbox.ModAPI.Ingame.IMyShipConnector.Status	pass	-	MyShipConnector.cs:599 reads m_connected, IsWorking, m_constraint
Sandbox.ModAPI.Ingame.IMyShipConnector.ThrowOut	pass	queue	MyShipConnector.cs:533 Sync<bool>
Sandbox.ModAPI.Ingame.IMyShipConnector.ToggleConnect	queue	-	MyShipConnector.cs:2529 TrySwitch; void
Sandbox.ModAPI.Ingame.IMyShipController.CalculateShipMass	pass	-	MyShipController.cs:3245 -> MyCubeGrid.GetCurrentMass :5557: group nodes into [ThreadStatic] m_tmpGroupNodes, MyGridShape mass fields, pilot/inventory mass reads
Sandbox.ModAPI.Ingame.IMyShipController.CanControlShip	pass	-	MyShipController.cs:864 m_enableShipControl
Sandbox.ModAPI.Ingame.IMyShipController.CenterOfMass	pass	-	MyShipController.cs:787 Physics.CenterOfMassWorld (Havok read + Offset)
Sandbox.ModAPI.Ingame.IMyShipController.ControlThrusters	pass	queue	MyShipController.cs:738 Sync read; setter checks IsMainCockpitFree then Sync set
Sandbox.ModAPI.Ingame.IMyShipController.ControlWheels	pass	queue	MyShipController.cs:723 Sync read; setter checks then Sync set
Sandbox.ModAPI.Ingame.IMyShipController.DampenersOverride	pass	queue	MyShipController.cs:768 grid MyEntityThrustComponent.DampenersEnabled auto-prop; setter CubeGrid.EnableDampingInternal
Sandbox.ModAPI.Ingame.IMyShipController.GetArtificialGravity	main	-	MyShipController.cs:3221 base.WorldMatrix of the controller block plus generator blocks' WorldMatrix/WorldAABB (MyGravityGenerator.cs:275/289, MyGravityGeneratorSphere.cs:256/267, possibly other grids): block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags). Tree query itself is thread-safe ([ThreadStatic] collector/stack, rw lock). Replace: position = block LocalMatrix x grid WorldMatrixRef (grid top-level, no lazy) and pre-warm generator WorldAABB on main before the phase, then call CalculateArtificialGravityInPoint
Sandbox.ModAPI.Ingame.IMyShipController.GetNaturalGravity	pass	-	planet gravity is thread-safe; the controller's lazy matrix is own-group. Agent: MyShipController.cs:3216 MyGravityProviderSystem.CalculateNaturalGravityInPoint is thread-safe (shared lock, pure planet math) but base.WorldMatrix of the controller block: block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags). Replace: translation = Vector3D.Transform(PositionComp.LocalMatrixRef.Translation, CubeGrid.PositionComp.WorldMatrixRef), then call it
Sandbox.ModAPI.Ingame.IMyShipController.GetShipSpeed	pass	-	MyShipController.cs:3231 Parent.Physics.LinearVelocity (Havok read)
Sandbox.ModAPI.Ingame.IMyShipController.GetShipVelocities	pass	-	MyShipController.cs:3237 Parent.Physics linear/angular velocity (Havok read)
Sandbox.ModAPI.Ingame.IMyShipController.GetTotalGravity	main	-	MyShipController.cs:3226 natural + artificial; same lazy block WorldMatrix and generator WorldAABB/WorldMatrix issue as GetArtificialGravity. Replace as GetArtificialGravity (non-lazy position, pre-warmed generators) then CalculateTotalGravityInPoint
Sandbox.ModAPI.Ingame.IMyShipController.HandBrake	pass	queue	MyShipController.cs:753 GridSystems.WheelSystem.HandBrake field; setter re-checks state and calls SwitchHandbrake (toggle), fine applied in order
Sandbox.ModAPI.Ingame.IMyShipController.HasWheels	pass	-	MyShipController.cs:866 WheelSystem.m_wheels.Count
Sandbox.ModAPI.Ingame.IMyShipController.IsMainCockpit	pass	-	MyShipController.cs:706 Sync read
Sandbox.ModAPI.Ingame.IMyShipController.IsUnderControl	pass	-	MyShipController.cs:721 ControllerInfo.Controller != null
Sandbox.ModAPI.Ingame.IMyShipController.MoveIndicator	pass	-	MyShipController.cs:883 auto-property read
Sandbox.ModAPI.Ingame.IMyShipController.RollIndicator	pass	-	MyShipController.cs:887 auto-property read
Sandbox.ModAPI.Ingame.IMyShipController.RotationIndicator	pass	-	MyShipController.cs:885 auto-property read
Sandbox.ModAPI.Ingame.IMyShipController.TryGetPlanetElevation	pass	-	as GetNaturalGravity. Agent: MyShipController.cs:3253 PositionComp.GetPosition and WorldAABB of the controller block: block WorldMatrix/GetPosition/WorldAABB lazily recompute (MyPositionComponentBase.RecalculateWorldMatrixHRIfNeeded :442, WorldAABB :105 write m_worldMatrix/counters/dirty flags). GetClosestPlanet ([ThreadStatic] list, locked AABB tree), planet GetPosition (top-level) and ProjectToSurface (cacheless heightmap) are read only. Replace: compute world matrix/AABB as LocalMatrix x grid WorldMatrixRef without caching, rest unchanged
Sandbox.ModAPI.Ingame.IMyShipController.TryGetPlanetPosition	pass	-	as GetNaturalGravity. Agent: MyShipController.cs:3284 same lazy GetPosition/WorldAABB of the controller block as TryGetPlanetElevation; same replacement
Sandbox.ModAPI.Ingame.IMyShipToolBase.IsActivated	pass	-	MyShipToolBase.cs:182 m_isActivated; MyShipDrill.cs:277 m_wantsToDrill
Sandbox.ModAPI.Ingame.IMyShipToolBase.UseConveyorSystem	pass	queue	MyShipToolBase.cs:170/144, MyShipDrill.cs:265/223 Sync<bool>
Sandbox.ModAPI.Ingame.IMyShipWelder.HelpOthers	pass	pass	MyShipWelder.cs:65 getter returns false, setter is empty
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomData	pass	queue	MyTerminalBlock.cs:290 Storage.TryGetValue read; setter SetCustomData_Internal :543 may create Storage component, RaiseCustomDataChanged network event
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomInfo	pass	-	MyTerminalBlock.cs:433 CustomInfo.ToString()
Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomName	pass	queue	lazy default name of the own block; DisplayNameText of the shared definition is made thread-safe by ScriptRunner. Agent: MyTerminalBlock.cs:417/252 getter lazily fills m_defaultCustomName via LoadDefaultCustomName :1364 when the block has no custom name (common for unnamed blocks). Replace: m_customName?.ToString() ?? (m_defaultCustomName.Length>0 ? it.ToString() : BlockDefinition.DisplayNameText + (NumberInGrid>1 ? "" ""+NumberInGrid : """")) without caching. Setter SetCustomName :668 events + RaiseEvent
Sandbox.ModAPI.Ingame.IMyTerminalBlock.DetailedInfo	main	-	MyTerminalBlock.cs:431/393 if m_detailedInfoDirty runs virtual UpdateDetailedInfo into shared m_detailedInfo (many overrides, MyTexts etc.). Could be replace only with a main-thread pre-phase refresh of dirty blocks
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetActionWithName	main	-	MyTerminalBlock.cs:1403 -> helper :67 uses shared static m_actionList. Replace with a local list from MyTerminalControlFactory.GetActions(GetType(), list) and the same Id/IsValidForToolbarType(ButtonPanel) match
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetActions	main	-	MyTerminalBlock.cs:1393 -> MyTerminalControlFactoryHelper.Static shared static m_actionList scratch (MyTerminalControlFactoryHelper.cs:33). Replace: MyTerminalControlFactory.GetActions(GetType(), local list) (read of static m_controls; lazy InitializeControls only for unregistered types) then filter like the helper
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetProperties	main	-	MyTerminalBlock.cs:1413 -> helper :105 uses shared static m_valueControls. Replace with local list from MyTerminalControlFactory.GetValueControls(GetType(), list)
Sandbox.ModAPI.Ingame.IMyTerminalBlock.GetProperty	main	-	MyTerminalBlock.cs:1408 -> helper :86 uses shared static m_valueControls. Replace with local list from MyTerminalControlFactory.GetValueControls(GetType(), list) and Id match
Sandbox.ModAPI.Ingame.IMyTerminalBlock.IsSameConstructAs	pass	-	MyTerminalBlock.cs:1418 -> MyCubeGrid.IsSameConstructAs :16421 MyCubeGridGroups.Mechanical.GetGroup dictionary reads
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowInInventory	pass	-	MyTerminalBlock.cs:338 Sync<bool> read; setter Sync + event
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowInTerminal	pass	-	MyTerminalBlock.cs:322 Sync<bool> read; setter Sync + event
Sandbox.ModAPI.Ingame.IMyTerminalBlock.ShowOnHUD	pass	queue	MyTerminalBlock.cs:306 Sync<bool> read; setter Sync + RaiseShowOnHUDChanged
Sandbox.ModAPI.Ingame.IMyTextPanel.GetPublicTitle	pass	-	MyTextPanel.cs:1269 m_publicTitleHelper.ToString()
Sandbox.ModAPI.Ingame.IMyTextPanel.WritePublicTitle	queue	-	as WriteText. Agent: MyTextPanel.cs:1274 writes m_publicTitleHelper and RaiseEvent (SendChangeTitleMessage), returns !m_isTextPanelOpen. Replace: result = !m_isTextPanelOpen (read), queue the real call
Sandbox.ModAPI.Ingame.IMyTextSurface.AddImageToSelection	queue	-	MyTextPanel.cs:1351 / MyTextPanelComponent.cs:2171 reads lists then AddImagesToSelectionRequest RaiseEvent + OnSelectedTextureChanged; void
Sandbox.ModAPI.Ingame.IMyTextSurface.Alignment	pass	queue	MyTextPanel.cs:452 / MyTextPanelComponent.cs:970/780 m_fontData Sync read; setter Sync set + NeedsUpdate
Sandbox.ModAPI.Ingame.IMyTextSurface.BackgroundAlpha	pass	queue	MyTextPanel.cs:391 / MyTextPanelComponent.cs:709 m_contentData Sync read; setter Sync + m_backgroundChanged + UpdateIsStaticContent
Sandbox.ModAPI.Ingame.IMyTextSurface.BackgroundColor	pass	queue	MyTextPanel.cs:376 / MyTextPanelComponent.cs:690 m_contentData Sync read; setter Sync + flags
Sandbox.ModAPI.Ingame.IMyTextSurface.ClearImagesFromSelection	queue	-	MyTextPanel.cs:1491 / MyTextPanelComponent.cs:2316 RemoveImagesFromSelectionRequest RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyTextSurface.ContentType	pass	queue	MyTextPanel.cs:486/167 / MyTextPanelComponent.cs:994/615 m_contentData Sync read; setter Sync + UpdateIsStaticContent
Sandbox.ModAPI.Ingame.IMyTextSurface.CurrentlyShownImage	pass	-	MyTextPanel.cs:421 / MyTextPanelComponent.cs:939 reads SelectedTexturesToDraw and m_currentSelectedTexture
Sandbox.ModAPI.Ingame.IMyTextSurface.DisplayName	pass	-	MyTextPanel.cs:564 / MyTextPanelComponent.cs:1060 m_displayName
Sandbox.ModAPI.Ingame.IMyTextSurface.DrawFrame	main	-	MyTextPanelComponent.cs:1951 frame (MySpriteDrawFrame, VRage.Game) is a struct the proxy cannot intercept; its Dispose calls DispatchSprites :1956 writing m_renderLayers/m_spriteQueue/m_areSpritesDirty. PoolManager is concurrent. Replace: return new MySpriteDrawFrame(cb) whose cb copies frame.ToCollection() and queues the real DrawFrame()+AddRange+Dispose on main
Sandbox.ModAPI.Ingame.IMyTextSurface.Font	pass	queue	MyTextPanel.cs:437 / MyTextPanelComponent.cs:955/745 new MyDefinitionId(type, name) -> MyStringHash.GetOrCompute (internally locked, thread-safe); setter definition lookup + Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.FontColor	pass	queue	MyTextPanel.cs:361 / MyTextPanelComponent.cs:728 m_fontData Sync read; setter Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.FontSize	pass	queue	MyTextPanel.cs:346 / MyTextPanelComponent.cs:762 m_fontData Sync read; setter rounds + Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.GetSprites	pass	-	MyTextPanel.cs:1537 / MyTextPanelComponent.cs:2360 copies m_definitions subtype names to caller list
Sandbox.ModAPI.Ingame.IMyTextSurface.GetText	pass	-	MyTextPanel.cs:1322 m_publicDescription.ToString(); MyTextPanelComponent.cs:2138 m_textHelper.ToString() (sees queued WriteText only after apply)
Sandbox.ModAPI.Ingame.IMyTextSurface.MeasureStringInPixels	pass	-	MyTextPanel.cs:1552 / MyTextPanelComponent.cs:2378 -> MyFontDefinition.MeasureStringRaw: m_fontsById lookup, MyFont.MeasureString glyph/kern dictionary reads; MyStringHash.GetOrCompute is lock-protected (adds entry only for unknown font names)
Sandbox.ModAPI.Ingame.IMyTextSurface.PreserveAspectRatio	pass	queue	MyTextPanel.cs:502 / MyTextPanelComponent.cs:895 m_contentData Sync; setter Sync + UpdateIsStaticContent
Sandbox.ModAPI.Ingame.IMyTextSurface.ReadText	pass	-	MyTextPanel.cs:1342 appends m_publicDescription; MyTextPanelComponent.cs:2162 appends Text, into caller buffer
Sandbox.ModAPI.Ingame.IMyTextSurface.RemoveImageFromSelection	queue	-	MyTextPanel.cs:1418 / MyTextPanelComponent.cs:2243 RemoveImagesFromSelectionRequest RaiseEvent; void
Sandbox.ModAPI.Ingame.IMyTextSurface.Script	pass	queue	MyTextPanel.cs:467 / MyTextPanelComponent.cs:982/798 m_scriptData Sync read; setter SetScriptData + NeedsUpdate
Sandbox.ModAPI.Ingame.IMyTextSurface.ScriptBackgroundColor	pass	queue	MyTextPanel.cs:532 / MyTextPanelComponent.cs:835 m_scriptData Sync read; setter SetScriptData + flags
Sandbox.ModAPI.Ingame.IMyTextSurface.ScriptForegroundColor	pass	queue	MyTextPanel.cs:547 / MyTextPanelComponent.cs:854 m_scriptData Sync read; setter SetScriptData
Sandbox.ModAPI.Ingame.IMyTextSurface.SurfaceSize	pass	-	MyTextPanel.cs:498 / MyTextPanelComponent.cs:1006 m_textureSize x static pure CalcAspectFactor (MyRenderComponentScreenAreas.cs:148)
Sandbox.ModAPI.Ingame.IMyTextSurface.TextPadding	pass	queue	MyTextPanel.cs:517 / MyTextPanelComponent.cs:913 m_contentData Sync read; setter rounds + Sync
Sandbox.ModAPI.Ingame.IMyTextSurface.TextureSize	pass	-	MyTextPanel.cs:500 / MyTextPanelComponent.cs:1008 m_textureSize
Sandbox.ModAPI.Ingame.IMyTextSurface.WriteText	queue	-	queued; returns true, the game returns false only while a player edits the panel text. Agent: MyTextPanel.cs:1303/1327 writes m_publicDescriptionHelper + m_descriptionPublicDirty, returns !m_isTextPanelOpen; MyTextPanelComponent.cs:2123/2143 writes m_textHelper + m_textHelperDirty, returns true. Replace: compute the result from those reads, copy the string/StringBuilder, queue the real call in order
Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider.GetSurface	pass	-	MyTextPanel.cs:1562 PanelComponent; MyFunctionalBlock.cs:612, MyCockpit.cs:2847, MyProjectorBase.cs:2507 -> MyMultiTextPanelComponent.GetSurface :863 m_panels[index] (proxy must wrap the MyTextPanelComponent)
Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider.SurfaceCount	pass	-	MyTextPanel.cs:566 constant 1; MyFunctionalBlock.cs:163, MyCockpit.cs:619, MyProjectorBase.cs:871 -> m_panels.Count
Sandbox.ModAPI.Ingame.IMyThrust.CurrentThrust	pass	-	MyThrust.cs:374 m_currentStrength x definition x multiplier
Sandbox.ModAPI.Ingame.IMyThrust.GridThrustDirection	pass	-	MyThrust.cs:378/276 reads session controlled entity / grid ControlSystem.CurrentShipController and block Orientation
Sandbox.ModAPI.Ingame.IMyThrust.MaxEffectiveThrust	pass	-	MyThrust.cs:362 -> MyEntityThrustComponent.GetLastThrustMultiplier :2173 -> MyThrusterBlockThrustComponent.CalculateForceMultiplier :378 field/definition math
Sandbox.ModAPI.Ingame.IMyThrust.MaxThrust	pass	-	MyThrust.cs:360 definition x m_thrustMultiplier
Sandbox.ModAPI.Ingame.IMyThrust.ThrustOverride	pass	queue	MyThrust.cs:294/195 Sync x multiplier x definition; setter converts + Sync
Sandbox.ModAPI.Ingame.IMyThrust.ThrustOverridePercentage	pass	queue	MyThrust.cs:306 Sync read; setter clamps + Sync
Sandbox.ModAPI.Ingame.IMyUserControllableGun.IsShooting	pass	-	MyUserControllableGun.cs:79 Sync<bool> m_isShooting
Sandbox.ModAPI.Ingame.IMyUserControllableGun.Shoot	pass	queue	MyUserControllableGun.cs:81 m_forceShoot read; setter SetShooting
Sandbox.ModAPI.Ingame.IMyUserControllableGun.ShootOnce	queue	-	MyUserControllableGun.cs:305 OnShootOncePressed; void
Sandbox.ModAPI.Ingame.IMyWarhead.Detonate	queue	-	MyWarhead.cs:644/386 Explode (damage, entity removal); void
Sandbox.ModAPI.Ingame.IMyWarhead.DetonationTime	pass	queue	MyWarhead.cs:197/183 m_countdownMs Sync; setter Sync
Sandbox.ModAPI.Ingame.IMyWarhead.IsArmed	pass	queue	MyWarhead.cs:165 Sync<bool>; setter Sync + emissive handler
Sandbox.ModAPI.Ingame.IMyWarhead.IsCountingDown	pass	-	MyWarhead.cs:195/143 auto-property
Sandbox.ModAPI.Ingame.IMyWarhead.StartCountdown	main	-	MyWarhead.cs:338 writes IsCountingDown, MyWarheads.AddWarhead (static list), events; returns bool
Sandbox.ModAPI.Ingame.MyGridProgram.Echo	pass	pass	Getter returns the delegate set in MyProgrammableBlock.CreateInstance (MyProgrammableBlock.cs:1195). Invoking the default delegate EchoTextToDetailInfo (:1217) only appends to this PB's own m_echoOutput, which only this PB's run touches (read back after the run in RunSandboxedProgramActionCore). Setter is the script assigning its own MyGridProgram field.
Sandbox.ModAPI.Ingame.MyGridProgram.GridTerminalSystem	pass	-	Auto-property on the script instance, set to the PB's MyGridTerminalWrapper by RunSandboxedProgramAction (MyProgrammableBlock.cs:819) and nulled after the run. Reading it writes nothing; the returned wrapper must be proxied.
Sandbox.ModAPI.Ingame.MyGridProgram.IGC	main	-	MyGridProgram.IGC calls m_IGC_ContextGetter, a lambda (MyProgrammableBlock.cs:1198) that lazily caches a context from MyIGCSystemSessionComponent.GetOrMakeContextFor (MyIGCSystemSessionComponent.cs:171), which adds to the shared m_perPBCommContexts Dictionary. Replace: call GetOrMakeContextFor for each PB on main before the phase and set IMyGridProgram.IGC_ContextGetter to return the cached (proxied) context; then the getter is a plain read.
Sandbox.ModAPI.Ingame.MyGridProgram.Me	pass	pass	Auto-property set to the PB in CreateInstance (MyProgrammableBlock.cs:1194). Read only; setter is the script assigning its own field (no game effect).
Sandbox.ModAPI.Ingame.MyGridProgram.Runtime	pass	-	Auto-property holding the PB's RuntimeInfo (set in CreateInstance, MyProgrammableBlock.cs:1191). Getter writes nothing; RuntimeInfo members are classified separately.
Sandbox.ModAPI.Ingame.MyGridProgram.Storage	pass	pass	MyGridProgram.cs:55 reads/writes the script instance's own m_storage. Only read by the PB on main (save, MyProgrammableBlock.cs:698/1108), which cannot run during the phase.
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.ApplyAction	queue	-	block.GetActionWithName(name).Apply(block[, params]). The lookup MyTerminalControlFactoryHelper.GetActionWithName (MyTerminalControlFactoryHelper.cs:67) uses the shared static scratch list m_actionList; Apply writes. Queue the whole call (copy the parameter list). An unknown action name then throws on main, not in the script; to keep that, validate on the worker by scanning MyTerminalControlFactory.GetActions(block.GetType()) (read-only UniqueListReader) and throw NullReferenceException there.
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.GetId	pass	-	Returns block.EntityId (MyEntity.m_entityId field).
Sandbox.ModAPI.Ingame.TerminalBlockExtentions.HasAction	main	-	Calls GetActionWithName, which uses the shared static scratch list MyTerminalControlFactoryHelper.m_actionList (MyTerminalControlFactoryHelper.cs:73). Would be pass with a worker-safe lookup over MyTerminalControlFactory.GetActions(type) (same helper as for ApplyAction validation).
Sandbox.ModAPI.Interfaces.ITerminalAction.Apply	queue	-	MyTerminalAction<TBlock>.Apply (MyTerminalAction.cs:244/253) runs the action delegate (Sync writes, RaiseEvent). Void; queue and copy the ListReader<TerminalActionParameter> argument.
Sandbox.ModAPI.Interfaces.ITerminalAction.Id	pass	-	MyTerminalAction.Id returns readonly m_id (MyTerminalAction.cs:80).
Sandbox.ModAPI.Interfaces.ITerminalAction.Name	pass	-	Returns the action's shared m_name StringBuilder (MyTerminalAction.cs:84); a read, but the script gets a shared mutable object (same as on main).
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetMaximum	main	-	Abstract per control type: MyTerminalControlCombobox.GetMaximum (MyTerminalControlCombobox.cs:280) fills the shared m_handlerItems scratch list; slider calls per-control Denormalizer delegates (not all reviewed). Checkbox/OnOff/Color/Textbox return constants.
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetMinimum	main	-	Same as GetMaximum: combobox uses shared m_handlerItems scratch (MyTerminalControlCombobox.cs:265), slider Denormalizer delegates unreviewed.
Sandbox.ModAPI.Interfaces.ITerminalProperty.GetValue	main	-	MyTerminalValueControl.GetValue (MyTerminalValueControl.cs:79) invokes the control's Getter delegate: hundreds of game lambdas plus mod-added controls, not all reviewed (some return shared StringBuilders or compute). Replace: pass only for an allowlist of reviewed (block type, property Id) getters that read Sync/fields (e.g. OnOff), main for the rest.
Sandbox.ModAPI.Interfaces.ITerminalProperty.Id	pass	-	MyTerminalValueControl: returns the control's Id field.
Sandbox.ModAPI.Interfaces.ITerminalProperty.SetValue	queue	-	MyTerminalValueControl.SetValue (MyTerminalValueControl.cs:84) calls Setter (Sync writes) and block.NotifyTerminalValueChanged; slider/color/combobox overrides clamp first. Void, queue.
Sandbox.ModAPI.Interfaces.ITerminalProperty.TypeName	pass	-	MyTerminalValueControl: typeof(TValue).Name.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.As	pass	-	property as ITerminalProperty<TValue>; pure type test.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.AsBool	pass	-	property as ITerminalProperty<bool>; pure type test (proxy must implement the generic interface).
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.AsColor	pass	-	property as ITerminalProperty<Color>; pure type test.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.AsFloat	pass	-	property as ITerminalProperty<float>; pure type test.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.Cast	pass	-	As<TValue>() plus an exception built from property.Id/TypeName; reads only.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetMaximum	main	-	block.GetProperty(id) uses the shared scratch list MyTerminalControlFactoryHelper.m_valueControls (MyTerminalControlFactoryHelper.cs:92); then control GetMaximum (combobox shared scratch, slider delegates).
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetMinimum	main	-	Same as GetMaximum extension: GetProperty shared scratch m_valueControls plus control GetMinimum.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValue	main	-	block.GetProperty(id) uses the shared static scratch list MyTerminalControlFactoryHelper.m_valueControls (MyTerminalControlFactoryHelper.cs:86-102), then the control's Getter delegate. Replace: look the property up by scanning MyTerminalControlFactory.GetControls(block.GetType()) (read-only; fall back to main if AreControlsCreated(type) is false), throw the same InvalidOperationException as Cast, then pass for an allowlist of reviewed (block type, Id) getters and main otherwise.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueBool	main	-	GetValue<bool>: see GetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueFloat	main	-	GetValue<float>: see GetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValue	queue	-	GetProperty(id) (shared scratch) + Cast + MyTerminalValueControl.SetValue (Sync writes, NotifyTerminalValueChanged). Void; queue the whole call. Unknown id or wrong type then throws on main instead of in the script; validate on the worker with a read-only GetControls scan if that matters.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueBool	queue	-	SetValue<bool>: see SetValue extension.
Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueFloat	queue	-	SetValue<float>: see SetValue extension.
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.CanPressurize	pass	-	lazy matrices of the own vent and grid. Agent: MyAirVent.CanPressurize (MyAirVent.cs:265) -> GetOxygenBlock (:1015): reads base.WorldMatrix of the vent (block: lazy RecalculateWorldMatrixHRIfNeeded writes, MyPositionComponentBase.cs:442) and CubeGrid.WorldToGridInteger (grid WorldMatrixNormalizedInv lazy cache, :294). Replace: same math with non-caching matrices (LocalMatrixRef * grid WorldMatrixRef, fresh inverse), then GasSystem.GetOxygenBlockAt and Room.IsAirtight (reads; breathing data is a ConcurrentDictionary).
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.Depressurize	pass	queue	Getter reads m_isDepressurizing Sync (MyAirVent.cs:224); setter writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.GetOxygenLevel	pass	-	lazy matrices of the own vent and grid. Agent: MyAirVent.GetOxygenLevel (MyAirVent.cs:1101) uses GetOxygenBlock (:1015) with the same lazy vent WorldMatrix and grid WorldMatrixNormalizedInv writes as CanPressurize. Replace with the non-caching matrix version, then MyOxygenBlock.OxygenLevel/Room.EnvironmentOxygen (reads).
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.PressurizationEnabled	pass	-	MySession.Static.Settings.EnableOxygenPressurization (MyAirVent.cs:320).
SpaceEngineers.Game.ModAPI.Ingame.IMyAirVent.Status	pass	-	Auto-property Status (MyAirVent.cs:236), written only by UpdateStatus on main.
SpaceEngineers.Game.ModAPI.Ingame.IMyGravityGeneratorBase.GravityAcceleration	pass	queue	MyGravityGeneratorBase.cs:89: getter reads m_gravityAcceleration Sync; setter clamps and writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.AutoLock	pass	queue	MyLandingGear.AutoLock (MyLandingGear.cs:375): Sync read; setter writes m_autoLock Sync.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.IsLocked	pass	-	LockMode == Locked (MyLandingGear.cs:328), field read.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.Lock	queue	-	RequestLandingGearLock -> RequestLock -> MyMultiplayer.RaiseEvent AttachRequest (MyLandingGear.cs:597/1534). Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.LockMode	pass	-	Returns m_lockMode field (MyLandingGear.cs:307).
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.ToggleLock	queue	-	RequestLandingGearSwitch (MyLandingGear.cs:585) -> RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear.Unlock	queue	-	RequestLandingGearUnlock -> RequestLock -> RaiseEvent (MyLandingGear.cs:605). Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyOxygenFarm.GetOutput	pass	-	MyOxygenFarm.cs:306: IsWorking ? SolarComponent.MaxOutput : 0; field reads.
SpaceEngineers.Game.ModAPI.Ingame.IMyShipMergeBlock.IsConnected	pass	-	MyShipMergeBlock.IsLocked: m_mergeState Sync read (MyShipMergeBlock.cs:160).
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.IsSoundSelected	pass	-	!string.IsNullOrEmpty(m_cueIdString) (MySoundBlock.cs:192).
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.LoopPeriod	pass	queue	MySoundBlock.LoopPeriod (MySoundBlock.cs:166): Sync read; setter writes m_loopPeriod Sync.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.Play	queue	-	RequestPlaySound -> MyMultiplayer.RaiseEvent PlaySound (MySoundBlock.cs:481). Void.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.SelectedSound	pass	queue	Getter returns m_cueIdString (MySoundBlock.cs:256); setter SelectSound writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMySoundBlock.Stop	queue	-	RequestStopSound (MySoundBlock.cs:569): RaiseEvent plus StopSoundInternal on the sound emitter. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.IsCountingDown	pass	-	Private auto-property (MyTimerBlock.cs:119).
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.StartCountdown	queue	-	StartBtn (MyTimerBlock.cs:576): RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.StopCountdown	queue	-	StopBtn (MyTimerBlock.cs:581): RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.Trigger	queue	-	OnTrigger (MyTimerBlock.cs:487): RaiseEvent Trigger. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTimerBlock.TriggerDelay	pass	queue	MyTimerBlock.cs:133: getter rounds m_timerSync; setter writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AIEnabled	pass	queue	m_allowAI Sync read/write (MyTurretControlBlock.cs:841).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AddTool	queue	-	MyTurretControlBlock.AddTool (MyTurretControlBlock.cs:3939) -> SyncToolSelection RaiseEvent. Void; unwrap the proxied tool argument when applying.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AngleDeviation	pass	queue	MyTurretControlBlock.cs:638: ToDegrees(m_angleDeviation) read; setter writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.AzimuthRotor	pass	queue	Getter returns m_azimuthor (MyTurretControlBlock.cs:1186); setter writes m_boundAzimuth Sync after IsInSameLogicalGroupAs (unwrap the proxied rotor).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.Camera	pass	queue	Getter returns m_camera (MyTurretControlBlock.cs:1224); setter writes m_boundCamera Sync (unwrap the proxied camera).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.ClearTools	queue	-	MyTurretControlBlock.cs:3955: SyncToolUnselection RaiseEvent. Void.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.ElevationRotor	pass	queue	Getter returns m_elevator (MyTurretControlBlock.cs:1205); setter writes m_boundElevation Sync (unwrap the proxied rotor).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetDirectionSource	pass	-	Returns m_directionBlock (MyTurretControlBlock.cs:3973).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetShootDirection	main	-	ShootDirection (MyTurretControlBlock.cs:1044) reads block WorldMatrix (lazy HR recompute writes) or IMyGunObject.GetShootDirection of the bound weapon (several implementations, unreviewed).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetTargetedEntity	main	-	MyDetectedEntityInfoHelper.Create(Target, ...) (MyTurretControlBlock.cs:3844): Target is usually an entity of another grid group; reads PositionComp.WorldAABB (lazy dirty-flag write on a foreign entity another worker may touch), BigOwners, Physics. Replace possible with a non-caching AABB/matrix copy of Create.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.GetTools	pass	-	Clears the script's list and adds values from m_boundTools (MyTurretControlBlock.cs:3883); reads game state, writes only the caller's list.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.HasTarget	pass	-	m_targetingSystem.Target != null (MyTurretControlBlock.cs:1184); a read (targeting may update it in its own background task, as in vanilla).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.IsSunTrackerEnabled	pass	-	MySunTrackingComponent.m_isSunTrackerEnabled read (MyTurretControlBlock.cs:853). Setter (RaiseEvent) unused by scripts.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.IsUnderControl	pass	-	ControllerInfo.Controller != null (MyTurretControlBlock.cs:1170).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.MoveIndicator	pass	-	Auto-property (MyTurretControlBlock.cs:1245).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.Range	pass	queue	ShootRangeSimple (MyTurretControlBlock.cs:1122): m_shootingRange Sync read; setter clamps and writes the Sync value.
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.RollIndicator	pass	-	Auto-property (MyTurretControlBlock.cs:1247).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.RotationIndicator	pass	-	Auto-property (MyTurretControlBlock.cs:1243).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.VelocityMultiplierAzimuthRpm	pass	-	Sync read times GetAzimuthSpeedMax (rotor MaxRotorAngularVelocity from MyGridPhysics settings) (MyTurretControlBlock.cs:785).
SpaceEngineers.Game.ModAPI.Ingame.IMyTurretControlBlock.VelocityMultiplierElevationRpm	pass	-	Sync read times GetElevatorSpeedMax (MyTurretControlBlock.cs:797).
VRage.Game.Components.Interfaces.IMyComponentContainer.Get	pass	-	MyComponentContainer.Get<T>: Dictionary TryGetValue on m_components. Returned components (e.g. MyResourceSinkComponent) are classes, not ingame interfaces: their members are not intercepted by proxies.
VRage.Game.Components.Interfaces.IMyComponentContainer.TryGet	pass	-	MyComponentContainer.TryGet<T>: m_components lookup plus MyComponentTypeFactory.TryGetComponentTypeFromModApiInterface (static dict read). Same caveat: returned component classes (MyResourceSinkComponent etc.) bypass the proxies.
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.Add	pass	-	Struct method, not interceptable: appends to the frame's own pooled MyList (m_sprites from PoolManager in the ctor), no shared state (MySpriteDrawFrame.cs:45).
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.AddRange	pass	-	Struct method: m_sprites.AddRange of the script's enumerable into the frame's own list (MySpriteDrawFrame.cs:50).
VRage.Game.GUI.TextPanel.MySpriteDrawFrame.Dispose	main	-	Struct method, not interceptable: invokes the submit callback, for LCDs MyTextPanelComponent.DispatchSprites (MyTextPanelComponent.cs:1956), which writes m_renderLayers, m_spriteQueue, m_areSpritesDirty; then returns the list to PoolManager (concurrent pool). Replace in the IMyTextSurface.DrawFrame proxy: return new MySpriteDrawFrame(cb) where cb snapshots the sprites (ToCollection) and queues on main: real DrawFrame(), AddRange(snapshot), Dispose(). Submit order per surface is kept.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.BlockDefinition	pass	-	SlimBlock.BlockDefinition.Id (MyCubeBlock.cs:371).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.CubeGrid	pass	-	SlimBlock.CubeGrid field (MyCubeBlock.cs:302/375).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.DefinitionDisplayNameText	pass	-	DisplayNameText of the shared definition is made thread-safe by ScriptRunner. Agent: BlockDefinition.DisplayNameText; MyCubeBlockDefinition.DisplayNameText (MyCubeBlockDefinition.cs:416) for definitions with DisplayNameVariant lazily creates and reuses the shared m_displayNameTextCache StringBuilder (definition shared by all blocks of that type in all groups). Replace: DisplayNameVariant == null ? base text : base text + "" "" + MyTexts.GetString(variant), no shared builder.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.DisplayNameText	pass	-	MyTerminalBlock.DisplayNameText = CustomName.ToString() (MyTerminalBlock.cs:276), new string; other blocks use the MyEntity auto-property.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.GetOwnerFactionTag	pass	-	MyCubeBlock.GetOwnerFactionTag (MyCubeBlock.cs:443): IDModule.Owner and MyFactionCollection.GetPlayerFaction (two Dictionary reads).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.IsBeingHacked	main	-	MyTerminalBlock.IsBeingHacked (MyTerminalBlock.cs:370) writes m_isBeingHackedPrevValue and raises IsBeingHackedChanged when the value flips.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.IsFunctional	pass	-	SlimBlock.ComponentStack.IsFunctional: integrity field comparisons (MyComponentStack.cs:131).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.IsWorking	pass	-	Auto-property IsWorking (MyCubeBlock.cs:327).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Mass	pass	-	GetMass() = BlockDefinition.Mass (MyCubeBlock.cs:1722); MySpaceBall override reads VirtualMass.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Max	pass	-	SlimBlock.Max field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Min	pass	-	SlimBlock.Min field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.NumberInGrid	pass	-	Auto-property (MyCubeBlock.cs:319).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Orientation	pass	-	SlimBlock.Orientation field.
VRage.Game.ModAPI.Ingame.IMyCubeBlock.OwnerId	pass	-	IDModule?.Owner (MyCubeBlock.cs:266).
VRage.Game.ModAPI.Ingame.IMyCubeBlock.Position	pass	-	SlimBlock.Position field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.CubeExists	pass	-	m_cubes.ContainsKey (MyCubeGrid.cs:8737).
VRage.Game.ModAPI.Ingame.IMyCubeGrid.CustomName	pass	queue	Getter returns DisplayName field; setter calls ChangeDisplayNameRequest (network event) (MyCubeGrid.cs:3652).
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GetCubeBlock	pass	-	m_cubes.TryGetValue plus FatBlock/IsAccessibleForProgrammableBlock field reads (MyCubeGrid.cs:7933/16382).
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GridIntegerToWorld	pass	-	Transform with base.WorldMatrix (MyCubeGrid.cs:6051); grids have no position parent, so RecalculateWorldMatrixHRIfNeeded returns without writing.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GridSize	pass	-	Auto-property GridSize.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.GridSizeEnum	pass	-	m_gridSizeEnum field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.IsSameConstructAs	pass	-	MyCubeGridGroups.Static.Mechanical.GetGroup x2 (MyGroups.GetGroup: Dictionary read) (MyCubeGrid.cs:16421). Casts the argument to MyCubeGrid: unwrap the proxy.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.IsStatic	pass	-	m_isStatic field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.Max	pass	-	m_max field.
VRage.Game.ModAPI.Ingame.IMyCubeGrid.Min	pass	-	m_min field.
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
VRage.Game.ModAPI.Ingame.IMyEntity.WorldMatrix	pass	-	lazy world matrix of an own-group entity; the grid does not move during the phase. Agent: PositionComp.WorldMatrixRef (MyPositionComponentBase.cs:58) -> RecalculateWorldMatrixHRIfNeeded (:442) lazily writes m_worldMatrix/counter/dirty flags for blocks. Root entities (grids) read without writes. Replace: child -> LocalMatrixRef * parent world matrix computed without caching.
VRage.Game.ModAPI.Ingame.IMyEntity.WorldVolume	pass	-	lazy volume of an own-group entity. Agent: PositionComp.WorldVolume (MyPositionComponentBase.cs:132) lazily recomputes m_worldVolume and clears its dirty flag (also for grids). Replace: center = Transform(LocalVolume.Center, non-caching world matrix), radius = LocalVolume.Radius.
VRage.Game.ModAPI.Ingame.IMyInventory.CanItemsBeAdded	pass	-	MyInventory.CanItemsBeAdded (MyInventory.cs:619): MyInventoryItemAdapter.Static is [ThreadStatic], definition lookups, Constraint.Check (HashSet reads).
VRage.Game.ModAPI.Ingame.IMyInventory.CanTransferItemTo	main	-	CanTransferTo (MyInventory.cs:2904) writes the per-inventory LRU m_connectionCache and runs MyGridConveyorSystem.ComputeCanTransfer pathfinding. Unwrap the proxied dst.
VRage.Game.ModAPI.Ingame.IMyInventory.CurrentMass	pass	-	m_currentMass + ExternalMass fields (MyInventory.cs:428).
VRage.Game.ModAPI.Ingame.IMyInventory.CurrentVolume	pass	-	m_currentVolume field (MyInventory.cs:414).
VRage.Game.ModAPI.Ingame.IMyInventory.FindItem	pass	-	FindFirstPositionOfType scans m_items; MakeAPIItem builds a new MyInventoryItem (MyInventory.cs:2714/754).
VRage.Game.ModAPI.Ingame.IMyInventory.GetAcceptedItems	pass	-	Reads Constraint ids or MyDefinitionManager.GetInventoryItemDefinitions (thread-safe Lazy); writes only the caller's list (MyInventory.cs:2783).
VRage.Game.ModAPI.Ingame.IMyInventory.GetItemAmount	pass	-	MyInventory.GetItemAmount (MyInventory.cs:736): sums over m_items.
VRage.Game.ModAPI.Ingame.IMyInventory.GetItemAt	pass	-	IsItemAt + m_items[index].MakeAPIItem (MyInventory.cs:2700).
VRage.Game.ModAPI.Ingame.IMyInventory.GetItems	pass	-	Iterates m_items, MakeAPIItem, filter, adds to the caller's list (MyInventory.cs:2724).
VRage.Game.ModAPI.Ingame.IMyInventory.IsConnectedTo	main	-	CanTransferTo(dst, null) (MyInventory.cs:2765/2904): LRU m_connectionCache write and conveyor pathfinding. Unwrap the proxied dst.
VRage.Game.ModAPI.Ingame.IMyInventory.IsFull	pass	-	Compares m_currentVolume/m_currentMass with MaxVolume/MaxMass (MyInventory.cs:472).
VRage.Game.ModAPI.Ingame.IMyInventory.ItemCount	pass	-	m_items.Count (MyInventory.cs:506).
VRage.Game.ModAPI.Ingame.IMyInventory.MaxVolume	pass	-	m_maxVolume times session multipliers (MyInventory.cs:347).
VRage.Game.ModAPI.Ingame.IMyInventory.Owner	pass	-	base.Entity as MyEntity (MyInventory.cs:430).
VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemTo	main	-	Returns bool; TransferItemsTo/From (MyInventory.cs:2933) checks CanTransferTo (LRU cache write, conveyor pathfinding) then Transfer (moves items or sends a network request).
VRage.Game.ModAPI.Ingame.IMySlimBlock.BlockDefinition	pass	-	BlockDefinition.Id (MySlimBlock.cs:1114).
VRage.Game.ModAPI.Ingame.IMySlimBlock.BuildIntegrity	pass	-	m_componentStack.BuildIntegrity field (MySlimBlock.cs:957).
VRage.Game.ModAPI.Ingame.IMySlimBlock.BuildLevelRatio	pass	-	m_componentStack.BuildRatio (MySlimBlock.cs:955).
VRage.Game.ModAPI.Ingame.IMySlimBlock.CubeGrid	pass	-	m_cubeGrid field (MySlimBlock.cs:884).
VRage.Game.ModAPI.Ingame.IMySlimBlock.CurrentDamage	pass	-	BuildIntegrity - Integrity (MySlimBlock.cs:969).
VRage.Game.ModAPI.Ingame.IMySlimBlock.DamageRatio	pass	-	2 - BuildIntegrity/MaxIntegrity (MySlimBlock.cs:971).
VRage.Game.ModAPI.Ingame.IMySlimBlock.FatBlock	pass	-	Auto-property FatBlock (MySlimBlock.cs:878).
VRage.Game.ModAPI.Ingame.IMySlimBlock.IsFullIntegrity	pass	-	m_componentStack.IsFullIntegrity (MySlimBlock.cs:943).
VRage.Game.ModAPI.Ingame.IMySlimBlock.MaxIntegrity	pass	-	Definition MaxIntegrity (MySlimBlock.cs:967).
VRage.Game.ModAPI.Ingame.MyPhysicalInventoryItemExtensions_ModAPI.GetItemInfo	pass	-	MyDefinitionManager.GetPhysicalItemDefinition (dictionary reads; missing definitions only log via MyLog) and a new MyItemInfo.
";
}
