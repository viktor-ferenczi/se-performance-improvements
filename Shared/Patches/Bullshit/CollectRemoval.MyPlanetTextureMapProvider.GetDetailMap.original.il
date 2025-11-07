// c41dd090
ldarg.0
ldarg.1
call Sandbox.Engine.Voxels.Planet.MyHeightDetailTexture Sandbox.Engine.Voxels.Planet.MyPlanetTextureMapProvider::GetDetailMapImpl(System.String path)
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
call static System.Int32 System.GC::get_MaxGeneration()
ldc.i4.0
callvirt abstract virtual System.Void VRage.IVRageSystem::CollectGC(System.Int32 generation, System.GCCollectionMode mode)
ret
