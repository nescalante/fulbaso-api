﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.544
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fulbaso.Common.Resources {
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
    internal class Html {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Html() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Fulbaso.Common.Resources.Html", typeof(Html).Assembly);
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
        ///   Looks up a localized string similar to script.
        /// </summary>
        internal static string ScriptTagName {
            get {
                return ResourceManager.GetString("ScriptTagName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to style.
        /// </summary>
        internal static string StyleTagName {
            get {
                return ResourceManager.GetString("StyleTagName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;[^&gt;]*(&gt;|$).
        /// </summary>
        internal static string Tag {
            get {
                return ResourceManager.GetString("Tag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;{0}&gt;.
        /// </summary>
        internal static string TagFormat {
            get {
                return ResourceManager.GetString("TagFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^&lt;a\shref=&quot;(\#\d+|(https?|ftp)://[-a-z0-9+&amp;@#/%?=~_|!:,.;\(\)]+)&quot;(\stitle=&quot;[^&quot;&lt;&gt;]+&quot;)?\s?&gt;$|^&lt;/a&gt;$.
        /// </summary>
        internal static string WhitelistedAnchor {
            get {
                return ResourceManager.GetString("WhitelistedAnchor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^&lt;img\ssrc=&quot;https?://[-a-z0-9+&amp;@#/%?=~_|!:,.;\(\)]+&quot;(\swidth=&quot;\d{1,3}&quot;)?(\sheight=&quot;\d{1,3}&quot;)?(\salt=&quot;[^&quot;&lt;&gt;]*&quot;)?(\stitle=&quot;[^&quot;&lt;&gt;]*&quot;)?\s?/?&gt;$.
        /// </summary>
        internal static string WhitelistedImage {
            get {
                return ResourceManager.GetString("WhitelistedImage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^&lt;/?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)&gt;$|^&lt;(b|h)r\s?/?&gt;$.
        /// </summary>
        internal static string WhitelistedTag {
            get {
                return ResourceManager.GetString("WhitelistedTag", resourceCulture);
            }
        }
    }
}
