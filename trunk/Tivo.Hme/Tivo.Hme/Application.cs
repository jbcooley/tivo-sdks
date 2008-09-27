// Copyright (c) 2008 Josh Cooley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using Tivo.Hme.Commands;

namespace Tivo.Hme
{
    public sealed class Application : IDisposable, INotifyPropertyChanged
    {
        #region fields
        private static HmeResourceCollection _fonts = new HmeResourceCollection();
        private static ImageResourceCollection _images = new ImageResourceCollection();
        private static HmeResourceCollection _sounds = new HmeResourceCollection();
        private View _root;
        private View _activeView;
        private bool _running = true;
        private bool _connected = true;
        private ResolutionInfo _currentResolution = new ResolutionInfo(640, 480, 1, 1);
        private ReadOnlyCollection<ResolutionInfo> _readonlySupportedResolutions =
            new ReadOnlyCollection<ResolutionInfo>(new ResolutionInfo[] { new ResolutionInfo(640, 480, 1, 1) });
        private int _commandThreadId = 0;
        private long _nextActionId = 1;
        private Dictionary<long, Delegate> _actions = new Dictionary<long, Delegate>();
        private Dictionary<long, object> _actionObjects = new Dictionary<long, object>();
        private Dictionary<Resource, Delegate> _fontCreated = new Dictionary<Resource, Delegate>();
        private Dictionary<Resource, object> _fontCreatedObject = new Dictionary<Resource, object>();
        private ResourceManager _resourceManager = new ResourceManager();
        private Host.HmeConnection _connection;
        #endregion

        #region Application API

        public static HmeResourceCollection Fonts
        {
            get { return _fonts; }
        }

        public static ImageResourceCollection Images
        {
            get { return _images; }
        }

        public static HmeResourceCollection Sounds
        {
            get { return _sounds; }
        }

        public View Root
        {
            get
            {
                if (_root == null)
                {
                    _root = new View();
                    _root.MakeRoot(this);
                }
                return _root;
            }
            set
            {
                if (_root != value)
                {
                    _root = value;
                    _root.MakeRoot(this);
                    OnPropertyChanged(new PropertyChangedEventArgs("Root"));
                }
            }
        }

        public View ActiveView
        {
            get { return _activeView; }
            set
            {
                if (_activeView != value)
                {
                    _activeView = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ActiveView"));
                }
            }
        }

        public bool IsRunning
        {
            get { return _running; }
            internal set
            {
                if (_running != value)
                {
                    _running = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsRunning"));
                    if (!_running)
                    {
                        Root.Dispose();
                        OnClosed(EventArgs.Empty);
                    }
                }
            }
        }

        public bool IsConnected
        {
            get { return _connected; }
            internal set
            {
                bool changed = _connected != value;
                _connected = value;
                if (changed && !_connected)
                    CloseDisconnected();
            }
        }

        public ResolutionInfo CurrentResolution
        {
            get { return _currentResolution; }
            set
            {
                if (_currentResolution != value)
                {
                    _currentResolution = value;
                    PostCommand(new ReceiverSetResolution(value));
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentResolution"));
                }
            }
        }

        public ReadOnlyCollection<ResolutionInfo> SupportedResolutions
        {
            get { return _readonlySupportedResolutions; }
        }

        public void DelayAction<T>(TimeSpan delayTime, T obj, Action<T> action)
        {
            lock (_actions)
            {
                _actionObjects.Add(_nextActionId, obj);
                _actions.Add(_nextActionId, action);
                PostCommand(ResourceSendEvent.CreateDelayEvent(_nextActionId++, delayTime));
            }
        }

        public void CreateTextStyle(TextStyle style)
        {
            GetResourceId(new Resource(style));
        }

        public void CreateTextStyle(TextStyle style, EventHandler<TextStyleCreatedArgs> styleCreated, object state)
        {
            lock (_fontCreated)
            {
                // TODO: call method even when style has already been created
                Resource styleResource = new Resource(style);
                _fontCreatedObject.Add(styleResource, state);
                _fontCreated.Add(styleResource, styleCreated);
                GetResourceId(styleResource);
            }
        }

