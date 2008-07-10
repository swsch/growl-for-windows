using System;
using System.Collections.Generic;
using System.Text;

namespace Vortex.Growl.DisplayStyle
{
    /// <summary>
    /// Provides the base implementation of the <see cref="IDisplay"/> interface.
    /// </summary>
    /// <remarks>
    /// Most developers should inherit their displays from this class, as it provides 
    /// useful implementation of most common properties and methods. If you choose not
    /// to inherit from this class and instead to implement <see cref="IDisplay"/> 
    /// directly, note that your display class must still inherit from <see cref="MarshalByRefObject"/>.
    /// </remarks>
    public abstract class Display : MarshalByRefObject, IDisplay
    {
        /// <summary>
        /// The full path to the installation directory of Growl
        /// </summary>
        private string growlApplicationPath;

        /// <summary>
        /// The full path to the installation directory of this display
        /// </summary>
        private string displayStylePath;

        /// <summary>
        /// The <see cref="SettingsPanelBase"/> used to allow user's to set display-specific settings.
        /// </summary>
        private SettingsPanelBase settingsPanel;

        /// <summary>
        /// A collection of user-configurable settings that can be modified by the associated <see cref="SettingsPanelBase"/>.
        /// </summary>
        private Dictionary<string, object> settingsCollection;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public Display()
        {
            this.settingsPanel = new DefaultSettingsPanel();
            this.settingsCollection = new Dictionary<string, object>();
        }

        #region IDisplay Members

        /// <summary>
        /// The name of the display as shown to the user in Growl's preference settings.
        /// </summary>
        /// <value>Ex: Mailman</value>
        public abstract string Name{get;}

        /// <summary>
        /// A short description of what the display is or does.
        /// </summary>
        /// <value>Ex: Mailman delivers Growl notifications via email</value>
        public abstract string Description { get;}

        /// <summary>
        /// The name of the author of the display.
        /// </summary>
        /// <value>Ex: Joe Schmoe</value>
        public abstract string Author { get;}

        /// <summary>
        /// The version of the display.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The display version is automatically set to return the version of the current assembly.
        /// </remarks>
        public virtual Version Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// The <see cref="SettingsPanelBase"/> used to allow user's to set display-specific settings.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// If your display has no user-configurable settings, you may use the <see cref="DefaultSettingsPanel"/>
        /// class.
        /// </remarks>
        public SettingsPanelBase SettingsPanel
        {
            get
            {
                return this.settingsPanel;
            }
            set
            {
                this.settingsPanel = value;
            }
        }

        /// <summary>
        /// The full path to the installation directory of the Growl program.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// This property is set by Growl when the display is loaded. Developers can use the information if
        /// required, but should not otherwise change the behavior of this property.
        /// </remarks>
        public string GrowlApplicationPath
        {
            get
            {
                return this.growlApplicationPath;
            }
            set
            {
                this.growlApplicationPath = value;
            }
        }

        /// <summary>
        /// The full path to the installation directory of this DisplayStyle.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// This property is set by Growl when the display is loaded. Developers can use the information if
        /// required, but should not otherwise change the behavior of this property.
        /// </remarks>
        public string DisplayStylePath
        {
            get
            {
                return this.displayStylePath;
            }
            set
            {
                this.displayStylePath = value;
            }
        }

        /// <summary>
        /// Stores a collection of user-configurable settings that can be modified by the associated
        /// <see cref="SettingsPanel"/>.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Although the dictionary allows storing any object, all settings must be serializable
        /// (either marked with the <see cref="SerializableAttribute"/> or implementing <see cref="ISerializable"/>).
        /// </remarks>
        public Dictionary<string, object> SettingsCollection
        {
            get
            {
                return this.settingsCollection;
            }
            set
            {
                this.settingsCollection = value;
            }
        }

        /// <summary>
        /// Called when the display is first loaded, generally used for any initialization-type actions.
        /// </summary>
        public virtual void Load()
        {
        }

        /// <summary>
        /// Called when the display is unloaded, generally used for any last-minute cleanup.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Handles displaying the notification. Called each time a notification is received that is to
        /// be handled by this display.
        /// </summary>
        /// <param name="notification">The <see cref="Notification"/> information</param>
        /// <param name="displayName">A string identifying the display name (used mainly by displays that provide multiple end-user selectable display styles)</param>
        public abstract void HandleNotification(Notification notification, string displayName);

        /// <summary>
        /// Returns a list of end-user selectable display names that this display supports.
        /// </summary>
        /// <returns>Array of display names</returns>
        /// <remarks>
        /// Most displays will only support a single end-user selectable display, so this method can usually
        /// just return:  string[] {this.Name};
        /// For developers who wish to support multiple displays with a single DisplayStyle engine, this
        /// method can return a list of display names that will appear as options for the user. When
        /// <see cref="HandleNotification"/> is called, the individual display name will be passed
        /// along with the notification.
        /// </remarks>
        public virtual string[] GetListOfAvailableDisplays()
        {
            return new string[] { this.Name };
        }

        #endregion

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"></see> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"></see> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/></PermissionSet>
        /// <remarks>
        /// Developers who do not inherit from this class and implement <see cref="IDisplay"/> directly must take care to manage
        /// their leases properly. If a lease expires while Growl is still running and the display is later accessed, a
        /// RemotingException will occur.
        /// </remarks>
        public override object InitializeLifetimeService()
        {
            // This lease never expires.
            return null;
        }
    }
}