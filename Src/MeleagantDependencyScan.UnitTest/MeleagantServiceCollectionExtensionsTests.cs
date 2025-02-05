using System.Data.SqlTypes;
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
        #region Simple services

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

        #endregion

        #region Keyed services

        [Fact]
        public void ServiceCollection_Should_Contain_Keyed_Transient_Visible_By_Interface_HelloWorldKeyedService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s => s.ServiceType == typeof(IHelloWorldKeyedServices)
                                           && s.IsKeyedService
                                           && s.ServiceKey!.ToString() == "TKeyA"
                                            && s.KeyedImplementationType == typeof(HelloWorldKeyedTransientServicesWithInterfaceKeyA) 
                                            && s.Lifetime == ServiceLifetime.Transient);
            
            // Assert
            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }
        
        [Theory]
        [InlineData("TKeyA")]
        [InlineData("TKeyB")]
        public void ServiceProvider_Should_Provide_TKey_Transient_Visible_By_Interface_HelloWorldKeyedService(string key)
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);
            var sp = sc.BuildServiceProvider();
            
            // Act

            var testResult = sp.GetKeyedService<IHelloWorldKeyedServices>(key)?.SayHiKeyed($"{key} service");
 
            // Assert
            testResult.Should().Be($"Hello Keyed World {key} service with interface");
        }
        
        [Fact]
        public void ServiceCollection_Should_Contain_Keyed_Scoped_Visible_By_Interface_HelloWorldKeyedService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s => s.ServiceType == typeof(IHelloWorldKeyedServices)
                                           && s.IsKeyedService
                                           && s.ServiceKey!.ToString() == "SKeyA"
                                           && s.KeyedImplementationType == typeof(HelloWorldKeyedScopedServicesWithInterfaceKeyA) 
                                           && s.Lifetime == ServiceLifetime.Scoped);
            
            // Assert
            
            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }
        
        [Theory]
        [InlineData("SKeyA")]
        [InlineData("SKeyB")]
        public void ServiceProvider_Should_Provide_SKey_Scoped_Visible_By_Interface_HelloWorldKeyedService(string key)
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);
            var sp = sc.BuildServiceProvider();
            
            // Act

            var testResult = sp.GetKeyedService<IHelloWorldKeyedServices>(key)?.SayHiKeyed($"{key} service");
 
            // Assert
            
            testResult.Should().Be($"Hello Keyed World {key} service with interface");
        }
        
        [Fact]
        public void ServiceCollection_Should_Contain_Keyed_Singleton_Visible_By_Interface_HelloWorldKeyedService()
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            // Act

            var testResult = sc.Where(s => s.ServiceType == typeof(IHelloWorldKeyedServices)
                                           && s.IsKeyedService
                                           && s.ServiceKey!.ToString() == "SiKeyA"
                                           && s.KeyedImplementationType == typeof(HelloWorldKeyedSingletonServicesWithInterfaceKeyA) 
                                           && s.Lifetime == ServiceLifetime.Singleton);
            
            // Assert
            
            testResult.Should().NotBeNullOrEmpty().And.HaveCount(1);
        }
        
        [Theory]
        [InlineData("SiKeyA")]
        [InlineData("SiKeyB")]
        public void ServiceProvider_Should_Provide_SiKey_Singleton_Visible_By_Interface_HelloWorldKeyedService(string key)
        {
            // Arrange

            var sc = new ServiceCollection();
            sc.Clear();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);
            var sp = sc.BuildServiceProvider();
            
            // Act

            var testResult = sp.GetKeyedService<IHelloWorldKeyedServices>(key)?.SayHiKeyed($"{key} service");
 
            // Assert
            
            testResult.Should().Be($"Hello Keyed World {key} service with interface");
        }

        #endregion

    }
}