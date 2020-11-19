using ServiceProvider.Controllers;
using ServiceProvider.Repositories;
using ServiceProvider.Services;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace ServiceProvider
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();

            var ctr = new InjectionConstructor(typeof(ICustomerService));



            container.RegisterType<AccountController>(ctr);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}