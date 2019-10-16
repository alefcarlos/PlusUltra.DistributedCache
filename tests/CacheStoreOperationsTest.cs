using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using PlusUltra.Testing;
using Shouldly;
using Xunit;

namespace PlusUltra.DistributedCache.Tests
{
    public class Model
    {
        public int Inteiro { get; set; }
        public string Texto { get; set; }

        public DateTime Data { get; set; }

        public EEnum Enum { get; set; }
    }

    public enum EEnum
    {
        Primeiro,
        Segundo,
        Terceiro
    }

    public class CacheStoreOperationsTest : TestHost<Startup>
    {
        public CacheStoreOperationsTest()
        {
            cache = GetService<IDistributedCache>();
        }

        private readonly IDistributedCache cache;

        [Fact]
        public async Task Create_Value_Should_Not_ThrowException()
        {
            //Arrange
            var value = new Model
            {
                Data = DateTime.Now,
                Enum = EEnum.Segundo,
                Inteiro = 50,
                Texto = "Modelo cacheado"
            };

            //Act
            await cache.RemoveAsync(nameof(Model));
            await cache.SetObjectAsync(nameof(Model), value);

            Assert.True(true);
        }

        [Fact]
        public async Task CreateAndGet_Value_Should_Be_Success()
        {
            //Arrange
            var value = new Model
            {
                Data = DateTime.Now,
                Enum = EEnum.Terceiro,
                Inteiro = 50,
                Texto = "Modelo cacheado"
            };

            //Act
            await cache.RemoveAsync(nameof(Model));
            await cache.SetObjectAsync(nameof(Model), value);

            var result = await cache.GetObjectAsync<Model>(nameof(Model));

            //Assert
            result.ShouldNotBeNull();

            result.Data.ShouldBe(value.Data);
            result.Enum.ShouldBe(value.Enum);
            result.Inteiro.ShouldBe(value.Inteiro);
            result.Texto.ShouldBe(value.Texto);
        }

        [Fact]
        public async Task CreateWithTimeout_Value_Should_Be_Success()
        {
            //Arrange
            var value = new Model
            {
                Data = DateTime.Now,
                Enum = EEnum.Primeiro,
                Inteiro = 50,
                Texto = "Modelo cacheado"
            };

            //Act
            await cache.RemoveAsync(nameof(Model));
            await cache.SetObjectAsync(nameof(Model), value, TimeSpan.FromSeconds(3));

            await Task.Delay(TimeSpan.FromSeconds(4));

            var result = await cache.GetObjectAsync<Model>(nameof(Model));

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task CreateAndGetWithFallbackTimeOut_Value_Should_Be_Success()
        {
            //Arrange
            var value = new Model
            {
                Data = DateTime.Now,
                Enum = EEnum.Segundo,
                Inteiro = 10,
                Texto = "Modelo cacheado"
            };

            //Act
            await cache.RemoveAsync(nameof(Model));
            var result = await cache.GetOrSetObjectAsync(nameof(Model), () => Task.FromResult(value), TimeSpan.FromSeconds(3));


            //Assert
            result.ShouldNotBeNull();

            result.Data.ShouldBe(value.Data);
            result.Enum.ShouldBe(value.Enum);
            result.Inteiro.ShouldBe(value.Inteiro);
            result.Texto.ShouldBe(value.Texto);
        }


        [Fact]
        public async Task CreateAndGetWithFallback_Value_Should_Be_Success()
        {
            //Arrange
            var value = new Model
            {
                Data = DateTime.Now,
                Enum = EEnum.Segundo,
                Inteiro = 50,
                Texto = "Modelo cacheado"
            };

            //Act
            await cache.RemoveAsync(nameof(Model));
            await cache.GetOrSetObjectAsync(nameof(Model), () => Task.FromResult(value));

            var result = await cache.GetObjectAsync<Model>(nameof(Model));

            //Assert
            result.ShouldNotBeNull();

            result.Data.ShouldBe(value.Data);
            result.Enum.ShouldBe(value.Enum);
            result.Inteiro.ShouldBe(value.Inteiro);
            result.Texto.ShouldBe(value.Texto);
        }
    }
}