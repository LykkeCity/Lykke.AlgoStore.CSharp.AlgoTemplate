using System;
using Autofac;
using Common.Log;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Client
{
    public static class AutofacExtension
    {
        public static void RegisterCSharpAlgoTemplateClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<CSharpAlgoTemplateClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ICSharpAlgoTemplateClient>()
                .SingleInstance();
        }

        public static void RegisterCSharpAlgoTemplateClient(this ContainerBuilder builder, CSharpAlgoTemplateServiceClientSettings settings, ILog log)
        {
            builder.RegisterCSharpAlgoTemplateClient(settings?.ServiceUrl, log);
        }
    }
}
