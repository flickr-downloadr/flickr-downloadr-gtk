namespace FloydPink.Flickr.Downloadr.BoundaryCrossingTests.LogicTests {
    using System.Threading;
    using Bootstrap;
    using Logic.Interfaces;
    using Model;
    using Model.Constants;
    using NUnit.Framework;

    [TestFixture]
    public class BrowserLogicTests {
        private bool _asynchronouslyLoggedIn;
        private IBrowserLogic _logic;
        private ILoginLogic _loginLogic;
        private User _user;

        [SetUp]
        public async void SetupTest() {
            // do everything to take us to the browser window...
            if (!(await _loginLogic.IsUserLoggedInAsync(ApplyLoggedInUser))) {
                _loginLogic.Login(ApplyLoggedInUser);
            }
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Bootstrapper.Initialize();
            _loginLogic = Bootstrapper.GetInstance<ILoginLogic>();
            _logic = Bootstrapper.GetInstance<IBrowserLogic>();
        }

        private void ApplyLoggedInUser(User user) {
            _user = user;
            _asynchronouslyLoggedIn = true;
        }

        private void WaitTillLoggedIn() {
            while (!_asynchronouslyLoggedIn) {
                Thread.Sleep(1000);
            }
        }

        [Test]
        public async void GetPublicPhotos_WillGetPublicPhotos() {
            WaitTillLoggedIn();
            var photosResponse =
                await
                    _logic.GetPhotosAsync(Methods.PeopleGetPublicPhotos, _user, Preferences.GetDefault(), 1,
                        null);
            Assert.IsNotNull(photosResponse.Photos);
        }

        [Test]
        public void ProofOfConceptFor_AsynchronousTesting() {
            WaitTillLoggedIn();
            Assert.IsNotNull(_user);
        }
    }
}
