using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace PlusUltra.DistributedCache
{
    public class CustomCacheResolver : IFormatterResolver
    {
        // Resolver should be singleton.
		public static readonly IFormatterResolver Instance = new CustomCacheResolver();

		private CustomCacheResolver()
		{
		}

		// GetFormatter<T>'s get cost should be minimized so use type cache.
		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return Cache<T>.formatter;
		}

		static class Cache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static Cache()
			{
				foreach (var item in new[] { NativeDateTimeResolver.Instance, ContractlessStandardResolver.Instance })
				{
					var f = item.GetFormatter<T>();
					if (f != null)
					{
						formatter = f;
						return;
					}
				}
			}
		}
    }
}