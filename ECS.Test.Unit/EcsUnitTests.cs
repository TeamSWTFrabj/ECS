using System;
using NSubstitute;
using NUnit.Framework;


namespace ECS.Test.Unit
{
    [TestFixture]
    public class EcsUnitTests
    {
        // member variables to hold uut and fakes
        private ITempSensor _fakeTempSensor;
        private IHeater _fakeHeater;
        private ECS _uut;
        private IWindow _fakeWindow;

        [SetUp]
        public void Setup()
        {
            // Create the fake stubs and mocks
            _fakeHeater = Substitute.For<IHeater>();
            _fakeTempSensor = Substitute.For<ITempSensor>();
            _fakeWindow = Substitute.For<IWindow>();
            // Inject them into the uut via the constructor
            _uut = new ECS(_fakeTempSensor, _fakeHeater, _fakeWindow, 25, 28);
        }

        #region Threshold tests

        [Test]
        public void Thresholds_ValidUpperTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = 27; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_ValidLowerTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw 
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = 26; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_UpperSetToLower_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = _uut.LowerTemperatureThreshold; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_LowerSetToUpper_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = _uut.UpperTemperatureThreshold; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_InvalidUpperTemperatureThresholdSet_ArgumentExceptionThrown()
        {
            // Check that it throws when upper is illegal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = 24; }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Thresholds_InvalidLowerTemperatureThresholdSet_ArgumentExceptionThrown()
        {
            // Check that it throws when lower is illegal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = 29; }, Throws.TypeOf<ArgumentException>());
        }

        #endregion

        #region Regulation tests

        #region T < Tlow

        [Test]
        public void Regulate_TempIsLow_HeaterIsTurnedOn()
        {
            // Setup stub with desired response
            _fakeTempSensor.GetTemp().Returns(24);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _fakeHeater.Received(1).TurnOn();
        }


        [Test]
        public void Regulate_TempIsLow_WindowIsClosed()
        {
            // Setup stub with desired response
            _fakeTempSensor.GetTemp().Returns(24);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _fakeWindow.Received(1).Close();
        }

        #endregion

        #region T == Tlow

        [Test]
        public void Regulate_TempIsAtLowerThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(25);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _fakeHeater.Received(1).TurnOff();
        }

        [Test]
        public void Regulate_TempIsAtLowerThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(25);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _fakeWindow.Received(1).Close();
        }

        #endregion

        #region Tlow < T < Thigh

        [TestCase(26)]
        [TestCase(27)]
        public void Regulate_TempIsBetweenLowerAndUpperThresholds_HeaterIsTurnedOff(int testTemp)
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(testTemp);
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _fakeHeater.DidNotReceive().TurnOn();
        }

        [Test]
        public void Regulate_TempIsBetweenLowerAndUpperThresholds_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(27);
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _fakeWindow.Received(1).Close();
        }

        #endregion

        #region T == Thigh

        [Test]
        public void Regulate_TempIsAtUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(27);
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _fakeHeater.Received(0).TurnOn();
        }

        [Test]
        public void Regulate_TempIsAtUpperThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(27);
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _fakeWindow.Received(1).Close();
        }

        #endregion

        #region T > Thigh

        [Test]
        public void Regulate_TempIsAboveUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(27);
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _fakeHeater.Received(1).TurnOff();
        }

        [Test]
        public void Regulate_TempIsAboveUpperThreshold_WindowIsOpened()
        {
            // Setup the stub with desired response
            _fakeTempSensor.GetTemp().Returns(29);
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _fakeWindow.Received(1).Open();
        }

        #endregion

        #endregion
    }
}
