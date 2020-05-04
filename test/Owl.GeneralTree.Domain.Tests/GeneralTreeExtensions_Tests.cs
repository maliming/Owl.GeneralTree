using System;
using System.Collections.Generic;
using System.Linq;
using Owl.GeneralTree.App;
using Shouldly;
using Xunit;

namespace Owl.GeneralTree
{
    public class GeneralTreeExtensions_Tests : GeneralTreeDomainTestBase
    {
        [Fact]
        public void ToTree_Test()
        {
            var beijingId = Guid.NewGuid();
            var hebeiId = Guid.NewGuid();
            var chengdeId = Guid.NewGuid();
            var regions = new List<Region>
            {
                new Region(beijingId)
                {
                    Name = "beijing",
                },
                new Region(Guid.NewGuid())
                {
                    Name = "dongcheng",
                    ParentId = beijingId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "xicheng",
                    ParentId = beijingId
                },
                new Region(hebeiId)
                {
                    Name = "hebei",
                },
                new Region(Guid.NewGuid())
                {
                    Name = "shijiazhuang",
                    ParentId = hebeiId
                },
                new Region(chengdeId)
                {
                    Name = "chengde",
                    ParentId = hebeiId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "shuangqiaoqu",
                    ParentId = chengdeId
                }
            };

            var tree = regions.ToTree<Region, Guid>().ToList();

            tree.ShouldNotBeNull();
            tree.Count.ShouldBe(2);
            tree.First().Children.Count.ShouldBe(2);
            tree.Last().Children.Count.ShouldBe(2);
            tree.Last().Children.Last().Children.Count.ShouldBe(1);
        }

        [Fact]
        public void ToTreeOrderBy_Test()
        {
            var beijingId = Guid.NewGuid();
            var hebeiId = Guid.NewGuid();
            var chengdeId = Guid.NewGuid();
            var regions = new List<Region>
            {
                new Region(beijingId)
                {
                    Name = "b-beijing",
                },
                new Region(Guid.NewGuid())
                {
                    Name = "b-dongcheng",
                    ParentId = beijingId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "a-xicheng",
                    ParentId = beijingId
                },
                new Region(hebeiId)
                {
                    Name = "a-hebei",
                },
                new Region(Guid.NewGuid())
                {
                    Name = "b-shijiazhuang",
                    ParentId = hebeiId
                },
                new Region(chengdeId)
                {
                    Name = "a-chengde",
                    ParentId = hebeiId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "b-shuangqiaoqu",
                    ParentId = chengdeId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "a-shuangluan",
                    ParentId = chengdeId
                }
            };

            var tree = regions.ToTreeOrderBy<Region, Guid, string>(x => x.Name).ToList();

            tree.First().Name.ShouldBe("a-hebei");
            tree.First().Children.First().Name.ShouldBe("a-chengde");
            tree.First().Children.First().Children.First().Name.ShouldBe("a-shuangluan");
        }

        [Fact]
        public void ToTreeOrderByDescending_Test()
        {
            var beijingId = Guid.NewGuid();
            var hebeiId = Guid.NewGuid();
            var chengdeId = Guid.NewGuid();
            var regions = new List<Region>
            {
                new Region(beijingId)
                {
                    Name = "b-beijing",
                },
                new Region(Guid.NewGuid())
                {
                    Name = "b-dongcheng",
                    ParentId = beijingId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "a-xicheng",
                    ParentId = beijingId
                },
                new Region(hebeiId)
                {
                    Name = "a-hebei",
                },
                new Region(Guid.NewGuid())
                {
                    Name = "b-shijiazhuang",
                    ParentId = hebeiId
                },
                new Region(chengdeId)
                {
                    Name = "a-chengde",
                    ParentId = hebeiId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "b-shuangqiaoqu",
                    ParentId = chengdeId
                },
                new Region(Guid.NewGuid())
                {
                    Name = "a-shuangluan",
                    ParentId = chengdeId
                }
            };

            var tree = regions.ToTreeOrderByDescending<Region, Guid, string>(x => x.Name).ToList();

            tree.First().Name.ShouldBe("b-beijing");
            tree.First().Children.First().Name.ShouldBe("b-dongcheng");
        }
    }
}
