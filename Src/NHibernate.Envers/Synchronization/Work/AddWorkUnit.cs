﻿using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
	public class AddWorkUnit : AbstractAuditWorkUnit
	{
		private readonly IDictionary<string, object> data;

		public AddWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg,
						   object id, IEntityPersister entityPersister, object[] state)
			: base(sessionImplementor, entityName, verCfg, id, RevisionType.Added)
		{
			State = state;
			data = new Dictionary<string, object>();
			verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, data,
					entityPersister.PropertyNames, state, null);
		}

		private AddWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg,
						   object id, IDictionary<string, object> data)
			: base(sessionImplementor, entityName, verCfg, id, RevisionType.Added)
		{
			this.data = data;
		}

		internal object[] State { get; }

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			FillDataWithId(data, revisionData);
			return data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			return new AddWorkUnit(SessionImplementor, EntityName, VerCfg, EntityId, mergeModifiedFlags(data, second.Data));
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			second.MergeCollectionModifiedData(data);
			return this;
		}

		public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
		{
			return FakeBidirectionalRelationWorkUnit.Merge(second, this, second.NestedWorkUnit);
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			return first.Merge(this);
		}

		private IDictionary<string, object> mergeModifiedFlags(IDictionary<string, object> lhs, IDictionary<string, object> rhs)
		{
			var mapper = this.VerCfg.EntCfg[this.EntityName].PropertyMapper;
			foreach (var propertyData in mapper.Properties.Keys)
			{
				if (propertyData.UsingModifiedFlag)
				{
					object lhsUntypedValue;
					if (lhs.TryGetValue(propertyData.ModifiedFlagPropertyName, out lhsUntypedValue))
					{
						var lhsValue = (bool)lhsUntypedValue;
						if (lhsValue)
						{
							rhs[propertyData.ModifiedFlagPropertyName] = true;
						}
					}
				}
			}
			return rhs;
		}
	}
}
