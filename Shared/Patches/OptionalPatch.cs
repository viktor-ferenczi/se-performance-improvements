using System;
using System.Linq;
using HarmonyLib;
using Shared.Plugin;

namespace Shared.Patches
{
    // Applies a patch class only while its option is on.
    //
    // Harmony replaces a patched method with a copy compiled once, outside tiered compilation
    // and dynamic PGO. That copy can be slower than the game's own method even when the patch
    // returns at once because its option is off: 27% slower on the PB ownership walk (SE-634).
    // So patches on hot paths are not applied at all while their option is off.
    //
    // The patch class returns Prepare() from its own Prepare method, so PatchAll applies it at
    // startup only if wanted. Update() is called on every update and applies or removes the
    // class when the option changes at runtime. Harmony never puts the game's method back: after
    // the removal the game runs an unpatched copy, which behaves like the original, but its speed
    // comes back only with a restart.
    public sealed class OptionalPatch
    {
        private readonly Type patchClass;
        private readonly Func<bool> wanted;
        private bool active;

        public OptionalPatch(Type patchClass, Func<bool> wanted)
        {
            this.patchClass = patchClass;
            this.wanted = wanted;
        }

        public bool Prepare()
        {
            active = wanted();
            return active;
        }

        public void Update()
        {
            var apply = wanted();
            if (apply == active)
                return;

            var harmony = new Harmony(patchClass.FullName);
            try
            {
                if (apply)
                    harmony.CreateClassProcessor(patchClass).Patch();
                else
                    Unpatch(harmony);
            }
            catch (Exception ex)
            {
                Common.Logger.Error(
                    ex,
                    $"Failed to {(apply ? "apply" : "remove")} {patchClass.Name}"
                );
            }

            // Also after a failure, so it is not retried on every update
            active = apply;
        }

        // Removes this class's patches whichever Harmony instance applied them: the plugin's at
        // startup, or this helper's on a later change
        private void Unpatch(Harmony harmony)
        {
            foreach (var original in Harmony.GetAllPatchedMethods().ToList())
            {
                var info = Harmony.GetPatchInfo(original);
                var patches = info
                    .Prefixes.Concat(info.Postfixes)
                    .Concat(info.Transpilers)
                    .Concat(info.Finalizers)
                    .Where(patch => patch.PatchMethod.DeclaringType == patchClass)
                    .ToList();

                foreach (var patch in patches)
                    harmony.Unpatch(original, patch.PatchMethod);
            }
        }
    }
}
