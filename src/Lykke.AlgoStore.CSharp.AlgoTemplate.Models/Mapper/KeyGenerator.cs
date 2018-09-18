﻿using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper
{
    public static class KeyGenerator
    {
        private const string AlgoPartitionKeyStatic = "algo";
        private const string ClientPartitionKeyStatic = "client";
        private const string PartitionKeySeparator = "_";
        private const string PartitionKeyPattern = "{0}{1}{2}";
        private const string WalletPartitionKeyStatic = "wallet";
        private const string AuthTokenPartitionKeyStatic = "authtoken";
        private const string EndDatePartitionKeyStatic = "StoppingEntity";
        private const string TcBuildPartitionKeyStatic = "TcBuildEntity";
        private const string FakeLimitOrderPrefixStatic = "FakeOrderId";

        public static string GenerateKey(string clientId, string algoId)
        {
            return string.Format(PartitionKeyPattern, clientId, PartitionKeySeparator, algoId);
        }

        public static string GenerateClientIdPartitionKey(string clientId)
        {
            return string.Format(PartitionKeyPattern, ClientPartitionKeyStatic, PartitionKeySeparator, clientId);
        }

        public static string GenerateWalletIdPartitionKey(string walletId)
        {
            return string.Format(PartitionKeyPattern, WalletPartitionKeyStatic, PartitionKeySeparator, walletId);
        }

        public static string GenerateAlgoIdPartitionKey(string algoId)
        {
            return string.Format(PartitionKeyPattern, AlgoPartitionKeyStatic, PartitionKeySeparator, algoId);
        }

        public static string GenerateAlgoIdAndClientIdPartitionKey(string algoId, string clientId)
        {
            return string.Format(PartitionKeyPattern, algoId, PartitionKeySeparator, clientId);
        }

        public static string GenerateAlgoIdAndInstanceTypePartitionKey(string algoId, AlgoInstanceType instanceType)
        {
            return string.Format(PartitionKeyPattern, algoId, PartitionKeySeparator, instanceType.GetDisplayName());
        }

        public static string GenerateAuthTokenPartitionKey(string authToken)
        {
            return string.Format(PartitionKeyPattern, AuthTokenPartitionKeyStatic, PartitionKeySeparator, authToken);
        }

        public static string GenerateStoppingEntityPartitionKey()
        {
            return EndDatePartitionKeyStatic;
        }

        public static string GenerateTcBuildEntityPartitionKey()
        {
            return TcBuildPartitionKeyStatic;
        }

        public static string GenerateStoppingEntityRowKey(DateTime instanceEndDate)
        {
            return instanceEndDate.Ticks.ToString();
        }

        public static string GenerateFakeLimitOrderPartitionKey(Guid limitOrderId)
        {
            return string.Format(PartitionKeyPattern, FakeLimitOrderPrefixStatic, PartitionKeySeparator, limitOrderId);
        }

        public static BaseAlgoData ParseKey(string partitionKey)
        {
            var values = partitionKey.Split(PartitionKeySeparator);
            if (values == null || values.Length != 2)
                return null;

            if (values[0] == AlgoPartitionKeyStatic)
            {
                return new BaseAlgoData
                {
                    ClientId = null,
                    AlgoId = values[1]
                };
            }
            else if (values[0] == ClientPartitionKeyStatic)
            {
                return new BaseAlgoData
                {
                    ClientId = values[1],
                    AlgoId = null
                };
            }
            else return null;     
        }       
    }
}
