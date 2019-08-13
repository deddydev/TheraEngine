using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class TMessageBox : TheraForm
    {
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   displayHelpButton:
        //     true to show the Help button; otherwise, false. The default is false.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box in front of the specified object and with the specified
        //     text, caption, and buttons.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box in front of the specified object and with the specified
        //     text, caption, buttons, and icon.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box in front of the specified object and with the specified
        //     text, caption, buttons, icon, and default button.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- defaultButton is not
        //     a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box in front of the specified object and with the specified
        //     text, caption, buttons, icon, default button, and options.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values the specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- defaultButton is not
        //     a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- options specified
        //     System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly or System.Windows.Forms.MessageBoxOptions.ServiceNotification
        //     and specified a value in the owner parameter. These two options should be used
        //     only if you invoke the version of this method that does not take an owner parameter.-or-
        //     buttons specified an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with specified text.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        public static DialogResult Show(string text)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with specified text and caption.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        public static DialogResult Show(string text, string caption)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with specified text, caption, and buttons.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with specified text, caption, buttons, and icon.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     The buttons parameter specified is not a member of System.Windows.Forms.MessageBoxButtons.-or-
        //     The icon parameter specified is not a member of System.Windows.Forms.MessageBoxIcon.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box in front of the specified object and with the specified
        //     text and caption.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, and default
        //     button.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- defaultButton is not
        //     a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file, HelpNavigator,
        //     and Help topic.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        //   navigator:
        //     One of the System.Windows.Forms.HelpNavigator values.
        //
        //   param:
        //     The numeric ID of the Help topic to display when the user clicks the Help button.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file, HelpNavigator,
        //     and Help topic.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        //   navigator:
        //     One of the System.Windows.Forms.HelpNavigator values.
        //
        //   param:
        //     The numeric ID of the Help topic to display when the user clicks the Help button.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file and HelpNavigator.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        //   navigator:
        //     One of the System.Windows.Forms.HelpNavigator values.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file and HelpNavigator.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        //   navigator:
        //     One of the System.Windows.Forms.HelpNavigator values.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file and Help keyword.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        //   keyword:
        //     The Help keyword to display when the user clicks the Help button.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file and Help keyword.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        //   keyword:
        //     The Help keyword to display when the user clicks the Help button.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, options, and Help button, using the specified Help file.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        //   helpFilePath:
        //     The path and name of the Help file to display when the user clicks the Help button.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box with the specified text, caption, buttons, icon, default
        //     button, and options.
        //
        // Parameters:
        //   text:
        //     The text to display in the message box.
        //
        //   caption:
        //     The text to display in the title bar of the message box.
        //
        //   buttons:
        //     One of the System.Windows.Forms.MessageBoxButtons values that specifies which
        //     buttons to display in the message box.
        //
        //   icon:
        //     One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon
        //     to display in the message box.
        //
        //   defaultButton:
        //     One of the System.Windows.Forms.MessageBoxDefaultButton values that specifies
        //     the default button for the message box.
        //
        //   options:
        //     One of the System.Windows.Forms.MessageBoxOptions values that specifies which
        //     display and association options will be used for the message box. You may pass
        //     in 0 if you wish to use the defaults.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     buttons is not a member of System.Windows.Forms.MessageBoxButtons.-or- icon is
        //     not a member of System.Windows.Forms.MessageBoxIcon.-or- The defaultButton specified
        //     is not a member of System.Windows.Forms.MessageBoxDefaultButton.
        //
        //   T:System.InvalidOperationException:
        //     An attempt was made to display the System.Windows.Forms.MessageBox in a process
        //     that is not running in User Interactive mode. This is specified by the System.Windows.Forms.SystemInformation.UserInteractive
        //     property.
        //
        //   T:System.ArgumentException:
        //     options specified both System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly
        //     and System.Windows.Forms.MessageBoxOptions.ServiceNotification.-or- buttons specified
        //     an invalid combination of System.Windows.Forms.MessageBoxButtons.
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            return DialogResult.OK;
        }
        //
        // Summary:
        //     Displays a message box in front of the specified object and with the specified
        //     text.
        //
        // Parameters:
        //   owner:
        //     An implementation of System.Windows.Forms.IWin32Window that will own the modal
        //     dialog box.
        //
        //   text:
        //     The text to display in the message box.
        //
        // Returns:
        //     One of the System.Windows.Forms.DialogResult values.
        public static DialogResult Show(IWin32Window owner, string text)
        {
            TMessageBox m = new TMessageBox();
            m.Message = text;
            m.Show(owner);
            return m.DialogResult;
        }
    }
}
