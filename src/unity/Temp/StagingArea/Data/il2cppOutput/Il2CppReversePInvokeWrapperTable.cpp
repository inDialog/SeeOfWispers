#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <stdint.h>

#include "codegen/il2cpp-codegen.h"
#include "il2cpp-object-internals.h"


// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;

struct EVENT_FILTER_DESCRIPTOR_t24FD3DB96806FFE8C96FFDB38B1B8331EA0D72BB ;
struct Guid_t ;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// System.Object


// System.ValueType
struct  ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_com
{
};

// System.IntPtr
struct  IntPtr_t 
{
public:
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(IntPtr_t, ___m_value_0)); }
	inline void* get_m_value_0() const { return ___m_value_0; }
	inline void** get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(void* value)
	{
		___m_value_0 = value;
	}
};

struct IntPtr_t_StaticFields
{
public:
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;

public:
	inline static int32_t get_offset_of_Zero_1() { return static_cast<int32_t>(offsetof(IntPtr_t_StaticFields, ___Zero_1)); }
	inline intptr_t get_Zero_1() const { return ___Zero_1; }
	inline intptr_t* get_address_of_Zero_1() { return &___Zero_1; }
	inline void set_Zero_1(intptr_t value)
	{
		___Zero_1 = value;
	}
};


// System.Void
struct  Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017__padding[1];
	};

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif

extern "C" void DEFAULT_CALL ReversePInvokeWrapper_EventProvider_EtwEnableCallBack_m32987ABF4E909DC5476F09C034714951CB4A8048(Guid_t * ___sourceId0, int32_t ___controlCode1, uint8_t ___setLevel2, int64_t ___anyKeyword3, int64_t ___allKeyword4, EVENT_FILTER_DESCRIPTOR_t24FD3DB96806FFE8C96FFDB38B1B8331EA0D72BB * ___filterData5, void* ___callbackContext6);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_OSSpecificSynchronizationContext_InvocationEntry_m056BCE43FF155AAE872FF7E565F8F72A50D26147(intptr_t ___arg0);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLInput_OnFocus_mCE51A3293B45E2E7C71E43D8AAC41107201B091A(int32_t ___id0);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLInput_OnBlur_mADB1B72A8D0C954E62E6180BA76674D79626BAEB(int32_t ___id0);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLInput_OnValueChange_mED7EAE4DBF4740FB47A6A0096A747A6A9E46B15A(int32_t ___id0, char* ___value1);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLInput_OnEditEnd_mF3C41E82F6C730D66530D19D1394AEE9F1450EDB(int32_t ___id0, char* ___value1);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLInput_OnTab_m2A12D1EEEB2237CE499AF01BF8CB8E3D82C21DFF(int32_t ___id0, int32_t ___value1);
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLWindow_OnWindowFocus_m5CB8F8D4720673D293808841107A7402E46ED829();
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_WebGLWindow_OnWindowBlur_m66E9428B4D404EADA32AE74403308668EFE54458();


extern const Il2CppMethodPointer g_ReversePInvokeWrapperPointers[];
const Il2CppMethodPointer g_ReversePInvokeWrapperPointers[9] = 
{
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_EventProvider_EtwEnableCallBack_m32987ABF4E909DC5476F09C034714951CB4A8048),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_OSSpecificSynchronizationContext_InvocationEntry_m056BCE43FF155AAE872FF7E565F8F72A50D26147),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLInput_OnFocus_mCE51A3293B45E2E7C71E43D8AAC41107201B091A),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLInput_OnBlur_mADB1B72A8D0C954E62E6180BA76674D79626BAEB),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLInput_OnValueChange_mED7EAE4DBF4740FB47A6A0096A747A6A9E46B15A),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLInput_OnEditEnd_mF3C41E82F6C730D66530D19D1394AEE9F1450EDB),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLInput_OnTab_m2A12D1EEEB2237CE499AF01BF8CB8E3D82C21DFF),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLWindow_OnWindowFocus_m5CB8F8D4720673D293808841107A7402E46ED829),
	reinterpret_cast<Il2CppMethodPointer>(ReversePInvokeWrapper_WebGLWindow_OnWindowBlur_m66E9428B4D404EADA32AE74403308668EFE54458),
};
