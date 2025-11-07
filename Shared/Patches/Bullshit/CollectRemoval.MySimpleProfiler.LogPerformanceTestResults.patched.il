// 4d859bb1
ldsfld System.Boolean VRage.MySimpleProfiler::m_performanceTestEnabled
brtrue.s L0
ret
L0:
call static VRage.IVRagePlatform VRage.MyVRage::get_Platform()
callvirt abstract virtual VRage.IVRageSystem VRage.IVRagePlatform::get_System()
ldc.i4 2147483647
ldc.i4.0
nop
pop
pop
pop
ldc.i4.1
call static System.Int64 System.GC::GetTotalMemory(System.Boolean forceFullCollection)
stloc.0
call static System.String VRage.FileSystem.MyFileSystem::get_UserDataPath()
ldstr "Performance_"
call static System.DateTime System.DateTime::get_Now()
stloc.3
ldloca.s 3 (System.DateTime)
ldstr "yyyy-MM-dd_HH-mm"
call System.String System.DateTime::ToString(System.String format)
ldstr ".csv"
call static System.String System.String::Concat(System.String str0, System.String str1, System.String str2)
call static System.String System.IO.Path::Combine(System.String path1, System.String path2)
ldc.i4.2
call static System.IO.Stream VRage.FileSystem.MyFileSystem::OpenWrite(System.String path, System.IO.FileMode mode)
stloc.1
ldloc.1
ldc.i4.0
ldc.i4.0
newobj System.Void System.Text.UTF8Encoding::.ctor(System.Boolean encoderShouldEmitUTF8Identifier, System.Boolean throwOnInvalidBytes)
newobj System.Void System.IO.StreamWriter::.ctor(System.IO.Stream stream, System.Text.Encoding encoding)
stloc.2
ldloc.2
ldstr "Update, Render, GPU, Memory"
callvirt virtual System.Void System.IO.TextWriter::WriteLine(System.String value)
ldc.i4.0
stloc.s 4 (System.Int32)
br L1
L5:
ldloc.s 4 (System.Int32)
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_renderTimes
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Count()
bge L2
ldloc.s 4 (System.Int32)
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_gpuTimes
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Count()
bge L3
ldloc.s 4 (System.Int32)
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_memoryAllocs
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Count()
bge.s L4
ldc.i4.4
newarr System.Object
dup
ldc.i4.0
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_updateTimes
ldloc.s 4 (System.Int32)
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Item(System.Int32 index)
box System.Int32
stelem.ref
dup
ldc.i4.1
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_renderTimes
ldloc.s 4 (System.Int32)
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Item(System.Int32 index)
box System.Int32
stelem.ref
dup
ldc.i4.2
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_gpuTimes
ldloc.s 4 (System.Int32)
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Item(System.Int32 index)
box System.Int32
stelem.ref
dup
ldc.i4.3
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_memoryAllocs
ldloc.s 4 (System.Int32)
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Item(System.Int32 index)
box System.Int32
stelem.ref
stloc.s 5 (System.Object[])
ldloc.2
ldstr "{0},{1},{2},{3}"
ldloc.s 5 (System.Object[])
callvirt virtual System.Void System.IO.TextWriter::WriteLine(System.String format, System.Object[] arg)
ldloc.s 4 (System.Int32)
ldc.i4.1
add
stloc.s 4 (System.Int32)
L1:
ldloc.s 4 (System.Int32)
ldsfld System.Collections.Generic.List`1<System.Int32> VRage.MySimpleProfiler::m_updateTimes
callvirt virtual System.Int32 System.Collections.Generic.List`1<System.Int32>::get_Count()
blt L5
L2:
L3:
L4:
ldloc.2
ldstr "Final memory: {0}"
ldloc.0
box System.Int64
callvirt virtual System.Void System.IO.TextWriter::WriteLine(System.String format, System.Object arg0)
ldloc.2
callvirt virtual System.Void System.IO.TextWriter::Close()
ldloc.1
callvirt virtual System.Void System.IO.Stream::Close()
ret
