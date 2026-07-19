using System;
using System.Collections.Generic;

namespace Enara.Core
{
    /// <summary>
    /// A tiny service locator. Singleton-ish global access to runtime systems that aren't
    /// MonoBehaviours (e.g. factories, pools, save service).
    ///
    /// MonoBehaviours should usually be injected via the Inspector, but some systems
    /// (like SaveSystem) are awkward to wire everywhere - register them here instead.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> s_services = new();

        /// <summary>Register <paramref name="service"/> under its concrete type.</summary>
        public static void Register<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            s_services[typeof(T)] = service;
        }

        /// <summary>Try to fetch a previously registered service.</summary>
        public static T Get<T>() where T : class
        {
            return s_services.TryGetValue(typeof(T), out var s) ? s as T : null;
        }

        /// <summary>True if a service of type <typeparamref name="T"/> is registered.</summary>
        public static bool Has<T>() where T : class => s_services.ContainsKey(typeof(T));

        /// <summary>Remove a registered service. Safe to call if not registered.</summary>
        public static void Unregister<T>() where T : class => s_services.Remove(typeof(T));

        /// <summary>Drop all services. Call on application quit.</summary>
        public static void Clear() => s_services.Clear();
    }
}
