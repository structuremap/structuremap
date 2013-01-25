using System;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Construction
{
    public class BuilderCompiler
    {
        public static InstanceBuilder CreateBuilder(Plugin plugin)
        {
            return getCompiler(plugin).CreateBuilder(plugin);
        }

        public static Func<IArguments, object> CompileCreator(Plugin plugin)
        {
            FuncCompiler compiler = getCompiler(plugin);

            return compiler.Compile(plugin);
        }

        private static FuncCompiler getCompiler(Plugin plugin)
        {
            Type compilerType = typeof (FuncCompiler<>).MakeGenericType(plugin.TPluggedType);
            return (FuncCompiler) Activator.CreateInstance(compilerType);
        }

        public static Action<IArguments, object> CompileBuildUp(Plugin plugin)
        {
            FuncCompiler compiler = getCompiler(plugin);

            return compiler.BuildUp(plugin);
        }

        #region Nested type: FuncCompiler

        public interface FuncCompiler
        {
            Func<IArguments, object> Compile(Plugin plugin);
            Action<IArguments, object> BuildUp(Plugin plugin);

            InstanceBuilder CreateBuilder(Plugin plugin);
        }

        public class FuncCompiler<T> : FuncCompiler
        {
            private readonly SetterBuilder<T> _setterBuilder = new SetterBuilder<T>();

            public InstanceBuilder CreateBuilder(Plugin plugin)
            {
                Func<IArguments, T> ctor = new ConstructorFunctionBuilder<T>().CreateBuilder(plugin);
                Action<IArguments, T> setters = buildUp(plugin);

                Func<IArguments, object> creator = args =>
                {
                    T target = ctor(args);
                    setters(args, target);
                    return target;
                };

                Action<IArguments, object> builder = (args, o) => setters(args, (T) o);

                return new InstanceBuilder(plugin.TPluggedType, creator, builder);
            }

            public Func<IArguments, object> Compile(Plugin plugin)
            {
                Func<IArguments, T> ctor = new ConstructorFunctionBuilder<T>().CreateBuilder(plugin);
                Action<IArguments, T> buildUp = this.buildUp(plugin);

                return args =>
                {
                    // Call the constructor
                    T target = ctor(args);
                    buildUp(args, target);

                    return target;
                };
            }

            public Action<IArguments, object> BuildUp(Plugin plugin)
            {
                Action<IArguments, T> func = buildUp(plugin);
                return (args, raw) => func(args, (T) raw);
            }

            private Action<IArguments, T> buildUp(Plugin plugin)
            {
                Action<IArguments, T>[] mandatories = plugin.Setters.Where(x => x.IsMandatory)
                    .Select(x => _setterBuilder.BuildMandatorySetter((PropertyInfo) x.Property))
                    .ToArray();


                Action<IArguments, T>[] optionals = plugin.Setters.Where(x => !x.IsMandatory)
                    .Select(x => _setterBuilder.BuildOptionalSetter(x.Property))
                    .ToArray();

                return (args, target) =>
                {
                    // Call the mandatory setters
                    for (int i = 0; i < mandatories.Length; i++)
                    {
                        mandatories[i](args, target);
                    }

                    // Call the optional setters
                    for (int i = 0; i < optionals.Length; i++)
                    {
                        optionals[i](args, target);
                    }
                };
            }
        }

        #endregion
    }
}