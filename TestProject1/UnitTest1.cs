using Xunit;
using FakeItEasy;
using UnityEngine;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Crea una simulazione di GameObject
            var fakeGameObject = A.Fake<GameObject>();

            // Configura le simulazioni necessarie
            A.CallTo(() => fakeGameObject.name).Returns("FakeObject");

            string speaksVerb = "speaks";
            const string clipNameToPass =  "clipAudioForTest";

            // Inietta l'oggetto simulato nel costruttore dell'Action
            ECARules4AllPack.Action action = new ECARules4AllPack.Action(fakeGameObject, speaksVerb, "Tests/" + clipNameToPass);

            // A questo punto puoi fare le asserzioni che ti servono
            Assert.True(true);
        }
    }
}