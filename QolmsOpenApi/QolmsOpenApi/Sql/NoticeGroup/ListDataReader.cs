using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi
{
    public class ListDataReader<T> : IDataReader
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly PropertyInfo[] _props;
        private readonly string[] _tableColumns; // テーブル列を明示的に指定
        private T _current;

        public ListDataReader(IEnumerable<T> list, params string[] tableColumns)
        {
            _enumerator = list.GetEnumerator();
            _tableColumns = tableColumns;

            // テーブル列に対応するプロパティのみを抽出
            var allProps = typeof(T).GetProperties();
            _props = allProps
                .Where(p => tableColumns.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                .ToArray();
        }

        public bool Read()
        {
            if (_enumerator.MoveNext())
            {
                _current = _enumerator.Current;
                return true;
            }
            return false;
        }

        public int FieldCount => _props.Length;

        public object GetValue(int i)
        {
            if (_current == null) return DBNull.Value;
            var value = _props[i].GetValue(_current);
            return value ?? DBNull.Value;
        }

        public string GetName(int i) => _props[i].Name;

        public int GetOrdinal(string name)
        {
            for (int i = 0; i < _props.Length; i++)
            {
                if (string.Equals(_props[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        public Type GetFieldType(int i) => _props[i].PropertyType;

        public void Dispose() => _enumerator?.Dispose();

        // 未使用メンバー
        public void Close() { }
        public bool NextResult() => false;
        public int Depth => 0;
        public bool IsClosed => false;
        public int RecordsAffected => 0;
        public DataTable GetSchemaTable() => null;
        public bool IsDBNull(int i) => GetValue(i) == DBNull.Value;

        public object this[int i] => GetValue(i);
        public object this[string name] => GetValue(GetOrdinal(name));
        public bool GetBoolean(int i) => (bool)GetValue(i);
        public byte GetByte(int i) => (byte)GetValue(i);
        public long GetBytes(int i, long f, byte[] b, int o, int c) => 0;
        public char GetChar(int i) => (char)GetValue(i);
        public long GetChars(int i, long f, char[] c, int o, int l) => 0;
        public IDataReader GetData(int i) => null;
        public string GetDataTypeName(int i) => GetFieldType(i).Name;
        public DateTime GetDateTime(int i) => (DateTime)GetValue(i);
        public decimal GetDecimal(int i) => (decimal)GetValue(i);
        public double GetDouble(int i) => (double)GetValue(i);
        public float GetFloat(int i) => (float)GetValue(i);
        public Guid GetGuid(int i) => (Guid)GetValue(i);
        public short GetInt16(int i) => (short)GetValue(i);
        public int GetInt32(int i) => (int)GetValue(i);
        public long GetInt64(int i) => (long)GetValue(i);
        public string GetString(int i) => (string)GetValue(i);
        public int GetValues(object[] values)
        {
            for (int i = 0; i < FieldCount; i++)
                values[i] = GetValue(i);
            return FieldCount;
        }
    }
}