using System;
using System.Linq;
using System.Reflection;
using MeleagantDependencyScan.Extensions;
using MeleagantDependencyScan.UnitTest.TestServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

            Assert.Single(testResult);
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

            Assert.Single(testResult);
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

            Assert.Single(testResult);
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

            Assert.Single(testResult);
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

            Assert.Single(testResult);
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

            Assert.Single(testResult);
        }

        [Fact]
        public void ServiceProvider_Should_Resolve_Closed_Generic_Service_From_Open_Generic_Registration()
        {
            var sc = new ServiceCollection();
            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);
            var sp = sc.BuildServiceProvider();

            var executor = sp.GetServices<IMockGenericInterface<MockRequest, MockResponse>>()
                .Single(service => service.GetType() == typeof(MockGenericClass<MockRequest, MockResponse>));

            Assert.Equal("Executed MeleagantDependencyScan.UnitTest.TestServices.MockRequest", executor.Execute(new MockRequest()));
        }

        [Theory]
        [InlineData(typeof(MockGenericClass<,>), ServiceLifetime.Scoped)]
        [InlineData(typeof(MockGenericTransientClass<,>), ServiceLifetime.Transient)]
        [InlineData(typeof(MockGenericSingletonClass<,>), ServiceLifetime.Singleton)]
        public void ServiceCollection_Should_Register_Open_Generic_Service_With_Expected_Lifetime(Type implementationType, ServiceLifetime lifetime)
        {
            var sc = new ServiceCollection();

            sc.ScanAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);

            var registration = sc.Single(service =>
                service.ServiceType == typeof(IMockGenericInterface<,>) &&
                service.ImplementationType == implementationType);

            Assert.Equal(lifetime, registration.Lifetime);
        }

        [Fact]
        public void ServiceCollection_Should_Throw_When_Exact_NonKeyed_Registration_Is_Duplicated()
        {
            // Arrange
            var sc = new ServiceCollection();
            sc.Add(new ServiceDescriptor(typeof(IHelloWorldService), typeof(HelloWorldTransientServicesWithInterface), ServiceLifetime.Transient));

            // Act
            var act = () => MeleagantServiceCollectionExtensions.ValidateNoDuplicateRegistration(sc, typeof(IHelloWorldService), typeof(HelloWorldTransientServicesWithInterface), key: null);

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void ServiceCollection_Should_Throw_When_Exact_Keyed_Registration_Is_Duplicated()
        {
            // Arrange
            var sc = new ServiceCollection();
            sc.Add(new ServiceDescriptor(typeof(IHelloWorldKeyedServices), "DuplicateKey", typeof(HelloWorldKeyedTransientServicesWithInterfaceKeyA), ServiceLifetime.Transient));

            // Act
            var act = () => MeleagantServiceCollectionExtensions.ValidateNoDuplicateRegistration(sc, typeof(IHelloWorldKeyedServices), typeof(HelloWorldKeyedTransientServicesWithInterfaceKeyA), key: "DuplicateKey");

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void ScanAssemblies_Should_Throw_When_Assembly_Name_Is_Empty()
        {
            var sc = new ServiceCollection();

            var act = () => sc.ScanAssemblies(string.Empty);

            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void ScanKeyedAssemblies_Should_Throw_When_Assembly_Names_Are_Missing()
        {
            var sc = new ServiceCollection();

            var act = () => sc.ScanKeyedAssemblies();

            Assert.Throws<ArgumentException>(act);
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
            Assert.Single(testResult);
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
            Assert.Equal($"Hello Keyed World {key} service with interface", testResult);
        }

        [Fact]
        public void ServiceProvider_Should_Provide_Keyed_MultiInterface_Service()
        {
            var sc = new ServiceCollection();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);
            var sp = sc.BuildServiceProvider();

            var service = sp.GetKeyedService<IHelloWorldKeyedSecondaryServices>("MultiKey");

            Assert.Equal("Hello Keyed Secondary World secondary service with interface", service?.SayHiKeyedSecondary("secondary service"));
        }

        [Fact]
        public void ServiceProvider_Should_Resolve_Keyed_Closed_Generic_Service_From_Open_Generic_Registration()
        {
            var sc = new ServiceCollection();
            sc.ScanKeyedAssemblies(Assembly.GetExecutingAssembly().GetName().Name!);
            var sp = sc.BuildServiceProvider();

            var service = sp.GetKeyedService<IKeyedMockGenericInterface<MockRequest, MockResponse>>("GenericKey");

            Assert.Equal("Executed keyed MeleagantDependencyScan.UnitTest.TestServices.MockRequest", service?.Execute(new MockRequest()));
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

            Assert.Single(testResult);
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

            Assert.Equal($"Hello Keyed World {key} service with interface", testResult);
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

            Assert.Single(testResult);
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

            Assert.Equal($"Hello Keyed World {key} service with interface", testResult);
        }

        #endregion

    }
}
