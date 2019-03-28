using EasyCaching.Core.Bus;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// 动态创建订阅者类
    /// 使用Emit生成类，并设置CAP所需要的TopicAttribute
    /// </summary>
    public class DynamicCreateSubscribeClass
    {
        /// <summary>
        /// 获取缓存订阅者的Type
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Type GetCacheSubscribeType(CapBusOptions options)
        {
            var type = typeof(CacheSubscribe);

            var moduleName = type.GetTypeInfo().Module.Name;
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());

            var module = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName);



            var builder = module.DefineType("CacheSubscribe", TypeAttributes.Class, typeof(CacheSubscribe));

            var met = type.GetMethods();

            MethodBuilder methodBuilder = builder.DefineMethod("ReceiveMessage", MethodAttributes.Public, null, new Type[] { typeof(EasyCachingMessage) });


            Type[] ctorParams = new Type[] { typeof(string), typeof(string) };

            //获取构造器信息
            ConstructorInfo classCtorInfo = typeof(EasyCachingSubscribeAttribute).GetConstructor(ctorParams);

            //动态创建CapSubscribeAttribute
            CustomAttributeBuilder myCABuilder = new CustomAttributeBuilder(
                           classCtorInfo,
                           new object[] { options.TopicName, options.QueuePrefixName + GetQueueSuffixName() });
            //将上面动态创建的Attribute附加到(动态创建的)类型MyType
            methodBuilder.SetCustomAttribute(myCABuilder);

            //生成指令
            ILGenerator numberGetIL = methodBuilder.GetILGenerator();
            numberGetIL.Emit(OpCodes.Ldarg_0);
            numberGetIL.Emit(OpCodes.Ldarg_1);
            numberGetIL.Emit(OpCodes.Call, type.GetMethod("ReceiveMessage", new Type[] { typeof(EasyCachingMessage) }));

            numberGetIL.Emit(OpCodes.Ret);

            return builder.CreateTypeInfo().AsType();
        }

        /// <summary>
        /// 获取队列后缀名
        /// </summary>
        /// <returns></returns>
        private static string GetQueueSuffixName()
        {
            //计算机名称和程序运行路径组成唯一标示
            //docker运行时每个容器中计算机名称都是不同的
            return (Environment.MachineName + "|" + Assembly.GetEntryAssembly().Location).ToMd5().ToLower();
        }

    }
}
