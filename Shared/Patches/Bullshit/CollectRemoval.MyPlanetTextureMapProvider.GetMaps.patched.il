// 07c69873
ldarg.0
call Sandbox.Game.GameSystems.MyHeightMapLoadingSystem Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::get_MapCache()
ldarg.0
ldfld System.String Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_path
ldloca.s 0 (Sandbox.Engine.Voxels.Planet.MyCubemap[])
callvirt System.Boolean Sandbox.Game.GameSystems.MyHeightMapLoadingSystem::TryGet(System.String path, Sandbox.Engine.Voxels.Planet.MyCubemap[]& materialMaps)
brtrue.s L0
ldarg.0
ldarg.0
ldfld System.String Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_path
ldarg.0
ldfld VRage.Game.MyModContext Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_mod
ldarg.1
ldloca.s 0 (Sandbox.Engine.Voxels.Planet.MyCubemap[])
call System.Void Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::GetPlanetMaps(System.String folder, VRage.Game.MyModContext context, VRage.Game.MyPlanetMapTypeSet mapsToUse, Sandbox.Engine.Voxels.Planet.MyCubemap[]& maps)
ldarg.0
call Sandbox.Game.GameSystems.MyHeightMapLoadingSystem Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::get_MapCache()
ldarg.0
ldfld System.String Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_path
ldloca.s 0 (Sandbox.Engine.Voxels.Planet.MyCubemap[])
callvirt System.Void Sandbox.Game.GameSystems.MyHeightMapLoadingSystem::Cache(System.String path, Sandbox.Engine.Voxels.Planet.MyCubemap[]& materialMaps)
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
call static System.Int32 System.GC::get_MaxGeneration()
ldc.i4.0
nop
pop
pop
pop
L0:
ldloc.0
ret
