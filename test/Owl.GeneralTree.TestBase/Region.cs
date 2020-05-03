using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Owl.GeneralTree.App
{
    public class Region : AggregateRoot<Guid>, IGeneralTree<Region, Guid>
    {
        public Region()
        {

        }

        public Region(Guid id)
            : base(id)
        {

        }

        public virtual string MyCustomData { get; set; }

        public virtual int SomeForeignKey { get; set; }

        public virtual string Name { get; set; }

        public virtual string FullName { get; set; }

        public virtual string Code { get; set; }

        public virtual int Level { get; set; }

        public virtual Region Parent { get; set; }

        public virtual Guid? ParentId { get; set; }

        public virtual ICollection<Region> Children { get; set; }
    }
}
