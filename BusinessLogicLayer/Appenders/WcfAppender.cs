using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoItBot.BotServices;
using log4net.Appender;
using log4net.Core;

namespace AutoItBot.Appenders
{
    public class WcfAppender : AppenderSkeleton
    {
        private Queue<LoggingEvent> _eventQueue;
        private WcfAppenderClient _proxyInstance = new WcfAppenderClient();

        public string RemoteAddress { get; set; }
        public int QueueSize { get; set; }
        public Level FlushLevel { get; set; }
        public SecurityMode SecurityMode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseDefaultWebProxy { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            _eventQueue.Enqueue(loggingEvent);
            if (_eventQueue.Count > QueueSize || loggingEvent.Level.Value >= FlushLevel.Value)
            {
                Action flush = FlushQueue;
                flush.BeginInvoke(null, null);
            }
        }

        protected override void OnClose()
        {
            FlushQueue();
            base.OnClose();
        }

        private void FlushQueue()
        {
            try
            {
                LoggingEventContainer[] container = new LoggingEventContainer[_eventQueue.Count];
                LoggingEvent[] events = _eventQueue.ToArray();

                for (int i = 0; i < events.Length; i++)
                {
                    LoggingEvent loggingEvent = events[i];
                    container[i] = new LoggingEventContainer()
                                       {
                                           Domain = loggingEvent.Domain,
                                           Exception = loggingEvent.GetExceptionString(),
                                           Identity = loggingEvent.Identity,
                                           LogLevel = loggingEvent.Level.ToString(),
                                           LoggerName = loggingEvent.LoggerName,
                                           Message = loggingEvent.RenderedMessage,
                                           ThreadName = loggingEvent.ThreadName,
                                           Timestamp = loggingEvent.TimeStamp
                                       };
                }
                
                ProxyInstance.AppendLoggingEvents(container);
                _eventQueue.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //a proxy can only fault once. an exception will cause the proxy to transition
            //to faulted state and we have to create a completely new instace of the proxy (done in the property)
            if (_proxyInstance != null && _proxyInstance.State == CommunicationState.Faulted)
            {
                _proxyInstance = null;
            }
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            _eventQueue = new Queue<LoggingEvent>(QueueSize + 1);
        }

        public WcfAppenderClient ProxyInstance
        {
            get
            {
                if (_proxyInstance == null)
                {
                    _proxyInstance = CreateProxyInstance();
                    return _proxyInstance;
                }
                return _proxyInstance;
            }
        }

        private WcfAppenderClient CreateProxyInstance()
        {
            EndpointAddress endpoint = new EndpointAddress(RemoteAddress);
            WSHttpBinding binding = new WSHttpBinding(SecurityMode); ;

            WcfAppenderClient client = new WcfAppenderClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = Username; 
            client.ClientCredentials.UserName.Password = Password;

            return client;
        }
    }
}
