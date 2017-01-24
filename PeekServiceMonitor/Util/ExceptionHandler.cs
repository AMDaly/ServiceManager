using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;


namespace Peek.Common
{
    /// <summary>
    /// The ExceptionHandler class provides assorted helper functions to provide appropriate
    /// exception handling, especially last chance exception handling for applications.
    /// </summary>
    public class ExceptionHandler
    {
        private static readonly string[] PermittedDomains = { "signalgroup", "semex" };
        private static readonly bool AutomaticallySubmitReportDefault = PermittedDomains.Any(p => p.Equals(Environment.UserDomainName.ToLower()));

        private static void NonInteractiveLastChanceExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            LastChanceExceptionHandler(sender, e, false);
        }

        private static void InteractiveLastChanceExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            LastChanceExceptionHandler(sender, e, true);
        }

        /// <summary>
        /// A list of predicates that can be applied to exceptions passed to the last chance exception handling. If an exception passed to
        /// the last chance exception handling matches any of the predicates in this list it will be logged, but otherwise ignored.
        /// </summary>
        /// <remarks>
        /// This is necessary to prevent undesirable prompts being shown to the user in scenarios such as the ObjectDisposedException which is thrown
        /// on some machines when a USB device providing SerialPort capability is removed.
        /// (See http://connect.microsoft.com/VisualStudio/feedback/details/140018/serialport-crashes-after-disconnect-of-usb-com-port)
        /// </remarks>
        public static readonly List<Predicate<Exception>> LastChanceExceptionFilters = new List<Predicate<Exception>>();

        /// <summary>
        /// The "Last chance" exception handler does whatever is necessary if something
        /// really bad happens! (Specifically, the exception will be logged and if possible submitted to the
        /// issue tracking software).
        /// </summary>
        /// <param name="sender">The source of the exception.</param>
        /// <param name="e">The unhandled exception event arguments.</param>
        /// <param name="showMessages">if set to <c>true</c> show messages related to the error to the user.</param>
        private static void LastChanceExceptionHandler(object sender, UnhandledExceptionEventArgs e, bool showMessages)
        {
            try
            {
                // Log the exception
                var logger = LogManager.GetLogger(typeof(ExceptionHandler));
                logger.FatalFormat("LastChanceException: ", showMessages);

                if (e != null)
                {
                    if (e.ExceptionObject != null)
                    {
                        var exceptionToSubmit = e.ExceptionObject as Exception;

                        var filtered = LastChanceExceptionFilters.Any(exceptionFilter => exceptionFilter(exceptionToSubmit));

                        if (filtered)
                        {
                            logger.Debug("Ignoring filtered exception in last chance exception handling.", exceptionToSubmit);
                        }
                        else
                        {
                            logger.Fatal("Fatal exception", exceptionToSubmit);
                        }
                    }
                    else
                    {
                        logger.FatalFormat("Fatal exception with no exception object. Exception event arguments = {0}", e);
                    }
                }
                else
                {
                    logger.Fatal("Last chance exception handler triggered, but with no event arguments.");

                    if (sender != null)
                    {
                        logger.DebugFormat("Last chance exception triggered by {0}", sender.GetType().FullName);
                    }
                    else
                    {
                        logger.Debug("Last chance exception handler triggered by a null sender.");
                    }
                }
            }
            catch (Exception ex)
            {
                // There was an exception in the exception handling. Nasty!
                var logger = LogManager.GetLogger(typeof(ExceptionHandler));
                logger.Fatal("Fatal exception while processing an unhandled exception", ex);
            }
        }

       

        private static String GetFriendlyExceptionMessage(Exception exceptionToSubmit)
        {
            var friendlyExceptionMessage = new StringBuilder();
            friendlyExceptionMessage.AppendFormat("{0} ", exceptionToSubmit.GetType().Name);
            var stackDetails = exceptionToSubmit.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (stackDetails.Length > 0)
            {
                friendlyExceptionMessage.Append(stackDetails[0].Trim());
            }

            return friendlyExceptionMessage.ToString();
        }

        /// <summary>
        /// Used to determine if the current build is a local developer one, or if it came from the build
        /// machine.
        /// </summary>
        /// <returns><c>true</c> if the error is being reported in the context of a local build;
        /// <c>false</c> otherwise.</returns>
        private static bool ApplicationIsLocallyBuiltByDeveloper()
        {
            var assemblyBuild = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            // Build 9999 is the old (Subversion) revision reference for a developer build. 999 is the new
            // (Mercurial) one.
            return assemblyBuild == 9999 || assemblyBuild == 999;
        }
        
        /// <summary>
        /// Attaches the appropriate last chance exception handlers to an application domain.
        /// </summary>
        /// <param name="appDomain">A <see cref="System.AppDomain"/> object</param>
        /// <param name="interactive">if set to <c>true</c> the interactive version of the error handler (which
        /// presents a message to the user indicating that a problem has occurred) will be used, otherwise a non-interactive
        /// one will be used.</param>
        /// <remarks>Typically, this will be used as follows:
        /// <code>
        /// [STAThread]
        /// static void Main()
        /// {
        /// // Attach the exception handlers....
        /// ExceptionHandler.BindToAppDomain(System.AppDomain.CurrentDomain, true);
        /// // ... Rest of application startup here ...
        /// }
        /// </code>
        /// <para>Any service application <b>must</b> use the non-interactive form, since at the time any error occurs
        /// the service may not be capable of interacting with the user via interactive means.</para>
        /// </remarks>
        public static void BindToAppDomain(AppDomain appDomain, bool interactive)
        {
            // Attach our "Last Chance" exception handler.
            if (appDomain != null)
            {
                if (interactive)
                {
                    appDomain.UnhandledException += InteractiveLastChanceExceptionHandler;
                }
                else
                {
                    appDomain.UnhandledException += NonInteractiveLastChanceExceptionHandler;
                }
            }
        }
    }
}
