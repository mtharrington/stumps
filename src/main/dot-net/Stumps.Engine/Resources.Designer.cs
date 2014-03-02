﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stumps {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Stumps.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://*:{0}/.
        /// </summary>
        internal static string HttpServerPattern {
            get {
                return ResourceManager.GetString("HttpServerPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Accept.
        /// </summary>
        internal static string LogAcceptError {
            get {
                return ResourceManager.GetString("LogAcceptError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to App errors on disconnect notification..
        /// </summary>
        internal static string LogAppDisconnectErrors {
            get {
                return ResourceManager.GetString("LogAppDisconnectErrors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ERROR]: .
        /// </summary>
        internal static string LogExceptionPrefix {
            get {
                return ResourceManager.GetString("LogExceptionPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [INFO]: .
        /// </summary>
        internal static string LogInformationPrefix {
            get {
                return ResourceManager.GetString("LogInformationPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception when populating headers..
        /// </summary>
        internal static string LogProxyHeaderError {
            get {
                return ResourceManager.GetString("LogProxyHeaderError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception during remote server call..
        /// </summary>
        internal static string LogProxyRemoteError {
            get {
                return ResourceManager.GetString("LogProxyRemoteError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception during request processing..
        /// </summary>
        internal static string LogRequestProcessingException {
            get {
                return ResourceManager.GetString("LogRequestProcessingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to resolve handles. Disconnect notifications will be ignored..
        /// </summary>
        internal static string LogUnableToSetup {
            get {
                return ResourceManager.GetString("LogUnableToSetup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected exception..
        /// </summary>
        internal static string LogUnexpectedException {
            get {
                return ResourceManager.GetString("LogUnexpectedException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to not:.
        /// </summary>
        internal static string NotPattern {
            get {
                return ResourceManager.GetString("NotPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The port is already in use..
        /// </summary>
        internal static string PortIsInUseError {
            get {
                return ResourceManager.GetString("PortIsInUseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to regex:.
        /// </summary>
        internal static string RegExPattern {
            get {
                return ResourceManager.GetString("RegExPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A stump with the same name already exists..
        /// </summary>
        internal static string StumpNameUsedError {
            get {
                return ResourceManager.GetString("StumpNameUsedError", resourceCulture);
            }
        }
    }
}
