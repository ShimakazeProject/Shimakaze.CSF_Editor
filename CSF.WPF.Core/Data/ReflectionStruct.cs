using System;
using System.Json;
using System.Reflection;

namespace CSF.WPF.Core.Data
{
    internal class ReflectionStruct
    {
        public Assembly Assembly { get; set; }
        public Type Class { get; set; }
        public PropertyInfo Name { get; set; }
        public MethodInfo Method { get; set; }

        public override string ToString()
        {
            return string.Format("Friendly Name:{4};\tAssembly:{0};Class:{1};Name:{2};Method:{3};", Assembly, Class, Name, Method, Name.GetValue(Class));
        }

        public static ReflectionStruct Reflection(string assemblyName, string className, string propertyName, string methodName)
        {
            var reflection = new ReflectionStruct();
            //获取并加载DLL类库中的程序集
            reflection.Assembly = Assembly.LoadFile(assemblyName);
            //获取类的类型：必须使用名称空间.类名称
            reflection.Class = reflection.Assembly.GetType(className);
            //获取类的属性：使用属性名
            reflection.Name = reflection.Class.GetProperty(propertyName);
            //获取类的成员方法：使用方法名
            reflection.Method = reflection.Class.GetMethod(methodName);
            return reflection;
        }
    }

}
