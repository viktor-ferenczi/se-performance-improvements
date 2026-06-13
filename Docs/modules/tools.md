# Shared Tools & Data Structures

The reusable toolbox the patches build on: time-bounded caches, reader/writer-locked collections, Harmony IL/transpiler helpers, IL verification, hashing, object pools and the assembly publicizer support types.

The `Shared/Tools` module is the foundation layer that every performance patch in the plugin builds on. It is divided into five theme groups:

**Caching** — [`Cache.cs`](../files/Shared/Tools/Cache.cs.md), [`TwoLayerCache.cs`](../files/Shared/Tools/TwoLayerCache.cs.md), and [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md) are the three generic cache primitives; each is a time-bounded key/value store backed by a [`RwLockDictionary.cs`](../files/Shared/Tools/RwLockDictionary.cs.md). `Cache` is the workhorse for most per-entity caches (safe zones, wind turbines, block-access rights). `TwoLayerCache` adds a lock-free immutable read path for look-up-heavy workloads. `UintCache` eliminates per-entry heap allocation by packing a `uint` value and expiry tick into a single `ulong`. [`CacheStat.cs`](../files/Shared/Tools/CacheStat.cs.md) and [`ConveyorStat.cs`](../files/Shared/Tools/ConveyorStat.cs.md) are companion statistics classes used in DEBUG builds to report hit rates and call frequencies.

**Concurrency** — [`RwLock.cs`](../files/Shared/Tools/RwLock.cs.md) implements a lightweight spin-based reader/writer lock over a plain `int` counter. [`RwLockDictionary.cs`](../files/Shared/Tools/RwLockDictionary.cs.md) and [`RwLockHashSet.cs`](../files/Shared/Tools/RwLockHashSet.cs.md) embed this counter directly into standard-library collection subclasses, keeping the lock co-located with the data for better cache locality.

**Harmony / IL patching helpers** — [`TranspilerHelpers.cs`](../files/Shared/Tools/TranspilerHelpers.cs.md) supplies the extension methods that all runtime Harmony transpilers use (instruction search, hash verification, debug IL recording, deep-clone). [`PreloaderHelpers.cs`](../files/Shared/Tools/PreloaderHelpers.cs.md) is its Mono.Cecil counterpart for preloader patches that rewrite assemblies before loading. [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) is a custom attribute and scanner that verifies the IL hash of every patched game method at plugin startup, yielding [`CodeChange.cs`](../files/Shared/Tools/CodeChange.cs.md) records when a game update alters a patched method. [`Hashing.cs`](../files/Shared/Tools/Hashing.cs.md) provides the underlying FNV-derived hash functions used by all three verifiers.

**Publicizer support** — [`GameAssembliesToPublicize.cs`](../files/Shared/Tools/GameAssembliesToPublicize.cs.md) declares the `[assembly: IgnoresAccessChecksTo("…")]` attributes that pair with the `<Publicize>` entries in the `.csproj` files, enabling direct access to internal game members without reflection. [`IgnoresAccessChecksToAttribute.cs`](../files/Shared/Tools/IgnoresAccessChecksToAttribute.cs.md) supplies the attribute class itself in production builds where Krafs.Publicizer does not inject it.

**Misc utilities** — [`MySessionExtensions.cs`](../files/Shared/Tools/MySessionExtensions.cs.md) wraps the publicized `MySession.m_updateAllowed` field; [`ObjectPools.cs`](../files/Shared/Tools/ObjectPools.cs.md) provides a process-wide `StringBuilder` pool to reduce GC pressure; [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) caps log-message frequency; [`Workarounds.cs`](../files/Shared/Tools/Workarounds.cs.md) patches missing framework APIs.

## Files

| File | Summary |
| --- | --- |
| [`Cache.cs`](../files/Shared/Tools/Cache.cs.md) | Generic time-bounded key/value cache guarded by a reader/writer lock with lazy expiry cleanup |
| [`CacheStat.cs`](../files/Shared/Tools/CacheStat.cs.md) | Lightweight hit-rate accumulator for DEBUG-mode cache diagnostics |
| [`CodeChange.cs`](../files/Shared/Tools/CodeChange.cs.md) | Data record describing an IL hash mismatch detected at startup by `EnsureCode.Verify()` |
| [`ConveyorStat.cs`](../files/Shared/Tools/ConveyorStat.cs.md) | Call-count and failure-rate accumulator for the conveyor reachability cache |
| [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) | Custom `[EnsureCode("hash")]` attribute that scans the plugin assembly at startup and reports patched methods whose IL has changed |
| [`GameAssembliesToPublicize.cs`](../files/Shared/Tools/GameAssembliesToPublicize.cs.md) | Assembly-attribute declarations listing every game assembly publicized by Krafs.Publicizer |
| [`Hashing.cs`](../files/Shared/Tools/Hashing.cs.md) | FNV-1a string hashing, IL-body fingerprinting for `MethodInfo`/`ConstructorInfo`, and a hash-code combiner |
| [`IgnoresAccessChecksToAttribute.cs`](../files/Shared/Tools/IgnoresAccessChecksToAttribute.cs.md) | Provides `IgnoresAccessChecksToAttribute` in production builds where Krafs.Publicizer does not inject it |
| [`MySessionExtensions.cs`](../files/Shared/Tools/MySessionExtensions.cs.md) | Extension methods exposing the internal `MySession.m_updateAllowed` field via publicized direct access |
| [`ObjectPools.cs`](../files/Shared/Tools/ObjectPools.cs.md) | Shared bucket-based `StringBuilder` pool for GC-pressure reduction in string-formatting hot paths |
| [`PreloaderHelpers.cs`](../files/Shared/Tools/PreloaderHelpers.cs.md) | Mono.Cecil helpers for preloader patches: instruction index search, hash verification, and debug IL recording |
| [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) | Token-bucket limiter that caps the number of log messages or other actions per reporting period |
| [`RwLock.cs`](../files/Shared/Tools/RwLock.cs.md) | Spin-based reader/writer lock over a plain `int` counter, with a convenience wrapper class |
| [`RwLockDictionary.cs`](../files/Shared/Tools/RwLockDictionary.cs.md) | `Dictionary<TKey, TValue>` subclass with embedded spin-based reader/writer locking |
| [`RwLockHashSet.cs`](../files/Shared/Tools/RwLockHashSet.cs.md) | `HashSet<T>` subclass with embedded spin-based reader/writer locking |
| [`TranspilerHelpers.cs`](../files/Shared/Tools/TranspilerHelpers.cs.md) | Harmony transpiler helpers: IL search, hash verification, debug IL recording, and deep-clone utilities |
| [`TwoLayerCache.cs`](../files/Shared/Tools/TwoLayerCache.cs.md) | Two-layer cache with a lock-free immutable read path and a synchronized mutable write path |
| [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md) | Allocation-free time-bounded cache that packs a `uint` value and expiry tick into a single `ulong` |
| [`Workarounds.cs`](../files/Shared/Tools/Workarounds.cs.md) | Shim extension methods compensating for missing game or .NET Framework APIs |

