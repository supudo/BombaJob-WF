﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;

using BombaJob.Utilities.Interfaces;
using BombaJob.ViewModels;

using Caliburn.Micro;
using Caliburn.Micro.Logging;

namespace BombaJob
{
    public class BombaJobBootstrapper : Bootstrapper<IShell>
    {
        private CompositionContainer container;

        static BombaJobBootstrapper()
        {
            if (AppSettings.CaliburnDebug)
                LogManager.GetLog = type => new TraceLogger(type);
        }

        protected override void Configure()
        {
            this.container = new CompositionContainer(
                new AggregateCatalog(
                    AssemblySource.Instance.Select(x => new AssemblyCatalog(x))
                )
            );

            var batch = new CompositionBatch();

            batch.AddExportedValue<ShellViewModel>(new ShellViewModel());
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(this.container);

            this.container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = this.container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new Exception(string.Format("Error! Could not locate any instances of {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            this.container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            var viewModel = IoC.Get<ShellViewModel>();
            IWindowManager windowManager;
            try
            {
                windowManager = IoC.Get<IWindowManager>();
            }
            catch
            {
                windowManager = new WindowManager();
            }
            viewModel.Bom();
            windowManager.ShowWindow(viewModel);
        }
    }
}
