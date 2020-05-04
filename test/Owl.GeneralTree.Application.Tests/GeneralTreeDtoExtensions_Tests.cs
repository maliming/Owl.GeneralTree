using System;
using System.Collections.Generic;
using System.Linq;
using Owl.GeneralTree.App;
using Shouldly;
using Xunit;

namespace Owl.GeneralTree
{
    public class GeneralTreeDtoExtensions_Tests : GeneralTreeApplicationTestBase
    {
        [Fact]
        public void ToTree_Test()
        {
            var beijingId = Guid.NewGuid();
            var hebeiId = Guid.NewGuid();
            var chengdeId = Guid.NewGuid();
            var RegionDtos = new List<RegionDto>
            {
                new RegionDto(beijingId)
                {
                    Name = "beijing",
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "dongcheng",
                    ParentId = beijingId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "xicheng",
                    ParentId = beijingId
                },
                new RegionDto(hebeiId)
                {
                    Name = "hebei",
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "shijiazhuang",
                    ParentId = hebeiId
                },
                new RegionDto(chengdeId)
                {
                    Name = "chengde",
                    ParentId = hebeiId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "shuangqiaoqu",
                    ParentId = chengdeId
                }
            };

            var tree = RegionDtos.ToTree<RegionDto, Guid>().ToList();

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
            var RegionDtos = new List<RegionDto>
            {
                new RegionDto(beijingId)
                {
                    Name = "b-beijing",
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "b-dongcheng",
                    ParentId = beijingId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "a-xicheng",
                    ParentId = beijingId
                },
                new RegionDto(hebeiId)
                {
                    Name = "a-hebei",
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "b-shijiazhuang",
                    ParentId = hebeiId
                },
                new RegionDto(chengdeId)
                {
                    Name = "a-chengde",
                    ParentId = hebeiId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "b-shuangqiaoqu",
                    ParentId = chengdeId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "a-shuangluan",
                    ParentId = chengdeId
                }
            };

            var tree = RegionDtos.ToTreeOrderBy<RegionDto, Guid, string>(x => x.Name).ToList();

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
            var RegionDtos = new List<RegionDto>
            {
                new RegionDto(beijingId)
                {
                    Name = "b-beijing",
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "b-dongcheng",
                    ParentId = beijingId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "a-xicheng",
                    ParentId = beijingId
                },
                new RegionDto(hebeiId)
                {
                    Name = "a-hebei",
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "b-shijiazhuang",
                    ParentId = hebeiId
                },
                new RegionDto(chengdeId)
                {
                    Name = "a-chengde",
                    ParentId = hebeiId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "b-shuangqiaoqu",
                    ParentId = chengdeId
                },
                new RegionDto(Guid.NewGuid())
                {
                    Name = "a-shuangluan",
                    ParentId = chengdeId
                }
            };

            var tree = RegionDtos.ToTreeOrderByDescending<RegionDto, Guid, string>(x => x.Name).ToList();

            tree.First().Name.ShouldBe("b-beijing");
            tree.First().Children.First().Name.ShouldBe("b-dongcheng");
        }
    }
}
