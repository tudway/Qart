﻿using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class Bootstrapper
    {
        public static WindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<ILogManager>().ImplementedBy<LogManager>());
            container.Register(Component.For<ITestSession>().ImplementedBy<LoggingTestSession>());
            container.Register(Component.For<TestSession>().ImplementedBy<TestSession>());
            container.Register(Component.For<ITestCaseProcessorResolver>().Instance(new TestCaseProcessorResolver(container)));
            container.Install(FromAssembly.InDirectory(new AssemblyFilter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))));
            return container;
        }
    }
}
