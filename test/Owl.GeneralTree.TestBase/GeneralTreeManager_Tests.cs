using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NSubstitute;
using Owl.GeneralTree.App;
using Owl.GeneralTree.Localization;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Xunit;

namespace Owl.GeneralTree
{
    public abstract class GeneralTreeManager_Tests<TStartupModule> : GeneralTreeTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IGeneralTreeManager<Region, Guid> _generalTreeManager;
        private readonly IGeneralTreeRepository<Region, Guid> _generalTreeRepository;
        private readonly IGeneralTreeCodeGenerator _generalTreeCodeGenerator;

        protected GeneralTreeManager_Tests()
        {
            _generalTreeManager = GetRequiredService<IGeneralTreeManager<Region, Guid>>();
            _generalTreeRepository = GetRequiredService<IGeneralTreeRepository<Region, Guid>>();
            _generalTreeCodeGenerator = GetRequiredService<IGeneralTreeCodeGenerator>();
        }

        #region
        private async Task<Region> GetRegion(string name)
        {
            return await _generalTreeRepository.GetByNameAsync(name);
        }

        private async Task<Region> CreateRegion(string name, Guid? parentId = null)
        {
            var region = new Region
            {
                Name = name,
                ParentId = parentId
            };
            await _generalTreeManager.CreateAsync(region);
            return region;
        }
        #endregion

        #region TestOK

