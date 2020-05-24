using System;

namespace MyTree
{
    public class MoveToDto
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
    }
}
