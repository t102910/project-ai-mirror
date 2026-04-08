using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public static class QjJsonResultBaseExtension
    {
        public static TEntity Copy<TEntity>(this TEntity target)
        {

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, target);
                ms.Position = 0;

                return (TEntity)bf.Deserialize(ms);
            }
        }
    }
}