using System;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;

namespace PlusUltra.DistributedCache
{
public static class CacheStoreExtensions
	{
		/// <summary>
		/// Cria um valor no Redis.
		/// </summary>
		/// <param name="key">Chave.</param>
		/// <param name="data">Objeto a ser inserido.</param>
		/// <param name="time">Expiração do valor, se não informado</param>
		/// <returns></returns>
		public static async Task SetObjectAsync(this IDistributedCache source, string key, object data, TimeSpan? time = null)
		{
			if (data == null)
				throw new ArgumentException("data", "Must not be null");


			var bytes = MessagePackSerializer.Serialize(data, CustomCacheResolver.Instance);

			DistributedCacheEntryOptions options = null;

			if (time.HasValue)
				options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(time.Value);

			if (options != null)
				await source.SetAsync(key, bytes, options);
			else
				await source.SetAsync(key, bytes);
		}

		public static void SetObject(this IDistributedCache source, string key, object data, TimeSpan? time = null)
		{
			if (data == null)
				throw new ArgumentException("data", "Must not be null");

			var bytes = MessagePackSerializer.Serialize(data, CustomCacheResolver.Instance);

			DistributedCacheEntryOptions options = null;

			if (time.HasValue)
				options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(time.Value);

			if (options != null)
				source.Set(key, bytes, options);
			else
				source.Set(key, bytes);
		}

		/// <summary>
		/// Obtém um valor de uma determinada chave no redis.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key">Chave</param>
		/// <typeparam name="T">Tipo a ser convertido.</typeparam>
		/// <returns></returns>
		public static async Task<T> GetObjectAsync<T>(this IDistributedCache source, string key)
		{
			var data = await source.GetAsync(key);
			if (data == null) return default;

			return MessagePackSerializer.Deserialize<T>(data, CustomCacheResolver.Instance);
		}

		/// <summary>
		/// Obtém o valor de uma chave no redis.
		/// Caso o item não exista, o valor da função fallback será retornado
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key">Chave do registro</param>
		/// <param name="fallback">Função que retorna o item a ser persistido, caso não exista a chave.</param>
		public static async Task<T> GetObjectAsync<T>(this IDistributedCache source, string key, Func<Task<T>> fallback)
		{
			//Validar se existe o dado cacheado
			var data = await source.GetObjectAsync<T>(key);
			if (data != null)
				return data;

			var result = await fallback();

			return result;
		}

		/// <summary>
		/// Obtém o valor de uma chave no redis.
		/// Caso o item não exista, o valor da função fallback será persistido.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key">Chave do registro</param>
		/// <param name="time">Tempo de expiração.</param>
		/// <param name="fallback">Função que retorna o item a ser persistido, caso não exista a chave.</param>
		public static async Task<T> GetOrSetObjectAsync<T>(this IDistributedCache source, string key, Func<Task<T>> fallback, TimeSpan? time = null)
		{
			//Validar se existe o dado cacheado
			var data = await source.GetObjectAsync<T>(key);
			if (data != null)
				return data;

			var result = await fallback();

			if (result != null)
				await source.SetObjectAsync(key, result, time);

			return result;
		}

		/// <summary>
		/// Obtém o valor de uma chave no redis.
		/// Caso o item não exista, o valor da função fetch será persistido.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key">Chave do registro</param>
		/// <param name="time">Tempo de expiração.</param>
		/// <param name="fallback">Função que retorna o item a ser persistido, caso não exista a chave.</param>
		public static async Task<string> GetOrSetStringAsync(this IDistributedCache source, string key, Func<Task<string>> fallback, TimeSpan? time = null)
		{
			//Validar se existe o dado cacheado
			var data = await source.GetStringAsync(key);
			if (data != null)
				return data;

			var result = await fallback();

			if (result == null)
				return result;

			DistributedCacheEntryOptions options = null;

			if (time.HasValue)
				options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(time.Value);

			if (options != null)
				await source.SetStringAsync(key, result, options);
			else
				await source.SetStringAsync(key, result);

			return result;
		}
	}
}