using System.Linq;
using System.Reflection;
using FluentAssertions;
using MeleagantDependencyScan.Extensions;
using MeleagantDependencyScan.UnitTest.TestServices;
using Microsoft.Extensions.DependencyInjection;

namespace MeleagantDependencyScan.UnitTest
{
    public class MeleagantServiceCollectionExtensionsTests
    {
        [Fact]
        public void ServiceCollection_Should_Contain_Transient_Visible_By_ItSelf_HelloWorldService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s =>
                s.ImplementationType == typeof(HelloWorldTransientServices) 
                && s.Lifetime == ServiceLifetime.Transient);

            // Assert
            
            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }

        [Fact]
        public void ServiceCollection_Should_Contain_Scoped_Visible_By_ItSelf_HelloWorldService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s =>
                s.ImplementationType == typeof(HelloWorldScopedServices) 
                && s.Lifetime == ServiceLifetime.Scoped);

            // Assert

            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }

        [Fact]
        public void ServiceCollection_Should_Contain_Singleton_Visible_By_ItSelf_HelloWorldService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s =>
                s.ImplementationType == typeof(HelloWorldSingletonServices) 
                && s.Lifetime == ServiceLifetime.Singleton);

            // Assert

            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }

        [Fact]
        public void ServiceCollection_Should_Contain_Transient_Visible_By_Interface_HelloWorldService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s => s.ServiceType == typeof(IHelloWorldService) 
                                           && s.ImplementationType == typeof(HelloWorldTransientServicesWithInterface) 
                                           && s.Lifetime == ServiceLifetime.Transient);

            // Assert

            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }

        [Fact]
        public void ServiceCollection_Should_Contain_Scoped_Visible_By_Interface_HelloWorldService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s => s.ServiceType == typeof(IHelloWorldService)
                                           && s.ImplementationType == typeof(HelloWorldScopedServicesWithInterface) 
                                           && s.Lifetime == ServiceLifetime.Scoped);

            // Assert

            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }

        [Fact]
        public void ServiceCollection_Should_Contain_Singleton_Visible_By_Interface_HelloWorldService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s => s.ServiceType == typeof(IHelloWorldService)
                && s.ImplementationType == typeof(HelloWorldSingletonServicesWithInterface) 
                && s.Lifetime == ServiceLifetime.Singleton);

            // Assert

            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }
    }
}