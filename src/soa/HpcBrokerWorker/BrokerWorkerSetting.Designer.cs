﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Hpc.Scheduler.Session.Internal.BrokerShim {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.8.0.0")]
    internal sealed partial class BrokerWorkerSetting : global::System.Configuration.ApplicationSettingsBase {
        
        private static BrokerWorkerSetting defaultInstance = ((BrokerWorkerSetting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new BrokerWorkerSetting())));
        
        public static BrokerWorkerSetting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Debug {
            get {
                return ((bool)(this["Debug"]));
            }
            set {
                this["Debug"] = value;
            }
        }
    }
}