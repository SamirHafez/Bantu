﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StorageService.Notification {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Notification.INotificationService")]
    public interface INotificationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/INotificationService/RegisterEndpoint", ReplyAction="http://tempuri.org/INotificationService/RegisterEndpointResponse")]
        void RegisterEndpoint(string username, string endpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/INotificationService/UnregisterEndpoint", ReplyAction="http://tempuri.org/INotificationService/UnregisterEndpointResponse")]
        void UnregisterEndpoint(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/INotificationService/Notify", ReplyAction="http://tempuri.org/INotificationService/NotifyResponse")]
        void Notify(string from, string to, string gameId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface INotificationServiceChannel : StorageService.Notification.INotificationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class NotificationServiceClient : System.ServiceModel.ClientBase<StorageService.Notification.INotificationService>, StorageService.Notification.INotificationService {
        
        public NotificationServiceClient() {
        }
        
        public NotificationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public NotificationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NotificationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NotificationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void RegisterEndpoint(string username, string endpoint) {
            base.Channel.RegisterEndpoint(username, endpoint);
        }
        
        public void UnregisterEndpoint(string username) {
            base.Channel.UnregisterEndpoint(username);
        }
        
        public void Notify(string from, string to, string gameId) {
            base.Channel.Notify(from, to, gameId);
        }
    }
}