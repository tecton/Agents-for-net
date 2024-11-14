// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Hosting.AspNetCore.BackgroundQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Agents.Protocols.Primitives;

namespace Microsoft.Agents.Hosting.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAsyncCloudAdapterSupport(this IServiceCollection services)
        {
            // Activity specific BackgroundService for processing authenticated activities.
            services.AddHostedService<HostedActivityService>();
            // Generic BackgroundService for processing tasks.
            services.AddHostedService<HostedTaskService>();

            // BackgroundTaskQueue and ActivityTaskQueue are the entry points for
            // the enqueueing activities or tasks to be processed by the BackgroundService.
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IActivityTaskQueue, ActivityTaskQueue>();
        }

        /// <summary>
        /// Add the default CloudAdapter.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="async"></param>
        public static void AddCloudAdapter(this IServiceCollection services, bool async = true)
        {
            if (!async)
                services.AddCloudAdapter<CloudAdapter>();
            else
                services.AddAsyncCloudAdapter<AsyncCloudAdapter>();
        }

        /// <summary>
        /// Add the derived CloudAdapter.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="async"></param>
        public static void AddCloudAdapter<T>(this IServiceCollection services) where T : CloudAdapter
        {
            services.AddSingleton<CloudAdapter, T>();
            services.AddSingleton<IBotHttpAdapter>(sp => sp.GetService<CloudAdapter>());
            services.AddSingleton<IBotAdapter>(sp => sp.GetService<CloudAdapter>());
        }

        /// <summary>
        /// Add the Derived AsyncCloudAdapter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        public static void AddAsyncCloudAdapter<T>(this IServiceCollection services) where T : AsyncCloudAdapter
        {
            AddAsyncCloudAdapterSupport(services);
            services.AddSingleton<AsyncCloudAdapter, T>();
            services.AddSingleton<IBotHttpAdapter>(sp => sp.GetService<AsyncCloudAdapter>());
            services.AddSingleton<IBotAdapter>(sp => sp.GetService<AsyncCloudAdapter>());
        }
    }
}
