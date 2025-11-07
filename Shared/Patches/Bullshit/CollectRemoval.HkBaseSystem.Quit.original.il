// 895bed34
ldc.i4.0
call static System.Void Havok.HkBaseSystem::set_IsInitialized(System.Boolean value)
call static System.Void System.GC::Collect()
call static System.Void System.GC::WaitForPendingFinalizers()
call static System.Void Havok.HkVDB::ReleaseResources()
call static System.Void Havok.HkTaskProfiler::ReleaseResources()
call static System.Void Havok.HkBaseSystem::HkBaseSystem_Quit()
call static System.Void Havok.HkBaseSystem::FreeThreadInfo()
call static System.Void Havok.HkShape::ReportOwnership()
ret
