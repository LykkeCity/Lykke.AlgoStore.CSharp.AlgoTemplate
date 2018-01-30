using System;
using Common.Log;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Client
{
    public class CSharpAlgoTemplateClient : ICSharpAlgoTemplateClient, IDisposable
    {
        private readonly ILog _log;

        public CSharpAlgoTemplateClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
