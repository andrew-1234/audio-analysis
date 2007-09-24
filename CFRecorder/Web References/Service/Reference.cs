﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.CompactFramework.Design.Data, Version 2.0.50727.312.
// 
namespace QUT.Service {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://mquter.qut.edu.au/sensors/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ActiveRecordHooksBase))]
    public partial class Service : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public Service() {
            this.Url = "http://www.mquter.qut.edu.au/sensor/demo/Service.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://mquter.qut.edu.au/sensors/TestConnection", RequestNamespace="http://mquter.qut.edu.au/sensors/", ResponseNamespace="http://mquter.qut.edu.au/sensors/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool TestConnection() {
            object[] results = this.Invoke("TestConnection", new object[0]);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginTestConnection(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("TestConnection", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndTestConnection(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://mquter.qut.edu.au/sensors/AddPhotoReading", RequestNamespace="http://mquter.qut.edu.au/sensors/", ResponseNamespace="http://mquter.qut.edu.au/sensors/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool AddPhotoReading(System.Guid deploymentID, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<System.Guid> readingGuid, System.DateTime time, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] buffer) {
            object[] results = this.Invoke("AddPhotoReading", new object[] {
                        deploymentID,
                        readingGuid,
                        time,
                        buffer});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginAddPhotoReading(System.Guid deploymentID, System.Nullable<System.Guid> readingGuid, System.DateTime time, byte[] buffer, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AddPhotoReading", new object[] {
                        deploymentID,
                        readingGuid,
                        time,
                        buffer}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndAddPhotoReading(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://mquter.qut.edu.au/sensors/AddAudioReading", RequestNamespace="http://mquter.qut.edu.au/sensors/", ResponseNamespace="http://mquter.qut.edu.au/sensors/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool AddAudioReading(System.Guid deploymentID, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<System.Guid> readingGuid, System.DateTime time, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] buffer) {
            object[] results = this.Invoke("AddAudioReading", new object[] {
                        deploymentID,
                        readingGuid,
                        time,
                        buffer});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginAddAudioReading(System.Guid deploymentID, System.Nullable<System.Guid> readingGuid, System.DateTime time, byte[] buffer, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AddAudioReading", new object[] {
                        deploymentID,
                        readingGuid,
                        time,
                        buffer}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndAddAudioReading(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://mquter.qut.edu.au/sensors/AddAudioReadingWithType", RequestNamespace="http://mquter.qut.edu.au/sensors/", ResponseNamespace="http://mquter.qut.edu.au/sensors/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool AddAudioReadingWithType(System.Guid deploymentID, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<System.Guid> readingGuid, System.DateTime time, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] buffer, string mimeType) {
            object[] results = this.Invoke("AddAudioReadingWithType", new object[] {
                        deploymentID,
                        readingGuid,
                        time,
                        buffer,
                        mimeType});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginAddAudioReadingWithType(System.Guid deploymentID, System.Nullable<System.Guid> readingGuid, System.DateTime time, byte[] buffer, string mimeType, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AddAudioReadingWithType", new object[] {
                        deploymentID,
                        readingGuid,
                        time,
                        buffer,
                        mimeType}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndAddAudioReadingWithType(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://mquter.qut.edu.au/sensors/GetLatestDeployment", RequestNamespace="http://mquter.qut.edu.au/sensors/", ResponseNamespace="http://mquter.qut.edu.au/sensors/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Deployment GetLatestDeployment(string sensorID) {
            object[] results = this.Invoke("GetLatestDeployment", new object[] {
                        sensorID});
            return ((Deployment)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetLatestDeployment(string sensorID, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetLatestDeployment", new object[] {
                        sensorID}, callback, asyncState);
        }
        
        /// <remarks/>
        public Deployment EndGetLatestDeployment(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((Deployment)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://mquter.qut.edu.au/sensors/StartDeployment", RequestNamespace="http://mquter.qut.edu.au/sensors/", ResponseNamespace="http://mquter.qut.edu.au/sensors/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Deployment StartDeployment(string sensorID, string name) {
            object[] results = this.Invoke("StartDeployment", new object[] {
                        sensorID,
                        name});
            return ((Deployment)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginStartDeployment(string sensorID, string name, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("StartDeployment", new object[] {
                        sensorID,
                        name}, callback, asyncState);
        }
        
        /// <remarks/>
        public Deployment EndStartDeployment(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((Deployment)(results[0]));
        }
    }
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://mquter.qut.edu.au/sensors/")]
    public partial class Deployment : ActiveRecordBaseOfDeployment {
        
        /// <remarks/>
        public System.Guid DeploymentID;
        
        /// <remarks/>
        public Hardware Hardware;
        
        /// <remarks/>
        public string Name;
        
        /// <remarks/>
        public System.DateTime DateStarted;
        
        /// <remarks/>
        public string Description;
        
        /// <remarks/>
        public string Longitude;
        
        /// <remarks/>
        public string Latitude;
    }
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://mquter.qut.edu.au/sensors/")]
    public partial class Hardware : ActiveRecordBaseOfHardware {
        
        /// <remarks/>
        public int HardwareID;
        
        /// <remarks/>
        public string UniqueID;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Hardware))]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://mquter.qut.edu.au/sensors/")]
    public abstract partial class ActiveRecordBaseOfHardware : ActiveRecordBase {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ActiveRecordBaseOfHardware))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Hardware))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ActiveRecordBaseOfDeployment))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Deployment))]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://mquter.qut.edu.au/sensors/")]
    public abstract partial class ActiveRecordBase : ActiveRecordHooksBase {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ActiveRecordBase))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ActiveRecordBaseOfHardware))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Hardware))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ActiveRecordBaseOfDeployment))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Deployment))]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://mquter.qut.edu.au/sensors/")]
    public abstract partial class ActiveRecordHooksBase {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Deployment))]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://mquter.qut.edu.au/sensors/")]
    public abstract partial class ActiveRecordBaseOfDeployment : ActiveRecordBase {
    }
}