        // TODO: implement this and make it work well with above method
        //public TextStyleInfo GetTextStyleInfo(TextStyle style)
        //{
        //    return null;
        //}

        public TrueTypeFontResource GetTrueTypeFontResource(string name)
        {
            long resourceId;
            switch (name)
            {
                case "default":
                    resourceId = 10;
                    break;
                case "system":
                    resourceId = 11;
                    break;
                default:
                    resourceId = GetResourceId(new Resource(name, ResourceType.TrueTypeFont));
                    break;
            }

            return new TrueTypeFontResource(this, name, resourceId);
        }

        public ImageResource GetImageResource(string name)
        {
            long resourceId = GetResourceId(new Resource(name, ResourceType.Image));
            return new ImageResource(this, name, resourceId);
        }

        public ImageResource StreamImageResource(string name, Uri uri, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            // call GetResourceId to initiate the stream
            // the ImageResource can be fetched from GetImageResource
            long resourceId = GetResourceId(new Resource(name, uri, imageFormat));
            return new ImageResource(this, name, resourceId);
        }

        public StreamedMusic GetStreamedMusic(Uri uri, string contentType)
        {
            return GetStreamedMusic(uri, contentType, MusicStart.AutoPlay);
        }

        public StreamedMusic GetStreamedMusic(Uri uri, string contentType, MusicStart state)
        {
            Resource resource = new Resource(uri, contentType, state);
            long resourceId = GetResourceId(resource);
            return new StreamedMusic(this, resource.Name, resourceId);
        }

        public Sound GetSound(string name)
        {
            long resourceId;
            switch (name)
            {
                case "bonk":
                    resourceId = 20;
                    break;
                case "updown":
                    resourceId = 21;
                    break;
                case "thumbsup":
                    resourceId = 22;
                    break;
                case "thumbsdown":
                    resourceId = 23;
                    break;
                case "select":
                    resourceId = 24;
                    break;
                case "tivo":
                    resourceId = 25;
                    break;
                case "left":
                    resourceId = 26;
                    break;
                case "right":
                    resourceId = 27;
                    break;
                case "pageup":
                    resourceId = 28;
                    break;
                case "pagedown":
                    resourceId = 29;
                    break;
                case "alert":
                    resourceId = 30;
                    break;
                case "deselect":
                    resourceId = 31;
                    break;
                case "error":
                    resourceId = 32;
                    break;
                case "slowdown1":
                    resourceId = 33;
                    break;
                case "speedup1":
                    resourceId = 34;
                    break;
                case "speedup2":
                    resourceId = 35;
                    break;
                case "speedup3":
                    resourceId = 36;
                    break;
                default:
                    resourceId = GetResourceId(new Resource(name, ResourceType.Sound));
                    break;
            }
            return new Sound(this, name, resourceId);
        }

        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyPress;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<ApplicationErrorArgs> ApplicationErrorOccurred;
        public event EventHandler<ApplicationStateChangedArgs> ApplicationStateChanged;
        public event EventHandler<DeviceConnectedArgs> DeviceConnected;
        public event EventHandler<TextStyleCreatedArgs> TextStyleCreated;
        public event EventHandler<IdleEventArgs> Idle;
        public event EventHandler<ResourceErrorArgs> ResourceErrorOccurred;
        public event EventHandler<ResourceStateChangedArgs> ResourceStateChanged;
        public event EventHandler<EventArgs> Closed;
        public event EventHandler<EventArgs> SupportedResolutionsChanged;
        public event EventHandler<ApplicationParametersReceivedArgs> ApplicationParametersReceived;

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        #region Host APIs

        internal Application(Host.HmeConnection connection)
        {
            _connection = connection;
        }

