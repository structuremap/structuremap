using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using NMock;
using StructureMap.Graph;

namespace StructureMap
{
	public delegate void Notify();

	/// <summary>
	/// The main static Facade for the StructureMap container
	/// </summary>
	[EnvironmentPermission(SecurityAction.Assert, Read="COMPUTERNAME")]
	public class ObjectFactory
	{
		private static string _profile = string.Empty;
		private static event Notify _notify;
		private static InstanceManager _manager;
		private static object _lockObject = new object();


		static ObjectFactory()
		{
			ObjectFactoryCacheCallback callback = new ObjectFactoryCacheCallback();
		}

		#region InstanceManager and setting defaults

		private static InstanceManager manager
		{
			get
			{
				if (_manager == null)
				{
					lock (_lockObject)
					{
						if (_manager == null)
						{
							Reset();
						}
					}
				}

				return _manager;
			}
		}


		internal static void Reset()
		{
			_manager = buildManager();

			if (_notify != null)
			{
				_notify();
			}
		}


		/// <summary>
		/// Fires when the ObjectFactory is refreshed
		/// </summary>
		public static event Notify Refresh
		{
			add { _notify += value; }
			remove { _notify -= value; }
		}


		/// <summary>
		/// Gets or sets the current named profile.  When set, overrides the default object instances
		/// according to the configured profile in StructureMap.config
		/// </summary>
		public static string Profile
		{
			set
			{
				lock (_lockObject)
				{
					_profile = value;

					PluginGraphBuilder builder = new PluginGraphBuilder();

					setDefaults(builder, manager);
				}
			}
			get { return _profile; }
		}

		private static void setDefaults(PluginGraphBuilder builder, InstanceManager instanceManager)
		{
			// The authenticated user may not have required privileges to read from Environment
			string machineName = InstanceDefaultManager.GetMachineName();

			InstanceDefaultManager instanceDefaultManager = builder.DefaultManager;
			Profile defaultProfile = instanceDefaultManager.CalculateDefaults(machineName, Profile);

            instanceManager.SetDefaults(defaultProfile);
		}


		/// <summary>
		/// Restores all default instance settings according to the StructureMap.config files
		/// </summary>
		public static void ResetDefaults()
		{
			try
			{
				lock (_lockObject)
				{
					manager.UnMockAll();
					Profile = string.Empty;
				}
			}
			catch (StructureMapException)
			{
				throw;
			}
			catch (TypeInitializationException ex)
			{
				if (ex.InnerException is StructureMapException)
				{
					throw ex.InnerException;
				}
				else
				{
					throw ex;
				}
			}
		}


		private static InstanceManager buildManager()
		{
			PluginGraphBuilder builder = new PluginGraphBuilder();
			PluginGraph graph = builder.Build();

			InstanceManager manager = new InstanceManager(graph);
			setDefaults(builder, manager);

			return manager;
		}

		#endregion

		#region GetInstance

		/// <summary>
		/// Returns the default instance of the requested System.Type
		/// </summary>
		/// <param name="TargetType"></param>
		/// <returns></returns>
		public static object GetInstance(Type TargetType)
		{
			return manager.CreateInstance(TargetType);
		}

        /// <summary>
        /// Returns the default instance of the requested System.Type
        /// </summary>
        /// <param name="TargetType"></param>
        /// <returns></returns>
        public static TargetType GetInstance<TargetType>()
        {
            return (TargetType)manager.CreateInstance(typeof(TargetType));
        }

		/// <summary>
		/// Builds an instance of the TargetType for the given InstanceMemento
		/// </summary>
		/// <param name="TargetType"></param>
		/// <param name="memento"></param>
		/// <returns></returns>
		public static object GetInstance(Type TargetType, InstanceMemento memento)
		{
			return manager.CreateInstance(TargetType, memento);
		}

        /// <summary>
        /// Builds an instance of the TargetType for the given InstanceMemento
        /// </summary>
        /// <param name="TargetType"></param>
        /// <param name="memento"></param>
        /// <returns></returns>
        public static TargetType GetInstance<TargetType>(InstanceMemento memento)
        {
            return (TargetType)manager.CreateInstance(typeof(TargetType), memento);
        }

