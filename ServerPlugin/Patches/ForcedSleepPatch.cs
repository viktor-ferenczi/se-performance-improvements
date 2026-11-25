// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Viktor Ferenczi
using System;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using VRageMath;

namespace ServerPlugin.Patches;

/// <summary>
/// ForcedSleepService, the game side: while a grid is held asleep the game's own wake calls are
/// skipped, and every control change releases it. See ForcedSleepService.cs for the rule.
/// </summary>
[HarmonyPatch]
public static class ForcedSleepPatch
{
    /// <summary>Control inputs are normalised (-1..1); smaller changes are float noise, not a pilot or AI.</summary>
    private const float ControlEpsilonSq = 1e-4f;

    // ---------------------------------------------------------------- held shut while asleep

    /// <summary>Dampeners against gravity, a seated controller, gravity changes, reactor switches.</summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MyCubeGrid), nameof(MyCubeGrid.ActivatePhysics))]
    private static bool ActivatePhysicsPrefix(MyCubeGrid __instance) =>
        !ForcedSleepService.IsSlept(__instance);

    /// <summary>A rotor or hinge whose motor has a non-zero target wakes base and top every frame.</summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MyMotorStator), "ActivatePhysics")]
    private static bool StatorActivatePrefix(MyMotorStator __instance) =>
        !ForcedSleepService.IsSlept(__instance.CubeGrid)
        && !ForcedSleepService.IsSlept(__instance.TopGrid);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MyMotorSuspension), "ActivatePhysics")]
    private static bool SuspensionActivatePrefix(MyMotorSuspension __instance) =>
        !ForcedSleepService.IsSlept(__instance.CubeGrid)
        && !ForcedSleepService.IsSlept(__instance.TopGrid);

    /// <summary>The thrust update: held constant controls would re-activate or push the body.</summary>
    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(MyThrusterBlockThrustComponent),
        nameof(MyThrusterBlockThrustComponent.UpdateBeforeSimulation),
        new[] { typeof(bool), typeof(VRage.Game.Entity.MyEntity) }
    )]
    private static bool ThrustUpdatePrefix(MyThrusterBlockThrustComponent __instance) =>
        !ForcedSleepService.IsSlept(__instance.CubeGrid);

    /// <summary>The gyro update: a constant override applies torque, which Havok treats as a wake.</summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MyGridGyroSystem), "Update")]
    private static bool GyroUpdatePrefix(MyGridGyroSystem __instance) =>
        !ForcedSleepService.IsSlept(__instance.Grid);

    /// <summary>
    /// The Havok-level hold: ANY managed Activate on a held body is skipped (and its caller counted), so
    /// a wake path not listed above cannot undo the sleep. Havok's own activation on contact is native
    /// and does not pass here - collisions still wake.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Havok.HkEntity), nameof(Havok.HkEntity.Activate))]
    private static bool HkActivatePrefix(Havok.HkEntity __instance) =>
        !ForcedSleepService.HoldActivation(__instance);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Havok.HkEntity), nameof(Havok.HkEntity.ActivateAsCriticalOperation))]
    private static bool HkActivateCriticalPrefix(Havok.HkEntity __instance) =>
        !ForcedSleepService.HoldActivation(__instance);

    // ---------------------------------------------------------------- control changes wake

    /// <summary>Pilot input from a cockpit or remote control.</summary>
    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(MyShipController),
        nameof(MyShipController.MoveAndRotate),
        new[] { typeof(Vector3), typeof(Vector2), typeof(float) }
    )]
    private static void MoveAndRotatePrefix(
        MyShipController __instance,
        Vector3 moveIndicator,
        Vector2 rotationIndicator,
        float rollIndicator
    )
    {
        if (
            moveIndicator.LengthSquared() > ControlEpsilonSq
            || rotationIndicator.LengthSquared() > ControlEpsilonSq
            || rollIndicator * rollIndicator > ControlEpsilonSq
        )
            ForcedSleepService.Wake(__instance.CubeGrid, "pilot-input");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MyCubeGrid), nameof(MyCubeGrid.EnableDampingInternal))]
    private static void DampenersPrefix(MyCubeGrid __instance, bool enableDampeners)
    {
        if (
            __instance.EntityThrustComponent != null
            && __instance.EntityThrustComponent.DampenersEnabled != enableDampeners
        )
            ForcedSleepService.Wake(__instance, "dampeners");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MyThrust), "ThrustOverrideValueChanged")]
    private static void ThrustOverridePostfix(MyThrust __instance) =>
        ForcedSleepService.Wake(__instance.CubeGrid, "thrust-override");

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MyGyro), "GyroOverrideChanged")]
    private static void GyroOverridePostfix(MyGyro __instance) =>
        ForcedSleepService.Wake(__instance.CubeGrid, "gyro-override");

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MyGyro), nameof(MyGyro.SetGyroTorque))]
    private static void GyroTorquePrefix(MyGyro __instance, Vector3 torque)
    {
        if ((torque - __instance.m_gyroOverrideVelocity.Value).LengthSquared() > ControlEpsilonSq)
            ForcedSleepService.Wake(__instance.CubeGrid, "gyro-override");
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(MyEntityThrustComponent),
        nameof(MyEntityThrustComponent.ControlThrust),
        MethodType.Setter
    )]
    private static void ControlThrustPrefix(MyEntityThrustComponent __instance, Vector3 value)
    {
        if ((value - __instance.m_controlThrust).LengthSquared() > ControlEpsilonSq)
            ForcedSleepService.Wake(__instance.Entity as MyCubeGrid, "control-thrust");
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(MyEntityThrustComponent),
        nameof(MyEntityThrustComponent.AutoPilotControlThrust),
        MethodType.Setter
    )]
    private static void AutopilotThrustPrefix(MyEntityThrustComponent __instance, Vector3 value)
    {
        if ((value - __instance.m_autoPilotControlThrust).LengthSquared() > ControlEpsilonSq)
            ForcedSleepService.Wake(__instance.Entity as MyCubeGrid, "autopilot-thrust");
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(MyGridGyroSystem),
        nameof(MyGridGyroSystem.ControlTorque),
        MethodType.Setter
    )]
    private static void ControlTorquePrefix(MyGridGyroSystem __instance, Vector3 value)
    {
        if ((value - __instance.m_controlTorque).LengthSquared() > ControlEpsilonSq)
            ForcedSleepService.Wake(__instance.Grid, "control-torque");
    }
}
