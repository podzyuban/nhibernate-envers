using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomType
{
	public class CLOBTestUserType : IUserType
	{
		private static readonly SqlType[] TYPES = { new StringClobSqlType() };

		public new bool Equals(object x, object y)
		{
			//noinspection ObjectEquality
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return NHibernateUtil.String.NullSafeGet(rs, names[0], session);
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			NHibernateUtil.String.NullSafeSet(cmd, value, index, session);
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public SqlType[] SqlTypes
		{
			get { return TYPES; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(string); }
		}

		public bool IsMutable
		{
			get { return false; }
		}
	}
}