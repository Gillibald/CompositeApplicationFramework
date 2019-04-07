#region Usings

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public static class FastActivator
    {
        private static readonly Hashtable CreatorCache = Hashtable.Synchronized(new Hashtable());

        public static TTarget CreateInstance<TTarget>()
        {
            var instance = default(TTarget);

            try
            {
                instance = (TTarget) GetConstructor<TTarget>()();
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static object CreateInstance(Type pTargetType)
        {
            object instance = null;

            try
            {
                instance = GetConstructor(pTargetType)();
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static CreateObject GetConstructor<TTarget>()
        {
            var lCreateDelegate = (CreateObject) CreatorCache[typeof (TTarget)];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register<TTarget>();

            return (CreateObject) CreatorCache[typeof (TTarget)];
        }

        public static CreateObject GetConstructor(Type pTargetType)
        {
            var lCreateDelegate = (CreateObject) CreatorCache[pTargetType];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register(pTargetType);

            return (CreateObject) CreatorCache[pTargetType];
        }

        public static void Register<TTarget>()
        {
            lock (CreatorCache.SyncRoot)
            {
                var lTargetType = typeof (TTarget);

                var constructor = CreatorCache[lTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = BuildConstructorDelegate(lTargetType, Type.EmptyTypes);

                CreatorCache.Add(lTargetType, constructor);
            }
        }

        public static void Register(Type pTargetType)
        {
            lock (CreatorCache.SyncRoot)
            {
                var constructor = CreatorCache[pTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = BuildConstructorDelegate(pTargetType, Type.EmptyTypes);

                CreatorCache.Add(pTargetType, constructor);
            }
        }

        public static CreateObject BuildConstructorDelegate(Type pObjType, Type[] pArgTypes)
        {
            CreateObject lConstructorDelegate;

            lock (CreatorCache.SyncRoot)
            {
                var ctor = pObjType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    pArgTypes,
                    null);

                var ctorParams = ctor?.GetParameters();

                if (ctor?.DeclaringType == null)
                {
                    return null;
                }
                var dynMethod = new DynamicMethod(
                    $"DM$OBJ_FACTORY_{pObjType.Name}",
                    pObjType,
                    new[] {typeof (object[])},
                    ctor.DeclaringType);

                var ilGen = dynMethod.GetILGenerator();

                for (var i = 0; i < ctorParams.Length; i++)
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                    switch (i)
                    {
                        case 0:
                            ilGen.Emit(OpCodes.Ldc_I4_0);
                            break;
                        case 1:
                            ilGen.Emit(OpCodes.Ldc_I4_1);
                            break;
                        case 2:
                            ilGen.Emit(OpCodes.Ldc_I4_2);
                            break;
                        case 3:
                            ilGen.Emit(OpCodes.Ldc_I4_3);
                            break;
                        case 4:
                            ilGen.Emit(OpCodes.Ldc_I4_4);
                            break;
                        case 5:
                            ilGen.Emit(OpCodes.Ldc_I4_5);
                            break;
                        case 6:
                            ilGen.Emit(OpCodes.Ldc_I4_6);
                            break;
                        case 7:
                            ilGen.Emit(OpCodes.Ldc_I4_7);
                            break;
                        case 8:
                            ilGen.Emit(OpCodes.Ldc_I4_8);
                            break;
                        default:
                            ilGen.Emit(OpCodes.Ldc_I4, i);
                            break;
                    }
                    ilGen.Emit(OpCodes.Ldelem_Ref);

                    var paramType = ctorParams[i].ParameterType;

                    ilGen.Emit(paramType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
                }

                ilGen.Emit(OpCodes.Newobj, ctor);

                ilGen.Emit(OpCodes.Ret);

                lConstructorDelegate = (CreateObject) dynMethod.CreateDelegate(typeof (CreateObject));
            }
            return lConstructorDelegate;
        }
    }

    public static class FastActivator<T1>
    {
        private static readonly Hashtable CreatorCache = Hashtable.Synchronized(new Hashtable());

        public static TTarget CreateInstance<TTarget>(params object[] args)
        {
            var instance = default(TTarget);

            try
            {
                instance = (TTarget) GetConstructor<TTarget>()(args);
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static object CreateInstance(Type pTargetType, params object[] args)
        {
            object instance = null;

            try
            {
                instance = GetConstructor(pTargetType)(args);
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static void Register<TTarget>()
        {
            lock (CreatorCache.SyncRoot)
            {
                var lTargetType = typeof (TTarget);

                Type[] lArgTypes = {typeof (T1)};

                var constructor = CreatorCache[lTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = FastActivator.BuildConstructorDelegate(lTargetType, lArgTypes);

                CreatorCache.Add(lTargetType, constructor);
            }
        }

        public static void Register(Type pTargetType)
        {
            lock (CreatorCache.SyncRoot)
            {
                var lArgTypes = new[] {typeof (T1)};

                var constructor = CreatorCache[pTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = FastActivator.BuildConstructorDelegate(pTargetType, lArgTypes);

                CreatorCache.Add(pTargetType, constructor);
            }
        }

        public static CreateObject GetConstructor<TTarget>()
        {
            var lCreateDelegate = (CreateObject) CreatorCache[typeof (TTarget)];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register<TTarget>();

            return (CreateObject) CreatorCache[typeof (TTarget)];
        }

        public static CreateObject GetConstructor(Type pTargetType)
        {
            var lCreateDelegate = (CreateObject) CreatorCache[pTargetType];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register(pTargetType);

            return (CreateObject) CreatorCache[pTargetType];
        }
    }

    public static class FastActivator<T1, T2>
    {
        private static readonly Hashtable CreatorCache = Hashtable.Synchronized(new Hashtable());

        public static TTarget CreateInstance<TTarget>(params object[] args)
        {
            var instance = default(TTarget);

            try
            {
                instance = (TTarget) GetConstructor<TTarget>()(args);
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static object CreateInstance(Type pTargetType, params object[] args)
        {
            object instance = null;

            try
            {
                instance = GetConstructor(pTargetType)(args);
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static void Register<TTarget>()
        {
            lock (CreatorCache.SyncRoot)
            {
                var lTargetType = typeof (TTarget);

                var lArgTypes = new[] {typeof (T1), typeof (T2)};

                var constructor = CreatorCache[lTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = FastActivator.BuildConstructorDelegate(lTargetType, lArgTypes);

                CreatorCache.Add(lTargetType, constructor);
            }
        }

        public static void Register(Type pTargetType)
        {
            lock (CreatorCache.SyncRoot)
            {
                var lArgTypes = new[] {typeof (T1), typeof (T2)};

                var constructor = CreatorCache[pTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = FastActivator.BuildConstructorDelegate(pTargetType, lArgTypes);

                CreatorCache.Add(pTargetType, constructor);
            }
        }

        public static CreateObject GetConstructor<TTarget>()
        {
            var lCreateDelegate = (CreateObject) CreatorCache[typeof (TTarget)];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register<TTarget>();

            return (CreateObject) CreatorCache[typeof (TTarget)];
        }

        public static CreateObject GetConstructor(Type pTargetType)
        {
            var lCreateDelegate = (CreateObject) CreatorCache[pTargetType];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register(pTargetType);

            return (CreateObject) CreatorCache[pTargetType];
        }
    }

    public static class FastActivator<T1, T2, T3>
    {
        private static readonly Hashtable CreatorCache = Hashtable.Synchronized(new Hashtable());

        public static TTarget CreateInstance<TTarget>(params object[] args)
        {
            var instance = default(TTarget);

            try
            {
                instance = (TTarget) GetConstructor<TTarget>()(args);
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static object CreateInstance(Type pTargetType, params object[] args)
        {
            object instance = null;

            try
            {
                instance = GetConstructor(pTargetType)(args);
            }
            catch (TypeLoadException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return instance;
        }

        public static void Register<TTarget>()
        {
            lock (CreatorCache.SyncRoot)
            {
                var lTargetType = typeof (TTarget);

                var lArgTypes = new[] {typeof (T1), typeof (T2), typeof (T3)};

                var constructor = CreatorCache[lTargetType] as CreateObject;

                if (constructor != null)
                {
                    return;
                }

                constructor = FastActivator.BuildConstructorDelegate(lTargetType, lArgTypes);

                CreatorCache.Add(lTargetType, constructor);
            }
        }

        public static void Register(Type pTargetType)
        {
            lock (CreatorCache.SyncRoot)
            {
                var lArgTypes = new[] {typeof (T1), typeof (T2), typeof (T3)};

                var constructor = CreatorCache[pTargetType] as CreateObject;
                if (constructor != null)
                {
                    return;
                }
                constructor = FastActivator.BuildConstructorDelegate(pTargetType, lArgTypes);
                CreatorCache.Add(pTargetType, constructor);
            }
        }

        public static CreateObject GetConstructor<TTarget>()
        {
            var lCreateDelegate = (CreateObject) CreatorCache[typeof (TTarget)];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register<TTarget>();

            return (CreateObject) CreatorCache[typeof (TTarget)];
        }

        public static CreateObject GetConstructor(Type pTargetType)
        {
            var lCreateDelegate = (CreateObject) CreatorCache[pTargetType];

            if (lCreateDelegate != null)
            {
                return lCreateDelegate;
            }

            Register(pTargetType);

            return (CreateObject) CreatorCache[pTargetType];
        }
    }
}