        internal int CommandThreadId
        {
            get { return _commandThreadId; }
            set { _commandThreadId = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion

        public void Close()
        {
            IsRunning = false;
            PostCommand(new ApplicationEnd());
        }

        public void CloseDisconnected()
        {
            IsRunning = false;
            IsConnected = false;
        }

        public void TransitionForward(string destination)
        {
            TransitionForward(destination, null, null);
        }

        public void TransitionForward(string destination, TivoTree parameters, byte[] savedData)
        {
            PostCommand(new ReceiverTransition(destination, Transition.Forward, parameters, savedData));
        }

        public void TransitionForward(Uri destination)
        {
            TransitionForward(destination.OriginalString);
        }

        public void TransitionForward(Uri destination, TivoTree parameters, byte[] savedData)
        {
            TransitionForward(destination.OriginalString, parameters, savedData);
        }

        public void TransitionBack()
        {
            TransitionBack(null);
        }

        public void TransitionBack(TivoTree parameters)
        {
            PostCommand(new ReceiverTransition(null, Transition.Back, parameters, null));
        }

        public void TransitionStartNew(string destination)
        {
            TransitionStartNew(destination, null);
        }

        public void TransitionStartNew(string destination, TivoTree parameters)
        {
            PostCommand(new ReceiverTransition(destination, Transition.Teleport, parameters, null));
        }

        public void TransitionStartNew(Uri destination)
        {
            TransitionStartNew(destination.OriginalString);
        }

        public void TransitionStartNew(Uri destination, TivoTree parameters)
        {
            TransitionStartNew(destination.OriginalString, parameters);
        }

        #region Events

        private void OnClosed(EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = Closed;
            if (handler != null)
                handler(this, eventArgs);
        }

        internal void OnApplicationInfoReceived(Events.ApplicationInfo applicationInfo)
        {
            string value;
            bool active;
            if (applicationInfo.Info.TryGetValue("active", out value) &&
                bool.TryParse(value, out active))
            {
                try
                {
                    EventHandler<ApplicationStateChangedArgs> handler = ApplicationStateChanged;
                    if (handler != null)
                    {
                        ApplicationStateChangedArgs args = new ApplicationStateChangedArgs();
                        args.ApplicationStarting = active;
                        args.ApplicationStopping = !active;
                        handler(this, args);
                    }
                }
                finally
                {
                    if (!active)
                        IsConnected = false;
                }
            }
            string errorText = null;
            applicationInfo.Info.TryGetValue("error.text", out errorText);
            if (errorText != null || applicationInfo.Info.TryGetValue("error.code", out value))
            {
                int errorCode;
                if (value == null || !int.TryParse(value, out errorCode))
                {
                    errorCode = (int)ApplicationErrorCode.ConsultErrorText;
                }
                EventHandler<ApplicationErrorArgs> handler = ApplicationErrorOccurred;
                if (handler != null)
                {
                    handler(this, new ApplicationErrorArgs((ApplicationErrorCode)errorCode, errorText ?? string.Empty));
                }
            }
            // TODO: propogate additional data if any
        }

        internal void OnApplicationParametersInfoReceived(Events.ApplicationParametersInfo applicationParametersInfo)
        {
            EventHandler<ApplicationParametersReceivedArgs> handler = ApplicationParametersReceived;
            if (handler != null)
            {
                handler(this, new ApplicationParametersReceivedArgs(applicationParametersInfo.Parameters, applicationParametersInfo.Data));
            }
        }

        internal void OnDeviceInfoReceived(Events.DeviceInfo deviceInfo)
        {
            EventHandler<DeviceConnectedArgs> handler = DeviceConnected;
            if (handler != null)
            {
                handler(this, new DeviceConnectedArgs(deviceInfo.Info));
            }
        }

        internal void OnFontInfoReceived(Events.FontInfo fontInfo)
        {
            TextStyleCreatedArgs args = new TextStyleCreatedArgs(new TextStyleInfo(
                    fontInfo.Ascent, fontInfo.Descent, fontInfo.Height, fontInfo.LineGap, fontInfo.GlyphInfo));
            EventHandler<TextStyleCreatedArgs> callback = null;
            object context = null;
            lock (_fontCreated)
            {
                foreach (KeyValuePair<Resource, Delegate> createdCallback in _fontCreated)
                {
                    // TODO: fix this in case two threads ask for a callback on the same font
                    if (fontInfo.FontId == GetResourceId(createdCallback.Key))
                    {
                        context = _fontCreatedObject[createdCallback.Key];
                        callback = (EventHandler<TextStyleCreatedArgs>)createdCallback.Value;
                        _fontCreated.Remove(createdCallback.Key);
                        _fontCreatedObject.Remove(createdCallback.Key);
                        break;
                    }
                }
            }
            if (callback != null)
            {
                try { callback(context, args); }
                catch (Exception ex) { StatusLog.Write(System.Diagnostics.TraceEventType.Warning, ex); }
            }
            EventHandler<TextStyleCreatedArgs> handler = TextStyleCreated;
            if (handler != null)
            {
                try { handler(this, args); }
                catch (Exception ex) { StatusLog.Write(System.Diagnostics.TraceEventType.Warning, ex); }
            }
        }

        internal void OnIdleInfoReceived(Events.IdleInfo idleInfo)
        {
            if (idleInfo.Idle)
            {
                EventHandler<IdleEventArgs> handler = Idle;
                IdleEventArgs args = new IdleEventArgs();
                if (handler != null)
                {
                    handler(this, args);
                }
                if (args.CancelScreenSaver)
                {
                    ReceiverAcknowledgeIdle acknowledge = new ReceiverAcknowledgeIdle();
                    acknowledge.Handled = true;
                    PostCommand(acknowledge);
                }
            }
            // TODO: what to do if this occurs when Idle is false?
        }

        internal void OnKeyInfoReceived(Events.KeyInfo keyInfo)
        {
            // TODO: ensure that keyInfo._resourceId always equals 1
            System.Diagnostics.Debug.Assert(keyInfo.ResourceId == 1);
            if (keyInfo.KeyCode == (long)KeyCode.Tivo)
            {
                // special processing for tivo key
                // this key can't be sent by the device, so it must be simulated.
                // currently using this as timer callback
                lock (_actions)
                {
                    object obj;
                    Delegate action;
                    if (_actionObjects.TryGetValue(keyInfo.RawCode, out obj))
                    {
                        _actionObjects.Remove(keyInfo.RawCode);
                    }
                    if (_actions.TryGetValue(keyInfo.RawCode, out action))
                    {
                        _actions.Remove(keyInfo.RawCode);
                        action.DynamicInvoke(obj);
                    }
                }
            }
            else
            {
                EventHandler<KeyEventArgs> handler;
                KeyEventArgs args = new KeyEventArgs((KeyCode)keyInfo.KeyCode, keyInfo.RawCode);
                switch (keyInfo.KeyAction)
                {
                    case Events.KeyAction.Press:
                        if (ActiveView != null)
                        {
                            ActiveView.OnKeyDown(args);
                        }
                        if (!args.Handled)
                        {
                            handler = KeyDown;
                            if (handler != null)
                            {
                                handler(this, args);
                            }
                        }
                        args.Handled = false;
                        if (ActiveView != null)
                        {
                            ActiveView.OnKeyPress(args);
                        }
                        if (!args.Handled)
                        {
                            handler = KeyPress;
                            if (handler != null)
                            {
                                handler(this, args);
                            }
                        }
                        break;
                    case Events.KeyAction.Repeat:
                        if (ActiveView != null)
                        {
                            ActiveView.OnKeyPress(args);
                        }
                        if (!args.Handled)
                        {
                            handler = KeyPress;
                            if (handler != null)
                            {
                                handler(this, args);
                            }
                        }
                        break;
                    case Events.KeyAction.Release:
                        if (ActiveView != null)
                        {
                            ActiveView.OnKeyUp(args);
                        }
                        if (!args.Handled)
                        {
                            handler = KeyUp;
                            if (handler != null)
                            {
                                handler(this, args);
                            }
                        }
                        break;
                }
            }
        }

        internal void OnResourceInfoReceived(Events.ResourceInfo resourceInfo)
        {
            if (resourceInfo.Status == (long)ResourceStatus.Error)
            {
                int errorCode;
                string value;
                if (!resourceInfo.Info.TryGetValue("error.code", out value) ||
                    !int.TryParse(value, out errorCode))
                {
                    errorCode = (int)ResourceErrorCode.Unknown;
                }
                string errorText;
                if (!resourceInfo.Info.TryGetValue("error.text", out errorText))
                {
                    errorText = string.Empty;
                }
                IHmeResource hmeResource = null;
                lock (_resourceManager)
                {
                    Resource resource;
                    if (_resourceManager.TryGetResource(resourceInfo.ResourceId, out resource))
                    {
                        hmeResource = ResourceToHmeResource(resourceInfo, resource);
                    }
                }
                EventHandler<ResourceErrorArgs> handler = ResourceErrorOccurred;
                if (handler != null)
                {
                    handler(this, new ResourceErrorArgs(hmeResource, (ResourceErrorCode)errorCode, errorText));
                }
            }
            else
            {
                IHmeResource hmeResource = null;
                lock (_resourceManager)
                {
                    Resource resource;
                    if (_resourceManager.TryGetResource(resourceInfo.ResourceId, out resource))
                    {
                        hmeResource = ResourceToHmeResource(resourceInfo, resource);
                    }
                }
                EventHandler<ResourceStateChangedArgs> handler = ResourceStateChanged;
                if (handler != null)
                {
                    handler(this, new ResourceStateChangedArgs(hmeResource, (ResourceStatus)resourceInfo.Status, resourceInfo.Info));
                }
            }
        }

        private IHmeResource ResourceToHmeResource(Events.ResourceInfo resourceInfo, Resource resource)
        {
            IHmeResource hmeResource = null;

            lock (_resourceManager)
            {
                if (_resourceManager.TryGetHmeResource(resourceInfo.ResourceId, out hmeResource))
                    return hmeResource;
            }
            if (resource.IsMusic)
            {
                hmeResource = new StreamedMusic(this, resource.Name, resourceInfo.ResourceId);
            }
            else if (resource.IsResourceType(ResourceType.Image))
            {
                hmeResource = new ImageResource(this, resource.Name, resourceInfo.ResourceId);
            }
            else if (resource.IsResourceType(ResourceType.Sound))
            {
                hmeResource = new Sound(this, resource.Name, resourceInfo.ResourceId);
            }
            else if (resource.IsResourceType(ResourceType.TrueTypeFont))
            {
                hmeResource = new TrueTypeFontResource(this, resource.Name, resourceInfo.ResourceId);
            }
            else
            {
                hmeResource = new GenericResource(this, resource.Name, resourceInfo.ResourceId);
            }

            return hmeResource;
        }

        internal void OnDisplayInfoReceived(Events.DisplayInfo displayInfo)
        {
            if (_readonlySupportedResolutions.Count != displayInfo.SupportedResolutions.Count ||
                !ListMatch(_readonlySupportedResolutions, displayInfo.SupportedResolutions))
            {
                _readonlySupportedResolutions = new ReadOnlyCollection<ResolutionInfo>(displayInfo.SupportedResolutions);
                EventHandler<EventArgs> handler = SupportedResolutionsChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            if (_currentResolution != displayInfo.CurrentResolution)
            {
                _currentResolution = displayInfo.CurrentResolution;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentResolution"));
            }
        }

        private static bool ListMatch(ReadOnlyCollection<ResolutionInfo> list1, List<ResolutionInfo> list2)
        {
            for (int i = 0; i < list1.Count && i < list2.Count; ++i)
            {
                if (list1[i] != list2[i])
                    return false;
            }
            return true;
        }

        #endregion
        #region Command

        internal void PostCommand(Commands.IHmeCommand command)
        {
            _connection.PostCommand(command);
        }

        internal void PostCommandBatch(IEnumerable<Commands.IHmeCommand> batch)
        {
            _connection.PostCommandBatch(batch);
        }

        #endregion
        #region Resource

        internal long GetResourceId(Resource resource)
        {
            long id;
            lock (_resourceManager)
            {
                if (_resourceManager.TryGetResourceId(resource, out id))
                {
                    return id;
                }
                // implied else here
                // not using else since I want to end the lock block early
                id = _resourceManager.GetNextResourceId();
                _resourceManager.Add(resource, id);
            }
            resource.AddResourceCommand.ResourceId = id;
            if (_commandThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                _connection.SendCommand(resource.AddResourceCommand);
            }
            else
            {
                PostCommand(resource.AddResourceCommand);
            }
            return id;
        }

        internal void ReleaseResourceId(long id)
        {
            bool found = false;
            lock (_resourceManager)
            {
                found = _resourceManager.RemoveResourceId(id);
            }
            if (found)
            {
                PostCommand(new Commands.ResourceRemove(id));
            }
        }

        internal void RegisterHmeResource(long id, IHmeResource hmeResource)
        {
            lock(_resourceManager)
            {
                _resourceManager.AddHmeResource(id, hmeResource);
            }
        }

        internal void UnregisterHmeResource(long id)
        {
            lock (_resourceManager)
            {
                _resourceManager.RemoveHmeResource(id);
            }
        }

        internal long GetNewViewId()
        {
            lock (_resourceManager)
            {
                return _resourceManager.GetNextResourceId();
            }
        }

        #endregion

        private sealed class ResourceManager
        {
            private long _resourceId = 2048;
            private Dictionary<Resource, long> _resources = new Dictionary<Resource, long>();
            private Dictionary<long, Resource> _reverse = new Dictionary<long, Resource>();
            private Dictionary<long, IHmeResource> _hmeResources = new Dictionary<long, IHmeResource>();

            internal bool TryGetResourceId(Resource resource, out long id)
            {
                return _resources.TryGetValue(resource, out id);
            }

            internal bool TryGetResource(long id, out Resource resource)
            {
                return _reverse.TryGetValue(id, out resource);
            }

            internal bool TryGetHmeResource(long id, out IHmeResource hmeResource)
            {
                return _hmeResources.TryGetValue(id, out hmeResource);
            }

            internal long GetNextResourceId()
            {
                return ++_resourceId;
            }

            internal void Add(Resource resource, long id)
            {
                _resources.Add(resource, id);
                _reverse.Add(id, resource);
            }

            internal void AddHmeResource(long id, IHmeResource hmeResource)
            {
                _hmeResources.Add(id, hmeResource);
            }

            internal bool RemoveResourceId(long id)
            {
                Resource resource;
                if (_reverse.TryGetValue(id, out resource))
                {
                    _resources.Remove(resource);
                    return _reverse.Remove(id);
                }
                return false;
            }

            internal bool RemoveHmeResource(long id)
            {
                return _hmeResources.Remove(id);
            }
        }

        private sealed class GenericResource : IHmeResource
        {
            private string _name;
            private Application _application;
            private long _resourceId;

            public GenericResource(Application application, string name, long resourceId)
            {
                _name = name;
                _application = application;
                _resourceId = resourceId;
            }

            #region IHmeResource Members

            public string Name
            {
                get { return _name; }
            }

            public void Close()
            {
                Dispose();
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                if (_resourceId >= 2048)
                    _application.ReleaseResourceId(_resourceId);
            }

            #endregion

            #region IEquatable<IHmeResource> Members

            public bool Equals(IHmeResource other)
            {
                return Equals((object)other);
            }

            #endregion

            public override bool Equals(object obj)
            {
                GenericResource genericResource = obj as GenericResource;
                if (genericResource != null)
                    return _resourceId == genericResource._resourceId;
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return _resourceId.GetHashCode();
            }
        }
    }
}
