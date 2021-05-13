using AppOfficina.Portable.Services;
using AppOfficina.Portable.ViewModels;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using AutoMapper;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
namespace AppOfficina.Android
{
    public class IocContainer
    {

        public static Autofac.IContainer RegisterDeps()
        {
            var builder = new ContainerBuilder();
            RegisterMaps(builder);
         
            builder.RegisterType<LoginViewModel>().AsSelf();
            builder.RegisterType<CommessaViewModel>().AsSelf();
            builder.RegisterType<SettingsViewModel>().AsSelf();
            builder.RegisterType<BarcodeViewModel>().AsSelf();
            builder.RegisterType<BaseViewModel>().AsSelf();
            builder.RegisterType<PhotoViewModel>().AsSelf();
            builder.RegisterType<PhotoUploadViewModel>().AsSelf();
            builder.RegisterType<PhotoCarouselViewModel>().AsSelf();
            builder.RegisterType<InconvenienteViewModel>().AsSelf();
            builder.RegisterType<InconvenienteExtraViewModel>().AsSelf();
            builder.RegisterType<SettingsApp>().AsSelf();
            builder.RegisterType<ApiServices>().AsSelf();
            builder.RegisterType<Logger>().AsSelf();
            builder.RegisterType<HistoryNotesPageViewModel>().AsSelf();
            builder.RegisterType<GestioneNoteViewModel>().AsSelf();

            var container = builder.Build();
            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);
            return container;
        }

        private static void RegisterMaps(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(assemblies)
                 .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                 .As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            }))
            .AsSelf()
            .AutoActivate()
            .SingleInstance();

            builder.Register(c =>
            {
                var scope = c.Resolve<ILifetimeScope>();
                return new Mapper(c.Resolve<MapperConfiguration>(), scope.Resolve);
            })
            .As<IMapper>()
            .SingleInstance();
        }
    }
}