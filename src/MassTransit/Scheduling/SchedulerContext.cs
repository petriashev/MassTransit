﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;


    public interface SchedulerContext<T> :
        SchedulerContext
        where T : class
    {
        /// <summary>
        /// Schedule the message to be redelivered after the specified delay.
        /// </summary>
        /// <param name="delay">The minimum delay before the message will be redelivered to the queue</param>
        /// <returns></returns>
        Task ScheduleRedelivery(TimeSpan delay);
    }


    public interface SchedulerContext
    {
        Task ScheduleSend<T>(T message, TimeSpan deliveryDelay, IPipe<SendContext<T>> sendPipe)
            where T : class;

        Task ScheduleSend<T>(T message, DateTime deliveryTime, IPipe<SendContext<T>> sendPipe)
            where T : class;
    }
}