        [Fact]
        public async Task CreateAsync()
        {
            //Act
            await CreateRegion("beijing");

            //Assert
            var region = await GetRegion("beijing");
            region.ShouldNotBeNull();
            region.Name.ShouldBe("beijing");
            region.FullName.ShouldBe("beijing");
            region.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1));
            region.Level.ShouldBe(1);
            region.ParentId.ShouldBeNull();
        }

        [Fact]
        public async Task Create_With_ParentId()
        {
            //Act
            var beijing = new Region
            {
                Name = "beijing"
            };
            await _generalTreeManager.CreateAsync(beijing);

            var xicheng = new Region
            {
                Name = "xicheng",
                ParentId = beijing.Id
            };
            await _generalTreeManager.CreateAsync(xicheng);

            var dongcheng = new Region
            {
                Name = "dongcheng",
                ParentId = beijing.Id
            };
            await _generalTreeManager.CreateAsync(dongcheng);

            //Assert
            var xc = await GetRegion("xicheng");
            xc.ShouldNotBeNull();
            xc.Name.ShouldBe("xicheng");
            xc.FullName.ShouldBe("beijing-xicheng");
            xc.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 1));
            xc.Level.ShouldBe(beijing.Level + 1);
            xc.ParentId.ShouldBe(beijing.Id);

            var dc = await GetRegion("dongcheng");
            dc.ShouldNotBeNull();
            dc.Name.ShouldBe("dongcheng");
            dc.FullName.ShouldBe("beijing-dongcheng");
            dc.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2));
            dc.Level.ShouldBe(beijing.Level + 1);
            dc.ParentId.ShouldBe(beijing.Id);
        }

        [Fact]
        public async Task Create_Should_Not_With_Same_Name_Test()
        {
            //Act
            await _generalTreeManager.CreateAsync(new Region
            {
                Name = "beijing"
            });

            //Assert
            var exception = await Record.ExceptionAsync(async () => await _generalTreeManager.CreateAsync(
                new Region
                {
                    Name = "beijing"
                }
            ));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<UserFriendlyException>();
            exception.Message.ShouldBe(GetRequiredService<IStringLocalizer<GeneralTreeResource>>()["GeneralTreeNameIsDuplicate", "beijing"]);
        }

        [Fact]
        public async Task BulkCreate_Test()
        {
            //Act
            var beijing = new Region(Guid.NewGuid())
            {
                Name = "beijing"
            };

            var xicheng = new Region(Guid.NewGuid())
            {
                Name = "xicheng",
                ParentId = beijing.Id
            };

            var xicheng_x = new Region(Guid.NewGuid())
            {
                Name = "xicheng_x",
                ParentId = xicheng.Id
            };

            var xicheng_y = new Region(Guid.NewGuid())
            {
                Name = "xicheng_y",
                ParentId = xicheng.Id
            };

            xicheng.Children = new List<Region>
            {
                xicheng_x,
                xicheng_y
            };

            beijing.Children = new List<Region>
            {
                xicheng,
                new Region(Guid.NewGuid())
                {
                    Name = "dongcheng",
                    ParentId = beijing.Id
                }
            };

            await _generalTreeManager.BulkCreateAsync(beijing);

            //Assert
            var bj = await GetRegion("beijing");
            bj.ShouldNotBeNull();
            bj.Name.ShouldBe("beijing");
            bj.FullName.ShouldBe("beijing");
            bj.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1));
            bj.Level.ShouldBe(1);
            bj.ParentId.ShouldBeNull();

            var xc = await GetRegion("xicheng");
            xc.ShouldNotBeNull();
            xc.Name.ShouldBe("xicheng");
            xc.FullName.ShouldBe("beijing-xicheng");
            xc.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 1));
            xc.Level.ShouldBe(beijing.Level + 1);
            xc.ParentId.ShouldBe(beijing.Id);

            var xc_x = await GetRegion("xicheng_x");
            xc_x.ShouldNotBeNull();
            xc_x.Name.ShouldBe("xicheng_x");
            xc_x.FullName.ShouldBe("beijing-xicheng-xicheng_x");
            xc_x.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 1, 1));
            xc_x.Level.ShouldBe(xc.Level + 1);
            xc_x.ParentId.ShouldBe(xc.Id);

            var dc = await GetRegion("dongcheng");
            dc.ShouldNotBeNull();
            dc.Name.ShouldBe("dongcheng");
            dc.FullName.ShouldBe("beijing-dongcheng");
            dc.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2));
            dc.Level.ShouldBe(beijing.Level + 1);
            dc.ParentId.ShouldBe(beijing.Id);
        }

        [Fact]
        public async Task BulkCreate_ExistTree_Test()
        {
            //Arrange
            var heibei = await CreateRegion("hebei");
            await CreateRegion("shijiazhuang", heibei.Id);

            var chengde = new Region(Guid.NewGuid())
            {
                Name = "chengde",
                ParentId = heibei.Id
            };

            chengde.Children = new List<Region>
            {
                new Region(Guid.NewGuid())
                {
                    Name = "shuangqiaoqu",
                    ParentId = chengde.Id,
                },
                new Region(Guid.NewGuid())
                {
                    Name = "shuangluanqu",
                    ParentId = chengde.Id,
                }
            };

            await _generalTreeManager.BulkCreateAsync(chengde);

            //Assert
            chengde = await GetRegion("chengde");
            chengde.ShouldNotBeNull();
            chengde.Name.ShouldBe("chengde");
            chengde.FullName.ShouldBe("hebei-chengde");
            chengde.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2));
            chengde.Level.ShouldBe(2);
            chengde.ParentId.ShouldBe(heibei.Id);

            var shuangqiaoqu = await GetRegion("shuangqiaoqu");
            shuangqiaoqu.ShouldNotBeNull();
            shuangqiaoqu.Name.ShouldBe("shuangqiaoqu");
            shuangqiaoqu.FullName.ShouldBe("hebei-chengde-shuangqiaoqu");
            shuangqiaoqu.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2, 1));
            shuangqiaoqu.Level.ShouldBe(chengde.Level + 1);
            shuangqiaoqu.ParentId.ShouldBe(chengde.Id);

            var shuangluanqu = await GetRegion("shuangluanqu");
            shuangluanqu.ShouldNotBeNull();
            shuangluanqu.Name.ShouldBe("shuangluanqu");
            shuangluanqu.FullName.ShouldBe("hebei-chengde-shuangluanqu");
            shuangluanqu.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2, 2));
            shuangluanqu.Level.ShouldBe(chengde.Level + 1);
            shuangluanqu.ParentId.ShouldBe(chengde.Id);
        }

        [Fact]
        public async Task BulkCreateChildrenAsync()
        {
            //Act
            var beijing = await CreateRegion("beijing");

            var xicheng = new Region(Guid.NewGuid())
            {
                Name = "xicheng",
                ParentId = beijing.Id
            };

            var dongcheng = new Region(Guid.NewGuid())
            {
                Name = "dongcheng",
                ParentId = beijing.Id
            };

            dongcheng.Children = new List<Region>
            {
                new Region(Guid.NewGuid())
                {
                    Name = "tiantan",
                    ParentId = dongcheng.Id,
                }
            };
            await _generalTreeManager.BulkCreateChildrenAsync(beijing, new List<Region>
            {
                xicheng,
                dongcheng
            });

            //Assert
            var xc = await GetRegion("xicheng");
            xc.ShouldNotBeNull();
            xc.Name.ShouldBe("xicheng");
            xc.FullName.ShouldBe("beijing-xicheng");
            xc.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 1));
            xc.Level.ShouldBe(beijing.Level + 1);
            xc.ParentId.ShouldBe(beijing.Id);

            var dc = await GetRegion("dongcheng");
            dc.ShouldNotBeNull();
            dc.Name.ShouldBe("dongcheng");
            dc.FullName.ShouldBe("beijing-dongcheng");
            dc.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2));
            dc.Level.ShouldBe(beijing.Level + 1);
            dc.ParentId.ShouldBe(beijing.Id);

            var tiantan = await GetRegion("tiantan");
            tiantan.ShouldNotBeNull();
            tiantan.Name.ShouldBe("tiantan");
            tiantan.FullName.ShouldBe("beijing-dongcheng-tiantan");
            tiantan.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2, 1));
            tiantan.Level.ShouldBe(dc.Level + 1);
            tiantan.ParentId.ShouldBe(dc.Id);
        }

        [Fact]
        public async Task FillUpAsync()
        {
            //Act
            var beijing = new Region
            {
                Name = "beijing"
            };

            var xicheng = new Region
            {
                Name = "xicheng",
                ParentId = beijing.Id
            };

            var dongcheng = new Region
            {
                Name = "dongcheng",
                ParentId = beijing.Id
            };

            var balizhuang = new Region
            {
                Name = "balizhuang",
                ParentId = dongcheng.Id
            };
            dongcheng.Children = new List<Region>
            {
                balizhuang
            };

            beijing.Children = new List<Region>
            {
                xicheng,
                dongcheng
            };

            await _generalTreeManager.FillUpAsync(beijing);

            //Assert
            beijing.FullName.ShouldBe("beijing");
            beijing.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1));
            beijing.Level.ShouldBe(1);
            beijing.ParentId.ShouldBeNull();
            beijing.Children.Count.ShouldBe(2);

            xicheng.FullName.ShouldBe("beijing-xicheng");
            xicheng.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 1));
            xicheng.Level.ShouldBe(beijing.Level + 1);
            xicheng.ParentId.ShouldBe(beijing.Id);

            dongcheng.FullName.ShouldBe("beijing-dongcheng");
            dongcheng.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2));
            dongcheng.Level.ShouldBe(beijing.Level + 1);
            dongcheng.ParentId.ShouldBe(beijing.Id);

            balizhuang.FullName.ShouldBe("beijing-dongcheng-balizhuang");
            balizhuang.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 2, 1));
            balizhuang.Level.ShouldBe(dongcheng.Level + 1);
            balizhuang.ParentId.ShouldBe(dongcheng.Id);
        }

        [Fact]
        public async Task UpdateNameAsync()
        {
            //Arrange
            await CreateRegion("beijing");

            //Act
            var beijing = await GetRegion("beijing");
            beijing.Name = "newbeijing";
            await _generalTreeManager.UpdateNameAsync(beijing);

            //Assert
            var newbeijing = await GetRegion("newbeijing");
            newbeijing.ShouldNotBeNull();
            newbeijing.Name.ShouldBe("newbeijing");
            newbeijing.FullName.ShouldBe("newbeijing");
            newbeijing.Code.ShouldBe(_generalTreeCodeGenerator.CreateCode(1));
        }

        [Fact]
        public async Task UpdateName_ChildrenAction_Test()
        {
            //Arrange
            var beijing = await CreateRegion("beijing");
            await CreateRegion("xicheng", beijing.Id);

            //Act
            beijing = await GetRegion("beijing");
            beijing.Name = "newbeijing";
            await _generalTreeManager.UpdateNameAsync(beijing, x => { x.MyCustomData = x.Code; });

            //Assert
            var xicheng = await GetRegion("xicheng");
            xicheng.FullName.ShouldBe("newbeijing-xicheng");
            xicheng.MyCustomData.ShouldBe(_generalTreeCodeGenerator.CreateCode(1, 1));
        }

        [Fact]
        public async Task UpdateName_Child_FullName_ShouldBe_Update_Test()
        {
            //Arrange
            var beijing = await CreateRegion("beijing");
            await CreateRegion("xicheng", beijing.Id);

            //Act
            beijing = await GetRegion("beijing");
            beijing.Name = "newbeijing";
            await _generalTreeManager.UpdateNameAsync(beijing);

            //Assert
            var xicheng = await GetRegion("xicheng");
            xicheng.FullName.ShouldBe("newbeijing-xicheng");
        }

        [Fact]
        public async Task UpdateName_FullName_ShouldBe_Update_With_Parent_FullName_Test()
        {
            //Arrange
            var beijing = await CreateRegion("beijing");
            await CreateRegion("xicheng", beijing.Id);

            //Act
            var xicheng = await GetRegion("xicheng");
            xicheng.Name = "newxicheng";
            await _generalTreeManager.UpdateNameAsync(xicheng);

            //Assert
            var newxicheng = await GetRegion("newxicheng");
            newxicheng.FullName.ShouldBe("beijing-newxicheng");
        }

        #endregion

        [Fact]
        public async Task MoveAsync()
        {
            //Act
            var beijing = await CreateRegion("beijing");
            await CreateRegion("dongcheng", beijing.Id);
            await CreateRegion("xicheng", beijing.Id);

            var hebei = await CreateRegion("hebei");
            await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);
            await CreateRegion("shaungqiao", chengde.Id);
            await CreateRegion("shaungluan", chengde.Id);

            await _generalTreeManager.MoveToAsync(chengde.Id, beijing.Id);

            //Assert
            var beijingLastChild = await GetRegion("xicheng");
            beijingLastChild.ShouldNotBeNull();
            var cd = await GetRegion(chengde.Name);
            cd.ShouldNotBeNull();
            cd.FullName.ShouldBe(beijing.FullName + "-" + chengde.Name);
            cd.ParentId.ShouldBe(beijing.Id);
            cd.Level.ShouldBe(beijing.Level + 1);
            cd.Code.ShouldBe(_generalTreeCodeGenerator.GetNextCode(beijingLastChild.Code));
        }

        [Fact]
        public async Task Move_ChildrenAction_Test()
        {
            //Act
            var beijing = await CreateRegion("beijing");
            await CreateRegion("dongcheng", beijing.Id);
            await CreateRegion("xicheng", beijing.Id);

            var hebei = await CreateRegion("hebei");
            await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);

            await CreateRegion("shaungqiao", chengde.Id);
            await CreateRegion("shaungluan", chengde.Id);

            var beijingLastChild = await GetRegion("xicheng");
            beijingLastChild.ShouldNotBeNull();
            await _generalTreeManager.MoveToAsync(chengde.Id, beijing.Id, x => { x.MyCustomData = x.Code; });

            //Assert
            var shaungqiao = await GetRegion("shaungqiao");
            shaungqiao.ShouldNotBeNull();
            shaungqiao.MyCustomData.ShouldBe(shaungqiao.Code);
        }

        [Fact]
        public async Task Move_Root_Parent_Test()
        {
            //Act
            var beijing = await CreateRegion("beijing");
            await CreateRegion("dongcheng", beijing.Id);
            await CreateRegion("xicheng", beijing.Id);

            var hebei = await CreateRegion("hebei");
            await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);

            var shuangqiao = await CreateRegion("shaungqiao", chengde.Id);
            await CreateRegion("shaungluan", chengde.Id);

            var beijingLastChild = await GetRegion("xicheng");
            beijingLastChild.ShouldNotBeNull();
            await _generalTreeManager.MoveToAsync(shuangqiao.Id, null);

            //Assert
            var shaungqiao = await GetRegion("shaungqiao");
            shaungqiao.ShouldNotBeNull();
            shaungqiao.FullName.ShouldBe("shaungqiao");
            shaungqiao.ParentId.ShouldBe(null);
            shaungqiao.Level.ShouldBe(1);
            shaungqiao.Code.ShouldBe(_generalTreeCodeGenerator.GetNextCode(hebei.Code));
        }

        [Fact]
        public async Task MoveToBeforeAsync()
        {
            //Act
            var beijing = await CreateRegion("beijing");
            await CreateRegion("dongcheng", beijing.Id);
            var xicheng = await CreateRegion("xicheng", beijing.Id);
            var changanjie = await CreateRegion("changanjie", xicheng.Id);

            var hebei = await CreateRegion("hebei");
            await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);
            await CreateRegion("shaungqiao", chengde.Id);
            await CreateRegion("shaungluan", chengde.Id);

            await _generalTreeManager.MoveToBeforeAsync(chengde.Id, xicheng.Id);

            //Assert
            var cd = await GetRegion(chengde.Name);
            cd.ShouldNotBeNull();
            cd.FullName.ShouldBe(beijing.FullName + "-" + chengde.Name);
            cd.ParentId.ShouldBe(beijing.Id);
            cd.Level.ShouldBe(beijing.Level + 1);
            cd.Code.ShouldBe(xicheng.Code);

            xicheng = await GetRegion("xicheng");
            xicheng.ShouldNotBeNull();
            xicheng.FullName.ShouldBe(beijing.FullName + "-" + xicheng.Name);
            xicheng.ParentId.ShouldBe(beijing.Id);
            xicheng.Level.ShouldBe(beijing.Level + 1);
            xicheng.Code.ShouldBe(_generalTreeCodeGenerator.GetNextCode(cd.Code));

            changanjie = await GetRegion("changanjie");
            changanjie.ShouldNotBeNull();
            changanjie.FullName.ShouldBe(beijing.FullName + "-" + xicheng.Name+ "-" + changanjie.Name);
            changanjie.ParentId.ShouldBe(xicheng.Id);
            changanjie.Level.ShouldBe(xicheng.Level + 1);
            changanjie.Code.ShouldBe(_generalTreeCodeGenerator.MergeCode(xicheng.Code,
                _generalTreeCodeGenerator.RemoveParentCode(changanjie.Code, changanjie.Level - 1)));

            var shaungqiao = await GetRegion("shaungqiao");
            shaungqiao.ShouldNotBeNull();
            shaungqiao.FullName.ShouldBe(cd.FullName + "-" + shaungqiao.Name);
            shaungqiao.ParentId.ShouldBe(cd.Id);
            shaungqiao.Level.ShouldBe(cd.Level + 1);
            shaungqiao.Code.ShouldBe(_generalTreeCodeGenerator.MergeCode(cd.Code, _generalTreeCodeGenerator.GetLastCode(shaungqiao.Code)));
        }

        [Fact]
        public async Task MoveToBefore_Children_Test()
        {
            //Act
            var beijing = await CreateRegion("beijing");
            await CreateRegion("dongcheng", beijing.Id);
            await CreateRegion("xicheng", beijing.Id);

            var hebei = await CreateRegion("hebei");
            await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);
            await CreateRegion("shaungqiao", chengde.Id);
            await CreateRegion("shaungluan", chengde.Id);

            await _generalTreeManager.MoveToBeforeAsync(chengde.Id, hebei.Id);

            //Assert
            var cd = await GetRegion(chengde.Name);
            cd.ShouldNotBeNull();
            cd.FullName.ShouldBe(chengde.Name);
            cd.ParentId.ShouldBe(beijing.ParentId);
            cd.Level.ShouldBe(beijing.Level);
            cd.Code.ShouldBe(_generalTreeCodeGenerator.GetNextCode(beijing.Code));

            var shaungqiao = await GetRegion("shaungqiao");
            shaungqiao.ShouldNotBeNull();
            shaungqiao.FullName.ShouldBe(cd.FullName + "-" + shaungqiao.Name);
            shaungqiao.ParentId.ShouldBe(cd.Id);
            shaungqiao.Level.ShouldBe(cd.Level + 1);
            shaungqiao.Code.ShouldBe(_generalTreeCodeGenerator.MergeCode(cd.Code, _generalTreeCodeGenerator.GetLastCode(shaungqiao.Code)));
        }

        [Fact]
        public async Task Delete_Test()
        {
            //Act
            var hebei = await CreateRegion("hebei");

            await CreateRegion("shijiazhuang", hebei.Id);

            var chengde = await CreateRegion("chengde", hebei.Id);
            await CreateRegion("shaungqiao", chengde.Id);
            await CreateRegion("shaungluan", chengde.Id);

            await _generalTreeManager.DeleteAsync(hebei.Id);

            //Assert
            var hb = await GetRegion("hebei");
            hb.ShouldBeNull();

            var sjz = await GetRegion("shijiazhuang");
            sjz.ShouldBeNull();

            var cd = await GetRegion("chengde");
            cd.ShouldBeNull();

            var cdsq = await GetRegion("shaungqiao");
            cdsq.ShouldBeNull();
        }

        [Fact]
        public async Task RegenerateAsync()
        {
            var hebei = await CreateRegion("hebei");
            var shijiazhuang = await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);
            var shaungqiao = await CreateRegion("shaungqiao", chengde.Id);
            var shaungluan = await CreateRegion("shaungluan", chengde.Id);

            var oldShijianzhuang = shijiazhuang.Code;
            var oldChengdeCode = chengde.Code;
            var oldShaungqiaoCode = shaungqiao.Code;
            var oldShaungluanCode = shaungluan.Code;

            chengde.Code = hebei.Code + "."  + _generalTreeCodeGenerator.CreateCode(5);
            shaungqiao.Code = chengde.Code + "."  + _generalTreeCodeGenerator.CreateCode(10);
            shaungluan.Code = chengde.Code + "."  + _generalTreeCodeGenerator.CreateCode(15);
            await _generalTreeRepository.UpdateAsync(chengde);
            await _generalTreeRepository.UpdateAsync(shaungqiao);
            await _generalTreeRepository.UpdateAsync(shaungluan);

            await _generalTreeManager.RegenerateAsync(hebei.Id);

            chengde = await GetRegion(chengde.Name);
            chengde.Code.ShouldBe(oldChengdeCode);
            shijiazhuang = await GetRegion(shijiazhuang.Name);
            shijiazhuang.Code.ShouldBe(oldShijianzhuang);
            shaungqiao = await GetRegion(shaungqiao.Name);
            shaungqiao.Code.ShouldBe(oldShaungqiaoCode);
            shaungluan = await GetRegion(shaungluan.Name);
            shaungluan.Code.ShouldBe(oldShaungluanCode);
        }

        [Fact]
        public async Task Regenerate_Root_Test()
        {
            var hebei = await CreateRegion("hebei");
            var shijiazhuang = await CreateRegion("shijiazhuang", hebei.Id);
            var chengde = await CreateRegion("chengde", hebei.Id);
            var shaungqiao = await CreateRegion("shaungqiao", chengde.Id);
            var shaungluan = await CreateRegion("shaungluan", chengde.Id);

            var beijing = await CreateRegion("beijing");
            var dongcheng = await CreateRegion("dongcheng", beijing.Id);
            var xicheng = await CreateRegion("xicheng", beijing.Id);
            var changanjie = await CreateRegion("changanjie", xicheng.Id);

            var oldChengdeCode = chengde.Code;
            var oldShaungqiaoCode = shaungqiao.Code;
            var oldShaungluanCode = shaungluan.Code;
            chengde.Code = hebei.Code  + "." + _generalTreeCodeGenerator.CreateCode(5);
            shaungqiao.Code = chengde.Code  + "." + _generalTreeCodeGenerator.CreateCode(10);
            shaungluan.Code = chengde.Code  + "." + _generalTreeCodeGenerator.CreateCode(15);
            await _generalTreeRepository.UpdateAsync(chengde);
            await _generalTreeRepository.UpdateAsync(shaungqiao);
            await _generalTreeRepository.UpdateAsync(shaungluan);

            var oldbeijingCode = beijing.Code;
            var oldXichengCode = xicheng.Code;
            var oldchanganjieCode = changanjie.Code;
            var oldDongcheng = dongcheng.Code;
            beijing.Code = _generalTreeCodeGenerator.CreateCode(5);
            dongcheng.Code = beijing.Code + "." + _generalTreeCodeGenerator.CreateCode(9);
            xicheng.Code = beijing.Code + "."  + _generalTreeCodeGenerator.CreateCode(10);
            changanjie.Code = xicheng.Code + "."  + _generalTreeCodeGenerator.CreateCode(15);
            await _generalTreeRepository.UpdateAsync(beijing);
            await _generalTreeRepository.UpdateAsync(xicheng);
            await _generalTreeRepository.UpdateAsync(changanjie);
            await _generalTreeRepository.UpdateAsync(dongcheng);

            await _generalTreeManager.RegenerateAsync();

            chengde = await GetRegion(chengde.Name);
            chengde.Code.ShouldBe(oldChengdeCode);
            shaungqiao = await GetRegion(shaungqiao.Name);
            shaungqiao.Code.ShouldBe(oldShaungqiaoCode);
            shaungluan = await GetRegion(shaungluan.Name);
            shaungluan.Code.ShouldBe(oldShaungluanCode);

            beijing = await GetRegion(beijing.Name);
            beijing.Code.ShouldBe(oldbeijingCode);
            xicheng = await GetRegion(xicheng.Name);
            xicheng.Code.ShouldBe(oldXichengCode);
            changanjie = await GetRegion(changanjie.Name);
            changanjie.Code.ShouldBe(oldchanganjieCode);
            dongcheng = await GetRegion(dongcheng.Name);
            dongcheng.Code.ShouldBe(oldDongcheng);
        }

        [Fact]
        public async Task FullName_Hyphen_Test()
        {
            using (var uow = GetRequiredService<IUnitOfWorkManager>().Begin())
            {
                var opt = Substitute.For<IOptions<GeneralTreeOptions>>();
                opt.Value.Returns(new GeneralTreeOptions
                {
                    Hyphen = "->"
                });

                var manager = new GeneralTreeManager<Region, Guid>(
                    GetRequiredService<IGeneralTreeCodeGenerator>(),
                    GetRequiredService<IGeneralTreeRepository<Region, Guid>>(),
                    opt,
                    GetRequiredService<IStringLocalizer<GeneralTreeResource>>()
                    );

                //Act
                var beijing = new Region
                {
                    Name = "beijing"
                };
                await manager.CreateAsync(beijing);
                await uow.SaveChangesAsync();

                var xicheng = new Region
                {
                    Name = "xicheng",
                    ParentId = beijing.Id
                };
                await manager.CreateAsync(xicheng);
                await uow.SaveChangesAsync();

                //Assert
                var xc = await GetRegion("xicheng");
                xc.ShouldNotBeNull();
                xc.Name.ShouldBe("xicheng");
                xc.FullName.ShouldBe("beijing->xicheng");
            }
        }

        [Fact]
        public async Task CheckSameNameExpression_Test()
        {
            using (var uow = GetRequiredService<IUnitOfWorkManager>().Begin())
            {
                var opt = Substitute.For<IOptions<GeneralTreeOptions>>();
                opt.Value.Returns(new GeneralTreeOptions
                {
                    CheckSameNameExpression = (regionThis, regionCheck) =>
                        ((Region)regionThis).SomeForeignKey == ((Region)regionCheck).SomeForeignKey
                });

                var manager = new GeneralTreeManager<Region, Guid>(
                    GetRequiredService<IGeneralTreeCodeGenerator>(),
                    GetRequiredService<IGeneralTreeRepository<Region, Guid>>(),
                    opt,
                    GetRequiredService<IStringLocalizer<GeneralTreeResource>>()
                );

                //Act
                await manager.CreateAsync(new Region
                {
                    Name = "beijing",
                    SomeForeignKey = 1
                });
                await uow.SaveChangesAsync();

                //Act
                await manager.CreateAsync(new Region
                {
                    Name = "beijing",
                    SomeForeignKey = 2
                });
                await uow.SaveChangesAsync();

                //Assert
                var beijing1 = (await _generalTreeRepository.GetListAsync()).Where(x => x.Name == "beijing" && x.SomeForeignKey == 1);
                beijing1.ShouldNotBeNull();

                var beijing2 = (await  _generalTreeRepository.GetListAsync()).Where(x => x.Name == "beijing" && x.SomeForeignKey == 2);
                beijing2.ShouldNotBeNull();
            }
        }
    }
}
