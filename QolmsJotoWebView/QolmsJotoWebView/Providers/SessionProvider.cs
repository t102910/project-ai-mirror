using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Providers
{
       
    // セッション管理用のインターフェース
    public interface ISessionProvider
        {
        void Set(string key, object value);
        T Get<T>(string key);
        void Remove(string key);
    }

    // セッション管理の実装
    public class SessionProvider : ISessionProvider
    {
        private readonly HttpSessionStateBase _session;

        public SessionProvider(HttpSessionStateBase session)
        {
            _session = session;
        }

        public void Set(string key, object value)
        {
            _session[key] = value;
        }

        public T Get<T>(string key)
        {
            return (T)_session[key];
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }
    }

}