## How it fits together

**Concurrency layer is the foundation.** [`RwLock.cs`](../files/Shared/Tools/RwLock.cs.md) implements the spin-wait protocol; [`RwLockDictionary.cs`](../files/Shared/Tools/RwLockDictionary.cs.md) and [`RwLockHashSet.cs`](../files/Shared/Tools/RwLockHashSet.cs.md) embed a lock counter directly in standard-library collection subclasses. Every cache primitive depends on `RwLockDictionary` as its backing store.

**Caches sit on top.** [`Cache.cs`](../files/Shared/Tools/Cache.cs.md) is the default: it stores an `Item` object holding the value and expiry, and requires `Cleanup()` to be called every simulation tick so the clock advances and lazy eviction runs. [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md) is a compact alternative where the value is a `uint`; it eliminates the `Item` allocation by packing expiry and value into a single `ulong` dictionary entry. [`TwoLayerCache.cs`](../files/Shared/Tools/TwoLayerCache.cs.md) avoids locking on the hot read path by maintaining a snapshot `IReadOnlyDictionary` (the immutable layer) promoted from the [`RwLockDictionary.cs`](../files/Shared/Tools/RwLockDictionary.cs.md) mutable layer by periodic calls to `FillImmutableCache()`. [`CacheStat.cs`](../files/Shared/Tools/CacheStat.cs.md) and [`ConveyorStat.cs`](../files/Shared/Tools/ConveyorStat.cs.md) are statistics sinks associated with caches in DEBUG builds; they do not affect the cache data path.

**IL-verification pipeline.** The hashing subsystem underpins all patch safety checks. [`Hashing.cs`](../files/Shared/Tools/Hashing.cs.md) provides `HashBody()` (Harmony `MethodInfo`/`ConstructorInfo`) and `HashInstructions()` (both Harmony `CodeInstruction` and Mono.Cecil `Instruction`). [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) is a `[Method]`-level attribute that records expected hashes at development time; its static `Verify()` scans the plugin assembly at startup and emits [`CodeChange.cs`](../files/Shared/Tools/CodeChange.cs.md) records for any mismatch. [`TranspilerHelpers.cs`](../files/Shared/Tools/TranspilerHelpers.cs.md) and [`PreloaderHelpers.cs`](../files/Shared/Tools/PreloaderHelpers.cs.md) expose the same `VerifyCodeHash` helper from within individual transpiler/preloader methods as an additional in-patch guard.

**Publicizer support is a two-file contract.** [`GameAssembliesToPublicize.cs`](../files/Shared/Tools/GameAssembliesToPublicize.cs.md) lists the assemblies (via `[assembly: IgnoresAccessChecksTo("…")]`) that match the `<Publicize>` entries in the `.csproj` files. [`IgnoresAccessChecksToAttribute.cs`](../files/Shared/Tools/IgnoresAccessChecksToAttribute.cs.md) provides the attribute class itself at runtime in Pulsar/Magnetar production builds. Together they allow the plugin to call internal methods and access private fields directly — for example, [`MySessionExtensions.cs`](../files/Shared/Tools/MySessionExtensions.cs.md) reads and writes `MySession.m_updateAllowed` without `AccessTools` reflection.

**Misc utilities are self-contained.** [`ObjectPools.cs`](../files/Shared/Tools/ObjectPools.cs.md) is a process-wide `StringBuilder` pool used by string-formatting patches. [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) is a simple token-bucket guard used where a log statement could fire thousands of times per minute. [`Workarounds.cs`](../files/Shared/Tools/Workarounds.cs.md) provides missing .NET Framework 4.8 APIs needed by client-side patches.

This module does not interact with any specific performance-fix module at the toolbox level; instead, every patch module in the plugin depends on it.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
