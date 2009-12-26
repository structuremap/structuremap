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
            var compilerType = typeof (FuncCompiler<>).MakeGenericType(plugin.PluggedType);
            return (FuncCompiler)Activator.CreateInstance(compilerType);
        }

        public static Action<IArguments, object> CompileBuildUp(Plugin plugin)
        {
            FuncCompiler compiler = getCompiler(plugin);

            return compiler.BuildUp(plugin);
        }

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
                var ctor = new ConstructorFunctionBuilder<T>().CreateBuilder(plugin);
                var setters = this.buildUp(plugin);

                Func<IArguments, object> creator = args =>
                {
                    var target = ctor(args);
                    setters(args, target);
                    return target;
                };

                Action<IArguments, object> builder = (args, o) => setters(args, (T) o);

                return new InstanceBuilder(plugin.PluggedType, creator, builder);
            }

            public Func<IArguments, object> Compile(Plugin plugin)
            {
                var ctor = new ConstructorFunctionBuilder<T>().CreateBuilder(plugin);
                var buildUp = this.buildUp(plugin);

                return args =>
                {
                    // Call the constructor
                    var target = ctor(args);
                    buildUp(args, target);

                    return target;
                };
            }

            private Action<IArguments, T> buildUp(Plugin plugin)
            {
                var mandatories = plugin.Setters.Where(x => x.IsMandatory)
                    .Select(x => _setterBuilder.BuildMandatorySetter((PropertyInfo)x.Property))
                    .ToArray();


                var optionals = plugin.Setters.Where(x => !x.IsMandatory)
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

            public Action<IArguments, object> BuildUp(Plugin plugin)
            {
                var func = buildUp(plugin);
                return (args, raw) => func(args, (T)raw);
            }
        }
    }
}