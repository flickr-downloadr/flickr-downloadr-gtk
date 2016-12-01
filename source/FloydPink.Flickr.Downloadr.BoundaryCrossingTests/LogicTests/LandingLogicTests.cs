using System;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using NUnit.Framework;

namespace FloydPink.Flickr.Downloadr.BoundaryCrossingTests.LogicTests
{
  [TestFixture]
  public class LandingLogicTests
  {
    [SetUp]
    public async Task SetupTest()
    {
      // do everything to take us to the browser window...
      if (!await _loginLogic.IsUserLoggedInAsync(ApplyLoggedInUser))
      {
        _loginLogic.Login(ApplyLoggedInUser);
      }
    }

    private bool _asynchronouslyLoggedIn;
    private ILandingLogic _logic;
    private ILoginLogic _loginLogic;
    private User _user;

    [OneTimeSetUp]
    public void SetupTestFixture()
    {
      Bootstrapper.Initialize();
      _loginLogic = Bootstrapper.GetInstance<ILoginLogic>();
      _logic = Bootstrapper.GetInstance<ILandingLogic>();
    }

    private void ApplyLoggedInUser(User user)
    {
      _user = user;
      _asynchronouslyLoggedIn = true;
    }

    private void WaitTillLoggedIn()
    {
      while (!_asynchronouslyLoggedIn)
      {
        Thread.Sleep(1000);
      }
    }

    [Test]
    public async Task GetPublicPhotos_WillGetPublicPhotos()
    {
      WaitTillLoggedIn();
      var photosResponse =
        await
          _logic.GetPhotosetsAsync(Methods.PhotosetsGetList, _user, Preferences.GetDefault(), 1,
            new Progress<ProgressUpdate>());
      Assert.IsNotNull(photosResponse.Photosets);
    }
  }
}
