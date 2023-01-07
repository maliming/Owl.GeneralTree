using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owl.GeneralTree;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace MyTree;

public class RegionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IGeneralTreeManager<Region, Guid> _generalTreeManager;

    public RegionDataSeedContributor(IGeneralTreeManager<Region, Guid> generalTreeManager)
    {
        _generalTreeManager = generalTreeManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var northAmerica = new Region(Guid.NewGuid())
        {
            Name = "North America",
            Children = new List<Region>
            {
                new Region(Guid.NewGuid())
                {
                    Name = "United States",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "New York"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "San Francisco"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Washington, D.C."
                        }
                    }
                },
                new Region(Guid.NewGuid())
                {
                    Name = "Canada",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "Abbotsford"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Armstrong"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Burnaby"
                        }
                    }
                },
                new Region(Guid.NewGuid())
                {
                    Name = "Mexico",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "Ciudad de México"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Ecatepec de Morelos"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Guadalajara"
                        }
                    }
                }
            }
        };

        var asia = new Region(Guid.NewGuid())
        {
            Name = "Asia",
            Children = new List<Region>
            {
                new Region(Guid.NewGuid())
                {
                    Name = "China",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "Beijing"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Shanghai"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Hong Kong"
                        }
                    }
                },
                new Region(Guid.NewGuid())
                {
                    Name = "Japan",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "Tokyo"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Hokkaido"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Nagoya"
                        }
                    }
                }
            }
        };

        var europe = new Region(Guid.NewGuid())
        {
            Name = "Europe",
            Children = new List<Region>
            {
                new Region(Guid.NewGuid())
                {
                    Name = "United Kingdom",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "London"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Manchester"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Birmingham"
                        }
                    }
                },
                new Region(Guid.NewGuid())
                {
                    Name = "Germany",
                    Children = new List<Region>()
                    {
                        new Region(Guid.NewGuid())
                        {
                            Name = "Berlin"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "Hamburg"
                        },
                        new Region(Guid.NewGuid())
                        {
                            Name = "München"
                        }
                    }
                }
            }
        };


        await _generalTreeManager.BulkCreateAsync(northAmerica);
        await _generalTreeManager.BulkCreateAsync(asia);
        await _generalTreeManager.BulkCreateAsync(europe);
    }
}