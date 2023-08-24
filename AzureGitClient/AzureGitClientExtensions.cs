using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AzureGitClient.Services;

namespace AzureGitClient
{
    public static class AzureGitClientExtensions
    {
        public static IServiceCollection AddAzureGitAutomation(this IServiceCollection services)
        {
            services.AddSingleton<IFileEditor, FileEditor>();
            services.AddSingleton<IAzureDevOpsHttpClient, AzureDevOpsHttpClient>();
           
            return services;
        }
    }
}