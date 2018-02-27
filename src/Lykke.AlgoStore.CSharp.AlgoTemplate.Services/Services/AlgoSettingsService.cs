using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IAlgoSettingsService"/> implementation for the algo settings
    /// </summary>
    public class AlgoSettingsService : IAlgoSettingsService
    {
        private string _settingsJson;
        private IDictionary<string, object> _settings;

        public void Initialize()
        {
            _settingsJson = Environment.GetEnvironmentVariable("ALGO_INSTANCE_PARAMS");

            if (String.IsNullOrEmpty(_settingsJson))
                throw new ArgumentException("Environment variable 'ALGO_INSTANCE_PARAMS' does not contain settings");

            dynamic dynamicSettings = JsonConvert.DeserializeObject<ExpandoObject>(_settingsJson);
            _settings = (IDictionary<string, object>) dynamicSettings;
        }

        public string GetSetting(string key)
        {
            if (!_settings.ContainsKey(key))
                return string.Empty;

            return _settings[key] as string;
        }

    }
}
