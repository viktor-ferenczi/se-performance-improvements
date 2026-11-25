# `Shared/Tools/PerformanceStorage.cs`

*Resolves the compilation cache folders and migrates plugin files from the previous layout.*

`Prepare` creates the instance’s `Storage` folder. Given an existing legacy `Performance` folder and a missing destination, it moves the entire folder. When both exist, it prunes recognized plugin files and empty directories from the legacy tree. Cleanup skips links.

`CompilationCache` resolves a relative or absolute root and appends `Cache`. `OpenCache` opens one of the two compilation subdirectories and expires old entries. Each game build has its own cache keys, so a node joining a shared root can reuse compatible assemblies immediately.

[Source](../../../../Shared/Tools/PerformanceStorage.cs) · [Shared tools](../../../modules/tools.md)
