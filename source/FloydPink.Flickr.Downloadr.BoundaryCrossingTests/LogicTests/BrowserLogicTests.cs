using System.Threading;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using NUnit.Framework;

namespace FloydPink.Flickr.Downloadr.BoundaryCrossingTests.LogicTests {
    [TestFixture]
    public class BrowserLogicTests {
        [SetUp]
        public async void SetupTest() {
            // do everything to take us to the browser window...
            if (! (await this._loginLogic.IsUserLoggedInAsync(ApplyLoggedInUser))) {
                this._loginLogic.Login(ApplyLoggedInUser);
            }
        }

        private ILoginLogic _loginLogic;
        private IBrowserLogic _logic;
        private User _user;
        private bool _asynchronouslyLoggedIn;

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Bootstrapper.Initialize();
            this._loginLogic = Bootstrapper.GetInstance<ILoginLogic>();
            this._logic = Bootstrapper.GetInstance<IBrowserLogic>();
        }

        private void ApplyLoggedInUser(User user) {
            this._user = user;
            this._asynchronouslyLoggedIn = true;
        }

        private void WaitTillLoggedIn() {
            while (!this._asynchronouslyLoggedIn) {
                Thread.Sleep(1000);
            }
        }

        [Test]
        public async void GetPublicPhotos_WillGetPublicPhotos() {
            WaitTillLoggedIn();
            PhotosResponse photosResponse =
                await
                    this._logic.GetPhotosAsync(Methods.PeopleGetPublicPhotos, this._user, Preferences.GetDefault(), 1,
                        null);
            Assert.IsNotNull(photosResponse.Photos);
        }

        [Test]
        public void ProofOfConceptFor_AsynchronousTesting() {
            WaitTillLoggedIn();
            Assert.IsNotNull(this._user);
        }
    }
}
