using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Proxima
{
    /// Proxima Inspector enables remote inspecting, debugging, and control of a Unity application.
    [HelpURL("https://www.unityproxima.com/docs")]
    public class ProximaInspector : MonoBehaviour
    {
        [SerializeField, Tooltip("The name displayed to show in the browser when connected.")]
        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }

        [SerializeField, Tooltip("The port number to host the embedded Proxima server.")]
        private int _port = 7759;
        public int Port
        {
            get => _port;
            set => _port = value;
        }

        [SerializeField, Tooltip("The password required to connect to Proxima. See unityproxima.com/docs/security for more information.")]
        private string _password = "";
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        [SerializeField, Tooltip("Enables and disables HTTPS for encryption. See unityproxima.com/docs/security for more information.")]
        private bool _useHttps = false;
        public bool UseHttps
        {
            get => _useHttps;
            set => _useHttps = value;
        }

        [SerializeField, Tooltip("Optional TLS certificate. By default Proxima uses Proxima/Resources/Proxima/ProximaEmbeddedCert.pfx.")]
        private PfxAsset _certificate;
        public PfxAsset Certificate
        {
            get => _certificate;
            set => _certificate = value;
        }

        [SerializeField, Tooltip("Password for the TLS certificate.")]
        private string _certificatePassword;
        public string CertificatePassword
        {
            get => _certificatePassword;
            set => _certificatePassword = value;
        }

        [SerializeField, Tooltip("Automatically starts the Proxima server when this component is enabled.")]
        private bool _runOnEnable = true;
        public bool StartOnEnable
        {
            get => _runOnEnable;
            set => _runOnEnable = value;
        }

        [SerializeField, Tooltip("Maximum number of log messages to keep in memory.")]
        private int _logBufferSize = 1000;
        public int LogBufferSize
        {
            get => _logBufferSize;
            set => ProximaLogCommands.SetLogCapacity(value);
        }

        // Instantiates Proxima/Resources/Proxima/ProximaStatusUI.prefab on startup.
        [SerializeField, Tooltip("Instantiates UI that shows the current status of Proxima at the bottom of your screen.")]
        private bool _instantiateStatusUI = true;
        public bool InstantiateStatusUI
        {
            get => _instantiateStatusUI;
            set => _instantiateStatusUI = value;
        }

        // Instantiates Proxima/Resources/Proxima/ProximaConnectUI.prefab on startup.
        [SerializeField, Tooltip("Instantiates UI that allows the user to start and stop Proxima.")]
        private bool _instantiateConnectUI = false;
        public bool InstantiateConnectUI
        {
            get => _instantiateConnectUI;
            set => _instantiateConnectUI = value;
        }

        // Adds the gameObject with the Proxima Inspector to the DontDestroyOnLoad scene,
        // which keeps connections alive when transitioning between scenes.
        [SerializeField, Tooltip("Adds the gameObject with the Proxima Inspector to the DontDestroyOnLoad scene.")]
        private bool _dontDestroyOnLoad = true;

        // When Proxima starts, sets Application.runInBackground to true. When Proxima stops,
        // sets Application.runInBackground back to its previous value. This allows Proxima
        // to work when connecting from a browseer on the same device, since normally Unity
        // will pause the app when focus is set to the browser.
        [SerializeField, Tooltip("When Proxima starts, sets Application.runInBackground to true.")]
        private bool _setRunInBackground = true;

        // Stores the current status of Proxima and raises events when it changes.
        public ProximaStatus Status = new ProximaStatus();

        public enum ConnectionTypes
        {
            LocalNetwork = 1,
            Internet = 3,
#if PROXIMA_DEBUG
            Demo = 2,
            Debug = 0
#endif
        }

        [SerializeField, Tooltip("Is the Proxima server running on the local network or proxied through the internet?")]
        private ConnectionTypes _connectionType = ConnectionTypes.LocalNetwork;
        public ConnectionTypes ConnectionType
        {
            get => _connectionType;
            set => _connectionType = value;
        }

        /// URL of the remote Proxima Server.
        [SerializeField, Tooltip("URL of the Proxima Proxy Server.")]
        private string _serverUrl = "";
        public string ServerUrl
        {
            get => _serverUrl;
            set => _serverUrl = value;
        }

        /// AppId used for remote access. Create an app at unityproxima.com
        [SerializeField, Tooltip("AppId used for remote access. Create an app at unityproxima.com")]
        private long _appId;
        public long AppId
        {
            get => _appId;
            set => _appId = value;
        }

        /// Unique to add to URL for internet connections. This should be unique PER DEVICE.
        /// If unset, a random code will be generated.
        [SerializeField, Tooltip("Unique to add to URL for internet connections. This should be unique PER DEVICE.")]
        private string _uniqueName = "";
        public string UniqueName
        {
            get => _uniqueName;
            set => _uniqueName = value;
        }

        /// Automatically upload logs to the Proxima server.
        [SerializeField, Tooltip("Automatically upload logs to the Proxima server.")]
        private bool _uploadLogs = false;
        public bool UploadLogs
        {
            get => _uploadLogs;
            set => _uploadLogs = value;
        }

        /// Performance options
        public static int MaxGameObjectUpdatesPerFrame = 10;
        public static int MaxComponentUpdateFrequency = 10;

        private struct OpenStream
        {
            public ProximaConnection Connection;
            public string Id;
            public string Guid;
            public StreamInfo Info;
        }

        private class StreamInfo
        {
            public MethodInfo StartMethod;
            public MethodInfo StopMethod;
            public MethodInfo UpdateMethod;
        }

        private ProximaServer _server;
        private static List<MethodInfo> _inits = new List<MethodInfo>();
        private static List<MethodInfo> _teardowns = new List<MethodInfo>();
        private static Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
        public static Dictionary<string, MethodInfo> Commands => _commands;
        private static bool _staticInitialized;

        private static Dictionary<string, StreamInfo> _streams = new Dictionary<string, StreamInfo>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<OpenStream>> _openStreams;

        private ProximaDispatcher _dispatcher;
        private ProximaStatusUI _statusUI;
        private ProximaConnectUI _connectUI;
        private bool _wasRunInBackgroundSet;

        internal static Type ProxyClass = typeof(ProximaInspector).Assembly.GetType("Proxima.ProximaProxyServer");
        internal static bool ProxyAvailable => ProxyClass != null;

        void Awake()
        {
            if (!_staticInitialized)
            {
                RegisterBuiltInCommands();
                ProximaLogCommands.SetLogCapacity(_logBufferSize);
                _staticInitialized = true;
            }

            _dispatcher = new ProximaDispatcher(this);

            if (string.IsNullOrEmpty(_displayName))
            {
                _displayName = Application.companyName + "." + Application.productName + "." + Application.version;
            }
        }

        void OnEnable()
        {
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (_runOnEnable)
            {
                Run();
            }

            if (_instantiateStatusUI)
            {
                var resource = Resources.Load<ProximaStatusUI>("Proxima/ProximaStatusUI");
                resource.gameObject.SetActive(false);
                _statusUI = Instantiate(resource);
                _statusUI.ProximaInspector = this;
                _statusUI.transform.SetParent(transform);
                _statusUI.gameObject.SetActive(true);
                resource.gameObject.SetActive(true);
            }

            if (_instantiateConnectUI)
            {
                var resource = Resources.Load<ProximaConnectUI>("Proxima/ProximaConnectUI");
                resource.gameObject.SetActive(false);
                _connectUI = Instantiate(resource);
                _connectUI.ProximaInspector = this;
                _connectUI.GetComponent<ProximaStatusUI>().ProximaInspector = this;
                _connectUI.transform.SetParent(transform);
                _connectUI.gameObject.SetActive(true);
                resource.gameObject.SetActive(true);
            }
        }

        void OnApplicationQuit()
        {
            Stop();
        }

        void OnDestroy()
        {
            Stop();
        }

        void OnDisable()
        {
            Stop();

            if (_statusUI)
            {
                Destroy(_statusUI.gameObject);
                _statusUI = null;
            }

            if (_connectUI)
            {
                Destroy(_connectUI.gameObject);
                _connectUI = null;
            }
        }

        // Starts the Proxima Server with the current configuration.
        public void Run()
        {
            if (_server != null)
            {
                Log.Warning("Run was called, but Proxima is already running.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_displayName))
            {
                Status.SetError("Display name is required to start Proxima.");
                Log.Error("Display name is required to start Proxima.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_password))
            {
                Status.SetError("Password is required to start Proxima.");
                Log.Error("Password is required to start Proxima.");
                return;
            }

            foreach (var method in _inits)
            {
                method.Invoke(null, null);
            }

            if (_setRunInBackground)
            {
                _wasRunInBackgroundSet = Application.runInBackground;
                Application.runInBackground = true;
            }

            Status.Reset();
            Status.SetRunning(true);

            try
            {
                switch (_connectionType)
                {
                    case ConnectionTypes.Internet:
                    {
                        var proxyClass = ProxyClass;
                        if (proxyClass == null)
                        {
                            throw new Exception("Internet connection type is only available in Proxima Pro.");
                        }
                        else
                        {
                            _server = Activator.CreateInstance(proxyClass, _dispatcher, Status,
                                _serverUrl, _appId, _uniqueName, _uploadLogs) as ProximaServer;
                        }
                        break;
                    }
                    case ConnectionTypes.LocalNetwork:
#if UNITY_WEBGL && !UNITY_EDITOR
                        _server = new ProximaWebGLServer(_dispatcher, Status);
#else
                        _server = new ProximaEmbeddedServer(_dispatcher, Status, _port, _useHttps, _certificate, _certificatePassword);
#endif
                        break;
#if PROXIMA_DEBUG
#if UNITY_WEBGL
                    case ConnectionTypes.Demo:
                        _server = new ProximaDemoServer(_dispatcher, Status);
                        break;
#endif
                    case ConnectionTypes.Debug:
                        _server = new ProximaRemoteServer(_dispatcher, Status, _serverUrl);
                        break;
#endif
                    default:
                        throw new Exception("Unknown proxima server type: " + _connectionType);
                }

                _server.Start(_displayName, _password);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    e = e.InnerException;
                }

                Log.Exception(e);
                Status.SetError(e.Message);
                Status.SetRunning(false);
                Cleanup();
            }
        }

        // Stops the Proxima Server, closing any connections.
        public void Stop()
        {
            if (_server != null)
            {
                Log.Info("Proxima shutting down.");
                foreach (var method in _teardowns)
                {
                    method.Invoke(null, null);
                }
            }

            _server?.Stop();
            Status.Reset();
            Cleanup();
        }

        private void Cleanup()
        {
            if (_server != null && _setRunInBackground)
            {
                Application.runInBackground = _wasRunInBackgroundSet;
            }

            _server = null;
            _openStreams = null;
        }

        void Update()
        {
            _dispatcher?.InvokeAll();

            if (_server == null)
            {
                return;
            }

            if (_server.TryGetMessage(out var item))
            {
                var (connection, message) = item;
                var response = HandleMessage(connection, message);
                if (response != null)
                {
                    try
                    {
                        connection.SendMessage(response);
                    }
                    catch
                    {
                        // Connection was likely closed
                    }
                }
            }

            UpdateStreams();
        }

        private MemoryStream HandleMessage(ProximaConnection connection, ProximaRequest request)
        {
            if (request.Type == ProximaRequestType.StartStream)
            {
                return HandleStreamStartRequest(connection, request);
            }
            else if (request.Type == ProximaRequestType.StopStream)
            {
                return HandleStreamStopRequest(connection, request);
            }
            else if (request.Type == ProximaRequestType.Command)
            {
                return HandleCommand(request);
            }
            else if (request.Type == ProximaRequestType.List)
            {
                return ProximaSerialization.DataResponse(request, "");
            }
            else if (request.Type == ProximaRequestType.Select)
            {
                return ProximaSerialization.ErrorResponse(request, "Already selected.");
            }
            else
            {
                return ProximaSerialization.ErrorResponse(request, "Invalid request type.");
            }
        }

        private MemoryStream HandleStreamStartRequest(ProximaConnection connection, ProximaRequest request)
        {
            var stream = request.Cmd;
            if (!_streams.TryGetValue(stream, out var streamInfo))
            {
                return ProximaSerialization.ErrorResponse(request, "Invalid stream.");
            }

            if (_openStreams == null)
            {
                _openStreams = new Dictionary<string, List<OpenStream>>(StringComparer.OrdinalIgnoreCase);
            }

            if (!_openStreams.ContainsKey(stream))
            {
                _openStreams.Add(stream, new List<OpenStream>());
            }

            var guid = Guid.NewGuid().ToString();
            if (streamInfo.StartMethod != null)
            {
                var args = new string[request.Args.Length + 1];
                args[0] = guid;
                Array.Copy(request.Args, 0, args, 1, request.Args.Length);
                if (!TryInvoke(streamInfo.StartMethod, args, out var data, out var error))
                {
                    return ProximaSerialization.ErrorResponse(request, error);
                }
            }

            var openStream = new OpenStream {
                Connection = connection,
                Id = request.Id,
                Guid = guid,
                Info = streamInfo
            };

            _openStreams[stream].Add(openStream);
            return null;
        }

        private MemoryStream HandleStreamStopRequest(ProximaConnection connection, ProximaRequest request)
        {
            var stream = request.Cmd;
            if (!_streams.TryGetValue(stream, out var streamInfo))
            {
                return ProximaSerialization.ErrorResponse(request, "Invalid stream name.");
            }

            if (_openStreams == null)
            {
                return ProximaSerialization.ErrorResponse(request, "Stream not open. (A)");
            }

            if (!_openStreams.TryGetValue(stream, out var listeners))
            {
                return ProximaSerialization.ErrorResponse(request, "Stream not open. (B)");
            }

            var idx = listeners.FindIndex(os => os.Connection == connection && os.Id == request.Id);
            if (idx < 0)
            {
                return ProximaSerialization.ErrorResponse(request, "Stream not open. (C)");
            }

            var guid = listeners[idx].Guid;
            listeners.RemoveAt(idx);

            if (!TryInvoke(streamInfo.StopMethod, new string[] { guid }, out var result, out var error))
            {
                return ProximaSerialization.ErrorResponse(request, error);
            }

            return ProximaSerialization.DataResponse(request, "<STREAM-END>");
        }

        private MemoryStream HandleCommand(ProximaRequest request)
        {
            if (!_commands.TryGetValue(request.Cmd, out var method))
            {
                return ProximaSerialization.ErrorResponse(request, $"Method {request.Cmd} not found.");
            }

            if (!TryInvoke(method, request.Args, out var data, out var error))
            {
                return ProximaSerialization.ErrorResponse(request, error);
            }

            return ProximaSerialization.DataResponse(request, data);
        }

        private static StreamInfo GetOrCreateStreamInfo(string name)
        {
            if (!_streams.TryGetValue(name, out var streamInfo))
            {
                streamInfo = new StreamInfo();
                _streams.Add(name, streamInfo);
            }

            return streamInfo;
        }

        private void RegisterBuiltInCommands()
        {
            RegisterCommands<ProximaInternalCommands>();
            RegisterCommands<ProximaGameObjectCommands>();
            RegisterCommands<ProximaComponentCommands>();
            RegisterCommands<ProximaLogCommands>();
            ProximaFeatures.RegisterProFeatures();
        }

        public static void RegisterCommands<T>()
        {
            RegisterCommands(typeof(T));
        }

        public static void RegisterCommands(Type type)
        {
            foreach (var method in type.GetRuntimeMethods())
            {
                if (!method.IsStatic) continue;

                var initAttribute = method.GetCustomAttribute<ProximaInitializeAttribute>();
                if (initAttribute != null)
                {
                    Log.Verbose("Found init: " + type.Name + "." + method.Name);
                    _inits.Add(method);
                }

                var teardownAttribute = method.GetCustomAttribute<ProximaTeardownAttribute>();
                if (teardownAttribute != null)
                {
                    Log.Verbose("Found teardown: " + type.Name + "." + method.Name);
                    _teardowns.Add(method);
                }

                var commandAttribute = method.GetCustomAttribute<ProximaCommandAttribute>();
                if (commandAttribute != null)
                {
                    if (_commands.ContainsKey(method.Name))
                    {
                        throw new Exception($"Multiple Proxima commands found with name {type.Name}.{method.Name}.");
                    }

                    Log.Verbose("Found command: " + type.Name + "." + method.Name);
                    _commands.Add(method.Name, method);
                    if (!string.IsNullOrWhiteSpace(commandAttribute.Alias))
                    {
                        Log.Verbose("Found command alias: " + commandAttribute.Alias);
                        _commands.Add(commandAttribute.Alias, method);
                    }
                }

                var streamStart = method.GetCustomAttribute<ProximaStreamStartAttribute>();
                if (streamStart != null)
                {
                    var streamInfo = GetOrCreateStreamInfo(streamStart.Name);
                    if (streamInfo.StartMethod != null)
                    {
                        throw new Exception($"Multiple Proxima stream start methods found for stream {type.Name}.{streamStart.Name}.");
                    }

                    Log.Verbose($"Found stream start: {type.Name}.{streamStart.Name}");
                    streamInfo.StartMethod = method;
                }

                var streamStop = method.GetCustomAttribute<ProximaStreamStopAttribute>();
                if (streamStop != null)
                {
                    var streamInfo = GetOrCreateStreamInfo(streamStop.Name);
                    if (streamInfo.StopMethod != null)
                    {
                        throw new Exception($"Multiple Proxima stream stop methods found for stream {type.Name}.{streamStop.Name}.");
                    }

                    Log.Verbose($"Found stream stop: {type.Name}.{streamStop.Name}");
                    streamInfo.StopMethod = method;
                }

                var streamUpdate = method.GetCustomAttribute<ProximaStreamUpdateAttribute>();
                if (streamUpdate != null)
                {
                    var streamInfo = GetOrCreateStreamInfo(streamUpdate.Name);
                    if (streamInfo.UpdateMethod != null)
                    {
                        throw new Exception($"Multiple Proxima stream update methods found for stream {type.Name}.{streamUpdate.Name}.");
                    }

                    Log.Verbose($"Found stream update: {type.Name}.{streamUpdate.Name}");
                    streamInfo.UpdateMethod = method;

                }
            }
        }

        private void UpdateStreams()
        {
            if (_openStreams != null)
            {
                foreach (var stream in _openStreams)
                {
                    var listeners = stream.Value;

                    // Close streams that disconnected
                    foreach (var listener in listeners)
                    {
                        if (!listener.Connection.Open)
                        {
                            TryInvoke(listener.Info.StopMethod, new string[] { listener.Guid });
                        }
                    }

                    listeners.RemoveAll(os => !os.Connection.Open);

                    foreach (var listener in listeners)
                    {
                        object data = null;
                        string error = null;

                        try
                        {
                            data = listener.Info.UpdateMethod?.Invoke(null, new object[] { listener.Guid });
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            error = e.Message;
                        }

                        if (!string.IsNullOrEmpty(error))
                        {
                            listener.Connection.SendMessage(ProximaSerialization.ErrorResponse(listener.Id, error));
                        }
                        else if (data != null)
                        {
                            listener.Connection.SendMessage(ProximaSerialization.DataResponse(listener.Id, data));
                        }
                    }
                }
            }
        }

        private bool TryInvoke(MethodInfo method, string[] args)
        {
            return TryInvoke(method, args, out var result, out var error);
        }

        private bool TryInvoke(MethodInfo method, string[] args, out object result, out string error)
        {
            result = null;
            error = string.Empty;
            if (method == null) return true;

            var parameters = method.GetParameters();
            var values = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (args == null || args.Length <= i)
                {
                    if (parameters[i].HasDefaultValue)
                    {
                        values[i] = parameters[i].DefaultValue;
                        continue;
                    }

                    error = $"Required argument {parameter.Name} not found.";
                    return false;
                }

                if (typeof(IPropertyOrValue).IsAssignableFrom(parameter.ParameterType))
                {
                    values[i] = Activator.CreateInstance(parameter.ParameterType, new object[] { args[i] });
                    continue;
                }

                if (ProximaSerialization.TryDeserialize(parameter.ParameterType, args[i], out var value))
                {
                    values[i] = value;
                    continue;
                }

                error = $"Unable to deserialize argument {parameter.Name} as {parameter.ParameterType.Name}.";
                return false;
            }

            try
            {
                if (method.ReturnType == typeof(void))
                {
                    method.Invoke(null, values);
                }
                else
                {
                    result = method.Invoke(null, values);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                error = e.InnerException.Message;
                return false;
            }

            return true;
        }
    }
}