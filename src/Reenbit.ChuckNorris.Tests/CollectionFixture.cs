using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Reenbit.ChuckNorris.Tests
{
    [CollectionDefinition("ShareCollection")]
    public class CollectionFixture : ICollectionFixture<BaseMocks>
    {
    }
}
