﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConferenceModel.ConferenceSpaceAsyncWebservice {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ConferenceSpaceAsyncWebservice.ConferenceSpaceAsyncWebserviceSoap")]
    public interface ConferenceSpaceAsyncWebserviceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/FillSyncServiceData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        void FillSyncServiceData(string conferenceName, string sharer, string uri, ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/FillSyncServiceData", ReplyAction="*")]
        System.IAsyncResult BeginFillSyncServiceData(string conferenceName, string sharer, string uri, ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType, System.AsyncCallback callback, object asyncState);
        
        void EndFillSyncServiceData(System.IAsyncResult result);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum FileType {
        
        /// <remarks/>
        docx,
        
        /// <remarks/>
        doc,
        
        /// <remarks/>
        xlsx,
        
        /// <remarks/>
        xls,
        
        /// <remarks/>
        pptx,
        
        /// <remarks/>
        ppt,
        
        /// <remarks/>
        one,
        
        /// <remarks/>
        jpg,
        
        /// <remarks/>
        png,
        
        /// <remarks/>
        ico,
        
        /// <remarks/>
        mp4,
        
        /// <remarks/>
        avi,
        
        /// <remarks/>
        mp3,
        
        /// <remarks/>
        xml,
        
        /// <remarks/>
        txt,
        
        /// <remarks/>
        record,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ConferenceSpaceAsyncWebserviceSoapChannel : ConferenceModel.ConferenceSpaceAsyncWebservice.ConferenceSpaceAsyncWebserviceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConferenceSpaceAsyncWebserviceSoapClient : System.ServiceModel.ClientBase<ConferenceModel.ConferenceSpaceAsyncWebservice.ConferenceSpaceAsyncWebserviceSoap>, ConferenceModel.ConferenceSpaceAsyncWebservice.ConferenceSpaceAsyncWebserviceSoap {
        
        private BeginOperationDelegate onBeginFillSyncServiceDataDelegate;
        
        private EndOperationDelegate onEndFillSyncServiceDataDelegate;
        
        private System.Threading.SendOrPostCallback onFillSyncServiceDataCompletedDelegate;
        
        public ConferenceSpaceAsyncWebserviceSoapClient() {
        }
        
        public ConferenceSpaceAsyncWebserviceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConferenceSpaceAsyncWebserviceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConferenceSpaceAsyncWebserviceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConferenceSpaceAsyncWebserviceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> FillSyncServiceDataCompleted;
        
        public void FillSyncServiceData(string conferenceName, string sharer, string uri, ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType) {
            base.Channel.FillSyncServiceData(conferenceName, sharer, uri, fileType);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginFillSyncServiceData(string conferenceName, string sharer, string uri, ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginFillSyncServiceData(conferenceName, sharer, uri, fileType, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndFillSyncServiceData(System.IAsyncResult result) {
            base.Channel.EndFillSyncServiceData(result);
        }
        
        private System.IAsyncResult OnBeginFillSyncServiceData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string conferenceName = ((string)(inValues[0]));
            string sharer = ((string)(inValues[1]));
            string uri = ((string)(inValues[2]));
            ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType = ((ConferenceModel.ConferenceSpaceAsyncWebservice.FileType)(inValues[3]));
            return this.BeginFillSyncServiceData(conferenceName, sharer, uri, fileType, callback, asyncState);
        }
        
        private object[] OnEndFillSyncServiceData(System.IAsyncResult result) {
            this.EndFillSyncServiceData(result);
            return null;
        }
        
        private void OnFillSyncServiceDataCompleted(object state) {
            if ((this.FillSyncServiceDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.FillSyncServiceDataCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void FillSyncServiceDataAsync(string conferenceName, string sharer, string uri, ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType) {
            this.FillSyncServiceDataAsync(conferenceName, sharer, uri, fileType, null);
        }
        
        public void FillSyncServiceDataAsync(string conferenceName, string sharer, string uri, ConferenceModel.ConferenceSpaceAsyncWebservice.FileType fileType, object userState) {
            if ((this.onBeginFillSyncServiceDataDelegate == null)) {
                this.onBeginFillSyncServiceDataDelegate = new BeginOperationDelegate(this.OnBeginFillSyncServiceData);
            }
            if ((this.onEndFillSyncServiceDataDelegate == null)) {
                this.onEndFillSyncServiceDataDelegate = new EndOperationDelegate(this.OnEndFillSyncServiceData);
            }
            if ((this.onFillSyncServiceDataCompletedDelegate == null)) {
                this.onFillSyncServiceDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnFillSyncServiceDataCompleted);
            }
            base.InvokeAsync(this.onBeginFillSyncServiceDataDelegate, new object[] {
                        conferenceName,
                        sharer,
                        uri,
                        fileType}, this.onEndFillSyncServiceDataDelegate, this.onFillSyncServiceDataCompletedDelegate, userState);
        }
    }
}