		/// <summary>
		/// Returns the named instance of the requested System.Type
		/// </summary>
		/// <param name="TargetType"></param>
		/// <param name="InstanceName"></param>
		/// <returns></returns>
		public static object GetNamedInstance(Type TargetType, string InstanceName)
		{
			return manager.CreateInstance(TargetType, InstanceName);
		}

        /// <summary>
        /// Returns the named instance of the requested System.Type
        /// </summary>
        /// <param name="TargetType"></param>
        /// <param name="InstanceName"></param>
        /// <returns></returns>
        public static TargetType GetNamedInstance<TargetType>(string InstanceName)
        {
            return (TargetType)manager.CreateInstance(typeof(TargetType), InstanceName);
        }

		/// <summary>
		/// Sets the default instance of the TargetType
		/// </summary>
		/// <param name="TargetType"></param>
		/// <param name="InstanceName"></param>
		public static void SetDefaultInstanceName(Type TargetType, string InstanceName)
		{
			manager.SetDefault(TargetType, InstanceName);
		}

        /// <summary>
        /// Sets the default instance of the TargetType
        /// </summary>
        /// <param name="TargetType"></param>
        /// <param name="InstanceName"></param>
        public static void SetDefaultInstanceName<TargetType>(string InstanceName)
        {
            manager.SetDefault(typeof(TargetType), InstanceName);
        }

		/// <summary>
		/// Sets the default instance of the TargetType
		/// </summary>
		/// <param name="TargetTypeName"></param>
		/// <param name="InstanceName"></param>
		public static void SetDefaultInstanceName(string TargetTypeName, string InstanceName)
		{
			manager.SetDefault(TargetTypeName, InstanceName);
		}


		/// <summary>
		/// Retrieves a list of all of the configured instances for a particular type
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static IList GetAllInstances(Type targetType)
		{
			return manager.GetAllInstances(targetType);
		}

        /// <summary>
        /// Retrieves a list of all of the configured instances for a particular type
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static IList<TargetType> GetAllInstances<TargetType>()
        {
            IList instances = manager.GetAllInstances(typeof(TargetType));
            IList<TargetType> specificInstances = new List<TargetType>();
            foreach (object instance in instances)
            {
                TargetType obj = (TargetType)instance;
                specificInstances.Add(obj);
            }
            
            return specificInstances;
        }

		#endregion

		/// <summary>
		/// Attempts to create a new instance of the requested type.  Automatically inserts the default
		/// configured instance for each dependency in the StructureMap constructor function.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object FillDependencies(Type type)
		{
			return _manager.FillDependencies(type);
		}

        public static T FillDependencies<T>()
        {
            return (T)_manager.FillDependencies(typeof(T));
        }
	    
		#region Mocking

		/// <summary>
		/// When called, returns an NMock.IMock instance for the TargetType.  Until UnMocked, calling 
		/// GetInstance(Type TargetType) will return the MockInstance member of the IMock
		/// </summary>
		/// <param name="TargetType"></param>
		/// <returns></returns>
		public static IMock Mock(Type TargetType)
		{
			return manager.Mock(TargetType);
		}

		/// <summary>
		/// Sets up the internal InstanceManager to return the object in the "stub" argument anytime
		/// any instance of the PluginType is requested
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="stub"></param>
		public static void InjectStub(Type targetType, object stub)
		{
			manager.InjectStub(targetType, stub);
		}

		/// <summary>
		/// Is the specified TargetType currently setup as an IMock
		/// </summary>
		/// <param name="TargetType"></param>
		/// <returns></returns>
		public static bool IsMocked(Type TargetType)
		{
			return manager.IsMocked(TargetType);
		}

		/// <summary>
		/// Release the NMock behavior of TargetType
		/// </summary>
		/// <param name="TargetType"></param>
		public static void UnMock(Type TargetType)
		{
			manager.UnMock(TargetType);
		}

		#endregion


		public static string WhatDoIHave()
		{
			StringBuilder sb = new StringBuilder();

			foreach (IInstanceFactory factory in manager)
			{
				sb.AppendFormat("PluginType {0}, Default: {1}\r\n", factory.PluginType.FullName, factory.DefaultInstanceKey);
			}

			return sb.ToString();
		}

	}
}