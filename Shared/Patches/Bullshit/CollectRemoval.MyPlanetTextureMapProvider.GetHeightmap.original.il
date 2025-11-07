// f5d680c7
ldarg.0
call Sandbox.Game.GameSystems.MyHeightMapLoadingSystem Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::get_MapCache()
ldarg.0
ldfld System.String Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_path
ldloca.s 0 (Sandbox.Engine.Voxels.Planet.MyHeightCubemap)
callvirt System.Boolean Sandbox.Game.GameSystems.MyHeightMapLoadingSystem::TryGet(System.String path, Sandbox.Engine.Voxels.Planet.MyHeightCubemap& heightmap)
brtrue.s L0
ldarg.0
ldarg.0
ldfld System.String Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_path
ldarg.0
ldfld VRage.Game.MyModContext Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_mod
call Sandbox.Engine.Voxels.Planet.MyHeightCubemap Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::GetHeightMap(System.String folderName, VRage.Game.MyModContext context)
stloc.0
ldarg.0
call Sandbox.Game.GameSystems.MyHeightMapLoadingSystem Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::get_MapCache()
ldarg.0
ldfld System.String Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::m_path
ldloca.s 0 (Sandbox.Engine.Voxels.Planet.MyHeightCubemap)
callvirt System.Void Sandbox.Game.GameSystems.MyHeightMapLoadingSystem::Cache(System.String path, Sandbox.Engine.Voxels.Planet.MyHeightCubemap& heightmap)
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
call static System.Int32 System.GC::get_MaxGeneration()
ldc.i4.0
callvirt abstract virtual System.Void VRage.IVRageSystem::CollectGC(System.Int32 generation, System.GCCollectionMode mode)
L0:
ldloc.0
ret
