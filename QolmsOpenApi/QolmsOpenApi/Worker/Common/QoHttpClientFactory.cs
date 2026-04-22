using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// HttpClientFactoryのラッパー
    /// </summary>
    public interface IQoHttpClientFactory
    {
        /// <summary>
        /// HttpClientを取得します。
        /// </summary>
        HttpClient GetClient();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class QoHttpClientFactory: IQoHttpClientFactory
    {
        static Lazy<IHttpClientFactory> _factory = new Lazy<IHttpClientFactory>(() =>
        {
            return new ServiceCollection()
            .AddHttpClient()
            .BuildServiceProvider()
            .GetRequiredService<IHttpClientFactory>();
        }, true);

        /// <inheritdoc/>
        public HttpClient GetClient() => _factory.Value.CreateClient();
    }
}