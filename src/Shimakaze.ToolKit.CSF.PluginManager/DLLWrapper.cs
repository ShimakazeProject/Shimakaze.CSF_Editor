using System;
using System.Runtime.InteropServices;

namespace Shimakaze.ToolKit.CSF.PluginManager
{
    /// <summary>
    /// DLL加载类
    /// </summary>
    internal class DLLWrapper
    {
        ///<summary>
        /// API LoadLibrary
        ///</summary>
        [DllImport("Kernel32")]
        public static extern int LoadLibrary(string funcname);

        ///<summary>
        /// API GetProcAddress
        ///</summary>
        [DllImport("Kernel32")]
        public static extern int GetProcAddress(int handle, string funcname);

        ///<summary>
        /// API FreeLibrary
        ///</summary>
        [DllImport("Kernel32")]
        public static extern int FreeLibrary(int handle);

        ///<summary>
        ///将表示函数地址的IntPtr实例转换成对应的委托, by jingzhongrong
        ///</summary>
        public static TDelegate GetDelegateFromIntPtr<TDelegate>(IntPtr address)
        {
            if (address == IntPtr.Zero)
                return default;
            else
                return Marshal.GetDelegateForFunctionPointer<TDelegate>(address);
        }

        ///<summary>
        ///将表示函数地址的int转换成对应的委托，by jingzhongrong
        ///</summary>
        public static TDelegate GetDelegateFromIntPtr<TDelegate>(int address)
            => GetDelegateFromIntPtr<TDelegate>(new IntPtr(address));

        ///<summary>
        ///通过非托管函数名转换为对应的委托, by jingzhongrong
        ///</summary>
        ///<param name="dllModule">Get DLL handle by LoadLibrary</param>
        ///<param name="functionName">Unmanaged function name</param>
        ///<param name="t">ManageR type对应的委托类型</param>
        ///<returns>委托实例，可强制转换为适当的委托类型</returns>
        public static TDelegate GetFunctionAddress<TDelegate>(int dllModule, string functionName)
            => GetDelegateFromIntPtr<TDelegate>(GetProcAddress(dllModule, functionName));
    }
}
