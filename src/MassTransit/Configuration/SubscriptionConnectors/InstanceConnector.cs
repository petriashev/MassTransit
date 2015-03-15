﻿// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.SubscriptionConnectors
{
    using System.Collections.Generic;
    using System.Linq;
    using Pipeline;
    using Policies;
    using Util;


    public interface InstanceConnector
    {
        ConnectHandle Connect(IConsumePipeConnector pipe, object instance, IRetryPolicy retryPolicy);
    }


    public class InstanceConnector<TConsumer> :
        InstanceConnector
        where TConsumer : class
    {
        readonly IEnumerable<InstanceMessageConnector> _connectors;

        public InstanceConnector()
        {
            if (TypeMetadataCache<TConsumer>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .Concat(ConsumesAll())
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToList();
        }

        public ConnectHandle Connect(IConsumePipeConnector pipe, object instance, IRetryPolicy retryPolicy)
        {
            return new MultipleConnectHandle(_connectors.Select(x => x.Connect(pipe, instance, retryPolicy)));
        }

        static IEnumerable<InstanceMessageConnector> Consumes()
        {
            return ConsumerMetadataCache<TConsumer>.ConsumerTypes.Select(x => x.GetInstanceConnector());
        }

        static IEnumerable<InstanceMessageConnector> ConsumesAll()
        {
            return ConsumerMetadataCache<TConsumer>.MessageConsumerTypes.Select(x => x.GetInstanceMessageConnector());
        }
    }
}