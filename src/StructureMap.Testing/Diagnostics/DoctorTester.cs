using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class DoctorTester
    {
        private DoctorReport fetchReport<T>(string config) where T : IBootstrapper
        {
            var doctor = new Doctor
            {
                BinaryPath = Path.GetFullPath("."),
                BootstrapperType = typeof (T).AssemblyQualifiedName
            };

            if (!string.IsNullOrEmpty(config))
            {
                var doc = new XmlDocument();
                doc.LoadXml(config.Replace("'", "\""));
                string filename = "config.xml";
                doc.Save(filename);

                doctor.ConfigFile = filename;
            }


            return doctor.RunReport();
        }


        public class ClassThatFails
        {
            [ValidationMethod]
            public void NotGoingToWork()
            {
                throw new NotImplementedException();
            }
        }

        public class NumberWidget : IWidget
        {
            public NumberWidget(int age)
            {
            }

            #region IWidget Members

            public void DoSomething()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [Test]
        public void Bootstrapper_throws_error()
        {
            DoctorReport report = fetchReport<BootstrapperThatThrowsError>("");
            report.Result.ShouldEqual(DoctorResult.BootstrapperFailure);
            report.ErrorMessages.ShouldContain(BootstrapperThatThrowsError.ERROR);
        }

        [Test]
        public void Cannot_find_bootstrapper_type()
        {
            var doctor = new Doctor
            {
                BootstrapperType = "SomethingThatCouldNotBeFound"
            };
            doctor.RunReport().Result.ShouldEqual(DoctorResult.BootstrapperCouldNotBeFound);
        }


        [Test]
        public void Happy_path_returns_successful()
        {
            fetchReport<HappyPathBootstrapper>("").Result.ShouldEqual(DoctorResult.Success);
        }

        [Test]
        public void Happy_path_returns_the_what_do_I_have_in_report()
        {
            fetchReport<HappyPathBootstrapper>("").WhatDoIHave.ShouldNotBeEmpty();
        }

        [Test]
        public void Successful_configuration_but_unable_to_build_an_instance()
        {
            fetchReport<BootstrapperThatWouldCauseABuildError>("").Result.ShouldEqual(DoctorResult.BuildErrors);
        }

        [Test]
        public void
            Successful_configuration_with_failed_validation_tests_returns_failure_and_whatdoihave_and_validation_report()
        {
            DoctorReport report = fetchReport<BootstrapperThatFailsValidationMethod>("");
            report.Result.ShouldEqual(DoctorResult.ValidationErrors);
            report.WhatDoIHave.ShouldNotBeEmpty();
            report.ErrorMessages.ShouldContain("NotGoingToWork");
        }

        [Test]
        public void Use_configuration_negative_case()
        {
            // "age" is numeric, so "abc" will blow up with parse errors

            DoctorReport report =
                fetchReport<BootstrapperThatDependsOnConfigFile>(
                    @"
<configuration>
  <appSettings>
    <add key='age' value='abc'/>
  </appSettings>
</configuration>
");

            report.Result.ShouldEqual(DoctorResult.BootstrapperFailure);
        }

        [Test]
        public void Use_configuration_positive_case()
        {
            // The "age" argument to the constructor is an integer, so 34 is okay

            DoctorReport report =
                fetchReport<BootstrapperThatDependsOnConfigFile>(
                    @"
<configuration>
  <appSettings>
    <add key='age' value='34'/>
  </appSettings>
</configuration>
");

            report.Result.ShouldEqual(DoctorResult.Success);
        }

        [Test]
        public void WriteAllNames()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (typeof (IBootstrapper).IsAssignableFrom(type))
                {
                    Debug.WriteLine(type.AssemblyQualifiedName);
                }
            }
        }
    }

    public class BootstrapperThatWouldCreateErrors : IBootstrapper
    {
        #region IBootstrapper Members

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(
                x => { x.For<IWidget>().Use(new ConfiguredInstance(typeof (ColorRule))); });
        }

        #endregion
    }

    public class BootstrapperThatFailsValidationMethod : IBootstrapper
    {
        #region IBootstrapper Members

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<DoctorTester.ClassThatFails>().Use<DoctorTester.ClassThatFails>();
            });
        }

        #endregion
    }

    public class BootstrapperThatThrowsError : IBootstrapper
    {
        public const string ERROR = "I threw up";

        #region IBootstrapper Members

        public void BootstrapStructureMap()
        {
            throw new NotImplementedException(ERROR);
        }

        #endregion
    }

    public class BootstrapperThatWouldCauseABuildError : IBootstrapper
    {
        #region IBootstrapper Members

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(
                x => { x.For<IWidget>().Use(() => { throw new NotImplementedException(); }); });
        }

        #endregion
    }

    public class HappyPathBootstrapper : IBootstrapper
    {
        #region IBootstrapper Members

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<IWidget>().Use(new ColorWidget("Red"));
            });
        }

        #endregion
    }

    public class BootstrapperThatDependsOnConfigFile : IBootstrapper
    {
        #region IBootstrapper Members

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<IWidget>().Use<DoctorTester.NumberWidget>()
                    .Ctor<int>("age").EqualToAppSetting("age");
            });
        }

        #endregion
    }
}