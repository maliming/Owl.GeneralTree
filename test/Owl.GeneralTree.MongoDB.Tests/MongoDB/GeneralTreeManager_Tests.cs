using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Owl.GeneralTree.MongoDB;

[Collection(MongoTestCollection.Name)]
public class GeneralTreeManager_Tests : GeneralTreeManager_Tests<GeneralTreeMongoDbTestModule>
{
}
