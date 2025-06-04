using ModelContextProtocol.Server;
using System.ComponentModel;

#if WINDOWS

using System.ServiceProcess;

#endif

namespace OnPremMCPAgent.Tools
{
    [McpServerToolType]
    public class ServiceTool
    {
#if WINDOWS
        private readonly TimeSpan waitForStatusTimeOut = TimeSpan.FromSeconds(60);
#endif

        [McpServerTool, Description("Lists all Windows services with their status.")]
        public List<string> ListServices()
        {
#if WINDOWS
            try
            {
                return [.. ServiceController.GetServices().Select(s => $"{s.ServiceName} ({s.Status})")];
            }
            catch (Exception ex)
            {
                return [$"Error: {ex.Message}"];
            }
#else
            return ["Error: Listing services is only supported on Windows."];
#endif
        }

        [McpServerTool, Description("Restarts a specific Windows service by name.")]
        public string RestartService(string serviceName)
        {
#if WINDOWS
            try
            {
                using var sc = new ServiceController(serviceName);

                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, waitForStatusTimeOut);
                }

                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, waitForStatusTimeOut);

                return $"Service '{serviceName}' restarted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error restarting '{serviceName}': {ex.Message}";
            }
#else
            return $"Error: Restarting services is only supported on Windows.";
#endif
        }

        [McpServerTool, Description("Starts a specific Windows service by name.")]
        public string StartService(string serviceName)
        {
#if WINDOWS
            try
            {
                using var sc = new ServiceController(serviceName);

                if (sc.Status == ServiceControllerStatus.Running)
                    return $"Service '{serviceName}' is already running.";

                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, waitForStatusTimeOut);

                return $"Service '{serviceName}' started successfully.";
            }
            catch (Exception ex)
            {
                return $"Error starting '{serviceName}': {ex.Message}";
            }
#else
            return $"Error: Starting services is only supported on Windows.";
#endif
        }

        [McpServerTool, Description("Stops a specific Windows service by name.")]
        public string StopService(string serviceName)
        {
#if WINDOWS
            try
            {
                using var sc = new ServiceController(serviceName);

                if (!sc.CanStop)
                    return $"Service '{serviceName}' cannot be stopped.";

                if (sc.Status == ServiceControllerStatus.Stopped)
                    return $"Service '{serviceName}' is already stopped.";

                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, waitForStatusTimeOut);

                return $"Service '{serviceName}' stopped successfully.";
            }
            catch (Exception ex)
            {
                return $"Error stopping '{serviceName}': {ex.Message}";
            }
#else
            return $"Error: Stopping services is only supported on Windows.";
#endif
        }

        [McpServerTool, Description("Checks the status of a specific Windows service by name.")]
        public string CheckStatus(string serviceName)
        {
#if WINDOWS
            try
            {
                using var sc = new ServiceController(serviceName);
                return $"Service '{serviceName}' status: {sc.Status}";
            }
            catch (Exception ex)
            {
                return $"Error checking status of '{serviceName}': {ex.Message}";
            }
#else
            return $"Error: Checking service status is only supported on Windows.";
#endif
        }
    }
}