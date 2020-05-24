using System;

namespace MyTree
{
    public class CreateDto
    {
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
}
