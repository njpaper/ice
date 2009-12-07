// **********************************************************************
//
// Copyright (c) 2003-2009 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

namespace IceInternal
{

    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System;

    public sealed class Instance
    {
        public bool destroyed()
        {
            return _state == StateDestroyed;
        }

        public Ice.InitializationData initializationData()
        {
            //
            // No check for destruction. It must be possible to access the
            // initialization data after destruction.
            //
            // No mutex lock, immutable.
            //
            return _initData;
        }
        
        public TraceLevels traceLevels()
        {
            // No mutex lock, immutable.
            Debug.Assert(_traceLevels != null);
            return _traceLevels;
        }
        
        public DefaultsAndOverrides defaultsAndOverrides()
        {
            // No mutex lock, immutable.
            Debug.Assert(_defaultsAndOverrides != null);
            return _defaultsAndOverrides;
        }
        
        public RouterManager routerManager()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_routerManager != null);
                return _routerManager;
            }
        }
        
        public LocatorManager locatorManager()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }

                Debug.Assert(_locatorManager != null);
                return _locatorManager;
            }
        }
        
        public ReferenceFactory referenceFactory()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_referenceFactory != null);
                return _referenceFactory;
            }
        }
        
        public ProxyFactory proxyFactory()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_proxyFactory != null);
                return _proxyFactory;
            }
        }
        
        public OutgoingConnectionFactory outgoingConnectionFactory()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_outgoingConnectionFactory != null);
                return _outgoingConnectionFactory;
            }
        }
        
        public ConnectionMonitor connectionMonitor()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_connectionMonitor != null);
                return _connectionMonitor;
            }
        }
        
        public ObjectFactoryManager servantFactoryManager()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_servantFactoryManager != null);
                return _servantFactoryManager;
            }
        }
        
        public ObjectAdapterFactory objectAdapterFactory()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_objectAdapterFactory != null);
                return _objectAdapterFactory;
            }
        }

        public int protocolSupport()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                return _protocolSupport;
            }
        }
        
        public ThreadPool clientThreadPool()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_clientThreadPool != null);
                return _clientThreadPool;
            }
        }
        
        public ThreadPool serverThreadPool()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                if(_serverThreadPool == null) // Lazy initialization.
                {
                    int timeout = _initData.properties.getPropertyAsInt("Ice.ServerIdleTime");
                    _serverThreadPool = new ThreadPool(this, "Ice.ThreadPool.Server", timeout);
                }
                
                return _serverThreadPool;
            }
        }

        public AsyncIOThread
        asyncIOThread()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }        
                
                if(_asyncIOThread == null) // Lazy initialization.
                {
                    _asyncIOThread = new AsyncIOThread(this);
                }
            
                return _asyncIOThread;
            }
        }

        public EndpointHostResolver endpointHostResolver()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }

                Debug.Assert(_endpointHostResolver != null);
                return _endpointHostResolver;
            }
        }

        public RetryQueue
        retryQueue()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_retryQueue != null);
                return _retryQueue;
            }
        }

        public Timer
        timer()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }

                Debug.Assert(_timer != null);
                return _timer;
            }
        }

        public EndpointFactoryManager endpointFactoryManager()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_endpointFactoryManager != null);
                return _endpointFactoryManager;
            }
        }
        
        public Ice.PluginManager pluginManager()
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Debug.Assert(_pluginManager != null);
                return _pluginManager;
            }
        }
        
        public int messageSizeMax()
        {
            // No mutex lock, immutable.
            return _messageSizeMax;
        }
        
        public int clientACM()
        {
            // No mutex lock, immutable.
            return _clientACM;
        }
        
        public int serverACM()
        {
            // No mutex lock, immutable.
            return _serverACM;
        }
        
        public Ice.ImplicitContextI getImplicitContext()
        {
            return _implicitContext;
        }

        public void flushBatchRequests()
        {
            OutgoingConnectionFactory connectionFactory;
            ObjectAdapterFactory adapterFactory;
            
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                connectionFactory = _outgoingConnectionFactory;
                adapterFactory = _objectAdapterFactory;
            }
            
            connectionFactory.flushBatchRequests();
            adapterFactory.flushBatchRequests();
        }

        public Ice.Identity stringToIdentity(string s)
        {
            return Ice.Util.stringToIdentity(s);
        }

        public string identityToString(Ice.Identity ident)
        {
            return Ice.Util.identityToString(ident);
        }

        public Ice.ObjectPrx 
        getAdmin()
        {
            Ice.ObjectAdapter adapter = null;
            string serverId = null;
            Ice.LocatorPrx defaultLocator = null;

            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                string adminOA = "Ice.Admin";
                
                if(_adminAdapter != null)
                {
                    return _adminAdapter.createProxy(_adminIdentity);
                }
                else if(_initData.properties.getProperty(adminOA + ".Endpoints").Length == 0)
                {
                    return null;
                }
                else
                {
                    serverId = _initData.properties.getProperty("Ice.Admin.ServerId");
                    string instanceName = _initData.properties.getProperty("Ice.Admin.InstanceName");
                    
                    defaultLocator = _referenceFactory.getDefaultLocator();
                    
                    if((defaultLocator != null && serverId.Length > 0) || instanceName.Length > 0)
                    {
                        if(_adminIdentity == null)
                        {
                            if(instanceName.Length == 0)
                            {
                                instanceName = System.Guid.NewGuid().ToString();
                            }
                            _adminIdentity = new Ice.Identity("admin", instanceName);
                            //
                            // Afterwards, _adminIdentity is read-only
                            //
                        }
                        
                        //
                        // Create OA
                        //
                        _adminAdapter = _objectAdapterFactory.createObjectAdapter(adminOA, null);
                
                        //
                        // Add all facets to OA
                        //
                        Dictionary<string, Ice.Object> filteredFacets = new Dictionary<string, Ice.Object>();

                        foreach(KeyValuePair<string, Ice.Object> entry in _adminFacets)
                        {
                            if(_adminFacetFilter.Count == 0 || _adminFacetFilter.Contains(entry.Key))
                            {
                                _adminAdapter.addFacet(entry.Value, _adminIdentity, entry.Key);
                            }
                            else
                            {
                                filteredFacets.Add(entry.Key, entry.Value);
                            }
                        }
                        _adminFacets = filteredFacets;

                        adapter = _adminAdapter;
                    }
                }
            }

            if(adapter == null)
            {
                return null;
            }
            else
            {
                try
                {
                    adapter.activate();
                }
                catch(Ice.LocalException)
                {
                    //
                    // We cleanup _adminAdapter, however this error is not recoverable
                    // (can't call again getAdmin() after fixing the problem)
                    // since all the facets (servants) in the adapter are lost
                    //
                    adapter.destroy();
                    lock(this)
                    {
                        _adminAdapter = null;
                    }
                    throw;
                }

                Ice.ObjectPrx admin = adapter.createProxy(_adminIdentity);
                if(defaultLocator != null && serverId.Length > 0)
                {    
                    Ice.ProcessPrx process = Ice.ProcessPrxHelper.uncheckedCast(admin.ice_facet("Process"));
                    try
                    {
                        //
                        // Note that as soon as the process proxy is registered, the communicator might be 
                        // shutdown by a remote client and admin facets might start receiving calls.
                        //
                        defaultLocator.getRegistry().setServerProcessProxy(serverId, process);
                    }
                    catch(Ice.ServerNotFoundException)
                    {
                        if(_traceLevels.location >= 1)
                        {
                            System.Text.StringBuilder s = new System.Text.StringBuilder();
                            s.Append("couldn't register server `" + serverId + "' with the locator registry:\n");
                            s.Append("the server is not known to the locator registry");
                            _initData.logger.trace(_traceLevels.locationCat, s.ToString());
                        }
                        
                        throw new Ice.InitializationException("Locator knows nothing about server '" + serverId +
                                                              "'");
                    }
                    catch(Ice.LocalException ex)
                    {
                        if(_traceLevels.location >= 1)
                        {
                            System.Text.StringBuilder s = new System.Text.StringBuilder();
                            s.Append("couldn't register server `" + serverId + "' with the locator registry:\n" + ex);
                            _initData.logger.trace(_traceLevels.locationCat, s.ToString());
                        }
                        throw ex; // TODO: Shall we raise a special exception instead of a non obvious local exception?
                    }
            
                    if(_traceLevels.location >= 1)
                    {
                        System.Text.StringBuilder s = new System.Text.StringBuilder();
                        s.Append("registered server `" + serverId + "' with the locator registry");
                        _initData.logger.trace(_traceLevels.locationCat, s.ToString());
                    }
                }
                return admin;
            }    
        }
        
        public void 
        addAdminFacet(Ice.Object servant, string facet)
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
            
                if(_adminAdapter == null || (_adminFacetFilter.Count == 0 && !_adminFacetFilter.Contains(facet)))
                {
                    if(_adminFacets.ContainsKey(facet))
                    {
                        throw new Ice.AlreadyRegisteredException("facet", facet);
                    }
                    _adminFacets.Add(facet, servant);
                }
                else
                {
                    _adminAdapter.addFacet(servant, _adminIdentity, facet);
                }
            }
        }


        public Ice.Object 
        removeAdminFacet(string facet)
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }
                
                Ice.Object result = null;
                if(_adminAdapter == null || (_adminFacetFilter.Count == 0 && !_adminFacetFilter.Contains(facet)))
                {
                    try
                    {
                        result = _adminFacets[facet];
                    }
                    catch(KeyNotFoundException)
                    {
                        throw new Ice.NotRegisteredException("facet", facet);
                    }
                    
                    _adminFacets.Remove(facet);
                }
                else
                {
                    result = _adminAdapter.removeFacet(_adminIdentity, facet);
                }
                return result;
            }
        }

        public void
        setDefaultLocator(Ice.LocatorPrx locator)
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }

                _referenceFactory = _referenceFactory.setDefaultLocator(locator);
            }
        }

        public void
        setDefaultRouter(Ice.RouterPrx router)
        {
            lock(this)
            {
                if(_state == StateDestroyed)
                {
                    throw new Ice.CommunicatorDestroyedException();
                }

                _referenceFactory = _referenceFactory.setDefaultRouter(router);
            }
        }

        public void
        setLogger(Ice.Logger logger)
        {
            //
            // No locking, as it can only be called during plug-in loading
            //
            _initData.logger = logger;
        }

        public void
        setThreadHook(Ice.ThreadNotification threadHook)
        {
            //
            // No locking, as it can only be called during plug-in loading
            //
            _initData.threadHook = threadHook;
        }

        //
        // Only for use by Ice.CommunicatorI
        //
        public Instance(Ice.Communicator communicator, Ice.InitializationData initData)
        {
            _state = StateActive;
            _initData = initData;
                
            try
            {
                if(_initData.properties == null)
                {
                    string[] args = new string[0];
                    _initData.properties = Ice.Util.createProperties(ref args);
                }

                lock(_staticLock)
                {
                    if(!_oneOffDone)
                    {
                        string stdOut = _initData.properties.getProperty("Ice.StdOut");
                        string stdErr = _initData.properties.getProperty("Ice.StdErr");
                        
                        System.IO.StreamWriter outStream = null;
                        
                        if(stdOut.Length > 0)
                        {
                            try
                            {                       
                                outStream = System.IO.File.AppendText(stdOut);
                            }
                            catch(System.IO.IOException ex)
                            {
                                Ice.FileException fe = new Ice.FileException(ex);
                                fe.path = stdOut;
                                throw fe;       
                            }
                            outStream.AutoFlush = true;
                            System.Console.Out.Close();
                            System.Console.SetOut(outStream);
                        }
                        if(stdErr.Length > 0)
                        {
                            if(stdErr.Equals(stdOut))
                            {
                                System.Console.SetError(outStream); 
                            }
                            else
                            {
                                System.IO.StreamWriter errStream = null;
                                try
                                {
                                    errStream = System.IO.File.AppendText(stdErr);
                                }
                                catch(System.IO.IOException ex)
                                {
                                    Ice.FileException fe = new Ice.FileException(ex);
                                    fe.path = stdErr;
                                    throw fe;   
                                }
                                errStream.AutoFlush = true;
                                System.Console.Error.Close();
                                System.Console.SetError(errStream);
                            }
                        }

                        _oneOffDone = true;
                    }
                }
                
                if(_initData.logger == null)
                {
                    string logfile = _initData.properties.getProperty("Ice.LogFile");
                    if(_initData.properties.getPropertyAsInt("Ice.UseSyslog") > 0)
                    {
                        if(logfile.Length != 0)
                        {
                            throw new Ice.InitializationException("Ice.LogFile and Ice.UseSyslog cannot both be set.");
                        }
                        _initData.logger = new Ice.SysLoggerI(_initData.properties.getProperty("Ice.ProgramName"),
                            _initData.properties.getPropertyWithDefault("Ice.SyslogFacility", "LOG_USER"));
                    }
                    else if(logfile.Length != 0 || Ice.Util.getProcessLogger() is Ice.LoggerI) 
                    {
                        //
                        // If Ice.LogFile set, default ConsoleTraceListener disabled.
                        // Otherwise default enabled.
                        //
                        bool console = 
                            _initData.properties.getPropertyAsIntWithDefault("Ice.ConsoleListener",
                                                                             logfile.Length == 0 ? 1 : 0) > 0;
                        _initData.logger = 
                            new Ice.TraceLoggerI(_initData.properties.getProperty("Ice.ProgramName"), logfile, console);
                    }
                    else
                    {
                        _initData.logger = Ice.Util.getProcessLogger();
                    }
                }
                
                _traceLevels = new TraceLevels(_initData.properties);
                
                _defaultsAndOverrides = new DefaultsAndOverrides(_initData.properties);
                
                {
                    const int defaultMessageSizeMax = 1024;
                    int num = 
                        _initData.properties.getPropertyAsIntWithDefault("Ice.MessageSizeMax", defaultMessageSizeMax);
                    if(num < 1)
                    {
                        _messageSizeMax = defaultMessageSizeMax * 1024; // Ignore non-sensical values.
                    }
                    else if(num > 0x7fffffff / 1024)
                    {
                        _messageSizeMax = 0x7fffffff;
                    }
                    else
                    {
                        _messageSizeMax = num * 1024; // Property is in kilobytes, _messageSizeMax in bytes
                    }
                }
                
                //
                // Client ACM enabled by default. Server ACM disabled by default.
                //
                _clientACM = _initData.properties.getPropertyAsIntWithDefault("Ice.ACM.Client", 60);
                _serverACM = _initData.properties.getPropertyAsInt("Ice.ACM.Server");

                _implicitContext = Ice.ImplicitContextI.create(_initData.properties.getProperty("Ice.ImplicitContext"));
                _routerManager = new RouterManager();
                
                _locatorManager = new LocatorManager(_initData.properties);
                
                _referenceFactory = new ReferenceFactory(this, communicator);
                
                _proxyFactory = new ProxyFactory(this);
                
                bool ipv4 = _initData.properties.getPropertyAsIntWithDefault("Ice.IPv4", 1) > 0;
                bool ipv6 = _initData.properties.getPropertyAsIntWithDefault("Ice.IPv6", 0) > 0;
                if(!ipv4 && !ipv6)
                {
                    throw new Ice.InitializationException("Both IPV4 and IPv6 support cannot be disabled.");
                }
                else if(ipv4 && ipv6)
                {
                    _protocolSupport = Network.EnableBoth;
                }
                else if(ipv4)
                {
                    _protocolSupport = Network.EnableIPv4;
                }
                else
                {
                    _protocolSupport = Network.EnableIPv6;
                }
                _endpointFactoryManager = new EndpointFactoryManager(this);
                EndpointFactory tcpEndpointFactory = new TcpEndpointFactory(this);
                _endpointFactoryManager.add(tcpEndpointFactory);
                EndpointFactory udpEndpointFactory = new UdpEndpointFactory(this);
                _endpointFactoryManager.add(udpEndpointFactory);
                
                _pluginManager = new Ice.PluginManagerI(communicator);

                _outgoingConnectionFactory = new OutgoingConnectionFactory(this);
                
                _servantFactoryManager = new ObjectFactoryManager();
                
                _objectAdapterFactory = new ObjectAdapterFactory(this, communicator);
                
                _retryQueue = new RetryQueue(this);

                string[] facetFilter = _initData.properties.getPropertyAsList("Ice.Admin.Facets");
                if(facetFilter.Length > 0)
                {
                    foreach(string s in facetFilter)
                    {
                        _adminFacetFilter.Add(s);
                    }
                }
                _adminFacets.Add("Properties", new PropertiesAdminI(_initData.properties));
                _adminFacets.Add("Process", new ProcessI(communicator));
            }
            catch(Ice.LocalException)
            {
                destroy();
                throw;
            }
        }
        
        public void finishSetup(ref string[] args)
        {
            //
            // Load plug-ins.
            //
            Debug.Assert(_serverThreadPool == null);
            Ice.PluginManagerI pluginManagerImpl = (Ice.PluginManagerI)_pluginManager;
            pluginManagerImpl.loadPlugins(ref args);
            
            //
            // Create threads.
            //
            try
            {
                if(initializationData().properties.getProperty("Ice.ThreadPriority").Length > 0)
                {
                    ThreadPriority priority = IceInternal.Util.stringToThreadPriority(
                                                initializationData().properties.getProperty("Ice.ThreadPriority"));
                    _timer = new Timer(this, priority);
                }
                else
                {
                    _timer = new Timer(this);
                }
            }
            catch(System.Exception ex)
            {
                string s = "cannot create thread for timer:\n" + ex;
                _initData.logger.error(s);
                throw ex;
            }
          
            try
            {
                _endpointHostResolver = new EndpointHostResolver(this);
            }
            catch(System.Exception ex)
            {
                string s = "cannot create thread for endpoint host resolver:\n" + ex;
                _initData.logger.error(s);
                throw ex;
            }

            _clientThreadPool = new ThreadPool(this, "Ice.ThreadPool.Client", 0);

            //
            // Get default router and locator proxies. Don't move this
            // initialization before the plug-in initialization!!! The proxies
            // might depend on endpoint factories to be installed by plug-ins.
            //
            Ice.RouterPrx r = Ice.RouterPrxHelper.uncheckedCast(_proxyFactory.propertyToProxy("Ice.Default.Router"));
            if(r != null)
            {
                _referenceFactory = _referenceFactory.setDefaultRouter(r);
            }

            Ice.LocatorPrx l = Ice.LocatorPrxHelper.uncheckedCast(_proxyFactory.propertyToProxy("Ice.Default.Locator"));
            if(l != null)
            {
                _referenceFactory = _referenceFactory.setDefaultLocator(l);
            }
            
            //
            // Show process id if requested (but only once).
            //
            lock(this)
            {
                if(!_printProcessIdDone && _initData.properties.getPropertyAsInt("Ice.PrintProcessId") > 0)
                {
                    using(Process p = Process.GetCurrentProcess())
                    {
                        System.Console.WriteLine(p.Id);
                    }
                    _printProcessIdDone = true;
                }
            }
             
            //
            // Create the connection monitor and ensure the interval for
            // monitoring connections is appropriate for client & server
            // ACM.
            //
            int interval = _initData.properties.getPropertyAsInt("Ice.MonitorConnections");
            _connectionMonitor = new ConnectionMonitor(this, interval);
            _connectionMonitor.checkIntervalForACM(_clientACM);
            _connectionMonitor.checkIntervalForACM(_serverACM);

            //
            // Server thread pool initialization is lazy in serverThreadPool().
            //

            //      
            // An application can set Ice.InitPlugins=0 if it wants to postpone
            // initialization until after it has interacted directly with the
            // plug-ins.
            //      
            if(_initData.properties.getPropertyAsIntWithDefault("Ice.InitPlugins", 1) > 0)
            {
                pluginManagerImpl.initializePlugins();
            }

            //
            // This must be done last as this call creates the Ice.Admin object adapter
            // and eventually registers a process proxy with the Ice locator (allowing 
            // remote clients to invoke on Ice.Admin facets as soon as it's registered).
            //
            if(_initData.properties.getPropertyAsIntWithDefault("Ice.Admin.DelayCreation", 0) <= 0)
            {
                getAdmin();
            }
        }
        
        //
        // Only for use by Ice.CommunicatorI
        //
        public bool destroy()
        {
            lock(this)
            {
                //
                // If the _state is not StateActive then the instance is
                // either being destroyed, or has already been destroyed.
                //
                if(_state != StateActive)
                {
                    return false;
                }
            
                //
                // We cannot set state to StateDestroyed otherwise instance
                // methods called during the destroy process (such as
                // outgoingConnectionFactory() from
                // ObjectAdapterI::deactivate() will cause an exception.
                //
                _state = StateDestroyInProgress;
            }
            
            if(_objectAdapterFactory != null)
            {
                _objectAdapterFactory.shutdown();
            }
            
            if(_outgoingConnectionFactory != null)
            {
                _outgoingConnectionFactory.destroy();
            }
            
            if(_objectAdapterFactory != null)
            {
                _objectAdapterFactory.destroy();
            }
            
            if(_outgoingConnectionFactory != null)
            {
                _outgoingConnectionFactory.waitUntilFinished();
            }
            
            if(_retryQueue != null)
            {
                _retryQueue.destroy();
            }

            ThreadPool serverThreadPool = null;
            ThreadPool clientThreadPool = null;
            AsyncIOThread asyncIOThread = null;
            EndpointHostResolver endpointHostResolver = null;

            lock(this)
            {
                _objectAdapterFactory = null;
                _outgoingConnectionFactory = null;
                _retryQueue = null;

                if(_connectionMonitor != null)
                {
                    _connectionMonitor.destroy();
                    _connectionMonitor = null;
                }
                
                if(_serverThreadPool != null)
                {
                    _serverThreadPool.destroy();
                    serverThreadPool = _serverThreadPool;
                    _serverThreadPool = null;
                }

                if(_clientThreadPool != null)
                {
                    _clientThreadPool.destroy();
                    clientThreadPool = _clientThreadPool;
                    _clientThreadPool = null;
                }

                if(_asyncIOThread != null)
                {
                    _asyncIOThread.destroy();
                    asyncIOThread = _asyncIOThread;
                    _asyncIOThread = null;
                }

                if(_endpointHostResolver != null)
                {
                    _endpointHostResolver.destroy();
                    endpointHostResolver = _endpointHostResolver;
                    _endpointHostResolver = null;
                }

                if(_timer != null)
                {
                    _timer.destroy();
                    _timer = null;
                }
                
                if(_servantFactoryManager != null)
                {
                    _servantFactoryManager.destroy();
                    _servantFactoryManager = null;
                }
                
                if(_referenceFactory != null)
                {
                    _referenceFactory.destroy();
                    _referenceFactory = null;
                }
                
                // No destroy function defined.
                // _proxyFactory.destroy();
                _proxyFactory = null;
                
                if(_routerManager != null)
                {
                    _routerManager.destroy();
                    _routerManager = null;
                }
                
                if(_locatorManager != null)
                {
                    _locatorManager.destroy();
                    _locatorManager = null;
                }
                
                if(_endpointFactoryManager != null)
                {
                    _endpointFactoryManager.destroy();
                    _endpointFactoryManager = null;
                }
                
                if(_pluginManager != null)
                {
                    _pluginManager.destroy();
                    _pluginManager = null;
                }

                _adminAdapter = null;
                _adminFacets.Clear();
                
                _state = StateDestroyed;
            }
            
            //
            // Join with threads outside the synchronization.
            //
            if(clientThreadPool != null)
            {
                clientThreadPool.joinWithAllThreads();
            }
            if(serverThreadPool != null)
            {
                serverThreadPool.joinWithAllThreads();
            }
            if(asyncIOThread != null)
            {
                asyncIOThread.joinWithThread();
            }
            if(endpointHostResolver != null)
            {
                endpointHostResolver.joinWithThread();
            }

            if(_initData.properties.getPropertyAsInt("Ice.Warn.UnusedProperties") > 0)
            {
                ArrayList unusedProperties = ((Ice.PropertiesI)_initData.properties).getUnusedProperties();
                if(unusedProperties.Count != 0)
                {
                    StringBuilder message = new StringBuilder("The following properties were set but never read:");
                    foreach(string s in unusedProperties)
                    {
                        message.Append("\n    ");
                        message.Append(s);
                    }
                    _initData.logger.warning(message.ToString());
                }
            }

            return true;
        }
        
        private const int StateActive = 0;
        private const int StateDestroyInProgress = 1;
        private const int StateDestroyed = 2;
        private int _state;
        private Ice.InitializationData _initData; // Immutable, not reset by destroy().
        private TraceLevels _traceLevels; // Immutable, not reset by destroy().
        private DefaultsAndOverrides _defaultsAndOverrides; // Immutable, not reset by destroy().
        private int _messageSizeMax; // Immutable, not reset by destroy().
        private int _clientACM; // Immutable, not reset by destroy().
        private int _serverACM; // Immutable, not reset by destroy().
        private Ice.ImplicitContextI _implicitContext; // Immutable
        private RouterManager _routerManager;
        private LocatorManager _locatorManager;
        private ReferenceFactory _referenceFactory;
        private ProxyFactory _proxyFactory;
        private OutgoingConnectionFactory _outgoingConnectionFactory;
        private ConnectionMonitor _connectionMonitor;
        private ObjectFactoryManager _servantFactoryManager;
        private ObjectAdapterFactory _objectAdapterFactory;
        private int _protocolSupport;
        private ThreadPool _clientThreadPool;
        private ThreadPool _serverThreadPool;
        private AsyncIOThread _asyncIOThread;
        private EndpointHostResolver _endpointHostResolver;
        private Timer _timer;
        private RetryQueue _retryQueue;
        private EndpointFactoryManager _endpointFactoryManager;
        private Ice.PluginManager _pluginManager;
        private Ice.ObjectAdapter _adminAdapter;
        private Dictionary<string, Ice.Object> _adminFacets = new Dictionary<string, Ice.Object>();
        private IceUtilInternal.Set _adminFacetFilter = new IceUtilInternal.Set();
        private Ice.Identity _adminIdentity;

        private static bool _printProcessIdDone = false;

        private static bool _oneOffDone = false;
        private static System.Object _staticLock = new System.Object();
    }

}
