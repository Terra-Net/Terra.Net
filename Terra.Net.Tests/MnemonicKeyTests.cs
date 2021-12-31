using Cosmos.Base.V1Beta1;
using Cosmos.Tx.V1Beta1;
using Newtonsoft.Json;
using NUnit.Framework;
using Terra.Net.Extensions;
using Terra.Net.Objects;
using Google.Protobuf;
namespace Terra.Net.Tests
{
    public class MnemonicKeyTests
    {
        MnemonicKey keyToTest;
        [SetUp]
        public void Setup()
        {
            keyToTest = new MnemonicKey("swamp increase solar renew twelve easily possible pig ostrich harvest more indicate lion denial kind target small dumb mercy under proud arrive gentle field");
        }

        [Test]
        public void Shoul_Get_Correct_Address_From_Mnemonic_Key()
        {
            string expectedAddress = "terra12dazwl3yq6nwrce052ah3fudkarglsgvacyvl9";
            string actualAddress = keyToTest.GetAccountAddress.Value;
            Assert.AreEqual(expectedAddress, actualAddress);
        }

    }

}