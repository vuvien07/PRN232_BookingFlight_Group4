using System.Reflection;

namespace BookingFlightServer.Utils
{
	public static class ServiceExtension
	{
		public static void AddAllServices(this IServiceCollection services, Assembly assembly)
		{
			var serviceTypes = assembly.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
				.ToList();
			foreach (var serviceType in serviceTypes)
			{
				var interfaceType = serviceType.GetInterfaces().FirstOrDefault();
				if (interfaceType != null)
				{
					services.AddScoped(interfaceType, serviceType);
				}
			}
		}

		public static void AddAllRepositories(this IServiceCollection services, Assembly assembly)
		{
			var repositoryTypes = assembly.GetTypes()
		.Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
		.ToList();
			foreach (var repositoryType in repositoryTypes)
			{
				var interfaceType = repositoryType.GetInterfaces().FirstOrDefault(i => i.Name == $"I{repositoryType.Name}");
				if (interfaceType != null)
				{
					services.AddScoped(interfaceType, repositoryType);
				}
			}
		}
	}
}
