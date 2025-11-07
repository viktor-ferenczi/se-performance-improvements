// 89453c1c
ldc.i4.0
call static System.Void Havok.HkBaseSystem::set_IsInitialized(System.Boolean value)
nop
call static System.Void System.GC::WaitForPendingFinalizers()
call static System.Void Havok.HkVDB::ReleaseResources()
call static System.Void Havok.HkTaskProfiler::ReleaseResources()
call static System.Void Havok.HkBaseSystem::HkBaseSystem_Quit()
call static System.Void Havok.HkBaseSystem::FreeThreadInfo()
call static System.Void Havok.HkShape::ReportOwnership()
ret
